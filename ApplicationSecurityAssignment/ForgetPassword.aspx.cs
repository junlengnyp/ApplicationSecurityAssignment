using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ApplicationSecurityAssignment
{
    public partial class ForgetPassword : System.Web.UI.Page
    {
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void changePassword(string userid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET passwordhash=@passwordhash, passwordsalt=@passwordsalt, accountLockout=@accountLockout, passwordage=@passwordage WHERE emailaddress=@emailaddress"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@passwordhash", finalHash);
                            cmd.Parameters.AddWithValue("@passwordsalt", salt);
                            cmd.Parameters.AddWithValue("@emailaddress", userid);
                            cmd.Parameters.AddWithValue("@accountLockout", 0);
                            cmd.Parameters.AddWithValue("@passwordage", System.DateTime.Now);
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

        protected string getRecoveryCode(string emailaddress)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT accountRecoveryPin FROM Account WHERE emailaddress=@emailaddress";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@emailaddress", emailaddress);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["accountRecoveryPin"] != null)
                        {
                            if (reader["accountRecoveryPin"] != DBNull.Value)
                            {
                                h = reader["accountRecoveryPin"].ToString();
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


        private int checkPassword(string password)
        {
            int score = 0;

            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[@!#$%^&*(),.?:{}|<>]"))
            {
                score++;
            }
            return score;
        }
        protected void changePassword(object sender, EventArgs e)
        {
            // implement codes for the button event
            // Extract data from textbox
            int scores = checkPassword(tb_password.Text);
            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Very Strong";
                    break;
                default:
                    break;
            }
            lbl_pwdchecker.Text = "Status : " + status;
            if (scores < 4)
            {
                lbl_pwdchecker.ForeColor = Color.Red;
                return;
            }
            lbl_pwdchecker.ForeColor = Color.Green;


            string rcode = tb_recovery.Text.ToString().Trim();
            string userid = tb_emailaddress.Text.ToString().Trim();
            DateTime newdate = DateTime.Now;
            DateTime comparedate = DateTime.Parse(getDate(userid));
            if ((newdate- comparedate).TotalMinutes < 5)
            {
                errorMsg.Text = "Password cannot be changed at the moment (Password Minimum Age)";
                errorMsg.ForeColor = Color.Red;
            }
            else
            {
                if (rcode.Equals(getRecoveryCode(userid)))
                {
                    string pwd = tb_password.ToString().Trim();
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];
                    //Fills array of bytes with a cryptographically strong sequence of random values.
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);
                    SHA512Managed hashing = new SHA512Managed();
                    string pwdWithSalt = pwd + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    finalHash = Convert.ToBase64String(hashWithSalt);
                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;
                    changePassword(userid);
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    Response.Write("<script>alert('Recovery Code Was Incorrect!');</script>");

                }
            }

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