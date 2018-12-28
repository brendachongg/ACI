<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="case-log-view-attachment.aspx.cs" Inherits="ACI_TMS.case_log_view_attachment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title>ACI - Case Log Attachment</title>

    <!-- Bootstrap Core CSS -->
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <!-- MetisMenu CSS -->
    <link href="css/metisMenu.min.css" rel="stylesheet" />

    <!-- tms css -->
    <link href="css/acitms.css" rel="stylesheet" />

    <!-- Custom Fonts -->
    <link href="font-awesome/css/font-awesome.min.css" rel="stylesheet" />
</head>
<body style="font-size:8pt;margin:50px 50px 0px 50px;background-color:white;">
    <form id="form1" runat="server">
    <div id ="page-wrapper">
    <div class="row">
        <asp:Image ID="lbtnAttachment" runat="server" width ="500px"/>
    </div>
    </div>
    </form>
</body>
</html>
