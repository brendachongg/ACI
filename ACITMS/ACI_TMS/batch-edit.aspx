<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="batch-edit.aspx.cs" Inherits="ACI_TMS.batch_edit" %>

<%@ Register Src="~/venue-search.ascx" TagPrefix="uc1" TagName="venuesearch" %>
<%@ Register Src="~/enrollment-letter-legend.ascx" TagPrefix="uc1" TagName="enrollmentletterlegend" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function setSelValue(hf, val) {
            $('#' + hf).val(val);
        }

        function closeErrorAlert() {
            $('#panelError').addClass("hidden");
        }

        function showModule(tabId) {
            $("#" + tabId).attr("class", "tab-pane fade in active");
            $('.nav-tabs a[href="#' + tabId + '"]').tab('show');
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

        function validateSessionInput(namingPrefix, index, module, session) {
            if ($('#' + namingPrefix + '_tbSessionDt' + '_' + index).val().length == 0 ||
                $('#' + namingPrefix + '_ddlSessionPeriod' + '_' + index + ' option:selected').val().length == 0) {
                $('#lblError').html("Session date and/or period cannot be empty.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            if (!isValidDate($('#' + namingPrefix + '_tbSessionDt' + '_' + index).val())) {
                $('#lblError').html("Invalid session date.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            //check capacity
            if (isNaN($('#<%=tbCapacity.ClientID%>').val())) {
                $('#lblError').html("Invalid capacity.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            //check if within batch dates
            var strStart = $('#<%=tbBatchStartDate.ClientID%>').val();
            var strEnd = $('#<%=tbBatchEndDate.ClientID%>').val();
            if (strStart == "" || strEnd == "") {
                $('#lblError').html("Commencement date cannot be empty.");
                $('#panelError').removeClass("hidden");
                return false;
            }
            if (!isValidDate(strStart) || !isValidDate(strEnd)) {
                $('#lblError').html("Invalid commencement date.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            var startDt = new Date(strStart);
            var endDt = new Date(strEnd);
            var sessDt = new Date($('#' + namingPrefix + '_tbSessionDt' + '_' + index).val());
            if (sessDt < startDt || sessDt > endDt) {
                $('#lblError').html("Session date cannot be earlier or later than commencement date.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            return true;
        }

        function showVenueDialog(namingPrefix, index, module, session) {
            if (!validateSessionInput(namingPrefix, index, module, session)) return;

            $('#<%=hfVenueSelModule.ClientID %>').val(module);
            $('#<%=hfVenueSelSession.ClientID %>').val(session);
            $('#diagSearchVenue').modal('show');
        }

        function refreshVenueAvailability(namingPrefix, index, module, session) {
            if (!validateSessionInput(namingPrefix, index, module, session)) return;

            if ($('#' + namingPrefix + '_tbSessionVenue_' + index).val().length == 0) {
                $('#lblError').html("Session value cannot be empty.");
                $('#panelError').removeClass("hidden");
                return;
            }

            $('#<%=hfSelVenueId.ClientID %>').val($('#' + namingPrefix + '_hfSessionVenueId_' + index).val());
            $('#<%=hfSelVenueLoc.ClientID %>').val($('#' + namingPrefix + '_tbSessionVenue_' + index).val());
            $('#<%=hfVenueSelModule.ClientID %>').val(module);
            $('#<%=hfVenueSelSession.ClientID %>').val(session);
            WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("ctl00$ContentPlaceHolder1$lbtnRefreshVenue", "", false, "", "", false, true));
        }

        function postponeSession(namingPrefix, index, module, session) {
            if (!validateSessionInput(namingPrefix, index, module, session)) return;

            if ($('#' + namingPrefix + '_tbSessionVenue_' + index).val().length == 0) {
                $('#lblError').html("Session venue cannot be empty.");
                $('#panelError').removeClass("hidden");
                return;
            }

            $('#<%=hfSelVenueId.ClientID %>').val($('#' + namingPrefix + '_hfSessionVenueId_' + index).val());
            $('#<%=hfSelVenueLoc.ClientID %>').val($('#' + namingPrefix + '_tbSessionVenue_' + index).val());
            $('#<%=hfSessionSelModule.ClientID %>').val(module);
            $('#<%=hfSessionSelSession.ClientID %>').val(session);
            WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("ctl00$ContentPlaceHolder1$lbtnPostpone", "", false, "", "", false, true));
        }

        function validateSessions(oSrc, args) {
            var tmp = $('#<%=hfSessionNamingContainer.ClientID %>').val();
            var containers = tmp.split(";");

            tmp = $('#<%=hfSessionCount.ClientID %>').val();
            var sessCnts = tmp.split(";");

            if (containers.length != sessCnts.length) {
                args.IsValid = false;
                return false;
            }

            var strStart = $('#<%=tbBatchStartDate.ClientID%>').val();
            var strEnd = $('#<%=tbBatchEndDate.ClientID%>').val();

            var batchStartDt = new Date(strStart);
            var batchEndDt = new Date(strEnd);

            //for each session check if date and period and venue are non empty, date within the commencement date
            for (c = 0; c < containers.length; c++) {
                for (s = 0; s < sessCnts[c]; s++) {
                    if ($('#' + containers[c] + '_tbSessionDt_' + s).val().length == 0 ||
                        $('#' + containers[c] + '_ddlSessionPeriod_' + s + ' option:selected').val().length == 0 ||
                        $('#' + containers[c] + '_tbSessionVenue_' + s).val().length == 0) {
                        args.IsValid = false;
                        return false;
                    }

                    if (!isValidDate($('#' + containers[c] + '_tbSessionDt_' + s).val())) {
                        args.IsValid = false;
                        return false;
                    }

                    var dt = new Date($('#' + containers[c] + '_tbSessionDt_' + s).val());
                    if (dt < batchStartDt || dt > batchEndDt) {
                        args.IsValid = false;
                        return false;
                    }
                }
            }

            args.IsValid = true;
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbTitle" runat="server" Font-Bold="true" Text="Edit Class"></asp:Label>
                </h3>
                <small>Please update the following as needed</small>
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
        <div class="alert alert-danger" id="panelSysError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSysError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <asp:HiddenField ID="hfBatchId" runat="server" />

        <fieldset>
            <legend style="font-size: 18px;">Programme Details</legend>
            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb1" runat="server" Text="Category" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgrammeCategory" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lb2" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgrammeLevel" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-10">
                    <asp:Label ID="lb3" runat="server" Text="Programme" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgramme" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>

                <div class="col-lg-2">
                    <asp:Label ID="lb4" runat="server" Text="Version" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgrammeVersion" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
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
            <legend style="font-size: 18px;">Class Details</legend>

            <div class="row">
                <div class="col-lg-3">
                    <asp:Label ID="lb7" runat="server" Text="Class Code" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbBatchCode" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-lg-6">
                    <asp:Label ID="lb8" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbClsType" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
                <div class="col-lg-3">
                    <asp:Label ID="lb9" runat="server" Text="Project Code" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbProjCode" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvProjCode" Display="None" ControlToValidate="tbProjCode" runat="server" ErrorMessage="Project code cannot be empty."></asp:RequiredFieldValidator>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb10" runat="server" Text="Registration Date" Font-Bold="true"></asp:Label>

                    <div class="input-group">
                        <asp:TextBox ID="tbRegStartDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <span class="input-group-addon" style="font-weight: bold;">to</span>
                        <asp:TextBox ID="tbRegEndDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvRegStartDate" Display="None" ControlToValidate="tbRegStartDate" runat="server" ErrorMessage="Registration start date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="rfvRegEndDate" Display="None" ControlToValidate="tbRegEndDate" runat="server" ErrorMessage="Registration end date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvRegDate" runat="server" Display="None" ControlToValidate="tbRegEndDate" ClientValidationFunction="validateRegistrationDate"
                            ErrorMessage="Registration end date cannot be earlier than start date<br/>OR date is not in dd MMM yyyy format" ValidateEmptyText="false"></asp:CustomValidator>
                    </div>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lb11" runat="server" Text="Commencement Date" Font-Bold="true"></asp:Label>

                    <div class="input-group">
                        <asp:TextBox ID="tbBatchStartDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <span class="input-group-addon" style="font-weight: bold;">to</span>
                        <asp:TextBox ID="tbBatchEndDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvBatchStartDate" Display="None" ControlToValidate="tbBatchStartDate" runat="server" ErrorMessage="Commencement start date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="rfvBatchEndDate" Display="None" ControlToValidate="tbBatchEndDate" runat="server" ErrorMessage="Commencement end date cannot be empty."></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvComDate" runat="server" Display="None" ControlToValidate="tbBatchEndDate" ClientValidationFunction="validateCommencementDate" ValidateEmptyText="false"
                            ErrorMessage="Commencement end date cannot be earlier than start date<br/>OR commencement start date cannot be earlier than registration end date<br/>OR date is not in dd MMM yyyy format"></asp:CustomValidator>
                    </div>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb12" runat="server" Text="Capacity" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbCapacity" runat="server" CssClass="form-control" MaxLength="3"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCapacity" Display="None" ControlToValidate="tbCapacity" runat="server" ErrorMessage="Capacity cannot be empty."></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revCapacity" ControlToValidate="tbCapacity" runat="server" ErrorMessage="Capacity must be a positive, non zero, whole number." Display="None" ValidationExpression="^[1-9]\d*$"></asp:RegularExpressionValidator>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lb13" runat="server" Text="Mode" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlMode" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvMode" Display="None" ControlToValidate="ddlMode" runat="server" ErrorMessage="Mode cannot be empty."></asp:RequiredFieldValidator>
                </div>
            </div>
        </fieldset>

        <br />

        <fieldset>
            <legend style="font-size: 18px;">Session Details</legend>
            <a name="session" />
            <div class="alert alert-danger hidden" id="panelError">
                <a href="#session" class="close" onclick="closeErrorAlert();">&times;</a>
                <span id="lblError" class="alert-link"></span>
            </div>

            <asp:HiddenField ID="hfSelModule" runat="server" />
            <asp:Repeater ID="rpModuleTabs" runat="server">
                <HeaderTemplate>
                    <ul class="nav nav-tabs">
                </HeaderTemplate>
                <ItemTemplate>
                    <li id="tabMod" runat="server"><a data-toggle="tab" onclick="setSelValue('<%=hfSelModule.ClientID %>', '<%# Eval("moduleId") %>');"
                        href='#<%# Eval("moduleId") %>'><%# Eval("moduleCode") %></a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>

            <asp:Repeater ID="rpModuleContent" runat="server" OnItemDataBound="rpModuleContent_ItemDataBound">
                <HeaderTemplate>
                    <div class="tab-content">
                </HeaderTemplate>
                <ItemTemplate>
                    <div id='<%# Eval("moduleId") %>' class="tab-pane fade">
                        <asp:HiddenField ID="hfModuleId" runat="server" Value='<%# Eval("moduleId") %>' />
                        <asp:HiddenField ID="hfModuleTitle" runat="server" Value='<%# Eval("moduleTitle") %>' />
                        <br />
                        <h4><b><asp:Label ID="lbModuleTitle" runat="server"><%# Eval("moduleTitle") %></asp:Label></b></h4>
                        <br />
                        <div class="row">
                            <div class="col-lg-6">
                                <asp:Label ID="lb1" runat="server" Text="Day" Font-Bold="true"></asp:Label>
                                <asp:Label ID="lbDay" runat="server" CssClass="form-control" ReadOnly="true"><%# Eval("dayDisp") %></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:Label ID="lb2" runat="server" Text="Date" Font-Bold="true"></asp:Label>&nbsp;
                                <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" data-placement="top" title="Date will be automatically adjusted by system depending on the session dates."></i>

                                <div class="input-group">
                                    <asp:Label ID="lbDtFrm" runat="server" CssClass="form-control" ReadOnly="true" Text='<%# Eval("startDateDisp") %>'></asp:Label>
                                    <span class="input-group-addon" style="font-weight: bold;">to</span>
                                    <asp:Label ID="lbDtTo" runat="server" CssClass="form-control" ReadOnly="true" Text='<%# Eval("endDateDisp") %>'></asp:Label>
                                    <asp:HiddenField ID="hfDtFrm" runat="server" Value='<%# Eval("startDateDisp") %>' />
                                    <asp:HiddenField ID="hfDtTo" runat="server" Value='<%# Eval("endDateDisp") %>' />
                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-lg-4">
                                <asp:Label ID="lb3" runat="server" Text="Trainer 1" Font-Bold="true"></asp:Label>
                                <asp:DropDownList ID="ddlTrainer1" runat="server" CssClass="form-control" DataTextField="userName" DataValueField="userId"></asp:DropDownList>
                            </div>
                            <div class="col-lg-4">
                                <asp:Label ID="lb4" runat="server" Text="Trainer 2" Font-Bold="true"></asp:Label>
                                <asp:DropDownList ID="ddlTrainer2" runat="server" CssClass="form-control" DataTextField="userName" DataValueField="userId"></asp:DropDownList>
                                <asp:CompareValidator ID="cmvTrainer" runat="server" ErrorMessage="Trainer 1 cannot be the same as trainer 2." Display="None" ControlToValidate="ddlTrainer2"
                                    ControlToCompare="ddlTrainer1" Operator="NotEqual"></asp:CompareValidator>
                            </div>
                            <div class="col-lg-4">
                                <asp:Label ID="lb5" runat="server" Text="Assessor" Font-Bold="true"></asp:Label>
                                <asp:DropDownList ID="ddlAssessor" runat="server" CssClass="form-control" DataTextField="userName" DataValueField="userId"></asp:DropDownList>
                            </div>
                        </div>
                        <br />
                        <h4>Sessions (<asp:Label ID="lblNumSession" runat="server"><%# Eval("ModuleNumOfSession") %></asp:Label>)&nbsp;
                        <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" data-placement="top" title="Sessions can also be edited through the session management"></i></h4>
                        <asp:HiddenField ID="hfNumSession" runat="server" Value='<%# Eval("ModuleNumOfSession") %>' />
                        <asp:Repeater ID="rpSessions" runat="server" OnItemDataBound="rpSessions_ItemDataBound">
                            <ItemTemplate>
                                <br />
                                <div class="row">
                                    <div class="col-lg-3">
                                        <asp:Label ID="lb1" runat="server" Text="Date" Font-Bold="true"></asp:Label>
                                        <div class="inner-addon right-addon">
                                            <asp:TextBox ID="tbSessionDt" runat="server" CssClass="datepicker form-control" Text='<%# Eval("sessionDateDisp") %>' placeholder="dd MMM yyyy"></asp:TextBox>
                                            <asp:Label class="glyphicon glyphicon-random" style="cursor: pointer;" ID="lbtnPostpone" runat="server" 
                                                data-toggle="tooltip" data-placement="top" title="Re-schedule other sessions based on this session date/period"></asp:Label>
                                            <asp:HiddenField ID="hfSessionNo" runat="server" Value='<%# Eval("sessionNo") %>' />
                                            <asp:HiddenField ID="hfSessionId" runat="server" Value='<%# Eval("sessionId") %>' />
                                        </div>
                                    </div>

                                    <div class="col-lg-3">
                                        <asp:Label ID="lb2" runat="server" Text="Period" Font-Bold="true"></asp:Label>
                                        <asp:DropDownList ID="ddlSessionPeriod" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-6">
                                        <div>
                                            <asp:Label ID="lb3" runat="server" Text="Venue" Font-Bold="true"></asp:Label>&nbsp;<asp:Label ID="lbVenueAva" runat="server"></asp:Label>
                                        </div>
                                        <div class="inner-addon right-addon">
                                            <asp:Label class="glyphicon glyphicon-search" Style="cursor: pointer;right: 23px;" ID="lbtnSearchVenue" runat="server"></asp:Label>
                                            <asp:Label class="glyphicon glyphicon-refresh" style="cursor: pointer;" ID="lbtnRefreshVenue" runat="server" 
                                            data-toggle="tooltip" data-placement="top" title="Refresh venue's availability"></asp:Label>
                                            <asp:TextBox ID="tbSessionVenue" runat="server" CssClass="form-control" ReadOnly="true" Text='<%# Eval("venueLocation") %>'></asp:TextBox>
                                            <asp:HiddenField ID="hfSessionVenueId" runat="server" Value='<%# Eval("venueId") %>' />
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </fieldset>

        <br />
        <hr />
        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnUpdate" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnUpdate_Click" />
                <button type="button" class="btn btn-default" data-toggle="modal" data-target="#diagReloadbatch">Clear</button>
                <asp:CustomValidator ID="cv" Display="None" ControlToValidate="hfSessionNamingContainer" ClientValidationFunction="validateSessions" runat="server" 
                    ErrorMessage="Session date, period and/or venue cannot be empty<br/>OR Invalid session date<br/>OR session date is not within commencement date"></asp:CustomValidator>
            </div>
        </div>
    </div>

    <div id="diagReloadbatch" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Discard Changes</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to discard all changes?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnClearBatch" runat="server" CssClass="btn btn-default" Text="OK" CausesValidation="false" OnClick="btnClearBatch_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <uc1:enrollmentletterlegend runat="server" ID="enrollmentletterlegend" />
    <uc1:venuesearch runat="server" ID="venuesearch" />
    <!---------------------hidden fields to access venue dialog---------------------------->
    <asp:HiddenField ID="hfVenueSelModule" runat="server" />
    <asp:HiddenField ID="hfVenueSelSession" runat="server" />
    <asp:HiddenField ID="hfSelVenueId" runat="server" />
    <asp:HiddenField ID="hfSelVenueLoc" runat="server" />
    <%-- this control exist in order to have a event handler for refreshing venue availability --%>
    <asp:LinkButton ID="lbtnRefreshVenue" runat="server" OnClick="lbtnRefreshVenue_Click" style="display:none;" CausesValidation="false"></asp:LinkButton>
    <!---------------------------------------------------------------------------------------------->
    <asp:TextBox ID="hfSessionNamingContainer" runat="server" style="display:none;" />
    <asp:TextBox ID="hfSessionCount" runat="server" style="display:none;" />
     <asp:HiddenField ID="hfSessionSelModule" runat="server" />
    <asp:HiddenField ID="hfSessionSelSession" runat="server" />
    <%-- this control exist in order to have a event handler for postpone schedule dates (for module that uses the same session data) --%>
    <asp:LinkButton ID="lbtnPostpone" runat="server" OnClick="lbtnPostpone_Click" style="display:none;" CausesValidation="false"></asp:LinkButton>
</asp:Content>
