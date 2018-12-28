<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="error.aspx.cs" Inherits="ACI_TMS.error" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title>ACI - Error</title>
    <!-- Bootstrap Core CSS -->
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <!-- MetisMenu CSS -->
    <link href="css/metisMenu.min.css" rel="stylesheet" />

    <!-- tms css -->
    <link href="css/acitms.css" rel="stylesheet" />

    <!-- Custom Fonts -->
    <link href="font-awesome/css/font-awesome.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <nav class="navbar navbar-default navbar-static-top" role="navigation" style="margin-bottom: 0">
                <div class="navbar-header">
                    <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                        <span class="sr-only">Toggle navigation</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <a class="navbar-brand"><asp:Label ID="lblTitle" runat="server" Text="ACI Trainee Management Systems"></asp:Label></a>
                </div>
            </nav>

            <div id="page-wrapper" style="text-align: center;background-color:white;">
                <br />
                <br />
                <br />
                <asp:Image runat="server" ImageUrl="~/Resource/images/sad_face.png" />
                <h1>OPPS!</h1>
                <i>
                    <h3>This is embrasssing. Something went wrong...</h3>
                </i>
                <br />
                <h4>
                    <asp:Label ID="lblErr" runat="server" ForeColor="Red"></asp:Label></h4>
                <br />
                <asp:Button ID="btnOk" runat="server" Text="OK" CssClass="btn btn-primary" OnClick="btnOk_Click" />
                <br />
                <br />
                <br />
            </div>
        </div>
    </form>
</body>
</html>
