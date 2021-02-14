<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgetPassword.aspx.cs" Inherits="ApplicationSecurityAssignment.ForgetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forget Password</title>
     <script type ="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;
            if (str.length < 8) {
                document.getElementById("lbl_pwdsuggestion").innerHTML = "Password Length must be at Least 8 Characters";
                document.getElementById("lbl_pwdsuggestion").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdsuggestion").innerHTML = "Password require at least 1 Lower Case Letter";
                document.getElementById("lbl_pwdsuggestion").style.color = "Red";
                return ("no_lower_case");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdsuggestion").innerHTML = "Password require at least 1 Upper Case Letter";
                document.getElementById("lbl_pwdsuggestion").style.color = "Red";
                return ("no_upper_case");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdsuggestion").innerHTML = "Password require at least 1 number";
                document.getElementById("lbl_pwdsuggestion").style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[@!#$%^&*(),.?:{}|<>]/) == -1) {
                document.getElementById("lbl_pwdsuggestion").innerHTML = "Password is suggested to contain at least 1 special character.";
                document.getElementById("lbl_pwdsuggestion").style.color = "Red";
                return ("no_number");
            }
            document.getElementById("lbl_pwdsuggestion").innerHTML = "Password is very strong!.";
            document.getElementById("lbl_pwdsuggestion").style.color = "Green";
        }
     </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <fieldset>
                <asp:Label ID="lbl_emailaddress" runat="server" Text="Email Address: "></asp:Label>
                <asp:TextBox ID="tb_emailaddress" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Label ID="lbl_password" runat="server" Text="Enter New Password: "></asp:Label>
                <asp:TextBox ID="tb_password" runat="server" TextMode="Password" onkeyup ="javascript:validate()"></asp:TextBox>
                <br />
                <asp:Label ID="lbl_pwdsuggestion" runat="server" Text=""></asp:Label>
                <br />
                <asp:Label ID="lbl_pwdchecker" runat="server" Text=""></asp:Label>
                <br />
                <br />
                <asp:Label ID="lbl_recovery" runat="server" Text="Recovery Key: "></asp:Label>
                <asp:TextBox ID="tb_recovery" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Label ID="errorMsg" runat="server" Text=""></asp:Label>
                <br />
                <br />
                <asp:Button ID ="recoverAccount" runat="server" Text="Confirm Change Password" OnClick="changePassword" Height ="27px" Width="200px"/>
            </fieldset>
        </div>
    </form>
</body>
</html>
