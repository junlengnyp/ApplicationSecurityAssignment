<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="ApplicationSecurityAssignment.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sign Up</title>
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
            <asp:Label ID="lbl_firstname" runat="server" Text="First Name: "></asp:Label>
            <asp:TextBox ID="tb_firstname" runat="server" required="true"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lbl_lastname" runat="server" Text="Last Name: "></asp:Label>
            <asp:TextBox ID="tb_lastname" runat="server" required="true"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lbl_creditCard" runat="server" Text="Credit Card Info: "></asp:Label>
            <asp:TextBox ID="tb_creditCard" runat="server" TextMode="Number"  required="true"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lbl_emailaddress" runat="server" Text="Email Address: "></asp:Label>
            <asp:TextBox ID="tb_emailaddress" runat="server" TextMode="Email" required="true"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lbl_password" runat="server" Text="Password: "></asp:Label>
            <asp:TextBox ID="tb_password" runat="server" TextMode ="Password" autocomplete ="off" onkeyup ="javascript:validate()" required="true"></asp:TextBox>
            <br />
            <asp:Label ID="lbl_pwdsuggestion" runat="server" Text=""></asp:Label>
            <br />
            <asp:Label ID="lbl_pwdchecker" runat="server" Text=""></asp:Label>
            <br /> 
            <br />
            <asp:Label ID="lbl_dob" runat="server" Text="Date Of Birth:"></asp:Label>
            <asp:TextBox ID="tb_dob" runat="server" TextMode="Date" required="true"></asp:TextBox>
            <br />
            <br />
            <asp:Label ID="lbl_recovery" runat="server" Text="Recovery Key: "></asp:Label>
            <asp:TextBox ID="tb_recovery" runat="server" required="true"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="signupbutton" runat="server" Text="Sign Up" OnClick="signupbutton_Click" />
            </fieldset>
        </div>
    </form>
</body>
</html>
