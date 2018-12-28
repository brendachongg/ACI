<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="batch-creation.aspx.cs" Inherits="ACI_TMS.batch_creation" %>

<%@ Register Src="~/venue-search.ascx" TagPrefix="uc1" TagName="venuesearch" %>
<%@ Register Src="~/enrollment-letter-legend.ascx" TagPrefix="uc1" TagName="enrollmentletterlegend" %>


<%@ Import Namespace="GeneralLayer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function promptConfirmation() {
            $('#diagClearDay').modal('show');
        }

        function validateDayPeriod(oSrc, args) {
            var t = $('#<%=ddlClsType.ClientID%> option:selected').val();
            var d = $('#<%=ddlDay.ClientID%> option:selected').val();
            var p = $('#<%=ddlPeriod.ClientID%> option:selected').val();
            
            if ((t == "<%=ClassType.WDY_E.ToString().Replace("_", "/")%>" && (d == "6" || d == "7" || p != "<%=DayPeriod.EVE.ToString()%>")) ||
                (t == "<%=ClassType.WDY_D.ToString().Replace("_", "/")%>" && (d == "6" || d == "7" || p == "<%=DayPeriod.EVE.ToString()%>")) ||
                (t == "<%=ClassType.SAT_E.ToString().Replace("_", "/")%>" && (d != "6" || p != "<%=DayPeriod.EVE.ToString()%>")) ||
                (t == "<%=ClassType.SAT_D.ToString().Replace("_", "/")%>" && (d != "6" || p == "<%=DayPeriod.EVE.ToString()%>")) ||
                (t == "<%=ClassType.SUN_E.ToString().Replace("_", "/")%>" && (d != "7" || p != "<%=DayPeriod.EVE.ToString()%>")) ||
                (t == "<%=ClassType.SUN_D.ToString().Replace("_", "/")%>" && (d != "7" || p == "<%=DayPeriod.EVE.ToString()%>")) ||
                (t == "<%=ClassType.WEN_E.ToString().Replace("_", "/")%>" && ((d != "6" && d != "7") || p != "<%=DayPeriod.EVE.ToString()%>")) ||
                (t == "<%=ClassType.WEN_D.ToString().Replace("_", "/")%>" && ((d != "6" && d != "7") || p == "<%=DayPeriod.EVE.ToString()%>"))) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validateModuleDate(oSrc, args) {
            var startStr = $('#<%=tbModStartDate.ClientID%>').val();
            var endStr = $('#<%=tbModEndDate.ClientID%>').val();
            if (startStr == "" || endStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(startStr) || !isValidDate(endStr)) {
                args.IsValid = false;
                return false;
            }

            var startDt = new Date(startStr);
            var endDt = new Date(endStr);
            if (endDt < startDt) {
                args.IsValid = false;
                return false;
            }

            //check module date within commencement date
            startStr = $('#<%=tbBatchStartDate.ClientID%>').val();
            endStr = $('#<%=tbBatchEndDate.ClientID%>').val();
            if (startStr == "" || endStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(startStr) || !isValidDate(endStr)) {
                args.IsValid = false;
                return false;
            }

            var batchStartDt = new Date(startStr);
            var batchEndDt = new Date(endStr);
            if (startDt < batchStartDt || startDt > batchEndDt || endDt < batchStartDt || endDt > batchEndDt) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validateRegistrationDate(oSrc, args) {
            var startStr = $('#<%=tbRegStartDate.ClientID%>').val();
            var endStr = $('#<%=tbRegEndDate.ClientID%>').val();
            if (startStr == "" || endStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(startStr) || !isValidDate(endStr)) {
                args.IsValid = false;
                return false;
            }

            var startDt = new Date(startStr);
            var endDt = new Date(endStr);
            if (endDt < startDt) {
                args.IsValid = false;
                return false;
            }

            //registration cannot be earlier than current date
            //TODO: enable validation
            //var today = new Date();
            //if (today > startDt) {
            //    args.IsValid = false;
            //    return false;
            //}

            args.IsValid = true;
            return true;
        }

        function validateCommencementDate(oSrc, args) {
            var startStr = $('#<%=tbBatchStartDate.ClientID%>').val();
            var endStr = $('#<%=tbBatchEndDate.ClientID%>').val();
            if (startStr == "" || endStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(startStr) || !isValidDate(endStr)) {
                args.IsValid = false;
                return false;
            }

            var startDt = new Date(startStr);
            var endDt = new Date(endStr);
            if (endDt < startDt) {
                args.IsValid = false;
                return false;
            }

            endStr = $('#<%=tbRegEndDate.ClientID%>').val();
            if (endStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(endStr)) {
                args.IsValid = false;
                return false;
            }

            //commencement date cannot be earlier than registration date
            var regEndDt = new Date(endStr);
            if (regEndDt > startDt) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function confirmDel(day, period) {
            $('#<%=hdSelDay.ClientID%>').val(day);
            $('#<%=hdSelPeriod.ClientID%>').val(period);
            $('#<%=hfDayMode.ClientID%>').val('DEL');
        }

        function checkChgOption() {
            if ($('#<%=hfHasDayRows.ClientID%>').val() == "1") {
                $('#diagClearDay').modal('show');
                return false;
            } else {
                setTimeout('__doPostBack(\'ctl00$ContentPlaceHolder1$cbSame\',\'\')', 0);
                return true;
            }
        }

        function validateVenueNShow() {
            Page_ClientValidate("venue");

            if (Page_IsValid) {
                $('#diagSearchVenue').modal('show');
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                <h3>
                    <asp:Label ID="lbBatchCreationHeader" runat="server" Font-Bold="true" Text="New Class"></asp:Label>
                </h3>
                <small>Please fill up the following
                </small>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click" CausesValidation="false"></asp:LinkButton>
            </div>
        </div>

        <hr />
        
        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <asp:ValidationSummary ID="vSummaryVenue" runat="server" CssClass="alert alert-danger alert-link" ValidationGroup="venue" HeaderText="Please correct the following:" />
        <asp:ValidationSummary ID="vSummaryDay" runat="server" CssClass="alert alert-danger alert-link" ValidationGroup="day" HeaderText="Please correct the following:" />

        <fieldset>
            <legend style="font-size: 18px;">Programme Details</legend>
            <asp:HiddenField ID="hfProgChgType" runat="server" />
            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb1" runat="server" Text="Category" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlProgrammeCategory" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue" OnSelectedIndexChanged="ddlProgrammeCategory_SelectedIndexChanged" AutoPostBack="true" CausesValidation="false"></asp:DropDownList>
                    <asp:HiddenField ID="hfNewProgCat" runat="server" />
                    <asp:HiddenField ID="hfSelProgCat" runat="server" />
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lb2" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlProgrammeLevel" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue" OnSelectedIndexChanged="ddlProgrammeLevel_SelectedIndexChanged" AutoPostBack="true" CausesValidation="false"></asp:DropDownList>
                    <asp:HiddenField ID="hfNewProgLvl" runat="server" />
                    <asp:HiddenField ID="hfSelProgLvl" runat="server" />
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-10">
                    <asp:Label ID="lb3" runat="server" Text="Programme" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlProgramme" runat="server" CssClass="form-control" Enabled="false" DataTextField="programmeTitle" DataValueField="programmeTitle" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged" AutoPostBack="true" CausesValidation="false"></asp:DropDownList>
                    <asp:HiddenField ID="hfNewProg" runat="server" />
                    <asp:HiddenField ID="hfSelProg" runat="server" />
                </div>

                <div class="col-lg-2">
                    <asp:Label ID="lb4" runat="server" Text="Version" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlProgrammeVersion" runat="server" CssClass="form-control" Enabled="false" DataTextField="programmeVersion" DataValueField="programmeId" OnSelectedIndexChanged="ddlProgrammeVersion_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    <asp:HiddenField ID="hfNewProgVersion" runat="server" />
                    <asp:HiddenField ID="hfSelProgVersion" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvProgramme" runat="server" ErrorMessage="Programme cannot be empty." ControlToValidate="ddlProgrammeVersion" Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb5" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgrammeCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lb6" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgrammeType" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
            </div>
        </fieldset>

        <br />

        <fieldset>
            <legend style="font-size: 18px;">Bundle Details</legend>

            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb7" runat="server" Text="Bundle" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbBundle" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                    <asp:HiddenField ID="hfBundleId" runat="server" />
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lb8" runat="server" Text="Effective Date" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbBundleEffDt" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lb10" runat="server" Text="Total Cost" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbBundleCost" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-12">
                    <asp:GridView ID="gvModule" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                        CssClass="table table-striped table-bordered dataTable no-footer hover gvv">
                        <Columns>
                            <asp:BoundField HeaderText="Module ID" DataField="moduleId" />
                            <asp:BoundField HeaderText="Module Code" DataField="moduleCode" ItemStyle-Width="200px" />
                            <asp:BoundField HeaderText="Version" DataField="moduleVersion" ItemStyle-Width="80px" />
                            <asp:BoundField HeaderText="Title" DataField="moduleTitle" />
                            <asp:BoundField HeaderText="Num. Of Session" DataField="ModuleNumOfSession" ItemStyle-Width="80px" />
                        </Columns>

                    </asp:GridView>
                </div>
            </div>

        </fieldset>

        <br />

        <fieldset>
            <legend style="font-size: 18px;">Class Details</legend>

            <div class="row">
                <div class="col-lg-3">
                    <asp:Label ID="lb11" runat="server" Text="Project Code" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbProjCode" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvProjCode" Display="None" ControlToValidate="tbProjCode" runat="server" ErrorMessage="Project code cannot be empty."></asp:RequiredFieldValidator>
                </div>
                <div class="col-lg-9">
                    <asp:Label ID="lb9" runat="server" Text="Class Code" Font-Bold="true"></asp:Label>
                    <div class="input-group">
                        <asp:TextBox ID="tbBatchCode" runat="server" CssClass="form-control" MaxLength="15"></asp:TextBox>
                        <span class="input-group-addon" style="font-weight: bold;">-</span>
                        <asp:DropDownList ID="ddlClsType" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                    </div>
                    <asp:RequiredFieldValidator ID="rfvBatchCode" Display="None" ControlToValidate="tbBatchCode" runat="server" ErrorMessage="Class code cannot be empty."></asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="rfvType" Display="None" ControlToValidate="ddlClsType" runat="server" ErrorMessage="Class type cannot be empty."></asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="rfvType1" ValidationGroup="venue" Display="None" ControlToValidate="ddlClsType" runat="server" ErrorMessage="Class type cannot be empty."></asp:RequiredFieldValidator>.
                    <asp:RequiredFieldValidator ID="rfvType2" ValidationGroup="day" Display="None" ControlToValidate="ddlClsType" runat="server" ErrorMessage="Class type cannot be empty."></asp:RequiredFieldValidator>
                </div> 
            </div>

            <br />

            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb12" runat="server" Text="Registration Date" Font-Bold="true"></asp:Label>

                    <div class="input-group">
                        <asp:TextBox ID="tbRegStartDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <span class="input-group-addon" style="font-weight: bold;">to</span>
                        <asp:TextBox ID="tbRegEndDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvRegStartDate" Display="None" ControlToValidate="tbRegStartDate" runat="server" ErrorMessage="Registration start date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="rfvRegEndDate" Display="None" ControlToValidate="tbRegEndDate" runat="server" ErrorMessage="Registration end date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvRegDate" runat="server" Display="None" ControlToValidate="tbRegEndDate" ClientValidationFunction="validateRegistrationDate"
                            ErrorMessage="Registration end date cannot be earlier than start date<br/>OR date is not in dd MMM yyyy format<br/>OR cannot be earlier than or same as today" ValidateEmptyText="false"></asp:CustomValidator>
                    </div>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lb13" runat="server" Text="Commencement Date" Font-Bold="true"></asp:Label>

                    <div class="input-group">
                        <asp:TextBox ID="tbBatchStartDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <span class="input-group-addon" style="font-weight: bold;">to</span>
                        <asp:TextBox ID="tbBatchEndDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvBatchStartDate" Display="None" ControlToValidate="tbBatchStartDate" runat="server" ErrorMessage="Commencement start date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="rfvBatchEndDate" Display="None" ControlToValidate="tbBatchEndDate" runat="server" ErrorMessage="Commencement end date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvComDate" runat="server" Display="None" ControlToValidate="tbBatchEndDate" ClientValidationFunction="validateCommencementDate" ValidateEmptyText="false"
                            ErrorMessage="Commencement end date cannot be earlier than start date<br/>OR commencement start date cannot be earlier than registration end date<br/>OR date is not in dd MMM yyyy format"></asp:CustomValidator>

                        <asp:RequiredFieldValidator ID="rfvBatchStartDate1" ValidationGroup="venue" Display="None" ControlToValidate="tbBatchStartDate" runat="server" ErrorMessage="Commencement start date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="rfvBatchEndDate1" ValidationGroup="venue" Display="None" ControlToValidate="tbBatchEndDate" runat="server" ErrorMessage="Commencement end date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvComDate1" runat="server" ValidationGroup="venue" Display="None" ControlToValidate="tbBatchEndDate" ClientValidationFunction="validateCommencementDate" ValidateEmptyText="false"
                            ErrorMessage="Commencement end date cannot be earlier than start date<br/>OR commencement start date cannot be earlier than registration end date<br/>OR date is not in dd MMM yyyy format"></asp:CustomValidator>
                    </div>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb14" runat="server" Text="Capacity" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbCapacity" runat="server" CssClass="form-control" MaxLength="3"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCapacity" Display="None" ControlToValidate="tbCapacity" runat="server" ErrorMessage="Capacity cannot be empty."></asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="rfvCapacity1" ValidationGroup="venue" Display="None" ControlToValidate="tbCapacity" runat="server" ErrorMessage="Capacity cannot be empty."></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revCapacity" ControlToValidate="tbCapacity" runat="server" ErrorMessage="Capacity must be a positive, non zero, whole number." Display="None" ValidationExpression="^[1-9]\d*$"></asp:RegularExpressionValidator>
                    <asp:RegularExpressionValidator ID="revCapacity1" ValidationGroup="venue" ControlToValidate="tbCapacity" runat="server" ErrorMessage="Capacity must be a positive, non zero, whole number." Display="None" ValidationExpression="^[1-9]\d*$"></asp:RegularExpressionValidator>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lb15" runat="server" Text="Mode" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlMode" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvMode" Display="None" ControlToValidate="ddlMode" runat="server" ErrorMessage="Mode cannot be empty."></asp:RequiredFieldValidator>
                </div>
            </div>
        </fieldset>

        <br />

        <fieldset>
            <legend style="font-size: 18px;">Session Details</legend>

            <div class="alert alert-danger" id="panelSessionError" runat="server" visible="false">
                <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                <asp:Label ID="lblSessionError" runat="server" CssClass="alert-link"></asp:Label>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <label>
                        <asp:CheckBox ID="cbSame" Checked="true" runat="server" OnCheckedChanged="cbSame_CheckedChanged" onClick="return checkChgOption();" />&nbsp;Applies to all modules</label>
                </div>
            </div>
            <div class="row" id="panelModule" runat="server" visible="false">
                <br />
                <div class="col-lg-6">
                    <asp:Label ID="lb16" runat="server" Text="Module" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlModule" runat="server" CssClass="form-control" DataValueField="moduleId" DataTextField="moduleTitle" OnSelectedIndexChanged="ddlModule_SelectedIndexChanged" AutoPostBack="true" CausesValidation="false"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvModule" ControlToValidate="ddlModule" runat="server" ErrorMessage="Module cannot be empty." ValidationGroup="day" Display="None"></asp:RequiredFieldValidator>
                </div>
                <div class="col-lg-6">
                     <asp:Label ID="lb17" runat="server" Text="Date" Font-Bold="true"></asp:Label>
                    <div class="input-group">
                        <asp:TextBox ID="tbModStartDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <span class="input-group-addon" style="font-weight: bold;">to</span>
                        <asp:TextBox ID="tbModEndDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvModStartDate" ValidationGroup="day" Display="None" ControlToValidate="tbModStartDate" runat="server" ErrorMessage="Module start date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="rfvModEndDate" ValidationGroup="day" Display="None" ControlToValidate="tbModEndDate" runat="server" ErrorMessage="Module end date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvModDate" ValidationGroup="day" runat="server" Display="None" ControlToValidate="tbModEndDate" ClientValidationFunction="validateModuleDate" ValidateEmptyText="false"
                            ErrorMessage="Class end date cannot be earlier than start date<br/>OR Class start and end date is not within commencement start and end date<br/>OR date is not in dd MMM yyyy format"></asp:CustomValidator>
                    </div>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-3">
                    <asp:Label ID="lb18" runat="server" Text="Day" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlDay" runat="server" CssClass="form-control">
                        <asp:ListItem Value="">--Select--</asp:ListItem>
                        <asp:ListItem Value="1">Monday</asp:ListItem>
                        <asp:ListItem Value="2">Tuesday</asp:ListItem>
                        <asp:ListItem Value="3">Wednesday</asp:ListItem>
                        <asp:ListItem Value="4">Thursday</asp:ListItem>
                        <asp:ListItem Value="5">Friday</asp:ListItem>
                        <asp:ListItem Value="6">Saturday</asp:ListItem>
                        <asp:ListItem Value="7">Sunday</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvDay" ControlToValidate="ddlDay" runat="server" ErrorMessage="Day cannot be empty." ValidationGroup="venue" Display="None"></asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="rfvDay1" ControlToValidate="ddlDay" runat="server" ErrorMessage="Day cannot be empty." ValidationGroup="day" Display="None"></asp:RequiredFieldValidator>

                    <asp:CustomValidator ID="cvDay" runat="server" ValidationGroup="venue" Display="None" ControlToValidate="ddlDay" ClientValidationFunction="validateDayPeriod"
                        ErrorMessage="Selected day and/or period does not match selected class type" ValidateEmptyText="false"></asp:CustomValidator>
                    <asp:CustomValidator ID="cvDay1" runat="server" ValidationGroup="day" Display="None" ControlToValidate="ddlDay" ClientValidationFunction="validateDayPeriod"
                        ErrorMessage="Selected day and/or period does not match selected class type" ValidateEmptyText="false"></asp:CustomValidator>
                </div>
                <div class="col-lg-3">
                    <asp:Label ID="lb19" runat="server" Text="Period" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlPeriod" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvPeriod" ControlToValidate="ddlPeriod" runat="server" ErrorMessage="Period cannot be empty." ValidationGroup="venue" Display="None"></asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="rfvPeriod1" ControlToValidate="ddlPeriod" runat="server" ErrorMessage="Period cannot be empty." ValidationGroup="day" Display="None"></asp:RequiredFieldValidator>
                </div>
                <div class="col-lg-6">
                    <div>
                        <asp:Label ID="lb20" runat="server" Text="Venue" Font-Bold="true"></asp:Label>
                    </div>
                    <div class="inner-addon right-addon">
                        <i class="glyphicon glyphicon-search" style="cursor: pointer;" onclick="validateVenueNShow()" id="lbtnSearchVenue" runat="server"></i>
                        <asp:TextBox ID="tbVenue" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        <asp:HiddenField ID="hfVenueId" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvVenue" ControlToValidate="tbVenue" runat="server" ErrorMessage="Venue cannot be empty." ValidationGroup="day" Display="None"></asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>

            <br />

            <div class="row" id="panelVenueBooking" runat="server">
                <div class="col-lg-12">
                    <asp:GridView ID="gvVenueBooking" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                        CssClass="table table-striped table-bordered dataTable no-footer hover gvv">
                        <Columns>
                            <asp:BoundField HeaderText="Date" DataField="dtDisp" ItemStyle-Width="200px" />
                            <asp:BoundField HeaderText="Period" DataField="periodDisp" ItemStyle-Width="100px" />
                            <asp:BoundField HeaderText="Venue Status" DataField="status" />
                        </Columns>

                    </asp:GridView>
                    <br />
                </div>
            </div>

            <div class="row text-right">
                <div class="col-lg-12">
                    <asp:Button ID="btnAddDay" runat="server" CssClass="btn btn-info" Text="Add" ValidationGroup="day" OnClick="btnAddDay_Click" />
                    <asp:Button ID="btnClearDay" runat="server" CssClass="btn btn-default" Text="Clear" CausesValidation="false" OnClick="btnClearDay_Click" />
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-12">
                    <asp:GridView ID="gvDay" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" OnRowDataBound="gvDay_RowDataBound"
                        CssClass="table table-striped table-bordered dataTable no-footer hover gvv">
                        <EmptyDataTemplate>
                            No available record
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField HeaderText="ModuleId" DataField="moduleId" />
                            <asp:BoundField HeaderText="Module" DataField="moduleTitle" />
                            <asp:BoundField HeaderText="Start" DataField="moduleStartDt" ItemStyle-Width="100px" dataformatstring="{0:dd MMM yyyy}" />
                            <asp:BoundField HeaderText="End" DataField="moduleEndDt" ItemStyle-Width="100px" dataformatstring="{0:dd MMM yyyy}" />
                            <asp:BoundField HeaderText="Day" DataField="dayDisp" ItemStyle-Width="100px" />
                            <asp:BoundField HeaderText="Period" DataField="periodDisp" ItemStyle-Width="100px" />
                            <asp:BoundField HeaderText="Venue" DataField="venueLocation" />
                            <asp:TemplateField ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <button id="btnDelDay" runat="server" type="button" class="btn btn-info" data-toggle="modal" data-target="#diagRemDay">Remove</button>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <asp:HiddenField ID="hfHasDayRows" runat="server" Value="0" />
                </div>
            </div>
        </fieldset>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnSessions" runat="server" CssClass="btn btn-primary" Text="Schedule Sessions" OnClick="btnSessions_Click" />
                <button type="button" class="btn btn-default" data-toggle="modal" data-target="#diagClearAll">Clear</button>
            </div>
        </div>

    </div>

    <asp:HiddenField ID="hdSelDay" runat="server" />
    <asp:HiddenField ID="hdSelPeriod" runat="server" />
    <div id="diagRemDay" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Remove Day</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to remove selected record?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnRemDay" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnRemDay_Click" CausesValidation="false" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="diagClearDay" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Clear Session</h4>
                </div>
                <div class="modal-body">
                    Changing this option will result in clearing existing session data, proceed?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnClearAllDay" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnClearAllDay_Click" CausesValidation="false" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="diagClearAll" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Clear All Data</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to clear all data?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnClear" runat="server" CssClass="btn btn-primary" Text="Clear" CausesValidation="false" OnClick="btnClear_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfDayMode" runat="server" />
    
    <uc1:enrollmentletterlegend runat="server" ID="enrollmentletterlegend" />
    <uc1:venuesearch runat="server" ID="venuesearch" />
</asp:Content>
