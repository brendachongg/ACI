<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="absentee-payment-receipt.aspx.cs" Inherits="ACI_TMS.absentee_payment_receipt" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <title>ACI - Receipt</title>

    <!-- Bootstrap Core CSS -->
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <!-- MetisMenu CSS -->
    <link href="css/metisMenu.min.css" rel="stylesheet" />

    <!-- tms css -->
    <link href="css/acitms.css" rel="stylesheet" />

    <!-- Custom Fonts -->
    <link href="font-awesome/css/font-awesome.min.css" rel="stylesheet" />

</head>
<body style="font-size:8pt;margin:50px 50px 0px 50px;">
    <form id="form1" runat="server">
        <div id="page-wrapper">
            <div class="row">
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/Resource/images/ACI_logo.png" Width="170" />
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
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                    
                </div>
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                    GST Registration Number: <asp:Label ID="lbGSTReg" runat="server" Text=""></asp:Label>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    Receipt Number<br />
                    <asp:Label ID="lbReceiptNum" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    Payment Mode<br />
                    <asp:Label ID="lbPaymentMode" runat="server" Text=""></asp:Label>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    Date of Payment<br />
                    <asp:Label ID="lbPaymentDate" runat="server" Text=""></asp:Label>
                </div>
            </div>

            <div class="row text-left">
                <div class="col-lg-12 text-center">
                    <h4 style="font-size:11pt;font-weight:bold;"><u>OFFICIAL RECEIPT</u></h4>
                </div>
            </div>

            <br />
            <br />

            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="font-weight:bold;">
                    Payment Received from:
                    <table style="width:100%">
                        <tr>
                            <td style="width:130px">the sum of dollars</td>
                            <td style="border-bottom:1px solid black">&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align:center">*Amount to be spelled in words*</td>
                        </tr>
                    </table>
                    
                </div>
            </div>

            <br />
            <asp:HiddenField ID="hfType" runat="server" />
            <asp:HiddenField ID="hfBundleId" runat="server" />
            <div class="row text-left">
                <div class="col-lg-9 col-md-9 col-sm-9 col-xs-9">
                    <p>Programme Title: <asp:Label ID="lbProgTitle" runat="server" Text=""></asp:Label></p>
                    <p>Course Code: <asp:Label ID="lbProgCseCode" runat="server" Text="Course Code"></asp:Label></p>
                    <p>Project Code: <asp:Label ID="lbBatchProjCode" runat="server" Text="Project Code"></asp:Label></p>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3 text-right">
                    <p>Start Date: <asp:Label ID="lbBatchStart" runat="server" Text=""></asp:Label></p>
                    <p>End Date: <asp:Label ID="lbBatchEnd" runat="server" Text=""></asp:Label></p>
                    <p>Class Code: <asp:Label ID="lbBatchCode" runat="server" Text=""></asp:Label></p>
                </div>
            </div>

            <br />
            <br />

            <div class="row text-left">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <p>Trainee: <asp:Label ID="lbTraineeName" runat="server" Text=""></asp:Label> (<asp:Label ID="lbTraineeId" runat="server" Text=""></asp:Label>)</p>
                </div>
            </div>

            <br />

            <div class="row text-left">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <table class="table table-responsive">
                        <thead>
                            <tr>
                                <th>Item</th>
                                <th class="text-right">Price (S$)</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr id="trProg" runat="server">
                                <td>Make-up Lesson(s)</td>
                                <td class="text-right"><asp:Label ID="lbAmt" runat="server"></asp:Label></td>
                            </tr>

                            <asp:Repeater ID="rpModules" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-&nbsp;<%# Eval("noOfSessions") %> session(s) for <%# Eval("moduleTitle") %> (<%# Eval("moduleCode") %>)</td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>

                            <tr>
                                <td>7% GST (Calculated by system)</td>
                                <td class="text-right"><asp:Label ID="lbGST" runat="server" Text=""></asp:Label></td>
                            </tr>

                            <tr>
                                <td style="width: 75%;">
                                    <b>Net Amount Payable</b>
                                    <br />
                                    <br />
                                    <br />
                                    <b>Amount Paid</b>
                                    <br />
                                    <i><asp:Label ID="lbPaymentRemarks" runat="server" Text=""></asp:Label></i>
                                </td>
                                <td class="text-right" style="vertical-align:top;">
                                    <b><asp:Label ID="lbTotal" runat="server" Text=""></asp:Label></b>
                                    <br />
                                    <br />
                                    <br />
                                    <b><asp:Label ID="lbAmtPaid" runat="server" Text=""></asp:Label></b>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

            <br />
            <br />
            <br />
            <br />
            <br />

            <div class="row text-left" style="font-weight:bold;">
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                    <table style="width:100%">
                        <tr>
                            <td style="border-bottom:1px solid black">&nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                Name of Collector:
                                <br />for Asian Culinary Institute Singapore (ACIS)
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                </div>
            </div>

            <br />
        </div>
    </form>

    <script type="text/javascript">
        window.print();
    </script>
</body>
</html>