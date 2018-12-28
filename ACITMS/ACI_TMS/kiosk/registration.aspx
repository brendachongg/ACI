<%@ Page Title="" Language="C#" MasterPageFile="~/kiosk/aci-kiosk.Master" AutoEventWireup="true" CodeBehind="registration.aspx.cs" Inherits="ACI_TMS.kiosk.registration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .checkBoxClass tr td label {
            margin-right: 80px;
            margin-left: 8px;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            var navListItems = $('div.setup-panel div a'),
                    allWells = $('.setup-content'),
                    allNextBtn = $('.nextBtn');

            allWells.hide();

            navListItems.click(function (e) {
                e.preventDefault();

                var $target = $($(this).attr('href')),
                        $item = $(this);

                //if (!$item.hasClass('disabled')) {
                if (!$item[0].hasAttribute("disabled")) {
                    navListItems.removeClass('btn-primary').addClass('btn-default');
                    $item.addClass('btn-primary');
                    allWells.hide();
                    $target.show();
                    $target.find('input:eq(0)').focus();
                }
            });

            allNextBtn.click(function () {
                var curStep = $(this).closest(".setup-content"),
                    curStepBtn = curStep.attr("id"),
                    nextStepWizard = $('div.setup-panel div a[href="#' + curStepBtn + '"]').parent().next().children("a"),
                    curInputs = curStep.find("input[type='text'],input[type='url']"),
                    isValid = true;

                //$(".form-group").removeClass("has-error");
                //for (var i = 0; i < curInputs.length; i++) {
                //    if (!curInputs[i].validity.valid) {
                //        isValid = false;
                //        $(curInputs[i]).closest(".form-group").addClass("has-error");
                //    }
                //}

                Page_ClientValidate(curStepBtn);

                if (Page_IsValid)
                    nextStepWizard.removeAttr('disabled').trigger('click');
            });

            $('div.setup-panel div a.btn-primary').trigger('click');
        });

        function validateId(oSrc, args) {
            if ($('#<%=ddlIdType.ClientID%> option:selected').val() == '<%=(int)GeneralLayer.IDType.Oth%>') {
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

        function validateCurrEmpl(oSrc, args) {
            if (!$('#<%=cbCurrEmpl.ClientID%>').is(":checked")) {
                args.IsValid = true;
                return true;
            }

            if ($('#<%=tbCurrCoName.ClientID%>').val().length == 0 || $('#<%=tbCurrEmplDept.ClientID%>').val().length == 0
                || $('#<%=tbCurrEmplDesignation.ClientID%>').val().length == 0 || $('#<%=tbCurrEmplSalary.ClientID%>').val().length == 0
                || $('#<%=tbCurrEmplStartDt.ClientID%>').val().length == 0 || $('#<%=ddlCurrEmplStatus.ClientID%> option:selected').val().length == 0
                || $('#<%=ddlCurrEmplOccupation.ClientID%> option:selected').val().length==0) {
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

        function validateTNC(oSrc, args) {
            if ($('#<%=cbAgreement1.ClientID%>').is(":checked") && $('#<%=cbAgreement2.ClientID%>').is(":checked") && $('#<%=cbAgreement3.ClientID%>').is(":checked")
                && $('#<%=cbAgreement4.ClientID%>').is(":checked") && $('#<%=cbAgreement5.ClientID%>').is(":checked")) {
                args.IsValid = true;
                return true;
            }

            args.IsValid = false;
            return false;
        }

        function showCurrEmpl() {
            if ($('#<%=cbCurrEmpl.ClientID%>').is(":checked")) {
                $('#divCurrEmpl').css("display", "block");
            } else {
                if ($('#<%=ddlSpon.ClientID%> option:selected').val() == "<%=GeneralLayer.Sponsorship.COMP.ToString()%>") {
                    $('#lblError').html("Current employment must be filled in when it is company sponsored.");
                    $('#panelError').removeClass("hidden");
                    $('#<%=cbCurrEmpl.ClientID%>').prop('checked', true);
                }else
                    $('#divCurrEmpl').css("display", "none");
            }
        }

        function showPrevEmpl() {
            if ($('#<%=cbPrevEmpl.ClientID%>').is(":checked")) {
                $('#divPrevEmpl').css("display", "block");
            }else{
                $('#divPrevEmpl').css("display", "none");
            }
        }

        function closeErrorAlert() {
            $('#panelError').addClass("hidden");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <div class="row text-center" style="margin-right:0px;">
        <h2>Registration Form</h2>
    </div>
    <br />
    <%-- Steps wizard --%>
    <div class="row" style="margin-right:0px;">
        <div class="col-lg-8 col-md-offset-2">
            <div class="stepwizard" style="margin-bottom: 10px;">
                <div class="stepwizard-row setup-panel">
                    <div class="stepwizard-step">
                        <a id="lnkStep1" runat="server" href="#step-1" type="button" class="btn btn-primary btn-circle">1</a>
                        <p>Course</p>
                    </div>
                    <div class="stepwizard-step">
                        <a id="lnkStep2" runat="server" href="#step-2" type="button" class="btn btn-default btn-circle" disabled="disabled">2</a>
                        <p>Particulars</p>
                    </div>
                    <div class="stepwizard-step">
                        <a id="lnkStep3" runat="server" href="#step-3" type="button" class="btn btn-default btn-circle" disabled="disabled">3</a>
                        <p>Contact</p>
                    </div>
                    <div class="stepwizard-step">
                        <a id="lnkStep4" runat="server" href="#step-4" type="button" class="btn btn-default btn-circle" disabled="disabled">4</a>
                        <p>Education</p>
                    </div>
                    <div class="stepwizard-step">
                        <a id="lnkStep5" runat="server" href="#step-5" type="button" class="btn btn-default btn-circle" disabled="disabled">5</a>
                        <p>Employment</p>
                    </div>
                    <div class="stepwizard-step">
                        <a id="lnkStep6" runat="server" href="#step-6" type="button" class="btn btn-default btn-circle" disabled="disabled">6</a>
                        <p>Complete</p>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row" style="margin-right:0px;">
        <div class="col-lg-8 col-md-offset-2">
            <div style="margin-bottom: 10px;">
                <asp:Label ID="lblResult" runat="server" ForeColor="Red">
                    Please complete the following information.<br/>
                    If you have filled in the information in the previous steps and wish to change, click on the steps above.
                </asp:Label>
                <div class="alert alert-danger" id="panelSysError" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblSysError" runat="server" CssClass="alert-link"></asp:Label>
                </div>
                <div class="alert alert-danger hidden" id="panelError">
                    <a href="#" class="close" onClick="closeErrorAlert();">&times;</a>
                    <span id="lblError" class="alert-link"></span>
                </div>
                <asp:ValidationSummary ID="vSummary1" ValidationGroup="step-1" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
                <asp:ValidationSummary ID="vSummary2" ValidationGroup="step-2" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
                <asp:ValidationSummary ID="vSummary3" ValidationGroup="step-3" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
                <asp:ValidationSummary ID="vSummary4" ValidationGroup="step-4" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
                <asp:ValidationSummary ID="vSummary5" ValidationGroup="step-5" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
                <asp:ValidationSummary ID="vSummary6" ValidationGroup="step-6" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
            </div>
        </div>
    </div>

    <div class="row setup-content" id="step-1" style="margin-right:0px;">
        <div class="col-lg-8 col-md-offset-2">
            <div class="form-group row">
                <div class="col-lg-12">
                    <asp:Label ID="lb1" runat="server" Font-Bold="true" Text="Programme Category"></asp:Label>
                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlProgCat" DataTextField="codeValueDisplay" DataValueField="codeValue" AutoPostBack="true" OnSelectedIndexChanged="ddlProgCat_SelectedIndexChanged" CausesValidation="false" >
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvProgCat" ControlToValidate="ddlProgCat" ValidationGroup="step-1" runat="server" ErrorMessage="Programme Category cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group row">
                <div class="col-lg-12">
                    <asp:Label ID="lb2" runat="server" Font-Bold="true" Text="Available Programme"></asp:Label>
                    <asp:DropDownList runat="server" Enabled="false" DataTextField="programmeTitle" DataValueField="programmeId" CssClass="form-control" ID="ddlProgTitle" AutoPostBack="true" OnSelectedIndexChanged="ddlProgTitle_SelectedIndexChanged" CausesValidation="false" >
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvProgTitle" ControlToValidate="ddlProgTitle" ValidationGroup="step-1" runat="server" ErrorMessage="Programme Title cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group row">
                <div class="col-lg-6">
                    <asp:Label ID="lb3" runat="server" Font-Bold="true" Text="Programme Date"></asp:Label>
                    <asp:DropDownList runat="server" Enabled="false" CssClass="form-control" ID="ddlProgDate" DataTextField="programmeDate" DataValueField="programmeBatchId" ></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvProgDate" ControlToValidate="ddlProgDate" ValidationGroup="step-1" runat="server" ErrorMessage="Programme date cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
                <div class="col-lg-6">
                    <asp:Label ID="lb17" runat="server" Font-Bold="true" Text="Sponorship"></asp:Label>
                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlSpon" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSpon" ControlToValidate="ddlSpon" ValidationGroup="step-1" runat="server" ErrorMessage="Sponsorship cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="btn-group pull-right">
                <button type="button" class="btn btn-info" data-toggle="modal" data-target="#diagClear">Clear</button>
                <button class="btn btn-primary nextBtn" type="button" >Next</button>             
            </div>    
        </div>
    </div>
   
    <div class="row setup-content" id="step-2" style="margin-right:0px;">
        <div class="col-lg-8 col-md-offset-2">
            <div class="form-group row">
                <div class="col-lg-12">
                    <asp:Label ID="lb4" runat="server" Font-Bold="true" Text="Full Name"></asp:Label>
                    <asp:TextBox ID="tbName" CssClass="form-control" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvName" ControlToValidate="tbName" ValidationGroup="step-2" runat="server" ErrorMessage="Full name cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group row">
                <div class="col-lg-4">
                    <asp:Label ID="lb5" runat="server" Font-Bold="true" Text="Identification Type"></asp:Label>
                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlIdType" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvIdType" ControlToValidate="ddlIdType" ValidationGroup="step-2" runat="server" ErrorMessage="Identification type cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
                <div class="col-lg-8">
                    <asp:Label ID="lb6" runat="server" Font-Bold="true" Text="Identification No."></asp:Label>
                    <asp:TextBox ID="tbId" Style="clear: both;" CssClass="form-control" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvId" ControlToValidate="tbId" ValidationGroup="step-2" runat="server" ErrorMessage="Identification No. cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cvId" runat="server" ControlToValidate="tbId" Display="None" ValidationGroup="step-2"
                    ErrorMessage="Invalid identification no." ClientValidationFunction="validateId" ValidateEmptyText="false"></asp:CustomValidator>
                </div>
            </div>
            <div class="form-group row">
                <div class="col-lg-6">
                    <asp:Label ID="lb7" runat="server" Font-Bold="true" Text="Nationality"></asp:Label>
                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlNationality" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvNationality" ControlToValidate="ddlNationality" ValidationGroup="step-2" runat="server" ErrorMessage="Nationality cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
                <div class="col-lg-6">
                    <asp:Label ID="lb8" runat="server" Font-Bold="true" Text="Race"></asp:Label>
                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlRace" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvRace" ControlToValidate="ddlRace" ValidationGroup="step-2" runat="server" ErrorMessage="Race cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group row">
                <div class="col-lg-6">
                    <asp:Label ID="lb9" runat="server" Font-Bold="true" Text="Gender"></asp:Label>
                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddlGender" >
                        <asp:ListItem Value="">--Select--</asp:ListItem>
                        <asp:ListItem Value="F">Female</asp:ListItem>
                        <asp:ListItem Value="M">Male</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvGender" ControlToValidate="ddlGender" ValidationGroup="step-2" runat="server" ErrorMessage="Gender cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
                <div class="col-lg-6">
                    <asp:Label ID="lb10" runat="server" Font-Bold="true" Text="Date of Birth"></asp:Label>
                    <asp:TextBox ID="tbDOB" Style="clear: both;" CssClass="form-control datepicker" runat="server" placeholder="dd MMM yyyy"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDOB" ControlToValidate="tbDOB" ValidationGroup="step-2" runat="server" ErrorMessage="Date of birth cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cvBirthDate" runat="server" ControlToValidate="tbDOB" Display="None" ValidationGroup="step-2" ClientValidationFunction="validatePastDate"
                    ErrorMessage="Date of birth is not in dd MMM yyyy format OR cannot be later than today." ValidateEmptyText="false"></asp:CustomValidator>
                </div>
            </div>
            <div class="btn-group pull-right">
                <button type="button" class="btn btn-info" data-toggle="modal" data-target="#diagClear">Clear</button>
                <button class="btn btn-primary nextBtn" type="button" >Next</button>             
            </div> 
        </div>
    </div>

    <div class="row setup-content" id="step-3" style="margin-right:0px;">
        <div class="col-lg-8 col-md-offset-2">
            <div class="form-group row">
                <div class="col-lg-12">
                    <asp:Label ID="lb11" runat="server" Font-Bold="true" Text="Email"></asp:Label>
                    <asp:TextBox ID="tbEmail" CssClass="form-control" runat="server"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="rfvEmail" ControlToValidate="tbEmail" ValidationGroup="step-3" runat="server" ErrorMessage="Email cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="rexEmail" Display="None" ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" runat="server" ErrorMessage="Invalid email." ControlToValidate="tbEmail" ValidationGroup="step-3"></asp:RegularExpressionValidator>
                </div>
            </div>
             <div class="form-group row">
                <div class="col-lg-6">
                    <asp:Label ID="lb12" runat="server" Font-Bold="true" Text="Contact No. (Mobile)"></asp:Label>
                    <asp:TextBox ID="tbContact1" CssClass="form-control" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvContact1" ControlToValidate="tbContact1" ValidationGroup="step-3" runat="server" ErrorMessage="Contact no. cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revContact1" Display="None" ValidationExpression="^\+?\d+$" runat="server" ErrorMessage="Contact number can only contain numbers." 
                        ControlToValidate="tbContact1" ValidationGroup="step-3"></asp:RegularExpressionValidator>
                </div>
                <div class="col-lg-6">
                    <asp:Label ID="lb13" runat="server" Font-Bold="true">Alternative Contact No. (Home/Others) <i>[Optional]</i></asp:Label>
                    <asp:TextBox ID="tbContact2" CssClass="form-control" runat="server"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revContact2" Display="None" ValidationExpression="^\+?\d+$" runat="server" ErrorMessage="Alternative contact no. can only contain numbers." 
                        ControlToValidate="tbContact2" ValidationGroup="step-3"></asp:RegularExpressionValidator>
                </div>
            </div>
            <div class="form-group row">
                <div class="col-lg-12">
                    <asp:Label ID="lb14" runat="server" Font-Bold="true" Text="Address"></asp:Label>
                    <asp:TextBox ID="tbAddr" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAddr" ControlToValidate="tbAddr" ValidationGroup="step-3" runat="server" ErrorMessage="Address cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group row">
                <div class="col-lg-12">
                    <asp:Label ID="lb15" runat="server" Font-Bold="true" Text="Postal Code"></asp:Label>
                    <asp:TextBox ID="tbPostalCode" CssClass="form-control" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPostalCode" ControlToValidate="tbPostalCode" ValidationGroup="step-3" runat="server" ErrorMessage="Postal Code cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="btn-group pull-right">
                <button type="button" class="btn btn-info" data-toggle="modal" data-target="#diagClear">Clear</button>
                <button class="btn btn-primary nextBtn" type="button" >Next</button>             
            </div> 
        </div>
    </div>

    <div class="row setup-content" id="step-4" style="margin-right:0px;">
        <div class="col-lg-8 col-md-offset-2">
            <div class="form-group">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Language</th>
                            <th>Spoken</th>
                            <th>Written</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td style="padding-left: 20px; padding-top: 15px; width:150px;">English</td>
                            <td>
                                <asp:DropDownList ID="ddlEngSpoken" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvEngSpoken" ControlToValidate="ddlEngSpoken" ValidationGroup="step-4" runat="server" ErrorMessage="English spoken proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlEngWritten" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvEngWriteen" ControlToValidate="ddlEngWritten" ValidationGroup="step-4" runat="server" ErrorMessage="English written proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 20px; padding-top: 15px;">Chinese</td>
                            <td>         
                                <asp:DropDownList ID="ddlChnSpoken" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvChnSpoken" ControlToValidate="ddlChnSpoken" ValidationGroup="step-4" runat="server" ErrorMessage="Chinese spoken proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlChnWritten" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvChnWritten" ControlToValidate="ddlChnWritten" ValidationGroup="step-4" runat="server" ErrorMessage="Chinese written proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 20px; padding-top: 15px;">Other</td>
                            <td>
                                <asp:DropDownList ID="ddlOthSpoken" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvOtherSpoken" ControlToValidate="ddlOthSpoken" ValidationGroup="step-4" runat="server" ErrorMessage="Other spoken proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlOthWritten" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvOtherWritten" ControlToValidate="ddlOthWritten" ValidationGroup="step-4" runat="server" ErrorMessage="Other written proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="form-group row">
                <div class="col-lg-12">
                    <asp:Label ID="lb16" runat="server" Font-Bold="true" Text="Highest Level of Education"></asp:Label>
                    <asp:DropDownList ID="ddlHighEdu" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvHighEdu" ControlToValidate="ddlHighEdu" ValidationGroup="step-4" runat="server" ErrorMessage="Highest level of education cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="btn-group pull-right">
                <button type="button" class="btn btn-info" data-toggle="modal" data-target="#diagClear">Clear</button>
                <button class="btn btn-primary nextBtn" type="button" >Next</button>             
            </div> 
        </div>
    </div>

    <div class="row setup-content" id="step-5" style="margin-right:0px;">
        <div class="col-lg-8 col-md-offset-2">
            <p><asp:CheckBox ID="cbCurrEmpl" runat="server" Checked="true" onchange="showCurrEmpl()" />&nbsp;<label for="<%=cbCurrEmpl.ClientID %>">Current Employment</label></p>
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
                <asp:CustomValidator ID="cvCurrEmpl" runat="server" ControlToValidate="tbCurrCoName" Display="None" ValidationGroup="step-5"
                    ErrorMessage="All fields under current employment cannot be empty" ClientValidationFunction="validateCurrEmpl" ValidateEmptyText="true"></asp:CustomValidator>
                <asp:CustomValidator ID="cvCurrEmplStartDt" runat="server" ControlToValidate="tbCurrEmplStartDt" Display="None" ValidationGroup="step-5" ClientValidationFunction="validateCurrEmplStartDt"
                    ErrorMessage="Start date under current employment is not in dd MMM yyyy format OR cannot be later than today." ValidateEmptyText="false"></asp:CustomValidator>
                <asp:CustomValidator ID="cvCurrEmplSalary" runat="server" ControlToValidate="tbCurrEmplSalary" Display="None" ValidationGroup="step-5"
                    ErrorMessage="Invalid salary under current employment." ClientValidationFunction="validateCurrEmplSalary" ValidateEmptyText="true"></asp:CustomValidator>
                <asp:CustomValidator ID="cvEmplDates" runat="server" ControlToValidate="tbCurrEmplStartDt" Display="None" ValidationGroup="step-5" ClientValidationFunction="validateEmplDates"
                    ErrorMessage="End date of previous employment cannot be later than start date of current employment." ValidateEmptyText="false"></asp:CustomValidator>
            </div>
            <p><asp:CheckBox ID="cbPrevEmpl" runat="server" onchange="showPrevEmpl()" />&nbsp;<label for="<%=cbPrevEmpl.ClientID %>">Previous Employment</label></p>
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
                        <div class="input-group">
                            <asp:TextBox ID="tbPrevEmplStartDt" CssClass="form-control datepicker" runat="server" placeholder="dd MMM yyyy"></asp:TextBox>
                            <span class="input-group-addon" style="font-weight: bold;">to</span>
                            <asp:TextBox ID="tbPrevEmplEndDt" CssClass="form-control datepicker" runat="server" placeholder="dd MMM yyyy"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <asp:CustomValidator ID="cvPrevEmpl" runat="server" ControlToValidate="tbPrevCoName" Display="None" ValidationGroup="step-5"
                    ErrorMessage="All fields under previous employment cannot be empty" ClientValidationFunction="validatePrevEmpl" ValidateEmptyText="true"></asp:CustomValidator>
                <asp:CustomValidator ID="cvPrevEmplDate" runat="server" ControlToValidate="tbPrevEmplEndDt" Display="None" ValidationGroup="step-5" ClientValidationFunction="validatePrevEmplDate"
                    ErrorMessage="Start and/or end date under previous employment is not in dd MMM yyyy format OR cannot be later than today OR start date cannot be later than end date." 
                    ValidateEmptyText="false"></asp:CustomValidator>
                <asp:CustomValidator ID="cvPrevEmplSalary" runat="server" ControlToValidate="tbPrevEmplSalary" Display="None" ValidationGroup="step-5" ClientValidationFunction="validatePrevEmplSalary"
                    ErrorMessage="Invalid salary under previous employment." ValidateEmptyText="false"></asp:CustomValidator>
            </div>
            <div class="btn-group pull-right">
                <button type="button" class="btn btn-info" data-toggle="modal" data-target="#diagClear">Clear</button>
                <button class="btn btn-primary nextBtn" type="button" >Next</button>             
            </div> 
        </div>
    </div>

    <div class="row setup-content" id="step-6" style="margin-right:0px;">
        <div class="col-lg-8 col-md-offset-2">
            <div class="form-group">
                <label>How did you get to know us</label>
                <asp:CheckBoxList ID="cbKnowLst" runat="server" class="checkBoxClass" RepeatColumns="2" DataTextField="codeValueDisplay" DataValueField="codeValue">
                </asp:CheckBoxList>
            </div>
            <br />
            <asp:CheckBox ID="cbAgreement1" runat="server" />
            I declare that all of the information provided by me in this application form is true and correct. 
            I understand that any false statement(s) and/or misrepresentation(s) is/are sufficient ground(s) for the rejection of my application. 
            <br /><br />
            <asp:CheckBox ID="cbAgreement2" runat="server" />
            I understand that a culinary or related course, which is a skill-based training programme conducted in a kitchen environment, 
            is physically and mentally demanding. I further declare that I am physically and mentally fit to undertake the training programme I apply, 
            and I will not hold ACI or its officers, trainers responsible for any physical discomfort or injuries which may occur from attending the programme. 
            <br /><br />
            <asp:CheckBox ID="cbAgreement3" runat="server" />
            I also declare that prior to this; I have not enrolled in the course/module(s) indicated above. Otherwise, I will bear the full course/module(s) 
            fees without any subsidy. 
            <br /><br />
            <asp:CheckBox ID="cbAgreement4" runat="server" />
            I authorize any investigation of the above information for the purpose of verification.
            <br /><br />
            <asp:CheckBox ID="cbAgreement5" runat="server" />
            I understand that ACI reserves the right not to accept your application and may cancel or change the class schedule at short notice.
            <br /><br />
            <asp:TextBox ID="hfAgreement" runat="server" style="display: none;"></asp:TextBox>
            <asp:CustomValidator ID="cvAgreement" runat="server" ControlToValidate="hfAgreement" Display="None" ValidationGroup="step-6" ClientValidationFunction="validateTNC"
                    ErrorMessage="Not all terms and conditions are checked." ValidateEmptyText="true"></asp:CustomValidator>

            <div class="btn-group pull-right">
                <button type="button" class="btn btn-info" data-toggle="modal" data-target="#diagClear">Clear</button>
                <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-primary nextBtn pull-right" OnClick="btnRegister_Click" ValidationGroup="step-6" />           
            </div>    
        </div>
    </div>

    <br /><br />

    <div id="diagClear" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Clear All Information</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to clear all information?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnClear" CssClass="btn btn-danger" runat="server" Text="Yes" CausesValidation="false" OnClick="btnClear_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">No</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
