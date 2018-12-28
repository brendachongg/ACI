<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="aci-staff-login.aspx.cs" Inherits="ACI_TMS.aci_staff_login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <!-- Bootstrap Core CSS -->

    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <!-- Custom Fonts -->
    <link href="font-awesome/css/font-awesome.min.css" rel="stylesheet" />

    <style>
        body {
            background: url('Resource/images/reception.jpg') no-repeat center center fixed;
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
            opacity: 0.9;
        }

        .container {
            z-index: 9999;
        }
        /*.panel, .panel-default .panel-heading{
            background-color: transparent;
        }*/
    </style>
</head>
<body>
    <div class="container">
        <div class="row" style="margin-top: 25%;">
            <div class="col-md-4 col-lg-offset-3">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <strong class="">Staff Login</strong>

                    </div>
                    <div class="panel-body">
                        <form class="form-horizontal" role="form" runat="server">

                            <%-- Staff email address --%>
                            <div class="form-group">
                                <label for="inputEmail3" class="col-sm-3 control-label">Email</label>
                                <div class="col-sm-9">
                                    <asp:TextBox ID="tbLoginId" CssClass="form-control" runat="server" placeholder="Login ID" required="" Text="kee_li_ren@nyp.edu.sg"></asp:TextBox>
                                </div>
                            </div>

                            <%-- Password --%>
                            <div class="form-group">
                                <label for="inputPassword3" class="col-sm-3 control-label">Password</label>
                                <div class="col-sm-9">
                                    <asp:TextBox ID="tbPassword"  runat="server" CssClass="form-control" required="" Text="nyp_sit"></asp:TextBox>
                                </div>
                            </div>

                            <%-- 20 dec 2016 : password will be setup at later date. For now please enter any chars --%>
                            <%-- Sign in button --%>
                            <div class="form-group last">
                                <div class="col-sm-offset-3 col-sm-9">                                    
                                    <asp:Button ID="btnSignIn" CssClass="btn btn-success btn-sm" runat="server" Text="Sign in" OnClick="btnSignIn_Click" />
                                    
                                    &nbsp;<asp:Label ID="lbUnauthorizedMessage" runat="server" Text="Access denied." ForeColor="Red" Visible="false"></asp:Label>
                                </div>
                                
                            </div>

                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
