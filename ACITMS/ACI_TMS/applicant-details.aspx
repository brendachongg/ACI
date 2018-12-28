<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="applicant-details.aspx.cs" Inherits="ACI_TMS.applicant_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>

        function checkFirstContact(oSrc, args) {
            var patt = new RegExp("[8||9][0-9]{7}");
            var res = patt.test($('#<%=tbContact1.ClientID%>').val());

            if (res == false) {
                args.IsValid = false;
                return false;
            }
            else {
                args.isValid = true;
                return true;
            }
        }

        function validateContactEmail(oSrc, args) {

            if ($('#<%=tbContact1.ClientID%>').val().length == 0 && $('#<%=tbEmail.ClientID%>').val().length == 0) {
                args.IsValid = false;
                return false;
            } else {

                args.IsValid = true;
                return true;
            }
        }

        function showPrev() {
            if ($('#<%=cbPrevEmpl.ClientID%>').is(":checked")) {
                $('#divPrevEmpl').css("display", "block");
            }
        }

        function showCurrEmpl() {
            if ($('#<%=cbCurrEmpl.ClientID%>').is(":checked")) {
                $('#divCurrEmpl').css("display", "block");
            } else {

                if ($('#<%=ddlSponsorship.ClientID%> option:selected').val() == "<%=GeneralLayer.Sponsorship.COMP.ToString()%>") {

                    $('#lblError').html("Current employment must be filled in when it is company sponsored.");
                    $('#Div1').css("display", "block");
                    $('#<%=cbCurrEmpl.ClientID%>').prop('checked', true);
                } else
                    $('#divCurrEmpl').css("display", "none");
            }
        }

        function showPrevEmpl() {
            if ($('#<%=cbPrevEmpl.ClientID%>').is(":checked")) {
                $('#divPrevEmpl').css("display", "block");
            } else {
                $('#divPrevEmpl').css("display", "none");
            }
        }


        function validateSalary(oSrc, args) {
            if (!/^\d+(\.\d{1,2})?$/g.test(args.Value)) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validateCurrEmplSalary(oSrc, args) {
            if (!$('#<%=cbCurrEmpl.ClientID%>').is(":checked")) {
                args.IsValid = true;
                return true;
            }

            return validateSalary(oSrc, args);
        }

        function validatePrevEmplSalary(oSrc, args) {
            if (!$('#<%=cbPrevEmpl.ClientID%>').is(":checked")) {
                args.IsValid = true;
                return true;
            }

            return validateSalary(oSrc, args);
        }

        function validateEmplDates(oSrc, args) {
            if (!$('#<%=cbPrevEmpl.ClientID%>').is(":checked") || !$('#<%=cbCurrEmpl.ClientID%>').is(":checked")) {
                args.IsValid = true;
                return true;
            }

            var currdt = new Date($('#<%=tbCurrEmplStartDt.ClientID%>').val());
            var prevdt = new Date($('#<%=tbPrevEmplEndDt.ClientID%>').val());

            if (prevdt > currdt) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validatePrevEmplDate(oSrc, args) {
            if (!$('#<%=cbPrevEmpl.ClientID%>').is(":checked")) {
                args.IsValid = true;
                return true;
            }

            var edtStr = $('#<%=tbPrevEmplEndDt.ClientID%>').val();
            var sdtStr = $('#<%=tbPrevEmplStartDt.ClientID%>').val();

            if (sdtStr == "" || edtStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(sdtStr) || !isValidDate(edtStr)) {
                args.IsValid = false;
                return false;
            }

            var sdt = new Date(sdtStr);
            var edt = new Date(edtStr);
            var today = new Date();

            if (edt < sdt) {
                args.IsValid = false;
                return false;
            }

            if (today < sdt || today < edt) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validateCurrEmplStartDt(oSrc, args) {
            if (!$('#<%=cbCurrEmpl.ClientID%>').is(":checked")) {
                args.IsValid = true;
                return true;
            }

            return validatePastDate(oSrc, args);
        }

        function validatePastDate(oSrc, args) {
            var dtStr = args.Value;

            if (dtStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(dtStr)) {
                args.IsValid = false;
                return false;
            }

            //DOB cannot be later than current date
            var today = new Date();
            var dt = new Date(dtStr);

            if (today < dt) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validateInterviewFQNeeded(oSrc, args) {
            if ($('#<%=hfCourseType.ClientID%>').val() == '<%=GeneralLayer.ProgrammeType.FQ.ToString() %>') {
                if ($('#<%=ddlInterviewStatus.ClientID%> option:selected').val() == '<%=GeneralLayer.InterviewStatus.NREQ.ToString() %>') {
                    args.IsValid = false;
                    return false;
                } else {
                    args.IsValid = true;
                    return true;
                }
            }
        }

        function validateInterviewFQ(oSrc, args) {
            if ($('#<%=hfCourseType.ClientID%>').val() == '<%=GeneralLayer.ProgrammeType.FQ.ToString() %>') {
                if ($('#<%=ddlInterviewStatus.ClientID%> option:selected').val() == '<%=GeneralLayer.InterviewStatus.FAILED.ToString() %>' || $('#<%=ddlInterviewStatus.ClientID%> option:selected').val() == '<%=GeneralLayer.InterviewStatus.PASSED.ToString() %>') {
                    if ($('#<%=ddlInterviewer.ClientID%> option:selected').val().length == 0 || $('#<%=tbInterviewDate.ClientID%>').val().length == 0) {
                        args.IsValid = false;
                        return false;
                    } else {
                        args.IsValid = true;
                        return true;
                    }
                }
            }
        }


        function validateInterviewSQ(oSrc, args) {

            if ($('#<%=hfCourseType.ClientID%>').val() != '<%=GeneralLayer.ProgrammeType.FQ.ToString() %>') {
                if ($('#<%=ddlInterviewStatus.ClientID%> option:selected').val() == '<%=GeneralLayer.InterviewStatus.NREQ.ToString() %>') {
                    args.IsValid = true;
                    return true;
                } else {
                    if ($('#<%=ddlInterviewer.ClientID%> option:selected').val().length == 0 || $('#<%=tbInterviewDate.ClientID%>').val().length == 0) {
                        args.IsValid = false;
                        return false;
                    } else {
                        args.IsValid = true;
                        return true;
                    }

                }
            }
<%--            if ($('#<%=ddlInterviewStatus.ClientID%> option:selected').val() == '<%=GeneralLayer.InterviewStatus.NREQ.ToString() %>') {
                if ($('#<%=hfCourseType.ClientID%>').val() == '<%=GeneralLayer.ProgrammeType.FQ.ToString() %>') {
                    if ($('#<%=ddlInterviewer.ClientID%> option:selected').val().length == 0 || $('#<%=tbInterviewDate.ClientID%>').val().length == 0) {
                        args.IsValid = false;
                        return false;
                    }
                } else {
                    args.IsValid = true;
                    return true;
                }
            } else if ($('#<%=ddlInterviewStatus.ClientID%> option:selected').val() == '<%=GeneralLayer.InterviewStatus.PD.ToString() %>' || $('#<%=ddlInterviewStatus.ClientID%> option:selected').val() == '<%=GeneralLayer.InterviewStatus.NYD.ToString() %>') {
                args.IsValid = true;
                return true;
            }

            if ($('#<%=ddlInterviewer.ClientID%> option:selected').val().length == 0 || $('#<%=tbInterviewDate.ClientID%>').val().length == 0) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;--%>
        }

        function validateCurrEmpl(oSrc, args) {
            if (!$('#<%=cbCurrEmpl.ClientID%>').is(":checked")) {
                args.IsValid = true;
                return true;
            }

            if ($('#<%=tbCurrCoName.ClientID%>').val().length == 0 || $('#<%=tbCurrEmplDept.ClientID%>').val().length == 0
                || $('#<%=tbCurrEmplDesignation.ClientID%>').val().length == 0 || $('#<%=tbCurrEmplSalary.ClientID%>').val().length == 0
                || $('#<%=tbCurrEmplStartDt.ClientID%>').val().length == 0 || $('#<%=ddlCurrEmplStatus.ClientID%> option:selected').val().length == 0
                || $('#<%=ddlCurrEmplOccupation.ClientID%> option:selected').val().length == 0) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validatePrevEmpl(oSrc, args) {
            if (!$('#<%=cbPrevEmpl.ClientID%>').is(":checked")) {
                args.IsValid = true;
                return true;
            }

            if ($('#<%=tbPrevCoName.ClientID%>').val().length == 0 || $('#<%=tbPrevEmplDept.ClientID%>').val().length == 0
                || $('#<%=tbPrevEmplDesignation.ClientID%>').val().length == 0 || $('#<%=tbPrevEmplSalary.ClientID%>').val().length == 0
                || $('#<%=tbPrevEmplStartDt.ClientID%>').val().length == 0 || $('#<%=ddlPrevEmplStatus.ClientID%> option:selected').val().length == 0
                || $('#<%=ddlPrevEmplOccupation.ClientID%> option:selected').val().length == 0 || $('#<%=tbPrevEmplEndDt.ClientID%>').val().length == 0) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }


        function validateId(oSrc, args) {
            if ($('#<%=ddlIDType.ClientID%> option:selected').val() == '<%=(int)GeneralLayer.IDType.Oth%>') {
                args.IsValid = true;
                return true;
            }

            var ic = args.Value.toUpperCase();

            if (ic.length != 9) {
                args.IsValid = false;
                return false;
            }

            var icArray = new Array(9);
            for (i = 0; i < 9; i++) {
                icArray[i] = ic.charAt(i);
            }
            icArray[1] *= 2;
            icArray[2] *= 7;
            icArray[3] *= 6;
            icArray[4] *= 5;
            icArray[5] *= 4;
            icArray[6] *= 3;
            icArray[7] *= 2;
            var weight = 0;
            for (i = 1; i < 8; i++) {
                weight += parseInt(icArray[i]);
            }
            var offset = (icArray[0] == "T" || icArray[0] == "G") ? 4 : 0;
            var temp = (offset + weight) % 11;
            var st = Array("J", "Z", "I", "H", "G", "F", "E", "D", "C", "B", "A");
            var fg = Array("X", "W", "U", "T", "R", "Q", "P", "N", "M", "L", "K");
            var theAlpha;
            if (icArray[0] == "S" || icArray[0] == "T") {
                theAlpha = st[temp];
            } else if (icArray[0] == "F" || icArray[0] == "G") {
                theAlpha = fg[temp];
            }

            if (icArray[8] != theAlpha) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validateWrittenOtherLang(oSrc, args) {
            if ($('#<%=ddlOtherLanguageWritten.ClientID%> option:selected').val() != "") {
                if ($('#<%=ddlOtherLanguageProWritten.ClientID%> option:selected').val() == "") {
                    args.IsValid = false;
                    return false;
                } else {
                    args.IsValid = true;
                    return true;
                }
            }
            args.IsValid = true;
            return true;
        }

        function validateSpokenOtherLang(oSrc, args) {
            if ($('#<%=ddlOtherLanguageSpoken.ClientID%> option:selected').val() != "") {
                if ($('#<%=ddlOtherLanguageProSpoken.ClientID%> option:selected').val() == "") {
                    args.IsValid = false;
                    return false;
                } else {
                    args.IsValid = true;
                    return true;
                }
            }
            args.IsValid = true;
            return true;
        }

        function checkBeforeEnroll(oSrc, args) {
            errorMsg = "";
            errorCount = 0;


            if ($('#<%=tbFullName.ClientID%>').val() == '' || $('#<%=ddlIDType.ClientID%> option:selected').val().length == 0
                || $('#<%=tbId.ClientID%>').val() == '' || $('#<%=ddlNationality.ClientID%> option:selected').val().length == 0 ||
                $('#<%=ddlRace.ClientID%> option:selected').val().length == 0 || $('#<%=ddlGender.ClientID%> option:selected').val().length == 0
                || $('#<%=tbContact1.ClientID%>').val() == '' || $('#<%=tbDOB.ClientID%>').val() == ''
                || $('#<%=tbAddr.ClientID%>').val() == '' || $('#<%=tbPostalCode.ClientID%>').val() == ''
                || $('#<%=ddlHighEdu.ClientID%> option:selected').val().length == 0 || $('#<%=ddlEngSpoken.ClientID%> option:selected').val().length == 0
                || $('#<%=ddlChnSpoken.ClientID%> option:selected').val().length == 0 || $('#<%=ddlEngWritten.ClientID%> option:selected').val().length == 0
                || $('#<%=ddlChnWritten.ClientID%> option:selected').val().length == 0) {
                errorCount++;
                errorMsg += "Please check applicant details for empty fields. <br>";

            }

            if ($('#<%=tbId.ClientID%>').val() != '') {

                if ($('#<%=ddlIDType.ClientID%> option:selected').val() != '<%=(int)GeneralLayer.IDType.Oth%>') {
                    var ic = $('#<%=tbId.ClientID%>').val()

                    if (ic.length != 9) {
                        args.IsValid = false;
                        return false;
                    }

                    var icArray = new Array(9);
                    for (i = 0; i < 9; i++) {
                        icArray[i] = ic.charAt(i);
                    }
                    icArray[1] *= 2;
                    icArray[2] *= 7;
                    icArray[3] *= 6;
                    icArray[4] *= 5;
                    icArray[5] *= 4;
                    icArray[6] *= 3;
                    icArray[7] *= 2;
                    var weight = 0;
                    for (i = 1; i < 8; i++) {
                        weight += parseInt(icArray[i]);
                    }
                    var offset = (icArray[0] == "T" || icArray[0] == "G") ? 4 : 0;
                    var temp = (offset + weight) % 11;
                    var st = Array("J", "Z", "I", "H", "G", "F", "E", "D", "C", "B", "A");
                    var fg = Array("X", "W", "U", "T", "R", "Q", "P", "N", "M", "L", "K");
                    var theAlpha;
                    if (icArray[0] == "S" || icArray[0] == "T") {
                        theAlpha = st[temp];
                    } else if (icArray[0] == "F" || icArray[0] == "G") {
                        theAlpha = fg[temp];
                    }

                    if (icArray[8] != theAlpha) {
                        errorCount++;
                        errorMsg += "Please check applicant's Identification No. <br>";
                    }
                }

            }

            if ($('#<%=tbContact1.ClientID%>').val() != '') {
                var isnum = /^\d+$/.test($('#<%=tbContact1.ClientID%>').val());

                if (isnum == false) {
                    errorCount++;
                    errorMsg += "Please check applicant's Contact No 1. <br>";
                }
            }

            if ($('#<%=tbContact2.ClientID%>').val() != '') {
                var isnum = /^\d+$/.test($('#<%=tbContact2.ClientID%>').val());

                if (isnum == false) {
                    errorCount++;
                    errorMsg += "Please check applicant's Contact No 2. <br>";
                }
            }


            if ($('#<%=ddlSponsorship.ClientID%> option:selected').val() == "<%=GeneralLayer.Sponsorship.COMP.ToString()%>") {
                if ($('#<%=tbCurrCoName.ClientID%>').val().length == 0 || $('#<%=tbCurrEmplDept.ClientID%>').val().length == 0
               || $('#<%=tbCurrEmplDesignation.ClientID%>').val().length == 0 || $('#<%=tbCurrEmplSalary.ClientID%>').val().length == 0
                || $('#<%=tbCurrEmplStartDt.ClientID%>').val().length == 0 || $('#<%=ddlCurrEmplStatus.ClientID%> option:selected').val().length == 0
                || $('#<%=ddlCurrEmplOccupation.ClientID%> option:selected').val().length == 0) {
                    errorMsg += "Applicant is company sponsored. Current Employment is required. <br>";
                    errorCount++;
                }
            }

            if ($('#<%=hfCourseType.ClientID%>').val() == "<%=GeneralLayer.ProgrammeType.FQ.ToString()%>") {
                if ($('#<%=tbInterviewDate.ClientID%>').val().length == 0 || $('#<%=ddlInterviewStatus.ClientID%>').val().length == 0
                     || $('#<%=ddlInterviewer.ClientID%>').val().length == 0) {
                    errorMsg += "Please enter the interview details. <br>";
                    errorCount++;
                }
            }

            if (errorCount <= 0) {
                args.IsValid = true;
                return true;
            } else {
                $('#lblError').html(errorMsg);
                $('#Div1').css("display", "block");

                args.IsValid = false;
                return false;
            }
        }



    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                <h3>
                    <asp:Label ID="lbApplicationHeader" runat="server" Text="Application"></asp:Label>
                </h3>

                <small>

                    <asp:Label ID="lbApplicantHeader" runat="server" Text="Application ID: "> </asp:Label>
                    <asp:Label ID="lbApplicantId" runat="server" Text=""></asp:Label>

                    <br />

                    <asp:Label ID="lbSubmittedDateText" runat="server" Text="This application was submitted on "></asp:Label>
                    <asp:Label ID="lbApplicationSubmittedDate" runat="server" Text=""></asp:Label>
                </small>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" CausesValidation="false" OnClick="lbtnBack_Click"></asp:LinkButton>
                <asp:LinkButton ID="btn_print" runat="server" CssClass="btn btn-sm btn-success" OnClick="btn_print_Click" CausesValidation="false"><i class="fa fa-print"></i>Print</asp:LinkButton>
                <asp:LinkButton ID="lbtnSendPaymentEmail" runat="server" class="btn btn-sm  btn-primary" Visible="false" CausesValidation="false" OnClick="lbtnSendPaymentEmail_Click"><i class="fa fa-envelope-open"></i>Send Payment Email</asp:LinkButton>
                <asp:LinkButton ID="btnRejectAppcalicantTop" runat="server" class="btn btn-sm btn-danger" OnClick="btnRejectAppcalicant_Click"><i class="fa fa-times"></i>Reject</asp:LinkButton>
                <asp:LinkButton ID="lkbtnEnrollApplicantTop" CssClass="btn btn-sm btn-info" runat="server" OnClick="lkbtnEnrollApplicant_Click" ValidationGroup="enrollment"><i class="fa fa-sign-in"></i>Enroll</asp:LinkButton>

                <%--   <asp:CustomValidator ID="cvEnrollment" runat="server" Display="None" ValidationGroup="enrollment" ClientValidationFunction="checkBeforeEnroll"
                    ValidateEmptyText="false"></asp:CustomValidator>--%>
            </div>

        </div>

        <hr />

        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link">Update successful</asp:Label>
        </div>
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblErrorMsg" runat="server" CssClass="alert-link">Update unsuccessful</asp:Label>

        </div>

        <div class="alert alert-warning" id="panelWarning" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lbWarningMsg" runat="server" CssClass="alert-link"></asp:Label>

        </div>



        <div class="alert alert-danger" id="Div1" style="display: none">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <label id="lblError" class="alert-link"></label>

        </div>

        <asp:ValidationSummary ID="vSummary1" ValidationGroup="personalparticulars" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <asp:ValidationSummary ID="vSummary2" ValidationGroup="employment" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <asp:ValidationSummary ID="vSummary3" ValidationGroup="interview" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />


        <asp:Panel ID="pnApplicant" runat="server" Enabled="true">
            <div class="row">
                <div class="text-left">

                    <h3>
                        <asp:Label ID="lbParticular" runat="server" Text="Personal Particulars"> </asp:Label>
                    </h3>
                </div>

            </div>
            <br />


            <div class="row">
                <div class="col-lg-12">
                    <asp:Label ID="lbParticularsInfo" runat="server" Text="Personal Particulars" CssClass="alert alert-info" Visible="false"> </asp:Label>
                </div>
            </div>
            <br />

            <asp:Panel ID="panelParticular" Enabled="true" runat="server">

                <div class="row">
                    <div class="col-lg-12">
                        <asp:Label ID="lbFullName" runat="server" Text="Full Name" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbFullName" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvFullName" runat="server" ErrorMessage="Applicant's Name cannot be empty" Display="None" ValidationGroup="personalparticulars" ControlToValidate="tbFullName"></asp:RequiredFieldValidator>
                    </div>
                </div>

                <br />

                <div class="row">
                    <div class="col-lg-6">
                        <asp:Label ID="lbIdentificationType" runat="server" Text="Identification Type" Font-Bold="True"></asp:Label>
                        <asp:DropDownList ID="ddlIDType" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue" CssClass="form-control">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvIdType" ControlToValidate="ddlIDType" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Identification Type. cannot be empty." Display="None"></asp:RequiredFieldValidator>

                    </div>
                    <div class="col-lg-6">
                        <asp:Label ID="lbIdentification" runat="server" Text="Identification No." Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbId" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvId" ControlToValidate="tbId" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Identification No. cannot be empty." Display="None"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvId" runat="server" ControlToValidate="tbId" Display="None" ValidationGroup="personalparticulars"
                            ErrorMessage="Invalid identification no." ClientValidationFunction="validateId" ValidateEmptyText="false" OnServerValidate="IsNRICValid"></asp:CustomValidator>
                    </div>
                </div>

                <br />

                <div class="row">
                    <div class="col-lg-4">
                        <asp:Label ID="lbNationality" runat="server" Text="Nationality" Font-Bold="True"></asp:Label>
                        <asp:DropDownList ID="ddlNationality" DataTextField="codeValueDisplay" DataValueField="codeValue" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvNationality" ControlToValidate="ddlNationality" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Nationality cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-lg-4">
                        <asp:Label ID="lbRace" runat="server" Text="Race" Font-Bold="True"></asp:Label>
                        <asp:DropDownList ID="ddlRace" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue" runat="server">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvRace" ControlToValidate="ddlRace" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Race cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    </div>

                    <div class="col-lg-4">
                        <asp:Label ID="lbGender" runat="server" Text="Gender" Font-Bold="True"></asp:Label>
                        <asp:DropDownList ID="ddlGender" CssClass="form-control" runat="server">
                            <asp:ListItem Value="">--Select--</asp:ListItem>
                            <asp:ListItem Value="M">Male</asp:ListItem>
                            <asp:ListItem Value="F">Female</asp:ListItem>
                        </asp:DropDownList>

                        <asp:RequiredFieldValidator ID="rfvGender" ControlToValidate="ddlGender" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Gender cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    </div>
                </div>

                <br />

                <div class="row">
                    <div class="col-lg-6">
                        <asp:Label ID="lbContact1" runat="server" Text="Contact Number 1 (Mobile)" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbContact1" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:CustomValidator ID="cvValidContactNo1" ErrorMessage="Contact Number 1 has to start with either 8 or 9." ValidateEmptyText="false" runat="server" ControlToValidate="tbContact1" Display="None" ValidationGroup="personalparticulars" ClientValidationFunction="checkFirstContact"></asp:CustomValidator>
                        <asp:CustomValidator ID="cvContactNo1" runat="server" ControlToValidate="tbContact1" Display="None" ValidationGroup="personalparticulars" ClientValidationFunction="validateContactEmail"
                            ErrorMessage="Either Contact Number 1 or Email must be provided." ValidateEmptyText="True" OnServerValidate="isContactEmailBothEmpty"></asp:CustomValidator>
                        <%--<asp:RequiredFieldValidator ID="rfvContact1" ControlToValidate="tbContact1" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Contact no. cannot be empty." Display="None"></asp:RequiredFieldValidator>--%>
                        <asp:RegularExpressionValidator ID="revContact1" Display="None" ValidationExpression="^\+?\d+$" runat="server" ErrorMessage="Contact number can only contain numbers."
                            ControlToValidate="tbContact1" ValidationGroup="personalparticulars"></asp:RegularExpressionValidator>
                    </div>
                    <div class="col-lg-6">
                        <asp:Label ID="lbContact2" runat="server" Text="Contact Number 2 (Home/Others)" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbContact2" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="revContact2" Display="None" ValidationExpression="^\+?\d+$" runat="server" ErrorMessage="Alternative contact no. can only contain numbers."
                            ControlToValidate="tbContact2" ValidationGroup="personalparticulars"></asp:RegularExpressionValidator>
                    </div>
                </div>

                <br />

                <div class="row">
                    <div class="col-lg-4">
                        <asp:Label ID="lbBirthDate" runat="server" Text="Date of Birth" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbDOB" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvDOB" ControlToValidate="tbDOB" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Date of birth cannot be empty." Display="None"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvBirthDate" runat="server" ControlToValidate="tbDOB" Display="None" ValidationGroup="personalparticulars" ClientValidationFunction="validatePastDate"
                            ErrorMessage="Date of birth is not in dd MMM yyyy format OR cannot be later than today." ValidateEmptyText="false"></asp:CustomValidator>
                    </div>
                    <div class="col-lg-8">
                        <asp:Label ID="lbEmailAddress" runat="server" Text="Email Address" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbEmail" runat="server" CssClass="form-control"></asp:TextBox>


                        <asp:RegularExpressionValidator ID="rexEmail" Display="None" runat="server" ErrorMessage="Invalid email." ControlToValidate="tbEmail" ValidationGroup="personalparticulars" ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"></asp:RegularExpressionValidator>
                    </div>
                </div>

                <br />

                <div class="row">
                    <div class="col-lg-6">
                        <asp:Label ID="lbAddressLine1" runat="server" Text="Address Line" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbAddr" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvAddr" ControlToValidate="tbAddr" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Address cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-lg-6">
                        <asp:Label ID="lbPostalCode" runat="server" Text="Postal Code" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbPostalCode" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPostalCode" ControlToValidate="tbPostalCode" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Postal Code cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    </div>
                </div>

                <br />

                <div class="row">
                    <div class="col-lg-6">
                        <asp:Label ID="lbHighestEducation" runat="server" Text="Highest Education" Font-Bold="True"></asp:Label>
                        <asp:DropDownList ID="ddlHighEdu" CssClass="form-control" runat="server"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvHighEdu" ControlToValidate="ddlHighEdu" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Highest level of education cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-lg-6">
                        <asp:Label ID="lbHighestEducationRemark" runat="server" Text="Highest Education Remarks" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbHighestEduRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>
                </div>

                <br />

                <div class="row">
                    <div class="text-left">
                        <h4>
                            <asp:Label ID="lbTextSpokenLanguage" runat="server" Text="Spoken Language" Font-Bold="True"></asp:Label></h4>
                    </div>
                </div>

                <br />

                <div class="row">
                    <div class="col-lg-6">

                        <asp:Label ID="lbEng" runat="server" CssClass="form-control" Text="English"></asp:Label>
                        <br />
                        <asp:Label ID="lblChi" runat="server" CssClass="form-control" Text="Chinese"></asp:Label>
                        <br />
                        <asp:DropDownList ID="ddlOtherLanguageSpoken" CssClass="form-control" runat="server"></asp:DropDownList>

                    </div>
                    <div class="col-lg-6">
                        <asp:DropDownList ID="ddlEngSpoken" CssClass="form-control" runat="server"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvEngSpoken" ControlToValidate="ddlEngSpoken" ValidationGroup="personalparticulars" runat="server" ErrorMessage="English spoken proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                        <br />
                        <asp:DropDownList ID="ddlChnSpoken" CssClass="form-control" runat="server"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvChnSpoken" ControlToValidate="ddlChnSpoken" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Chinese spoken proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                        <br />
                        <asp:DropDownList ID="ddlOtherLanguageProSpoken" CssClass="form-control" runat="server"></asp:DropDownList>
                        <asp:CustomValidator ID="cvOtherSpokenLanguage" runat="server" ControlToValidate="ddlOtherLanguageSpoken" Display="None" ValidationGroup="personalparticulars" ClientValidationFunction="validateSpokenOtherLang"
                            ErrorMessage="Other Spoken Language Proficiency cannot be empty" ValidateEmptyText="false"></asp:CustomValidator>


                    </div>
                </div>
                <br />

                <div class="row">
                    <div class="text-left">
                        <h4>
                            <asp:Label ID="lbTextWritternLang" runat="server" Text="Written Language" Font-Bold="True"></asp:Label>
                        </h4>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-lg-6">
                        <asp:Label ID="lbWEng" runat="server" CssClass="form-control" Text="English"></asp:Label>
                        <br />
                        <asp:Label ID="lbWChi" runat="server" CssClass="form-control" Text="Chinese"></asp:Label>
                        <br />
                        <asp:DropDownList ID="ddlOtherLanguageWritten" CssClass="form-control" runat="server">
                        </asp:DropDownList>
                    </div>
                    <div class="col-lg-6">
                        <asp:DropDownList ID="ddlEngWritten" CssClass="form-control" runat="server"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvEngWriteen" ControlToValidate="ddlEngWritten" ValidationGroup="personalparticulars" runat="server" ErrorMessage="English written proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                        <br />
                        <asp:DropDownList ID="ddlChnWritten" CssClass="form-control" runat="server"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvChnWritten" ControlToValidate="ddlChnWritten" ValidationGroup="personalparticulars" runat="server" ErrorMessage="Chinese written proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                        <br />
                        <asp:DropDownList ID="ddlOtherLanguageProWritten" CssClass="form-control" runat="server">
                        </asp:DropDownList>
                        <asp:CustomValidator ID="cvOtherLangWritten" runat="server" ControlToValidate="ddlOtherLanguageWritten" Display="None" ValidationGroup="personalparticulars" ClientValidationFunction="validateWrittenOtherLang"
                            ErrorMessage="Other Written Language Proficiency cannot be empty" ValidateEmptyText="false"></asp:CustomValidator>
                    </div>
                </div>

                <br />

                <div class="row">
                    <div class="col-lg-12">
                        <asp:Label ID="lbGetToKnowChannel" runat="server" Text="Get To Know Channel" Font-Bold="True"></asp:Label>
                        <asp:CheckBoxList ID="cblGetToKnowChannel" CssClass="col-lg-12 control-label" runat="server" RepeatDirection="Horizontal" RepeatColumns="2"></asp:CheckBoxList>
                    </div>

                </div>



                <br />

                <div class="row">
                    <div class="col-lg-6">
                        <asp:Label ID="lbSponsorship" runat="server" Text="Sponsorship" Font-Bold="True"></asp:Label>
                        <asp:DropDownList ID="ddlSponsorship" CssClass="form-control" DataTextField="codeValueDisplay" AutoPostBack="true" DataValueField="codeValue" runat="server"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvSponsorShip" ControlToValidate="ddlSponsorship" ValidationGroup="personalparticulars" runat="server" ErrorMessage="English written proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col-lg-6">
                        <asp:Label ID="Label6" runat="server" Text="Sponsored By" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbSponsorshipCompany" CssClass="form-control" Enabled="false" runat="server" Text=""></asp:TextBox>
                    </div>

                </div>

                <br />

                <div class="row">
                    <div class="col-lg-12">
                        <asp:Label ID="lbRemarks" runat="server" Text="Applicant Remarks" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="tbApplicantRemarks" CssClass="form-control" Enabled="true" runat="server" Text=""></asp:TextBox>
                    </div>

                </div>

                <br />

                <div class="row">
                    <div class="btn-group pull-right">
                        <asp:Button ID="btnUpdateApplicantDetails" CssClass="btn btn-primary" OnClick="btnUpdateApplicantDetails_Click" ValidationGroup="personalparticulars" runat="server" Text="Update" />
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnPaymentMode" runat="server" Visible="false">
                <div class="row">
                    <div class="col-lg-12">
                        <asp:Label ID="labelPaymentMode" runat="server" Text="Preferred Payment Mode" Font-Bold="True"></asp:Label>
                        <asp:CheckBoxList ID="cbPreferredPaymentMode" Enabled="false" CssClass="col-lg-12 control-label" runat="server" RepeatDirection="Horizontal" RepeatColumns="2"></asp:CheckBoxList>
                    </div>

                </div>
            </asp:Panel>
            <asp:Panel ID="pnDocuments" runat="server" Visible="false">
                <div class="row">
                    <div class="text-left">
                        <h3>
                            <asp:Label ID="lbDocumentations" runat="server" Text="Applicants Documents"> </asp:Label>
                        </h3>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12">
                        <asp:HyperLink ID="hlIdentificationDocuments" Visible="true" runat="server"></asp:HyperLink><br />
                        <asp:HyperLink ID="hlWTS" Visible="true" runat="server"></asp:HyperLink>
                        <br />
                        <asp:HyperLink ID="hlCerts" Visible="true" runat="server"></asp:HyperLink>
                    </div>
                </div>

            </asp:Panel>



            <div class="row">
                <div class="text-left">
                    <h3>
                        <asp:Label ID="lbEmployment" runat="server" Text="Employment"> </asp:Label>
                    </h3>
                </div>

            </div>
            <div class="row">
                <div class="col-lg-12">


                    <p>
                        <asp:CheckBox ID="cbCurrEmpl" runat="server" Checked="true" onchange="showCurrEmpl()" />&nbsp;<label for="<%=cbCurrEmpl.ClientID %>">Current Employment</label>
                    </p>
                    <asp:HiddenField ID="hdfCurrEmpHisId" runat="server" />
                    <div id="divCurrEmpl">
                        <div class="form-group row">
                            <div class="col-lg-12">
                                <asp:Label ID="lb18" runat="server" Font-Bold="true" Text="Company Name"></asp:Label>
                                <asp:TextBox ID="tbCurrCoName" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-lg-6">
                                <asp:Label ID="lb19" runat="server" Font-Bold="true" Text="Department"></asp:Label>
                                <asp:TextBox ID="tbCurrEmplDept" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-lg-6">
                                <asp:Label ID="lb20" runat="server" Font-Bold="true" Text="Designation"></asp:Label>
                                <asp:TextBox ID="tbCurrEmplDesignation" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-lg-6">
                                <asp:Label ID="lb21" runat="server" Font-Bold="true" Text="Employment Type"></asp:Label>
                                <asp:DropDownList ID="ddlCurrEmplStatus" CssClass="form-control" runat="server" DataValueField="codeValue" DataTextField="codeValueDisplay">
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-6">
                                <asp:Label ID="lb22" runat="server" Font-Bold="true" Text="Designation Type"></asp:Label>
                                <asp:DropDownList ID="ddlCurrEmplOccupation" CssClass="form-control" runat="server" DataValueField="codeValue" DataTextField="codeValueDisplay">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-lg-6">
                                <asp:Label ID="lb23" runat="server" Font-Bold="true" Text="Salary (S$)"></asp:Label>
                                <asp:TextBox ID="tbCurrEmplSalary" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-lg-6">
                                <asp:Label ID="lb24" runat="server" Font-Bold="true" Text="Start Date"></asp:Label>
                                <asp:TextBox ID="tbCurrEmplStartDt" CssClass="form-control datepicker" runat="server" placeholder="dd MMM yyyy"></asp:TextBox>
                            </div>
                        </div>

                        <asp:CustomValidator ID="cvCurrEmpl" runat="server" ControlToValidate="tbCurrCoName" Display="None" ValidationGroup="employment"
                            ErrorMessage="All fields under current employment cannot be empty" ClientValidationFunction="validateCurrEmpl" ValidateEmptyText="true"></asp:CustomValidator>
                        <asp:CustomValidator ID="cvCurrEmplStartDt" runat="server" ControlToValidate="tbCurrEmplStartDt" Display="None" ValidationGroup="employment" ClientValidationFunction="validateCurrEmplStartDt"
                            ErrorMessage="Start date under current employment is not in dd MMM yyyy format OR cannot be later than today." ValidateEmptyText="false"></asp:CustomValidator>
                        <asp:CustomValidator ID="cvCurrEmplSalary" runat="server" ControlToValidate="tbCurrEmplSalary" Display="None" ValidationGroup="employment"
                            ErrorMessage="Invalid salary under current employment." ClientValidationFunction="validateCurrEmplSalary" ValidateEmptyText="true"></asp:CustomValidator>
                        <asp:CustomValidator ID="cvEmplDates" runat="server" ControlToValidate="tbCurrEmplStartDt" Display="None" ValidationGroup="employment" ClientValidationFunction="validateEmplDates"
                            ErrorMessage="End date of previous employment cannot be later than start date of current employment." ValidateEmptyText="false"></asp:CustomValidator>

                    </div>
                    <p>
                        <asp:CheckBox ID="cbPrevEmpl" runat="server" onchange="showPrevEmpl()" />&nbsp;<label for="<%=cbPrevEmpl.ClientID %>">Previous Employment</label>
                    </p>

                    <asp:HiddenField ID="hdfPrevEmpHist" runat="server" />
                    <div id="divPrevEmpl" style="display: none;">
                        <div class="form-group row">
                            <div class="col-lg-12">
                                <asp:Label ID="lb25" runat="server" Font-Bold="true" Text="Company Name"></asp:Label>
                                <asp:TextBox ID="tbPrevCoName" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-lg-6">
                                <asp:Label ID="lb26" runat="server" Font-Bold="true" Text="Department"></asp:Label>
                                <asp:TextBox ID="tbPrevEmplDept" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-lg-6">
                                <asp:Label ID="lb27" runat="server" Font-Bold="true" Text="Designation"></asp:Label>
                                <asp:TextBox ID="tbPrevEmplDesignation" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-lg-6">
                                <asp:Label ID="lb28" runat="server" Font-Bold="true" Text="Employment Type"></asp:Label>
                                <asp:DropDownList ID="ddlPrevEmplStatus" CssClass="form-control" runat="server" DataValueField="codeValue" DataTextField="codeValueDisplay">
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-6">
                                <asp:Label ID="lb29" runat="server" Font-Bold="true" Text="Designation Type"></asp:Label>
                                <asp:DropDownList ID="ddlPrevEmplOccupation" CssClass="form-control" runat="server" DataValueField="codeValue" DataTextField="codeValueDisplay">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group row">
                            <div class="col-lg-4">
                                <asp:Label ID="lb30" runat="server" Font-Bold="true" Text="Salary (S$)"></asp:Label>
                                <asp:TextBox ID="tbPrevEmplSalary" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-lg-8">
                                <asp:Label ID="lb31" runat="server" Font-Bold="true" Text="Employment Date"></asp:Label>
                                <div class="inputgroup">
                                    <asp:TextBox ID="tbPrevEmplStartDt" CssClass="form-control datepicker" runat="server" placeholder="dd MMM yyyy"></asp:TextBox>
                                    <span class="input-group-addon" style="font-weight: bold;">to</span>
                                    <asp:TextBox ID="tbPrevEmplEndDt" CssClass="form-control datepicker" runat="server" placeholder="dd MMM yyyy"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <asp:CustomValidator ID="cvPrevEmpl" runat="server" ControlToValidate="tbPrevCoName" Display="None" ValidationGroup="employment"
                            ErrorMessage="All fields under previous employment cannot be empty" ClientValidationFunction="validatePrevEmpl" ValidateEmptyText="true"></asp:CustomValidator>
                        <asp:CustomValidator ID="cvPrevEmplDate" runat="server" ControlToValidate="tbPrevEmplEndDt" Display="None" ValidationGroup="employment" ClientValidationFunction="validatePrevEmplDate"
                            ErrorMessage="Start and/or end date under previous employment is not in dd MMM yyyy format OR cannot be later than today OR start date cannot be later than end date."
                            ValidateEmptyText="false"></asp:CustomValidator>
                        <asp:CustomValidator ID="cvPrevEmplSalary" runat="server" ControlToValidate="tbPrevEmplSalary" Display="None" ValidationGroup="employment" ClientValidationFunction="validatePrevEmplSalary"
                            ErrorMessage="Invalid salary under previous employment." ValidateEmptyText="false"></asp:CustomValidator>
                    </div>
                    <div class="btn-group pull-right">

                        <asp:Button ID="btn_SaveEmployment" ValidationGroup="employment" OnClick="btn_SaveEmployment_Click" runat="server" Text="Save" class="btn btn-primary" CssClass="btn btn-primary nextBtn" CausesValidation="true" />
                    </div>
                </div>

            </div>

            <br />

            <asp:Panel ID="pnProgram" Enabled="true" runat="server">


                <div class="row">
                    <div class="text-left">

                        <h3>
                            <asp:Label ID="lbProgramApplied" runat="server" Text="Programme Applied"> </asp:Label>
                        </h3>
                    </div>

                </div>
                <br />


                <div class="row">
                    <div class="col-lg-12">
                        <asp:Label ID="lbCourseError" runat="Server" Visible="false" CssClass="alert alert-danger" Font-Size="small" />
                    </div>

                </div>
                <br />
                <div class="row">
                    <div class="text-left">
                        <asp:Label ID="lbProgMsg" runat="server" Text="" CssClass="alert-info"> </asp:Label>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-lg-12">
                        <asp:Label ID="lbCourseApplied" runat="server" Text="Programme Applied" Font-Bold="true"> </asp:Label>
                        <asp:DropDownList ID="ddlCourseApplied" runat="server" AutoPostBack="True" CssClass="form-control" OnSelectedIndexChanged="ddlCourseApplied_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:HiddenField ID="hfCourseType" runat="server" />
                    </div>

                </div>

                <br />

                <div class="row">
                    <div class="col-lg-6">
                        <asp:Label ID="lbCourseCode" runat="server" Text="Course Code" Font-Bold="true"> </asp:Label>
                        <asp:TextBox ID="tbCourseCodeValue" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col-lg-6">
                        <asp:Label ID="lbCourseDate" runat="server" Text="Programme Date" Font-Bold="true"> </asp:Label>
                        <asp:TextBox ID="tbCourseStartDate" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                    </div>
                </div>

                <br />

                <div class="row">
                    <div class="btn-group pull-right">
                        <asp:Button ID="btnUpdateCourseDetails" CssClass="btn btn-primary" runat="server" Text="Update Course Details" OnClick="btnUpdateCourseDetails_Click" />
                    </div>
                </div>
            </asp:Panel>

            <br />

            <div class="row">
                <div class="text-left">
                    <h3>
                        <asp:Label ID="lbExemptedModule" runat="server" Text="Exempted Module"> </asp:Label>
                    </h3>
                </div>
            </div>



            <div class="row">
                <div class="text-left">
                    <asp:Label ID="lbExemptedMsg" runat="server" Text="" CssClass="alert-info"> </asp:Label>
                </div>
                <div class="btn-group pull-right">
                    <button id="btnExemptedModule" runat="server" class="btn btn-primary" onserverclick="btnExemptedModule_ServerClick"><i class="fa fa-edit"></i>Exempt Modules</button>

                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <asp:Repeater ID="rptExemptedModule" runat="server">
                        <ItemTemplate>
                            <div class="row">
                                <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4">
                                    <li>
                                        <asp:Label ID="lbExemptedModuleCode" runat="server" Text='<%#Eval("WSQCompetencyCode") %>'></asp:Label>
                                    </li>
                                </div>
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                                    <asp:Label ID="lbExemptedModuleTitle" runat="server" Text='<%#Eval("moduleTitle") %>'></asp:Label>

                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="pull-left">
                        <asp:Label ID="lbNoExemptedModuleMsg" runat="server" Visible="false" Text="No exempted module"> </asp:Label>
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="text-left">
                    <h3>
                        <asp:Label ID="lb1" runat="server" Text="Interview"> </asp:Label>
                    </h3>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-4">
                    <asp:Label ID="lbInterviewDate" runat="server" Text="Interview Date" Font-Bold="True"></asp:Label>
                    <asp:TextBox ID="tbInterviewDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>

                </div>
                <div class="col-lg-4">
                    <asp:Label ID="lbInterviewStatus" runat="server" Text="Interview Status" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlInterviewStatus" CssClass="form-control" runat="server"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvInterviewStatus" runat="server" ErrorMessage="Interview Status cannot be empty" Display="None" ValidationGroup="interview" ControlToValidate="ddlInterviewStatus"></asp:RequiredFieldValidator>
                </div>
                <div class="col-lg-4">
                    <div class="checkbox">
                        <asp:CheckBox ID="cbShortlisted" Text="Tick to shortlist" runat="server" Font-Bold="true" />
                    </div>
                </div>
            </div>

            <br />
            <div class="row">
                <div class="col-lg-12">
                    <asp:Label ID="lbInterviewer" runat="server" Text="Interviewer" Font-Bold="true"> </asp:Label>
                    <asp:DropDownList ID="ddlInterviewer" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-12">
                    <asp:Label ID="lbInterviewRemarks" runat="server" Text="Interview remarks" Font-Bold="true"> </asp:Label>
                    <asp:TextBox ID="tbInterviewRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                </div>
            </div>
            <br />

            <asp:CustomValidator ID="cvValidateInterviewSQ" runat="server" Display="None" ValidationGroup="interview"
                ErrorMessage="Interviewer and Interview Date cannot be empty" ClientValidationFunction="validateInterviewSQ" ValidateEmptyText="false"></asp:CustomValidator>
            <asp:CustomValidator ID="cvValidateInterviewFQ" runat="server" Display="None" ValidationGroup="interview"
                ErrorMessage="Interviewer and Interview Date cannot be empty" ClientValidationFunction="validateInterviewFQ" ValidateEmptyText="false"></asp:CustomValidator>
            <asp:CustomValidator ID="cvValidateInterviewFQNeeded" runat="server" Display="None" ValidationGroup="interview"
                ErrorMessage="Interview is needed for Full Qualification Courses" ClientValidationFunction="validateInterviewFQNeeded" ValidateEmptyText="false"></asp:CustomValidator>

            <asp:CustomValidator ID="cvEnrollValidator" runat="server" Display="None"
                ErrorMessage="Interview is needed for Full Qualification Courses" OnServerValidate="isInterviewNeeded" ValidateEmptyText="false" ValidationGroup="interview"></asp:CustomValidator>



            <div class="row">
                <div class="btn pull-right">
                    <asp:Button ID="btnUpdateInterview" CssClass="btn btn-primary" runat="server" Text="Update Interview" OnClick="btnUpdateInterview_ServerClick" CausesValidation="true" ValidationGroup="interview" />
                </div>
            </div>

            <div class="row">
                <div class="text-left">
                    <h4>
                        <asp:Label ID="lbPayment" Text="Payment" runat="server"></asp:Label></h4>
                </div>

            </div>
            <div class="row">
                <div class="btn pull-left">
                    <h4>
                        <asp:CheckBox ID="cbCombinePayments" runat="server" CssClass="col-lg-12 control-label" Text="Combine Payment" AutoPostBack="true" OnCheckedChanged="cbCombinePayments_CheckedChanged" />
                    </h4>
                </div>


            </div>



            <div class="row">
                <div class="btn pull-right">
                    <asp:LinkButton ID="lbPayRegFees" runat="server" class="btn btn-primary" Visible="false" CausesValidation="false" OnClick="btn_PayRegFees_Click"><i class="fa fa-money"></i> Pay Registration Fees</asp:LinkButton>
                    <asp:LinkButton ID="lbPayCourseFees" runat="server" class="btn btn-primary" Visible="false" CausesValidation="false" OnClick="btnPayCourseFee_Click"><i class="fa fa-money"></i> Pay Course Fees</asp:LinkButton>
                    <asp:LinkButton ID="lbPayBoth" runat="server" class="btn btn-primary" Visible="false" CausesValidation="false" OnClick="btnCombinedPayment_Click"><i class="fa fa-money"></i> Pay Combined Fees</asp:LinkButton>

                </div>

            </div>

        </asp:Panel>


    </div>
</asp:Content>
