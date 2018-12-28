<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="batch-session-creation.aspx.cs" Inherits="ACI_TMS.batch_session_creation" %>

<%@ Register Src="~/venue-search.ascx" TagPrefix="uc1" TagName="venuesearch" %>


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

        function validateSessionInput(namingPrefix, index, module, session) {
            if ($('#' + namingPrefix + '_tbSessionDt_' + index).val().length == 0 ||
                $('#' + namingPrefix + '_ddlSessionPeriod_' + index + ' option:selected').val().length == 0) {
                $('#lblError').html("Session date and/or period cannot be empty.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            if (!isValidDate($('#' + namingPrefix + '_tbSessionDt_' + index).val())) {
                $('#lblError').html("Invalid session date.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            //check if within batch dates
            var startDt = new Date('<%=hfBatchStartDate.Value%>');
            var endDt = new Date('<%=hfBatchEndDate.Value%>');
            var sessDt = new Date($('#' + namingPrefix + '_tbSessionDt_' + index).val());
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
                $('#lblError').html("Session venue cannot be empty.");
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

            var batchStartDt = new Date('<%=hfBatchStartDate.Value%>');
            var batchEndDt = new Date('<%=hfBatchEndDate.Value%>');

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
            <div class="col-lg-9 col-md-9 col-sm-9 col-xs-9">

                <h3>
                    <asp:Label ID="lbTitle" runat="server" Font-Bold="true" Text="Schedule Sessions"></asp:Label>
                </h3>
                <small>
                    Please fill up and verify the following.
                    <br /><span style="color:red;">Fields that are in red indicate the date has exceeded the class commencement date.</span>
                </small>
            </div>
            <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="btnBack_Click" CausesValidation="false"></asp:LinkButton>
            </div>
        </div>
        <hr />
        <div class="alert alert-danger" id="panelSysError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSysError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger hidden" id="panelError">
            <a href="#" class="close" onClick="closeErrorAlert();">&times;</a>
            <span id="lblError" class="alert-link"></span>
        </div>
        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <div class="row">
            <div class="col-lg-12">
                <asp:Label ID="lb1" runat="server" Text="Class Commencement Date:" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbBatchStartDate" runat="server"></asp:Label>&nbsp;to&nbsp;
                <asp:Label ID="lbBatchEndDate" runat="server"></asp:Label>
            </div>
        </div>
        <br />
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
                    <br />
                    <h4><b><asp:Label ID="lbModuleTitle" runat="server" ><%# Eval("moduleTitle") %></asp:Label></b></h4>
                    <asp:HiddenField ID="hfModuleTitle" runat="server" Value='<%# Eval("moduleTitle") %>' />
                    <br />
                    <div class="row">
                        <div class="col-lg-6">
                            <asp:Label ID="lb1" runat="server" Text="Day" Font-Bold="true"></asp:Label>
                            <asp:TextBox ID="tbDay" runat="server" CssClass="form-control" ReadOnly="true" Text='<%# Eval("dayDisp") %>'></asp:TextBox>
                            <asp:HiddenField ID="hfDay" runat="server" Value='<%# Eval("day") %>' />
                        </div>
                        <div class="col-lg-6">
                            <asp:Label ID="lb2" runat="server" Text="Module Date" Font-Bold="true"></asp:Label>

                            <div class="input-group">
                                <asp:TextBox ID="tbDtFrm" runat="server" CssClass="form-control" ReadOnly="true" Text='<%# Eval("startDateDisp") %>'></asp:TextBox>
                                <span class="input-group-addon" style="font-weight: bold;">to</span>
                                <asp:TextBox ID="tbDtTo" runat="server" CssClass="form-control" ReadOnly="true" Text='<%# Eval("endDateDisp") %>'></asp:TextBox>
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
                    <h4>Sessions (<asp:Label ID="lblNumSession" runat="server"><%# Eval("ModuleNumOfSession") %></asp:Label>)</h4>
                    <asp:HiddenField ID="hfNumSession" runat="server" Value='<%# Eval("ModuleNumOfSession") %>' />
                    <asp:Repeater ID="rpSessions" runat="server" OnItemDataBound="rpSessions_ItemDataBound" >
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
                                    </div>           
                                </div>

                                <div class="col-lg-3">
                                    <asp:Label ID="lb2" runat="server" Text="Period" Font-Bold="true"></asp:Label>
                                    <asp:DropDownList ID="ddlSessionPeriod" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue" ></asp:DropDownList>
                                </div>

                                <div class="col-lg-6">
                                    <div>
                                        <asp:Label ID="lb3" runat="server" Text="Venue" Font-Bold="true"></asp:Label>&nbsp;<asp:Label ID="lbVenueAva" runat="server"></asp:Label>
                                    </div>
                                    <div class="inner-addon right-addon">
                                        <asp:Label class="glyphicon glyphicon-search" style="cursor: pointer;right: 23px;" ID="lbtnSearchVenue" runat="server"></asp:Label>
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

        <hr />
        <br />
        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnCreate" runat="server" CssClass="btn btn-primary" Text="Create" OnClick="btnCreate_Click"  CausesValidation="true"  />
                <asp:CustomValidator ID="cv" Display="None" ControlToValidate="hfSessionNamingContainer" ClientValidationFunction="validateSessions" runat="server" 
                    ErrorMessage="Session date, period and/or venue cannot be empty<br/>OR Invalid session date<br/>OR session date is not within commencement date"></asp:CustomValidator>
            </div>
        </div>
    </div>

    <asp:TextBox ID="hfSessionNamingContainer" runat="server" style="display:none;" />
    <asp:TextBox ID="hfSessionCount" runat="server" style="display:none;" />

    <asp:HiddenField ID="hfBatchStartDate" runat="server" />
    <asp:HiddenField ID="hfBatchEndDate" runat="server" />
    <!---------------------hidden fields to access venue dialog---------------------------->
    <asp:HiddenField ID="hfVenueSelModule" runat="server" />
    <asp:HiddenField ID="hfVenueSelSession" runat="server" />
    <asp:HiddenField ID="hfSelVenueId" runat="server" />
    <asp:HiddenField ID="hfSelVenueLoc" runat="server" />
    <%-- this control exist in order to have a event handler for refreshing venue availability --%>
    <asp:LinkButton ID="lbtnRefreshVenue" runat="server" OnClick="lbtnRefreshVenue_Click" style="display:none;" CausesValidation="false"></asp:LinkButton>
    <!---------------------------------------------------------------------------------------------->
    <asp:HiddenField ID="hfSessionSelModule" runat="server" />
    <asp:HiddenField ID="hfSessionSelSession" runat="server" />
    <%-- this control exist in order to have a event handler for postpone schedule dates (for module that uses the same session data) --%>
    <asp:LinkButton ID="lbtnPostpone" runat="server" OnClick="lbtnPostpone_Click" style="display:none;" CausesValidation="false"></asp:LinkButton>

    <uc1:venuesearch runat="server" ID="venuesearch" />

    <asp:HiddenField ID="hfStatusMsg" runat="server" />
</asp:Content>
