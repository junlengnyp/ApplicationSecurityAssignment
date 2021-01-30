<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogIn.aspx.cs" Inherits="ApplicationSecurityAssignment.LogIn" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Log In</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LfqyBcaAAAAAMTz9ScQ7Xsv7260rvtlAoJS6eiG"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <fieldset>
                <legend>Login</legend>
                <p>Email Address: <asp:TextBox ID="tb_emailaddress" runat="server" Height="25px" Width="137px" /></p>
                <p>Password: <asp:TextBox ID="tb_password" TextMode="Password" runat="server" Height="25px" Width="137px" /></p>
                <p><asp:Button ID ="loginBtn" runat="server" Text="Login" OnClick="LogInMe" Height ="27px" Width="133px"/>
                 &nbsp <asp:Button ID ="signUpButton" runat="server" Text="Sign Up" OnClick="SignUpMe" Height ="27px" Width="133px"/>
                <br />
                    <br />
                <asp:Button ID ="forgotPassword" runat="server" Text="Forget Password" OnClick="forgetPassword" Height ="27px" Width="133px"/>
                <br />
                <br />
                <asp:Label ID="errorMsg" runat="server" Text ="" EnableViewState="false"></asp:Label>
                </p>
                <asp:Label ID="lbl_gScore" runat="server" Text="" style ="visibility:hidden"></asp:Label>
                <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            </fieldset>
        </div>
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6LfqyBcaAAAAAMTz9ScQ7Xsv7260rvtlAoJS6eiG', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
</html>
