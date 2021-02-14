using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Net;
using System.Configuration;
using System.Data;

namespace ApplicationSecurityAssignment
{
    public partial class LogIn : System.Web.UI.Page
    {
        string MYDBConnectionString = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string getDBHash(string emailaddress)
        {
            string MYDBConnectionString = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select passwordhash FROM Account WHERE emailaddress=@emailaddress";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@emailaddress", emailaddress);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string getDBSalt(string emailaddress)
        {
            string MYDBConnectionString = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select passwordsalt FROM ACCOUNT WHERE emailaddress=@emailaddress";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@emailaddress", emailaddress);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }


        protected void LogInMe(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {

                string pwd = HttpUtility.HtmlEncode(tb_password.Text).ToString();
                string userid = HttpUtility.HtmlEncode(tb_emailaddress.Text).ToString();

                {
                    if (userid == "" || pwd == "")
                    {
                        errorMsg.Text = "Please fill in your log in details.";
                        errorMsg.ForeColor = Color.Red;
                    }


                    else
                    {
                        if (checkValidEmail(userid) == null)
                        {
                            errorMsg.Text = "Email or password is invalid.";
                            errorMsg.ForeColor = Color.Red;
                        }
                        else
                        {
                            if (checkAccountLockout(userid) == "True")
                            {
                                errorMsg.Text = "Account is locked out.";
                                errorMsg.ForeColor = Color.Red;

                            }
                            else
                            {
                                DateTime newdate = System.DateTime.Now;
                                DateTime comparedate = DateTime.Parse(getDate(userid));
                                if ((newdate - comparedate).TotalMinutes > 15)
                                {
                                    faccountLockout(userid);
                                    errorMsg.Text = "Account locked out due to password expiry.";
                                }
                                else
                                {
                                    SHA512Managed hashing = new SHA512Managed();
                                    string dbHash = getDBHash(userid);
                                    string dbSalt = getDBSalt(userid);
                                    try
                                    {
                                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                                        {
                                            string pwdWithSalt = pwd + dbSalt;
                                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                            string userHash = Convert.ToBase64String(hashWithSalt);
                                            if (userHash.Equals(dbHash))
                                            {
                                                Session["LoggedIn"] = tb_emailaddress.Text.Trim();
                                                string guid = Guid.NewGuid().ToString();
                                                Session["AuthToken"] = guid;
                                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                                Response.Redirect("HomePage.aspx", false);

                                            }
                                            else
                                            {
                                                if (Session["LogInAttempt" + userid] == null)
                                                {
                                                    Session["LogInAttempt" + userid] = 2;
                                                    int intAttempt = (int)Session["LogInAttempt" + userid];
                                                    errorMsg.Text = "Email or password is not valid. Please try again. You have " + intAttempt + " left.";
                                                    errorMsg.ForeColor = Color.Red;

                                                }

                                                else
                                                {
                                                    int intAttempt = (int)Session["LogInAttempt" + userid];
                                                    intAttempt -= 1;
                                                    Session["LogInAttempt" + userid] = intAttempt;
                                                    if (intAttempt < 0)
                                                    {
                                                        SqlConnection connection = new SqlConnection(MYDBConnectionString);
                                                        string sql = "UPDATE Account SET accountLockout = 1 WHERE emailaddress=@emailaddress";
                                                        SqlCommand command = new SqlCommand(sql, connection);
                                                        command.Parameters.AddWithValue("@emailaddress", userid);
                                                        try
                                                        {
                                                            connection.Open();
                                                            SqlDataReader reader = command.ExecuteReader();
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            throw new Exception(ex.ToString());
                                                        }
                                                        finally { connection.Close(); }
                                                        errorMsg.Text = "This account has been locked.";
                                                        errorMsg.ForeColor = Color.Red;
                                                    }
                                                    else
                                                    {
                                                        errorMsg.Text = "Email or password is not valid. Please try again. You have " + intAttempt + " left.";
                                                        errorMsg.ForeColor = Color.Red;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.ToString());
                                    }
                                    finally { }
                                }
                            }
                        }

                    }
                }
            }
        }
        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LfqyBcaAAAAAJxYIsNHw1XakgwR37k0zzTk1910 &response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        lbl_gScore.Text = jsonResponse.ToString();
                        JavaScriptSerializer js = new JavaScriptSerializer();

                        WebForm1 jsonObject = js.Deserialize<WebForm1>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                    return result;
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected string checkValidEmail(string emailaddress)
        {
            string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select emailaddress FROM Account WHERE emailaddress=@emailaddress";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@emailaddress", emailaddress);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["emailaddress"] != null)
                        {
                            if (reader["emailaddress"] != DBNull.Value)
                            {
                                h = reader["emailaddress"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected string checkAccountLockout(string emailaddress)
        {
            string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select accountLockout FROM Account WHERE emailaddress=@emailaddress";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@emailaddress", emailaddress);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["accountLockout"] != null)
                        {
                            if (reader["accountLockout"] != DBNull.Value)
                            {
                                h = reader["accountLockout"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }

        protected void SignUpMe(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx");
        }

        protected void forgetPassword(object sender, EventArgs e)
        {
            Response.Redirect("ForgetPassword.aspx");
        }

        public void faccountLockout(string userid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET accountLockout=@accountLockout WHERE emailaddress=@emailaddress"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@emailaddress", userid);
                            cmd.Parameters.AddWithValue("@accountLockout", 1);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            Response.Redirect("Login.aspx");
        }


        protected string getDate(string emailaddress)
        {
            string MYDBConnectionString = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select passwordage FROM ACCOUNT WHERE emailaddress=@emailaddress";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@emailaddress", emailaddress);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordage"] != null)
                        {
                            if (reader["passwordage"] != DBNull.Value)
                            {
                                s = reader["passwordage"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

    }
}