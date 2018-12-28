<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="trainee-details.aspx.cs" Inherits="ACI_TMS.trainee_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>
        $(document).ready(function () {
            showPrev();

        });

        function validateWithdrawReason(oSrc, args) {
            if ($('#<%=tbReason.ClientID%>').val().length > 200) {
                args.IsValid = false;
                return false;
            } else {

                args.IsValid = true;
                return true;
            }
        }

        function validateContactEmail(oSrc, args) {

            if ($('#<%=tbContactNo1.ClientID%>').val().length == 0 && $('#<%=tbEmailAdd.ClientID%>').val().length == 0) {
                args.IsValid = false;
                return false;
            } else {

                args.IsValid = true;
                return true;
            }
        }

        function gen_all_es(e) {
            e.preventDefault();
            // your code
        }

        function showPrev() {
            if ($('#<%=cbPrevEmpl.ClientID%>').is(":checked")) {
                $('#divPrevEmpl').css("display", "block");
            }
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

        function validateOtherWrittenLang(OSrc, args) {
            if ($('#<%=ddlWOtherLanguage.ClientID%> option:selected').index() == 0) {
                args.IsValid = true;
                return true;
            }
            else {
                if ($('#<%=ddlWOtherLangPro.ClientID%> option:selected').index() == 0) {

                    args.IsValid = false;
                    return false;
                } else {
                    args.IsValid = true;
                    return true;
                }
            }
        }

        function validateOtherLang(oSrc, args) {

            if ($('#<%=ddlOtherLanguage.ClientID%> option:selected').index() == 0) {
                args.IsValid = true;
                return true;
            }
            else {
                if ($('#<%=ddlOtherLangPro.ClientID%> option:selected').index() == 0) {

                    args.IsValid = false;
                    return false;
                } else {
                    args.IsValid = true;
                    return true;
                }
            }
        }

        function validateId(oSrc, args) {
            if ($('#<%=ddlIdentificationType.ClientID%> option:selected').val() == '<%=(int)GeneralLayer.IDType.Oth%>') {
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

        function clearAll() {
            var panelblock = $("#<%= panelParticular.ClientID %>");
            panelblock.find('input[type=text]').val('');
            $("#<%= panelParticular.ClientID %>").find("select").val('');
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="page-wrapper">

        <div class="row text-left">
            <h3>
                <asp:Label ID="lb1" runat="server" Text="Trainee"></asp:Label>
            </h3>
        </div>

        <div class="row text-right">
            <br />
            <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click" CausesValidation="false"></asp:LinkButton>
            <button id="btnCancelTrainee" runat="server" class="btn btn-sm btn-default" visible="false" causesvalidation="false" onserverclick="btnCancelTrainee_ServerClick"><i class="fa fa-edit"></i>Cancel</button>
            <button id="btnEditTrainee" runat="server" class="btn btn-sm btn-default" visible="true" onserverclick="btnEditTrainee_ServerClick" causesvalidation="false"><i class="fa fa-edit"></i>Edit</button>
            <asp:LinkButton ID="btn_print" runat="server" OnClick="btn_print_Click" CssClass="btn btn-sm btn-success" CausesValidation="false"><i class="fa fa-print"></i>Print</asp:LinkButton>
            <button id="btnCfmWithdrawTrainee" runat="server" class="btn btn-sm btn-danger" data-toggle="modal" data-target="#diagWithdraw" onclick="gen_all_es(event)" causesvalidation="false" visible="true"><i class="glyphicon glyphicon-share"></i>Withdraw</button>


        </div>

        <br />

        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <br />
        <asp:Panel ID="panelParticular" Enabled="false" runat="server">

            <div class="row">
                <div class="col-lg-12">
                    <h4><asp:Label runat="server" ID="lblWithdrawReason" Text="" Font-Bold="true" CssClass="alert-danger"></asp:Label></h4>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-3">
                    <asp:Label runat="server" ID="lbTraineeId" Text="Trainee ID" Font-Bold="true"></asp:Label>
                    <asp:TextBox runat="server" ID="tbTraineeId" Enabled="false" CssClass="form-control"></asp:TextBox>

                </div>

                <div class="col-lg-9">
                    <asp:Label runat="server" ID="lbFullName" Text="Full Name" Font-Bold="true"></asp:Label>
                    <asp:TextBox runat="server" ID="tbFullName" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFullName" runat="server" ErrorMessage="Full Name cannot be empty" ControlToValidate="tbFullName" Display="none"></asp:RequiredFieldValidator>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lbIdNo" runat="server" Text="Identification" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbIdNo" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvId" ControlToValidate="tbIdNo" runat="server" ErrorMessage="Identification No. cannot be empty." Display="None"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cvId" runat="server" ControlToValidate="tbIdNo" Display="None"
                        ErrorMessage="Invalid identification no." ClientValidationFunction="validateId" ValidateEmptyText="false"></asp:CustomValidator>
                </div>
                <div class="col-lg-6">
                    <asp:Label ID="lbIdType" runat="server" Text="Identification Type" Font-Bold="true"></asp:Label>

                    <asp:DropDownList ID="ddlIdentificationType" runat="server" CssClass="form-control">
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="rfvIdType" ControlToValidate="ddlIdentificationType" runat="server" ErrorMessage="Identification type cannot be empty." Display="None"></asp:RequiredFieldValidator>

                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-4">
                    <asp:Label ID="lbNationality" runat="server" Text="Nationality" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlNationalityValue" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvNationality" ControlToValidate="ddlNationalityValue" runat="server" ErrorMessage="Nationality cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-4">
                    <asp:Label ID="lbRace" runat="server" Text="Race" Font-Bold="true"></asp:Label>

                    <asp:DropDownList ID="ddlRaceValue" CssClass="form-control" runat="server">
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="rfvRace" ControlToValidate="ddlRaceValue" runat="server" ErrorMessage="Race cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-4">
                    <asp:Label ID="lbGender" runat="server" Text="Gender" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlGenderValue" CssClass="form-control" runat="server">
                        <asp:ListItem Value="">--Select--</asp:ListItem>
                        <asp:ListItem Value="M">Male</asp:ListItem>
                        <asp:ListItem Value="F">Female</asp:ListItem>
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="rfvGender" ControlToValidate="ddlGenderValue" runat="server" ErrorMessage="Gender cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lbContactNo1" runat="server" Text="Contact Number 1 (Mobile)" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbContactNo1" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                    <%--          <asp:RequiredFieldValidator ID="rfvContact1" ControlToValidate="tbContactNo1" runat="server" ErrorMessage="Contact no. cannot be empty." Display="None"></asp:RequiredFieldValidator>--%>
                    <asp:RegularExpressionValidator ID="revContact1" Display="None" ValidationExpression="^\+?\d+$" runat="server" ErrorMessage="Contact number can only contain numbers."
                        ControlToValidate="tbContactNo1"></asp:RegularExpressionValidator>
                    <asp:CustomValidator ID="cvContactNo1" runat="server" ControlToValidate="tbContactNo1" Display="None" ClientValidationFunction="validateContactEmail"
                        ErrorMessage="Either Contact Number 1 or Email must be provided." ValidateEmptyText="True" OnServerValidate="isContactEmailBothEmpty"></asp:CustomValidator>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lbContactNo2" runat="server" Text="Contact Number 2 (Home/Others)" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbContactNo2" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revContact2" Display="None" ValidationExpression="^\+?\d+$" runat="server" ErrorMessage="Alternative contact no. can only contain numbers."
                        ControlToValidate="tbContactNo2"></asp:RegularExpressionValidator>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lbEmailAdd" runat="server" Text="Email Address" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbEmailAdd" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="rexEmail" Display="None" ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" runat="server" ErrorMessage="Invalid email." ControlToValidate="tbEmailAdd"></asp:RegularExpressionValidator>
                </div>

                <div class="col-lg-6">

                    <asp:Label ID="lbDOB" runat="server" Text="Date of Birth" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbDOB" runat="server" placeholder="" CssClass="datepicker form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDOB" runat="server" ErrorMessage="Date of birth cannot be empty." Display="None" ControlToValidate="tbDOB"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cvBirthDate" runat="server" ControlToValidate="tbDOB" Display="None" ClientValidationFunction="validatePastDate"
                        ErrorMessage="Date of birth is not in dd MMM yyyy format OR cannot be later than today." ValidateEmptyText="false"></asp:CustomValidator>

                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-9">
                    <asp:Label ID="lbAddress" runat="server" Text="Address Line" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbAddress" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAddr" ControlToValidate="tbAddress" runat="server" ErrorMessage="Address cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lbPostalCode" runat="server" Text="Postal Code" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbPostalCode" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPostalCode" ControlToValidate="tbPostalCode" runat="server" ErrorMessage="Postal Code cannot be empty." Display="None"></asp:RequiredFieldValidator>

                </div>
            </div>
            <br />

            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lbHighestEdu" runat="server" Text="Highest Education" Font-Bold="true"></asp:Label>

                    <asp:DropDownList ID="ddlHighestEducationValue" CssClass="form-control" runat="server"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvHighEdu" ControlToValidate="ddlHighestEducationValue" runat="server" ErrorMessage="Highest level of education cannot be empty." Display="None"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lbHighestEduRemark" runat="server" Text="Highest Education Remark" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbHighestEduRemark" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>

                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lbTextSpokenLanguage" runat="server" Text="Spoken Language" Font-Bold="true"></asp:Label>
                    <div class="row">
                        <div class="col-lg-6">
                            <asp:Label ID="lbEng" runat="server" CssClass="form-control" Text="English"></asp:Label>
                        </div>
                        <div class="col-lg-6">
                            <asp:DropDownList ID="ddlEngPro" CssClass="form-control" runat="server"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvEngSpoken" ControlToValidate="ddlEngPro" runat="server" ErrorMessage="English spoken proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-lg-6">
                            <asp:Label ID="lbChi" runat="server" CssClass="form-control" Text="Chinese"></asp:Label>
                        </div>
                        <div class="col-lg-6">
                            <asp:DropDownList ID="ddlChnPro" CssClass="form-control" runat="server"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvChnSpoken" ControlToValidate="ddlChnPro" runat="server" ErrorMessage="Chinese spoken proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                        </div>

                        <div class="col-lg-6">
                            <asp:DropDownList ID="ddlOtherLanguage" CssClass="form-control" runat="server"></asp:DropDownList>
                        </div>
                        <div class="col-lg-6">
                            <asp:DropDownList ID="ddlOtherLangPro" CssClass="form-control" runat="server">
                                <asp:ListItem Value="">--Select--</asp:ListItem>
                            </asp:DropDownList>
                            <asp:CustomValidator ID="cvOtherLang" runat="server" ControlToValidate="ddlOtherLangPro" Display="None" ClientValidationFunction="validateOtherLang"
                                ErrorMessage="Other Spoken Language Proficiency cannot be empty if you have selected other language." ValidateEmptyText="true"></asp:CustomValidator>
                        </div>

                    </div>
                </div>
                <div class="col-lg-6">
                    <asp:Label ID="lbTextWritternLang" runat="server" Text="Written Language" Font-Bold="True"></asp:Label>
                    <div class="row">
                        <div class="col-lg-6">
                            <asp:Label ID="lbWEng" runat="server" CssClass="form-control" Text="English"></asp:Label>
                        </div>
                        <div class="col-lg-6">
                            <asp:DropDownList ID="ddlWEngPro" CssClass="form-control" runat="server"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvEngWriteen" ControlToValidate="ddlWEngPro" runat="server" ErrorMessage="English written proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-lg-6">
                            <asp:Label ID="lbWChi" runat="server" CssClass="form-control" Text="Chinese"></asp:Label>
                        </div>
                        <div class="col-lg-6">
                            <asp:DropDownList ID="ddlWChiPro" CssClass="form-control" runat="server"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvChnWritten" ControlToValidate="ddlWChiPro" runat="server" ErrorMessage="Chinese written proficiency cannot be empty." Display="None"></asp:RequiredFieldValidator>
                        </div>

                        <div class="col-lg-6">
                            <asp:DropDownList ID="ddlWOtherLanguage" CssClass="form-control" runat="server"></asp:DropDownList>
                        </div>
                        <div class="col-lg-6">
                            <asp:DropDownList ID="ddlWOtherLangPro" CssClass="form-control" runat="server">
                            </asp:DropDownList>

                            <asp:CustomValidator ID="cvWrittenLang" runat="server" ControlToValidate="ddlWOtherLangPro" Display="None" ClientValidationFunction="validateOtherWrittenLang"
                                ErrorMessage="Other Written Language Proficiency cannot be empty if you have selected other language." ValidateEmptyText="true"></asp:CustomValidator>
                        </div>

                    </div>
                </div>
            </div>

            <br />

            <div class="row text-right">
                <div class="col-lg-12">
                    <asp:Button ID="btnUpdate" runat="server" class="btn btn-primary" Text="Update" OnClick="btnUpdate_Click" />
                    <asp:Button ID="btnClear" runat="server" class="btn btn-default" Text="Clear" CausesValidation="false" OnClientClick="clearAll()" />
                </div>
            </div>

        </asp:Panel>

        <div class="row text-center">


            <h3>
                <asp:Label ID="lb3" runat="server" Text="Trainee Employment History"></asp:Label>
            </h3>


        </div>
        <asp:Panel ID="pnNoEmploymentHistory" Enabled="false" runat="server">
            <div class="row">
                <div class="col-lg-12">
                    <asp:Label ID="lbNoHistory" runat="server" Text="Trainee Employment History"></asp:Label>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnEmploymentHistory" Enabled="false" runat="server" Visible="false">
            <div class="row">
                <div class="col-lg-12">
                    <p>
                        <asp:CheckBox ID="cbCurrEmpl" runat="server" Checked="false" onchange="showCurrEmpl()" />&nbsp;<label for="<%=cbCurrEmpl.ClientID %>">Current Employment</label>
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
                            <div class="col-lg-6">
                                <asp:Label ID="lb30" runat="server" Font-Bold="true" Text="Salary (S$)"></asp:Label>
                                <asp:TextBox ID="tbPrevEmplSalary" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                            <div class="col-lg-6">
                                <asp:Label ID="lb31" runat="server" Font-Bold="true" Text="Employment Date"></asp:Label>
                                <div class="inputgroup">
                                    <asp:TextBox ID="tbPrevEmplStartDt" CssClass="form-control datepicker" runat="server" placeholder="dd MMM yyyy"></asp:TextBox>
                                    <span class="input-group-addon" style="font-weight: bold;">to</span>
                                    <asp:TextBox ID="tbPrevEmplEndDt" CssClass="form-control datepicker" runat="server" placeholder="dd MMM yyyy"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </asp:Panel>

        <div class="row text-center">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">

                <h3>
                    <asp:Label ID="lb2" runat="server" Text="Enrolled Programme Details"></asp:Label>
                </h3>

            </div>
        </div>

        <asp:Panel ID="pnEnrolledDetails" Enabled="false" runat="server">
            <asp:HiddenField ID="hfBatchId" runat="server" />
            <div class="row">
                <div class="col-lg-12">
                    <asp:Label ID="lbProjCode" runat="server" Text="Project Code" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbProjCode" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="row">

                <div class="col-lg-12">
                    <asp:Label ID="lbCourseCode" runat="server" Text="Course Code" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbCourseCode" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                </div>
            </div>

            <div class="row">

                <div class="col-lg-12">
                    <asp:Label ID="lbBatchCode" runat="server" Text="Class Code" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbBatchCode" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                </div>
            </div>

            <div class="row">

                <div class="col-lg-12">
                    <asp:Label ID="lbProgrammeTitle" runat="server" Text="Programme Title" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbProgrammeTitle" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lbProgrammeStartDate" runat="server" Text="Programme Start Date" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbProgrammeStartDate" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lbProgrammeEndDate" runat="server" Text="Programme End Date" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbProgrammeEndDate" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                </div>

            </div>
        </asp:Panel>

    </div>

    <div id="diagWithdraw" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Confirm Withdraw</h4>
                </div>
                <div class="modal-body">
                    <asp:ValidationSummary ID="vsReasonSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="Reason" />
                    <br />
                    Are you sure you want to withdraw this trainee? This action cannot be reversed.
                    <br />
                    <br />
                    Enter the reason for withdrawal:
                     <asp:TextBox ID="tbReason" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvReason" runat="server" ErrorMessage="Withdrawal Reason cannot be empty" ControlToValidate="tbReason" ValidationGroup="Reason" Display="None"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cvReason" runat="server" ErrorMessage="Withdrawal Reason cannot be more than 200 characters." ClientValidationFunction="validateWithdrawReason" ControlToValidate="tbReason" Display="None" ValidationGroup="Reason"></asp:CustomValidator>

                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnWithdraw" runat="server" CssClass="btn btn-danger" Text="OK" OnClick="btnWithdraw_Click" CausesValidation="True" ValidationGroup="Reason" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
