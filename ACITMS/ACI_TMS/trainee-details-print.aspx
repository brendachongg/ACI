<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="trainee-details-print.aspx.cs" Inherits="ACI_TMS.trainee_details_print" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <!-- Bootstrap Core JavaScript -->
    <script src="/Scripts/bootstrap.min.js"></script>

    <script type="text/javascript">
        function PrintScreen() {
            window.print();
        }
    </script>

    <style>
        .table-borderless > tbody > tr > td,
        .table-borderless > tbody > tr > th,
        .table-borderless > tfoot > tr > td,
        .table-borderless > tfoot > tr > th,
        .table-borderless > thead > tr > td,
        .table-borderless > thead > tr > th {
            border: none;
        }
    </style>
</head>
<body onload="window.print()">
    <form id="form1" runat="server">
        <div id="container">
            <div class="row text-center">
                <div class="col-lg-6 col-lg-offset-3 ">
                    <asp:Image ID="imgACILogo" runat="server" ImageUrl="~/Resource/images/ACI_logo.png" ImageAlign="AbsMiddle" Width="200" />
                    <br />
                    <br />
                    <b>Asian Culinary Institute Singapore</b>
                    <br />
                    <small>11 Eunos Road 8, Lifelong Learning Institute #03-01, Singapore 408601
                            <br />
                        Tel:  (65) 6417 3318 | Fax: (65) 6747 9506 www.aci.edu.sg
                    </small>
                </div>
            </div>

            <div class="row text-center">
                <h4>
                    <asp:Label ID="lb1" runat="server" Text="Trainee Details"></asp:Label>
                </h4>
            </div>

            <div class="table-responsive">

                <table class="table table-borderless">
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lbTraineeId" Text="Trainee ID" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbTraineeIdText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lbFullName" Text="Full Name" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbFullNameText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbIdNo" runat="server" Text="Identification" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbIdNoText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbIdType" runat="server" Text="Identification Type" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbIdTypeText" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="lbNationality" runat="server" Text="Nationality" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbNationalityText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbContactNo1" runat="server" Text="Contact Number 1 (Mobile)" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbContactNo1Text" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbContactNo2" runat="server" Text="Contact Number 2 (Home/Others)" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbContactNo2Text" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbRace" runat="server" Text="Race" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbRaceText" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="lbGender" runat="server" Text="Gender" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbGenderText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td colspan="2">
                            <asp:Label ID="lbEmailAdd" runat="server" Text="Email Address" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbEmailAddText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbDOB" runat="server" Text="Date of Birth" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbDOBText" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lbAddress" runat="server" Text="Address Line" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbAddressText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbPostalCode" runat="server" Text="Postal Code" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbPostalCodeText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbHighestEdu" runat="server" Text="Highest Education" Font-Bold="true"></asp:Label>

                            <asp:Label ID="lbHighestEduText" runat="server" Text="Label"></asp:Label>
                        </td>

                    </tr>
                    <tr>
                        <td colspan="4">

                            <asp:Label ID="lbHighestEduRemark" runat="server" Text="Highest Education Remark" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbHighestEduRemarkText" runat="server" Text="Label"></asp:Label>

                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="lbTextSpokenLanguage" runat="server" Text="Spoken Language" Font-Bold="true"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbEng" runat="server" Text="English" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbEngText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbChi" runat="server" Text="Chinese" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbChiText" runat="server" Text="Label"></asp:Label>
                        </td>

                        <td>
                            <asp:Label ID="lbOtherLangPro" runat="server" Text="Label" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbOtherLangProText" runat="server" Text="Label"></asp:Label>
                        </td>

                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="lbTextWritternLang" runat="server" Text="Written Language" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbWEng" runat="server" Text="English" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbWEngText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbWChi" runat="server" Text="Chinese" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbWChiText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbWItherLangPro" runat="server" Text="Label" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbWItherLangProText" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>


                </table>
            </div>

            <div class="row text-center">
                <h4>
                    <asp:Label ID="Label2" runat="server" Text="Trainee Employment History"></asp:Label>
                </h4>
            </div>

            <asp:Panel ID="pnNoEmploymentHistory" Enabled="false" runat="server" Visible="false">
                <div class="row text-center">
                    <div class="col-lg-12">

                        <asp:Label ID="lbNoHistory" runat="server" Text="Trainee Employment History"></asp:Label>

                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnCurrEmploymentHistory" Enabled="false" runat="server" Visible="false">
                <div class="row text-left">
                    <div class="col-lg-12">
                        <h5>
                            <asp:Label ID="lbCurr" runat="server" Text="Current Employment"></asp:Label>
                        </h5>
                    </div>
                </div>
                <div class="table-responsive">

                    <table class="table table-borderless">
                        <tr>
                            <td>
                                <asp:Label ID="lb3" runat="server" Font-Bold="true" Text="Company Name"></asp:Label>
                                <asp:Label ID="lbCurrCompanyName" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lb4" runat="server" Font-Bold="true" Text="Department"></asp:Label>
                                <asp:Label ID="lbCurrDepartment" runat="server"></asp:Label>
                            </td>


                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lb5" runat="server" Font-Bold="true" Text="Designation"></asp:Label>
                                <asp:Label ID="lbCurrentDesignation" runat="server"></asp:Label>
                            </td>

                            <td>
                                <asp:Label ID="lb6" runat="server" Font-Bold="true" Text="Employment Type"></asp:Label>
                                <asp:Label ID="lbCurrEmplStatus" runat="server"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lb7" runat="server" Font-Bold="true" Text="Desingation Type"></asp:Label>
                                <asp:Label ID="lbCurrEmplOccupation" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lb8" runat="server" Font-Bold="true" Text="Salary (S$)"></asp:Label>
                                <asp:Label ID="lbCurrSalary" runat="server"></asp:Label>
                            </td>

                            <td colspan="2">
                                <asp:Label ID="lb9" runat="server" Font-Bold="true" Text="Start Date"></asp:Label>
                                <asp:Label ID="lbCurrStartDate" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnPrevEmploymentHistory" Enabled="false" runat="server" Visible="false">
                <div class="row text-left">
                    <div class="col-lg-12">
                        <h5>
                            <asp:Label ID="Label1" runat="server" Text="Previous Employment"></asp:Label>
                        </h5>
                    </div>
                </div>
                <div class="table-responsive">

                    <table class="table table-borderless">
                        <tr>
                            <td>
                                <asp:Label ID="lb10" runat="server" Font-Bold="true" Text="Company Name"></asp:Label>
                                <asp:Label ID="lbPrevCompanyName" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lb11" runat="server" Font-Bold="true" Text="Department"></asp:Label>
                                <asp:Label ID="lbPrevDept" runat="server"></asp:Label>
                            </td>


                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lb12" runat="server" Font-Bold="true" Text="Designation"></asp:Label>
                                <asp:Label ID="lbPrevDesignation" runat="server"></asp:Label>
                            </td>

                            <td>
                                <asp:Label ID="lb13" runat="server" Font-Bold="true" Text="Employment Type"></asp:Label>
                                <asp:Label ID="lbPrevEmplType" runat="server"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <asp:Label ID="lb14" runat="server" Font-Bold="true" Text="Desingation Type"></asp:Label>
                                <asp:Label ID="lbPrevOccupationType" runat="server"></asp:Label>
                            </td>

                            <td>
                                <asp:Label ID="lb15" runat="server" Font-Bold="true" Text="Salary (S$)"></asp:Label>
                                <asp:Label ID="lbPrevSalary" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>


                            <td>
                                <asp:Label ID="lb16" runat="server" Font-Bold="true" Text="Start Date"></asp:Label>
                                <asp:Label ID="lbPrevStartDate" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lb17" runat="server" Font-Bold="true" Text="End Date"></asp:Label>
                                <asp:Label ID="lbPrevEndDate" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>

            <div class="row text-center">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">

                    <h4>
                        <asp:Label ID="lb2" runat="server" Text="Enrolled Programme Details"></asp:Label>
                    </h4>

                </div>
            </div>

            <asp:Panel ID="pnEnrolledDetails" Enabled="false" runat="server">
                <div class="table-responsive">

                    <table class="table table-borderless">
                        <tr>
                            <td>
                                <asp:Label ID="lbProjCode" runat="server" Text="Project Code" Font-Bold="true"></asp:Label>
                                <asp:Label ID="tbProjCode" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lbCourseCode" runat="server" Text="Course Code" Font-Bold="true"></asp:Label>
                                <asp:Label ID="tbCourseCode" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lbProgrammeTitle" runat="server" Text="Programme Title" Font-Bold="true"></asp:Label>
                                <asp:Label ID="tbProgrammeTitle" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lbClassCode" runat="server" Text="Class Codee" Font-Bold="true"></asp:Label>
                                <asp:Label ID="tbClassCode" runat="server"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <asp:Label ID="lbProgrammeStartDate" runat="server" Text="Programme Start Date" Font-Bold="true"></asp:Label>
                                <asp:Label ID="tbProgrammeStartDate" runat="server"></asp:Label>
                            </td>

                            <td>
                                <asp:Label ID="lbProgrammeEndDate" runat="server" Text="Programme End Date" Font-Bold="true"></asp:Label>
                                <asp:Label ID="tbProgrammeEndDate" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>

            </asp:Panel>

        </div>
    </form>
</body>
</html>
