<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="temp.aspx.cs" Inherits="ACI_TMS.temp" %>
<%@ Import Namespace="GeneralLayer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        Text: <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox><asp:Button ID="Encrypt" runat="server" Text="Encrypt" OnClick="Button1_Click" /><asp:Button ID="Decrypt" runat="server" Text="Decrypt" OnClick="Button2_Click" />
        <br /><asp:Label ID="Label1" runat="server" Text="Label"></asp:Label><asp:Button ID="Enroll" runat="server" Text="Enroll" OnClick="Enroll_Click" />
        <br />
        <asp:Button ID="btnReport" runat="server" Text="Report" OnClick="btnReport_Click" />
        <asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label>
        <hr />
        Name:<asp:TextBox ID="tbName" runat="server"></asp:TextBox><br />
        Email:<asp:TextBox ID="tbEmail" runat="server"></asp:TextBox><br />
        ID:<asp:TextBox ID="tbId" runat="server"></asp:TextBox><br />
        <asp:Button ID="btnAddUser" runat="server" Text="Add" OnClick="btnAddUser_Click" />
    </form>
    
</body>
</html>
