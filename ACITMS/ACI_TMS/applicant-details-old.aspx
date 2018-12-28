<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="applicant-details-old.aspx.cs" Inherits="ACI_TMS.applicant_details_old" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/WebForms/MSAjax/MicrosoftAjax.js"></script>
    <script src="js/WebForms/MSAjax/MicrosoftAjaxWebForms.js"></script>
    <style>
        .checkbox label {
            padding-left: 0;
        }

        .lbShortListed {
            margin-left: -20px;
        }
    </style>
    <script>


        $(document).ready(function () {
            showPrev();
        });

        function showPrev() {
            if ($('#<%=cbPrevEmpl.ClientID%>').is(":checked")) {
                $('#divPrevEmpl').css("display", "block");
            }
        }
        //function showEnrollmentDialog() {
        //    $('#diaConfirmEnrollUpdate').modal('show');
        //}

        //function openModel() {
        //    $("#openModel").click();
        //}

        //function closeModel() {
        //    $("#closeModel").click(function () {
        //        location.reload();
        //    });
        //    $('.modal-backdrop').remove();
        //}

    </script>
    <script type="text/javascript">
        $(function () {
            SetDatePicker();
        });

        $(function () {
            SetDatePickerInterview();
        });


        //On UpdatePanel Refresh.
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (prm != null) {
            prm.add_endRequest(function (sender, e) {
                if (sender._postBackSettings.panelsToUpdate != null) {
                    SetDatePicker();
                    SetDatePickerInterview();
                }
            });
        };


        function SetDatePickerInterview() {
            var start = new Date();
            start.setFullYear();

            $(".datepickerInterview").datepicker({
                dateFormat: "dd M yy",
                changeMonth: true,
                changeYear: true,
                yearRange: start + ':+10'
            });
        }

        function SetDatePicker() {


            $(".datepicker").datepicker({
                dateFormat: "dd M yy",
                changeMonth: true,
                changeYear: true,
                yearRange: '1930:+0'
            });
        }

        function hideDiv(obj) {
            if (obj.checked == true) {
                document.getElementById("endDate").style.display = 'none';
            }
            else {
                document.getElementById("endDate").style.display = 'block';
            }
        }

        <%--function hide() {


            if (document.getElementById("<%= cbSetCurrentEmployment.ClientID %>").checked == true) {
                document.getElementById("endDate").style.display = 'none';
            }
            else {
                document.getElementById("endDate").style.display = 'block';
            }
        }--%>

        function showCurrEmpl() {
            if ($('#<%=cbCurrEmpl.ClientID%>').is(":checked")) {
                $('#divCurrEmpl').css("display", "block");
            } else {
                if ($('#<%=ddlSponsorship.ClientID%> option:selected').val() == "<%=GeneralLayer.Sponsorship.COMP.ToString()%>") {
                    $('#lblError').html("Current employment must be filled in when it is company sponsored.");
                    $('#panelError').removeClass("hidden");
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

        

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div id="page-wrapper">
       <%-- <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
<%--        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>
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
                        <asp:LinkButton ID="lkbtnBack" runat="server" OnClick="lkbtnBack_Click" CssClass="btn btn-sm btn-default" Text="Back" CausesValidation="false"></asp:LinkButton>
                        <asp:LinkButton ID="btn_print" runat="server" OnClick="btn_print_Click" CssClass="btn btn-sm btn-success"><i class="fa fa-print"></i>Print</asp:LinkButton>
                        <asp:LinkButton ID="btnRejectAppcalicantTop" runat="server" class="btn btn-sm btn-danger" OnClick="btnRejectAppcalicant_Click"><i class="fa fa-times"></i>Reject</asp:LinkButton>
                        <asp:LinkButton ID="lkbtnEnrollApplicantTop" CssClass="btn btn-sm btn-info" OnClick="lkbtnEnrollApplicant_Click" runat="server"><i class="fa fa-sign-in"></i>Enroll</asp:LinkButton>
                    </div>

                </div>

                <hr />

                <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link">Update successful</asp:Label>
                </div>
                <div class="alert alert-danger" id="panelError" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblError" runat="server" CssClass="alert-link">Please correct the following:</asp:Label>
                    <br />
                    <asp:Label ID="lblErrorMsg" runat="server" CssClass="alert-link"></asp:Label>

                </div>
        
                    <asp:ValidationSummary ID="vSummary5" ValidationGroup="step-5" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
             
                <br />

                <div class="row text-left">

                    <%-- Left column--%>
                    <div class="col-lg-6">

                        <%-- Applicant's details--%>
                        <div class="row">
                            <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                                <h4>
                                    <asp:Label ID="lbParticular" runat="server" Text="Personal Particulars"> </asp:Label>
                                </h4>
                            </div>
                            <div class="col-lg-9 col-md-9 col-sm-9 col-xs-9 text-right">
                                <button id="btnCancelApplicant" runat="server" class="btn btn-sm btn-default" visible="false" onserverclick="btnCancelApplicant_ServerClick"><i class="fa fa-edit"></i>Cancel</button>
                                <button id="btnEditApplicant" runat="server" class="btn btn-sm btn-default" onserverclick="btnEditApplicant_ServerClick"><i class="fa fa-edit"></i>Edit</button>
                            </div>
                        </div>
                        <br />

                        <asp:Panel ID="panelParticular" Enabled="false" runat="server">

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbFullName" runat="server" Text="Full Name" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbFullNameValue" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbIdentification" runat="server" Text="Identification" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbIdentificationValue" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbIdentificationType" runat="server" Text="Identification Type" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:DropDownList ID="ddlIdentificationType" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbNationality" runat="server" Text="Nationality" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:DropDownList ID="ddlNationalityValue" DataTextField="codeValueDisplay" DataValueField="codeValue" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbGender" runat="server" Text="Gender" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:DropDownList ID="ddlGenderValue" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="">--Select--</asp:ListItem>
                                        <asp:ListItem Value="M">Male</asp:ListItem>
                                        <asp:ListItem Value="F">Female</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbRace" runat="server" Text="Race" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:DropDownList ID="ddlRaceValue" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue" runat="server">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbContact1" runat="server" Text="Contact Number 1" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbContact1Value" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbContact2" runat="server" Text="Contact Number 2" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbContact2Value" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbEmailAddress" runat="server" Text="Email Address" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbEmailAddressValue" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbBirthDate" runat="server" Text="Date of Birth" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbBirthDateValue" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbAddressLine1" runat="server" Text="Address Line" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbAddressLine1Value" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbPostalCode" runat="server" Text="Postal Code" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbPostalCodeValue" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <br />

                            <h4>
                                <asp:Label ID="Education" runat="server" Text="Education"> </asp:Label>
                            </h4>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbHighestEducation" runat="server" Text="Highest Education" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">

                                    <asp:DropDownList ID="ddlHighestEducationValue" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbHighestEducationRemark" runat="server" Text="Highest Education Remarks" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbHighestEducationRemarkValue" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>

                            <br />

                            <%-- <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbSpokenLanguage" runat="server" Text="Spoken Language" Font-Bold="True"></asp:Label>

                                </div>
                                <div class="col-lg-9">
                                    <%--<asp:TextBox ID="tbSpokenLanguageValue" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    <asp:Repeater ID="rptSpokenLanguage" runat="server" OnItemDataBound="rptSpokenLanguage_ItemDataBound">
                                        <ItemTemplate>
                                            <div class="row">
                                                <div class="col-lg-6">
                                                    <asp:Label ID="lbSLanguages" runat="server" CssClass="form-control" Text='<%#Eval("codeValueDisplay") %>'></asp:Label>
                                                    <asp:HiddenField ID="hdfSLangCodeValue" runat="server" Value='<%#Eval("codeValue") %>' />
                                                </div>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="ddlSLanguagesPro" CssClass="form-control" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>--%>



                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbTextSpokenLanguage" runat="server" Text="Spoken Language" Font-Bold="True"></asp:Label>

                                </div>
                                <div class="col-lg-9">
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <asp:Label ID="lbEng" runat="server" CssClass="form-control" Text="English"></asp:Label>
                                        </div>
                                        <div class="col-lg-6">
                                            <asp:DropDownList ID="ddlEngPro" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-6">
                                            <asp:Label ID="lblChi" runat="server" CssClass="form-control" Text="Chinese"></asp:Label>
                                        </div>
                                        <div class="col-lg-6">
                                            <asp:DropDownList ID="ddlChnPro" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>


                                    <div class="row">
                                        <div class="col-lg-6">
                                            <asp:DropDownList ID="ddlOtherLanguage" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                        <div class="col-lg-6">
                                            <asp:DropDownList ID="ddlOtherLangPro" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbTextWritternLang" runat="server" Text="Written Language" Font-Bold="True"></asp:Label>

                                </div>
                                <div class="col-lg-9">
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <asp:Label ID="lbWEng" runat="server" CssClass="form-control" Text="English"></asp:Label>
                                        </div>
                                        <div class="col-lg-6">
                                            <asp:DropDownList ID="ddlWEngPro" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-6">
                                            <asp:Label ID="lbWChi" runat="server" CssClass="form-control" Text="Chinese"></asp:Label>
                                        </div>
                                        <div class="col-lg-6">
                                            <asp:DropDownList ID="ddlWChiPro" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>


                                    <div class="row">
                                        <div class="col-lg-6">
                                            <asp:DropDownList ID="ddlWOtherLanguage" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                        <div class="col-lg-6">
                                            <asp:DropDownList ID="ddlWOtherLangPro" CssClass="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <%--  <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbWrittenLanguage" runat="server" Text="Written Language" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:Repeater ID="rptWrittenLanguage" runat="server" OnItemDataBound="rptWrittenLanguage_ItemDataBound">
                                        <ItemTemplate>
                                            <div class="row">
                                                <div class="col-lg-6">
                                                    <asp:Label ID="lbWLanguages" runat="server" CssClass="form-control" Text='<%#Eval("codeValueDisplay") %>'></asp:Label>
                                                    <asp:HiddenField ID="hdfWLangCodeValue" runat="server" Value='<%#Eval("codeValue") %>' />
                                                </div>
                                                <div class="col-lg-6">
                                                    <asp:DropDownList ID="ddlWLanguagesPro" CssClass="form-control" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>--%>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbGetToKnowChannel" runat="server" Text="Get To Know Channel" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <%--<asp:TextBox ID="tbGetToKnowChannelValue" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>--%>
                                    <asp:CheckBoxList ID="cblGetToKnowChannel" CssClass="col-md-12 control-label" runat="server" RepeatDirection="Horizontal" RepeatColumns="2" OnDataBound="cblGetToKnowChannel_DataBound"></asp:CheckBoxList>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbSponsorship" runat="server" Text="Sponsorship" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <%--<asp:TextBox ID="tbHighestEducationValue" runat="server" CssClass="form-control"></asp:TextBox>--%>
                                    <asp:DropDownList ID="ddlSponsorship" CssClass="form-control" DataTextField="codeValueDisplay" AutoPostBack="true" DataValueField="codeValue" runat="server" OnSelectedIndexChanged="ddlSponsorship_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>


                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="Label6" runat="server" Text="Sponsored By" Font-Bold="True"></asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbSponsorshipCompany" CssClass="form-control" Enabled="false" runat="server" Text=""></asp:TextBox>
                                </div>
                            </div>


                            <br />

                        </asp:Panel>

                        <div class="row">
                            <div class="col-lg-12 text-center">
                                <small class="text-muted">
                                    <asp:Label ID="lbLastModifyDateTime" runat="server" Text="Applicant's details last modified on "></asp:Label>
                                    <br />
                                    <asp:Label ID="lbLastModifyDateTimeValue" runat="server" Text=""></asp:Label>
                                </small>
                            </div>
                        </div>

                        <br />

                        <asp:Panel ID="panelUpdateApplicantDetails" Visible="false" runat="server">
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:Button ID="btnUpdateApplicantDetails" CssClass="btn btn-warning btn-block" OnClick="btnUpdateApplicantDetails_Click" runat="server" Text="Update Applicant's Details" />
                                </div>
                            </div>
                        </asp:Panel>

                        <br />

                        <hr />


<%--                        <%-- Employment history--%>
                        <div class="row">
                            <div class="col-lg-3 col-md-3">
                                <h4>
                                    <asp:Label ID="lbEmployment" runat="server" Text="Employment"> </asp:Label>
                                </h4>
                            </div>
                            <div class="col-lg-9 text-right">
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-lg-12">
 

                      <%--          <div class="row">

                                    <div class="col-lg-8 col-md-offset-2">--%>
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
                                            <asp:CustomValidator ID="cvCurrEmpl" runat="server" ControlToValidate="tbCurrCoName" Display="None" ValidationGroup="step-5"
                                                ErrorMessage="All fields under current employment cannot be empty" ClientValidationFunction="validateCurrEmpl" ValidateEmptyText="true"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvCurrEmplStartDt" runat="server" ControlToValidate="tbCurrEmplStartDt" Display="None" ValidationGroup="step-5" ClientValidationFunction="validateCurrEmplStartDt"
                                                ErrorMessage="Start date under current employment is not in dd MMM yyyy format OR cannot be later than today." ValidateEmptyText="false"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvCurrEmplSalary" runat="server" ControlToValidate="tbCurrEmplSalary" Display="None" ValidationGroup="step-5"
                                                ErrorMessage="Invalid salary under current employment." ClientValidationFunction="validateCurrEmplSalary" ValidateEmptyText="true"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvEmplDates" runat="server" ControlToValidate="tbCurrEmplStartDt" Display="None" ValidationGroup="step-5" ClientValidationFunction="validateEmplDates"
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
                                            <asp:CustomValidator ID="cvPrevEmpl" runat="server" ControlToValidate="tbPrevCoName" Display="None" ValidationGroup="step-5"
                                                ErrorMessage="All fields under previous employment cannot be empty" ClientValidationFunction="validatePrevEmpl" ValidateEmptyText="true"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvPrevEmplDate" runat="server" ControlToValidate="tbPrevEmplEndDt" Display="None" ValidationGroup="step-5" ClientValidationFunction="validatePrevEmplDate"
                                                ErrorMessage="Start and/or end date under previous employment is not in dd MMM yyyy format OR cannot be later than today OR start date cannot be later than end date."
                                                ValidateEmptyText="false"></asp:CustomValidator>
                                            <asp:CustomValidator ID="cvPrevEmplSalary" runat="server" ControlToValidate="tbPrevEmplSalary" Display="None"  ValidationGroup="step-5" ClientValidationFunction="validatePrevEmplSalary"
                                                ErrorMessage="Invalid salary under previous employment." ValidateEmptyText="false"></asp:CustomValidator>
                                        </div>
                                        <div class="btn-group pull-right">
                                    <%--        <button type="button" class="btn btn-info" data-toggle="modal" data-target="#diagClear">Clear</button>--%>
                                           <%-- <button class="btn btn-primary nextBtn" type="button">Next</button>--%>
                                            <asp:Button ID="btn_SaveEmployment" ValidationGroup="step-5" runat="server" Text="Save" OnClick="btn_SaveEmployment_Click" class="btn btn-primary nextBtn" />
                                        </div>
                                    </div>
                              <%--  </div>
                       
                            </div>--%>
                        </div>
            <%--            <div class="row">
                            <div class="col-lg-12 text-center">
                                <br />
                                <small class="text-muted text-center">
                                    <asp:Label ID="lbLastModifyEmpDateTime" runat="server" Text="Employment details last modified on "></asp:Label>
                                    <br />
                                    <asp:Label ID="lbLastModifyEmpDateTimeValue" runat="server" Text=""></asp:Label>
                                </small>
                            </div>
                        </div>--%>
                        <!-- Modal -->
                       <%-- <button id="openModel" type="button" class="hidden" data-toggle="modal" data-target="#myModal"></button>

                        <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" id="closeModel" data-dismiss="modal" data-target="#myModal" aria-hidden="true">&times;</button>
                                        <h4 class="modal-title" id="myModalLabel">Employment Record</h4>

                                        <div class="alert alert-success" id="panelSuccessModal" runat="server" visible="false">
                                            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                                            <asp:Label ID="lblSuccessModal" runat="server" CssClass="alert-link">Update successful</asp:Label>
                                        </div>
                                        <div class="alert alert-danger" id="panelErrorModal" runat="server" visible="false">
                                            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                                            <asp:Label ID="lblErrorModal" runat="server" CssClass="alert-link">Please correct the following:</asp:Label>
                                            <br />
                                            <asp:Label ID="lblErrorMsgModal" runat="server" CssClass="alert-link"></asp:Label>

                                        </div>
                                        <br />

                                    </div>
                                    <div class="modal-body">

                                        <div class="row">
                                            <div class="col-lg-12 text-right">
                                                <div class="checkbox">

                                                    <asp:CheckBox ID="cbSetCurrentEmployment" Visible="true" Text="Current Employment?" runat="server" onclick="hideDiv(this)" />

                                                </div>

                                                <asp:HiddenField ID="hdfEmploymentHistoryId" runat="server" />
                                            </div>
                                        </div>

                                        <br />

                                        <div class="row">
                                            <div class="col-lg-3">
                                                <asp:Label ID="lbCompanyName" runat="server" Text="Company" Font-Bold="true"> </asp:Label>
                                            </div>
                                            <div class="col-lg-9">
                                                <asp:TextBox ID="tbCompanyNameValue" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>

                                        <br />

                                        <div class="row">
                                            <div class="col-lg-3">
                                                <asp:Label ID="lbEmploymentStatus" runat="server" Text="Employment Status" Font-Bold="true"> </asp:Label>
                                            </div>
                                            <div class="col-lg-9">
                                                <asp:DropDownList ID="ddlEmploymentStatus" CssClass="form-control" runat="server">--%>

                        </div>

                    <%-- Right column --%>
                    <div class="col-lg-6">

                        <%-- Course details --%>
                        <div class="row">
                            <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">
                                <h4>
                                    <asp:Label ID="lbCourseDetails" runat="server" Text="Programme Details"> </asp:Label>
                                </h4>
                            </div>

                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                <button id="btnCancelCourseDetails" runat="server" class="btn btn-sm btn-default" visible="false" onserverclick="btnCancelCourseDetails_ServerClick"><i class="fa fa-edit"></i>Cancel</button>
                                <button id="btnEditCourseDetails" runat="server" class="btn btn-sm btn-default" onserverclick="btnEditCourseDetails_ServerClick"><i class="fa fa-edit"></i>Edit</button>
                            </div>

                        </div>

                        <br />

                        <asp:Panel ID="panelCourseApplied" Enabled="false" runat="server">
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:Label ID="lbCourseError" runat="Server" Visible="false" CssClass="alert-danger" Font-Size="Medium" />
                                </div>

                            </div>



                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbCourseApplied" runat="server" Text="Programme Applied" Font-Bold="true"> </asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:DropDownList ID="ddlCourseAppliedValue" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCourseAppliedValue_SelectedIndexChanged" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbCourseCode" runat="server" Text="Course Code" Font-Bold="true"> </asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbCourseCodeValue" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>

                                </div>
                            </div>

                            <br />

                            <%--  <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbProjectCode" runat="server" Text="Project Code" Font-Bold="true"> </asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:DropDownList ID="ddlProjectCodeValue" runat="server" CssClass="form-control" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlProjectCodeValue_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <br />--%>

                            <div class="row">
                                <div class="col-lg-3">
                                    <asp:Label ID="lbCourseDate" runat="server" Text="Programme Date" Font-Bold="true"> </asp:Label>
                                </div>
                                <div class="col-lg-9">
                                    <asp:TextBox ID="tbCourseStartDate" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                        </asp:Panel>

                        <br />

                        <asp:Panel ID="panelUpdateCourseDetails" Visible="false" runat="server">
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:Button ID="btnUpdateCourseDetails" CssClass="btn btn-warning btn-block" runat="server" OnClick="btnUpdateCourseDetails_Click" Text="Update Course Details" />
                                </div>
                            </div>
                        </asp:Panel>

                        <hr />


                        <%-- Interview status --%>
                        <asp:Panel ID="panelInterviewDetails" Enabled="false" runat="server">

                            <div class="row">
                                <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">
                                    <h4>
                                        <asp:Label ID="lbInterviewDetails" runat="server" Text="Interview"> </asp:Label>
                                    </h4>

                                </div>
                                <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                    <button id="btnCancelInterviewDetails" runat="server" class="btn btn-sm btn-default" visible="false" onserverclick="btnCancelInterviewDetails_ServerClick"><i class="fa fa-edit"></i>Cancel</button>
                                    <button id="btnEditInterviewDetails" runat="server" class="btn btn-sm btn-default" onserverclick="btnEditInterviewDetails_ServerClick"><i class="fa fa-edit"></i>Edit</button>

                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                                    <asp:Label ID="lbInterviewDate" runat="server" Text="Interview is scheduled on:"> </asp:Label>
                                    <asp:TextBox ID="tbInterviewDateValue" CssClass="datepickerInterview form-control" runat="server"></asp:TextBox>


                                </div>
                                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                                    <asp:Label ID="lbInterviewStatus" runat="server" Text="Status: "> </asp:Label>
                                    <br />
                                    <asp:Label ID="lbInterviewStatusValue" CssClass="btn btn-default disabled" runat="server" Text=""> </asp:Label>
                                </div>

                                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                                    <asp:Label ID="lbShortListed" CssClass="lbShortListed" runat="server" Text="Shortlisted: "> </asp:Label>
                                    <br />
                                    <div class="checkbox">

                                        <asp:CheckBox ID="cbShortlisted" Text="Tick to shortlist" runat="server" />

                                    </div>
                                </div>

                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:Button ID="btnISNotYetDone" CssClass="btn btn-sm btn-info btnInterviewStatus" OnCommand="btnUpdateInterview_Command" runat="server" Text="Not Yet Done" Width="100" />
                                    <asp:Button ID="btnISPending" CssClass="btn btn-sm btn-warning btnInterviewStatus" OnCommand="btnUpdateInterview_Command" runat="server" Text="Pending" Width="100" />
                                    <asp:Button ID="btnISPass" CssClass="btn btn-sm btn-success btnInterviewStatus" OnCommand="btnUpdateInterview_Command" runat="server" Text="Pass" Width="100" />
                                    <asp:Button ID="btnISFail" CssClass="btn btn-sm btn-danger btnInterviewStatus" OnCommand="btnUpdateInterview_Command" runat="server" Text="Fail" Width="100" />
                                    <asp:Button ID="btnNotRequired" CssClass="btn btn-sm btn-primary btnInterviewStatus" CommandName="NREQ" OnCommand="btnUpdateInterview_Command" runat="server" Text="Not Required" Width="100" />

                                </div>
                            </div>

                            <br />

                            <div class="row">
                                <div class="col-lg-12">
                                    <h5>
                                       
                                    </h5>
                                     <asp:Label ID="Label5" runat="server" Text="Interviewer"> </asp:Label>
                                    <asp:DropDownList ID="ddlInterviewer" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-12">
                                    <h5>
                                        <asp:Label ID="lbInterviewRemarks" runat="server" Text="Interview remarks"> </asp:Label>
                                    </h5>
                                    <asp:Label ID="Label7" runat="server" Text="Interview remarks"> </asp:Label>
                                    <asp:TextBox ID="tbInterviewRemarksValue" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                </div>
                            </div>

                        </asp:Panel>

                        <br />

                        <asp:Panel ID="panelUpdateInterviewDetails" Visible="false" runat="server">
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:Button ID="btnUpdateInterviewDetails" CssClass="btn btn-warning btn-block" runat="server" OnClick="btnUpdateInterviewDetails_Click" Text="Update Interview Details" />
                                </div>
                            </div>
                        </asp:Panel>


                        <hr />

                        <%-- Exempted modules --%>
                        <div class="row">
                            <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">
                                <h4>
                                    <asp:Label ID="lbExemptedModule" runat="server" Text="Exempted Module"> </asp:Label>
                                </h4>

                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                <button id="btnExemptedModule" onserverclick="btnExemptedModule_ServerClick" runat="server" class="btn btn-sm btn-default"><i class="fa fa-edit"></i>Edit</button>

                            </div>
                        </div>

                        <br />

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
                                <div class="text-center">
                                    <asp:Label ID="lbNoExemptedModuleMsg" runat="server" Visible="false" Text="No exempted module"> </asp:Label>
                                </div>
                            </div>
                        </div>

                        <br />

                        <hr />

                        <%-- Applicant's remarks --%>
                        <div class="row">
                            <div class="col-lg-12">
                                <h4>
                                    <asp:Label ID="lbApplicantRemarks" runat="server" Text="Applicant Remarks"> </asp:Label>
                                </h4>
                            </div>
                        </div>

                        <br />

                        <div class="row">
                            <div class="col-lg-12">
                                <asp:TextBox ID="tbApplicantRemarksValue" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                            </div>
                        </div>

                        <br />

                        <asp:Panel ID="panelSaveRemarks" runat="server">
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:Button ID="btnSaveRemarks" CssClass="btn btn-warning btn-block" OnClick="btnSaveRemarks_Click" runat="server" Text="Save Remarks" />
                                </div>
                            </div>
                        </asp:Panel>


                        <br />

                        <hr />

                        <%-- Payment --%>

                        <div class="row">
                            <div class="col-lg-12">
                                <h4>
                                    <asp:HiddenField ID="Hdf_paymentSeperated" runat="server" />
                                    <asp:Label ID="lbPayment" runat="server" Text="Payments"> </asp:Label>

                                    &nbsp;
                                  
                                        <asp:CheckBox ID="cb_combinepayment" runat="server" Text="&lt;small&gt;(Combine both fees)&lt;/small&gt;" AutoPostBack="True" OnCheckedChanged="cb_combinepayment_CheckedChanged" Visible="False" />
                                    <%--<asp:CheckBox ID="cb_seperatePayment" runat="server" AutoPostBack="True" OnCheckedChanged="cb_seperatePayment_CheckedChanged" Text="&lt;small&gt;(Seperate both fees)&lt;/small&gt;" Visible="False" />--%>
                                </h4>
                            </div>
                        </div>

                     <%--   <asp:UpdatePanel ID="UpdatePanel2" runat="server">--%>
                         <%--   <ContentTemplate>--%>
                                <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                                    <asp:View ID="View1" runat="server">

                                        <div class="row">
                                            <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">




                                                <asp:Label ID="lbRegistrationFee" runat="server" Text="Registration Fee" Font-Bold="true"> </asp:Label>
                                                &nbsp;<br />

                                                <asp:Label ID="lbRegistrationFeeStatus" runat="server" Text="Incomplete"> </asp:Label>

                                            </div>
                                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                                <asp:Button ID="btnViewRegistationFeeReceipt" runat="server" OnClick="btnViewRegistationFeeReceipt_Click" CssClass="btn btn-info" Visible="false" Text="View Receipt" />
                                                <asp:Button ID="btnPayRegistrationFee" runat="server" OnClick="btnPayRegistrationFee_Click" CssClass="btn btn-default" Text="Payment" />
                                                <br />
                                                <asp:HiddenField ID="hdfRegistrationPaymentId" runat="server" />
                                                <small>
                                                    <asp:Label ID="lbRegistrationFeeMsg" runat="server" Text="(For registration fee)"> </asp:Label>
                                                </small>

                                            </div>
                                        </div>
                                        <br />
                                        <div class="row">
                                            <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">

                                                <asp:Label ID="lbCourseFee" runat="server" Text="Programme Fee" Font-Bold="true"> </asp:Label>

                                                <br />
                                                <asp:Label ID="lbCourseFeeStaus" runat="server" Text="Incomplete"> </asp:Label>

                                            </div>
                                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                                <asp:Button ID="btnCourseFeeReceipt" runat="server" CssClass="btn btn-info" Visible="false" Text="View Receipt" OnClick="btnCourseFeeReceipt_Click" />
                                                <asp:Button ID="btnPayCourseFee" runat="server" OnClick="btnPayCourseFee_Click" CssClass="btn btn-default" Text="Payment" Enabled="True" />
                                                <br />
                                                <asp:HiddenField ID="hdf_selctedPaymentIdForCourseFee" runat="server" />
                                                <small>
                                                    <asp:Label ID="lbCoureFeeMsg" runat="server" Text="(For programme fee)"> </asp:Label>
                                                </small>
                                            </div>
                                        </div>
                                    </asp:View>
                                    <asp:View ID="View2" runat="server">
                                        <%-- creating a payment button that includes ONLY course fees for short course. This button will redirect user to  applicant-registration-course-payment.aspx--%>
                                        <div class="row">
                                            <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">

                                                <asp:Label ID="lblRnCFee" runat="server" Text="Programme Fee" Font-Bold="True"></asp:Label>

                                                <br />
                                                <asp:Label ID="lblCourseFeeOnlyStatus" runat="server" Text="Incomplete"></asp:Label>

                                                <asp:HiddenField ID="HiddenField1" runat="server" />

                                            </div>
                                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                                <asp:Button ID="btnCourseFeeReceiptOnly" runat="server" CssClass="btn btn-info" Text="View Receipt" OnClick="btnCourseFeeReceiptOnly_Click" Visible="False" />
                                                <asp:Button ID="btnCourseFeeOnly" runat="server" OnClick="btnCourseFeeOnly_Click" Visible="True" CssClass="btn btn-default" Text="Payment" />
                                                <br />
                                                <small>
                                                    <asp:Label ID="Label3" runat="server" Text="(For programme fee)"></asp:Label>
                                                </small>
                                            </div>
                                        </div>
                                    </asp:View>
                                    <asp:View ID="View3" runat="server">
                                        <%-- creating a payment button that includes both course fees and registration fee for FQ. This button will redirect user to  applicant-registration-and-course-payment.aspx--%>
                                        <div class="row">
                                            <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">

                                                <br />

                                                <asp:Label ID="lblRnC" runat="server" Text="Registration &amp; Course Fees" Font-Bold="True"></asp:Label>

                                                <br />
                                                <asp:Label ID="lblRnCStatus" runat="server" Text="Incomplete"></asp:Label>

                                            </div>
                                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                                <asp:Button ID="btnRnCReceipt" runat="server" CssClass="btn btn-info" Text="View Receipt" Visible="False" OnClick="btnRnCReceipt_Click" />
                                                <asp:Button ID="btnRnCPayment" runat="server" Visible="True" CssClass="btn btn-default" Text="Payment" OnClick="btnRnCPayment_Click" />
                                                <br />
                                                <small>(<asp:Label ID="Label4" runat="server" Text="For full payment"></asp:Label>)
                                                </small>
                                            </div>
                                        </div>




                                    </asp:View>
                                </asp:MultiView>
                 <%--           </ContentTemplate>
                        </asp:UpdatePanel>--%>



                        <%--<div class="row">
                            <div class="col-lg-12">
                                <h4>
                                    <asp:Label ID="lbPayment" runat="server" Text="Payments"> </asp:Label>
                                </h4>
                            </div>
                        </div>

                        <br />

                        <div class="row">
                            <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">

                                <asp:Label ID="lbRegistrationFee" runat="server" Text="Registration Fee" Font-Bold="true"> </asp:Label>
                                <br />

                                <asp:Label ID="lbRegistrationFeeStatus" runat="server" Text="Incomplete"> </asp:Label>

                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                <asp:Button ID="btnViewRegistationFeeReceipt" runat="server" OnClick="btnViewRegistationFeeReceipt_Click" CssClass="btn btn-info" Visible="false" Text="View Receipt" />
                                <asp:Button ID="btnPayRegistrationFee" runat="server" OnClick="btnPayRegistrationFee_Click" Visible="false" CssClass="btn btn-default" Text="Make Payment" />
                                <br />
                                <asp:HiddenField ID="hdfRegistrationPaymentId" runat="server" />
                                <small>
                                    <asp:Label ID="lbRegistrationFeeMsg" runat="server" Text="(For registration fee)"> </asp:Label>
                                </small>

                            </div>
                        </div>

                        <br />


                        <div class="row">
                            <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">

                                <asp:Label ID="lbCourseFee" runat="server" Text="Course Fee" Font-Bold="true"> </asp:Label>

                                <br />
                                <asp:Label ID="lbCourseFeeStaus" runat="server" Text="Incomplete"> </asp:Label>

                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                <asp:Button ID="btnCourseFeeReceipt" runat="server" CssClass="btn btn-default" Visible="false" Text="View Receipt" />
                                <asp:Button ID="btnPayCourseFee" runat="server" OnClick="btnPayCourseFee_Click" Visible="false" CssClass="btn btn-default" Text="Make Payment" />
                                <br />
                                <small>
                                    <asp:Label ID="lbCoureFeeMsg" runat="server" Text="(For course fee)"> </asp:Label>
                                </small>
                            </div>
                        </div>--%>

                        <%--<br />--%>

                        <%-- Receipts --%>
                        <%--<div class="row">
                            <div class="col-lg-12">
                                <h4>
                                    <asp:Label ID="lbAllReceipt" runat="server" Text="Receipt"> </asp:Label>
                                </h4>
                            </div>
                        </div>--%>

                        <%--<hr />
                        <asp:Repeater ID="rptReceipt" runat="server">
                            <ItemTemplate>
                                <div class="row">

                                    <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">
                                        <asp:Label ID="lbReceiptItem" runat="server" Text="Receipt: "> </asp:Label>
                                        <asp:LinkButton ID="btnReceipt" CommandArgument='<%#Eval("paymentId") %>' OnClick="btnReceipt_Click" runat="server">
                                                <%#Eval("receiptNumber") %>
                                        </asp:LinkButton>

                                        <small>
                                            <div>Paid on <%#Eval("lastModifiedDate") %></div>
                                        </small>
                                    </div>
                                    <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                        <%#Eval("paymentAmount", "{0:C}") %>
                                    </div>

                                </div>

                                <br />
                            </ItemTemplate>
                        </asp:Repeater>--%>
                        <br />

                        <div class="row">
                            <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">
                                <asp:Label ID="lbTotalAmountPaid" runat="server" Text="Total Amount Paid" Font-Bold="true"> </asp:Label>
                                <br />
                                <small>
                                    <asp:Label ID="Label2" runat="server" Text="(For programme fee)"></asp:Label>
                                </small>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                <asp:Label ID="lbTotalAmountPaidValue" runat="server" Text="0.00"> </asp:Label>
                            </div>
                        </div>

                        <br />

                        <div class="row">
                            <div class="col-lg-8 col-md-8 col-sm-8 col-xs-8">
                                <asp:Label ID="lbOutstandingAmount" runat="server" Text="Outstanding Amount" Font-Bold="true"> </asp:Label>
                                <br />
                                <small>
                                    <asp:Label ID="Label1" runat="server" Text="(For programme fee)"></asp:Label>
                                </small>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-4 col-xs-4 text-right">
                                <asp:Label ID="lbOutstandingAmountValue" runat="server" Text="0.00"> </asp:Label>

                            </div>
                        </div>

                        <br />

                        <div class="row">
                            <div class="col-lg-12 text-right">

                                <asp:LinkButton ID="btnRejectAppcalicantBottom" runat="server" class="btn btn-sm btn-danger" OnClick="btnRejectAppcalicant_Click"><i class="fa fa-times"></i>Reject</asp:LinkButton>
                                <asp:LinkButton ID="lkbtnEnrollApplicantBottom" OnClick="lkbtnEnrollApplicant_Click" CssClass="btn btn-sm btn-info" runat="server"><i class="fa fa-sign-in"></i>Enroll</asp:LinkButton>

                            </div>
                        </div>
                        <br />
                        <br />
                    </div>

                </div>
      <%--      </ContentTemplate>
        </asp:UpdatePanel>--%>


    </div>

</asp:Content>
