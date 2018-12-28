<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="applicant-details-print.aspx.cs" Inherits="ACI_TMS.applicant_details_print" EnableEventValidation="false" %>


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
                    <asp:Label ID="lb1" runat="server" Text="Application Form"></asp:Label>
                </h4>
            </div>

            <div class="table-responsive">

                <table class="table table-borderless small">
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lbTraineeId" Text="Applicant ID" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbApplicantId" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lbFullName" Text="Full Name" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lblName" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbIdNo" runat="server" Text="Identification" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lblNric" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lbIdType" runat="server" Text="Identification Type" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbltype" runat="server" Text="Label"></asp:Label>
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
                            <asp:Label ID="lbSponsored" runat="server" Text="Sponsored" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbSpon" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lbHighestEdu" runat="server" Text="Highest Education" Font-Bold="true"></asp:Label>

                            <asp:Label ID="lbHighestEduText" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td colspan="2">

                            <asp:Label ID="lbHighestEduRemark" runat="server" Text="Highest Education Remark" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lbHighestEduRemarkText" runat="server" Text="Label"></asp:Label>

                        </td>

                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="lbTextSpokenLanguage" runat="server" Text="Spoken Language" Font-Bold="true"></asp:Label>
                        </td>
                        <td colspan="3">
                            <asp:Label ID="lblSL" runat="server" Text=""></asp:Label>
                        </td>


                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="lbTextWritternLang" runat="server" Text="Written Language" Font-Bold="True"></asp:Label>
                        </td>
                        <td colspan="3">

                            <asp:Label ID="lblWL" runat="server" Text=""></asp:Label>
                        </td>

                    </tr>

                    <tr>
                        <td colspan="4">
                            <asp:Label ID="lbWhere" runat="server" Text="Where do you get to know us?" Font-Bold="true"></asp:Label>

                            <br />
                            <b>Get To Know Channel:</b>
                            <asp:Label ID="lblGTKL" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>

            <div class="row text-center">
                <h4>
                    <asp:Label ID="Label2" runat="server" Text="Applicant Current Employment Details"></asp:Label>
                </h4>
            </div>

            <asp:Panel ID="pnCurrEmploymentHistory" Enabled="false" runat="server" Visible="false">

                <div class="table-responsive">

                    <table class="table table-borderless small">
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
                                <asp:Label ID="lb5" runat="server" Font-Bold="true" Text="Position"></asp:Label>
                                <asp:Label ID="lblCE" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lb9" runat="server" Font-Bold="true" Text="Start Date"></asp:Label>
                                <asp:Label ID="lbCurrStartDate" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>

            <div class="row text-center">
                <div class="col-lg-12">

                    <h4>
                        <asp:Label ID="lb2" runat="server" Text="Programme Details"></asp:Label>
                    </h4>

                </div>
            </div>


            <div class="table-responsive">

                <table class="table table-borderless small">
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

            <div class="row text-center small">
                <div class="col-lg-12">

                    <h4>
                        <asp:Label ID="Label1" runat="server" Text="Declaration"></asp:Label>
                    </h4>

                </div>
            </div>
            <p class="small">
                Have you worked in the F&B industry before?
            <br />
                你有曾在食饮业工作吗？
            <br />
                <asp:CheckBox ID="CheckBox1" runat="server" Text="Yes" />
                <asp:CheckBox ID="CheckBox4" runat="server" Text="No" />
                <br />
                <br />
                Have you taken any food hygiene related courses before?
            <br />
                你有曾上过有关餐饮安全与卫生的课程吗？
             <br />
                <asp:CheckBox ID="CheckBox3" runat="server" Text="Yes" />

                <asp:CheckBox ID="CheckBox5" runat="server" Text="No" />


                <br />

                <br />
                <span class="auto-style11"><strong>WorkFare Training Subsidy
                </strong></span>
                <br />
                I declare that I am *eligible / not eligible  for WTS funding at the point of course registration. *please delete one
            <br />
                我申报我在注册时 符合资格/ 不符合资格 的WTS资助 *请删除一个 
            <br />

                <br />
                <asp:CheckBox ID="CheckBox2" runat="server" />
                I declare that all of the information provided by me in this application form is true and correct. I understand that any false statement(s) and/or misrepresentation(s) is/are sufficient ground(s) 
            for the rejection of my application. 
            <br />
                我声明，在这份表格里所提供的一切资料是正确与如实的。我明白任何有误导性或不真实的资料足以导致申请被拒。

            <br />

                <br />
                <asp:CheckBox ID="cbAgreement1" runat="server" />
                I declare that all of the information provided by me in this application form is true and correct. I understand that any false statement(s) and/or misrepresentation(s) is/are sufficient ground(s) 
            for the rejection of my application. 
            <br />
                我声明，在这份表格里所提供的一切资料是正确与如实的。我明白任何有误导性或不真实的资料足以导致申请被拒。

            <br />
                <br />
                <asp:CheckBox ID="cbAgreement2" runat="server" />
                I understand that a culinary or related course, which is a skill-based training programme conducted in a kitchen environment, 
            is physically and mentally demanding. I further declare that I am physically and mentally fit to undertake the training programme I apply, 
            and I will not hold ACI or its officers, trainers responsible for any physical discomfort or injuries which may occur from attending the programme. 
            <br />
                我理解在厨房学习烹饪的课程里， 需要一个健 康的身体和精神上的要求。我声明我的身心健康。 如果我在课程中受伤或觉得身体不舒服，不会责怪新加坡亚洲烹饪学院（ACI）的人员和导师。
            <br />
                <br />
                <asp:CheckBox ID="cbAgreement3" runat="server" />
                I also declare that prior to this; I have not enrolled in the course/module(s) indicated above. Otherwise, I will bear the full course/module(s) 
            fees without any subsidy. 
            <br />
                我声明，在参加这个课程之前，我没上过以上 所申请的课程，否则愿意完全付没有任何折扣的课程价钱。
            <br />
                <br />
                <asp:CheckBox ID="cbAgreement5" runat="server" />
                I understand that ACI reserves the right not to accept my application and may cancel or change the class schedule at short notice.
            <br />
                我理解新加坡亚洲烹饪学院 (ACI)有权利不接受我的申请，并可能在短时间内取消或更改上课时间。
            </p>
            <br />
            <br />

            <div class="row text-center small">
                <div class="col-lg-12">

                    <h4>
                        <asp:Label ID="Label3" runat="server" Text="Refund Policy"></asp:Label>
                    </h4>

                </div>
            </div>

            <div class="table-responsive small">

                <table border="1">
                    <tr>
                        <td>Refund Policy
                        </td>
                        <td>WSQ Programme (<u>Except WSQ Basic Food Hygiene course</u>)
                        </td>
                        <td>Non – WSQ Programme
                        </td>
                    </tr>

                    <tr>
                        <td>Withdrawal notification received at least 14 calendar days before course commencement
                        </td>
                        <td><100% refund of paid fees</td>
                        <td>75% refund of paid fees</td>
                    </tr>

                    <tr>
                        <td>Withdrawal notification received less than 14 calendar days from the course commencement
                        </td>
                        <td>50% refund of paid fees
                        </td>
                        <td>No refund of paid fees
                        </td>
                    </tr>

                    <tr>
                        <td>Withdrawal notification received upon course commencement</td>
                        <td colspan="2">No refund of paid fees
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <div class="table-responsive">

                <table class="table table-borderless small">
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server"><br/><hr/></asp:Label>
                            <br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        Name of Applicant&nbsp;/ 申请人姓名
                        </td>

                        <td>
                            <asp:Label ID="Label6" runat="server" Visible="false"></asp:Label>
                            <asp:Image ID="imgSig" runat="server" Visible="false" />
                            <br />
                            <hr />
                            <br />

                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Signature
                            / 申请人签名 
                        </td>

                        <td>
                            <asp:Label ID="Label5" runat="server"><br/><hr/></asp:Label>
                            <br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Date / 日期 </td>
                    </tr>
                </table>
            </div>
            <div class="row text-left small">
                <div class="col-lg-12">

                    <h4>
                        <asp:Label ID="Label7" runat="server" Text="Official Use" Font-Underline="True"></asp:Label>
                    </h4>
                    <asp:Label ID="Label8" runat="server" Text="Received By,"></asp:Label>

                </div>
            </div>
            <br />

            <div class="table-responsive">

                <table class="table table-borderless small">
                    <tr>
                        <td>
                            <asp:Label ID="Label9" runat="server"><br/><hr/></asp:Label>
                            <br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        Name 
                        </td>

                        <td>
                            <asp:Label ID="Label10" runat="server"><br/><hr/></asp:Label>
                            <br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Signature
                                    
                        </td>

                        <td>
                            <asp:Label ID="Label11" runat="server"><br/><hr/></asp:Label>
                            <br />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Date
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>


