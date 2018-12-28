<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="enrollment-letter-print.aspx.cs" Inherits="ACI_TMS.enrollment_letter_print" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title>ACI - Enrollment Letter</title>

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
        <div id="page-wrapper">
            <div class="row">
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Resource/images/ACISReceiptLogo.jpg" Width="150" />
                </div>
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                    <b>Asian Culinary Institute Singapore</b>
                    <br />
                    <span style="font-size:6pt">
                    11 Eunos Road 8, Lifelong Learning Institute
                    <br />
                    #03-01, Singapore 408601
                    <br />
                    Tel: (65) 6417 3318   Fax: (65) 6747 9506
                    <br />
                    www.aci.edu.sg
                    </span>
                </div>
            </div>

            <hr style="border-color:black" />

            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" id="divContent" runat="server">

                </div>
            </div>
        </div>
    </form>
    <script type="text/javascript">
        window.print();
    </script>
</body>
</html>
