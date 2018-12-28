<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="bundle-edit-sessions.aspx.cs" Inherits="ACI_TMS.bundle_edit_sessions" %>

<%@ Register Src="~/bundle-module-new-session.ascx" TagPrefix="uc1" TagName="bundlemodulenewsession" %>
<%@ Register Src="~/bundle-module-rem-session.ascx" TagPrefix="uc1" TagName="bundlemoduleremsession" %>
<%@ Register Src="~/venue-search.ascx" TagPrefix="uc1" TagName="venuesearch" %>
<%@ Register Src="~/day-select.ascx" TagPrefix="uc1" TagName="dayselect" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function showModule(tabId) {
            $("#" + tabId).attr("class", "tab-pane fade in active");
            $('.nav-tabs a[href="#' + tabId + '"]').tab('show');
        }

        function showBatch(pillId) {
            $("#" + pillId).attr("class", "tab-pane fade in active");
            $('.nav-pills a[href="#' + pillId + '"]').tab('show');
        }

        function showDayDialog(module, prog, batch) {
            $('#<%=hfDaySelModule.ClientID %>').val(module);
            $('#<%=hfDaySelProg.ClientID %>').val(prog);
            $('#<%=hfDaySelBatch.ClientID %>').val(batch);
            $('#diagSelectDay').modal('show');
        }

        function setSelValue(hf, val) {
            $('#' + hf).val(val);
        }



        function validateSessionInput(namingPrefix, index, module, prog, batch, session, dtStr) {
            if ($('#' + namingPrefix + '_tbNewSessionDt_' + index).val().length == 0 ||
                $('#' + namingPrefix + '_ddlNewSessionPeriod_' + index + ' option:selected').val().length == 0) {
                $('#lblError').html("Session date and/or period cannot be empty.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            if (!isValidDate($('#' + namingPrefix + '_tbNewSessionDt_' + index).val())) {
                $('#lblError').html("Invalid session date.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            var batchDt = new Date(dtStr);
            var modDt = new Date($('#' + namingPrefix + '_tbNewSessionDt_' + index).val());
            if (modDt < batchDt) {
                $('#lblError').html("Session date cannot be earlier than class commencement start date.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            return true;
        }

        function showVenueDialog(namingPrefix, index, module, prog, batch, session, dtStr) {
            if (!validateSessionInput(namingPrefix, index, module, prog, batch, session, $('#' + dtStr).val())) return;

            $('#<%=hfVenueSelModule.ClientID %>').val(module);
            $('#<%=hfVenueSelProg.ClientID %>').val(prog);
            $('#<%=hfVenueSelBatch.ClientID %>').val(batch);
            $('#<%=hfVenueSelSession.ClientID %>').val(session);
            $('#diagSearchVenue').modal('show');
        }

        function refreshVenueAvailability(namingPrefix, index, module, prog, batch, session, dtStr) {
            if (!validateSessionInput(namingPrefix, index, module, prog, batch, session, $('#' + dtStr).val())) return;

            if ($('#' + namingPrefix + '_tbNewSessionVenue_' + index).val().length == 0) {
                $('#lblError').html("Session venue cannot be empty.");
                $('#panelError').removeClass("hidden");
                return;
            }

            $('#<%=hfVenueSelModule.ClientID %>').val(module);
            $('#<%=hfVenueSelProg.ClientID %>').val(prog);
            $('#<%=hfVenueSelBatch.ClientID %>').val(batch);
            $('#<%=hfVenueSelSession.ClientID %>').val(session);

            WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("ctl00$ContentPlaceHolder1$lbtnRefreshVenue", "", false, "", "", false, true));
        }

        function checkModuleDate(dtctl, dyctl, dtStr, btnctl) {
            if ($('#' + dtctl).val().length == 0) {
                $('#lblError').html("Module start date cannot be empty.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            if ($('#' + dyctl).val().length == 0) {
                $('#lblError').html("Module day cannot be empty.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            if (!isValidDate($('#' + dtctl).val())) {
                $('#lblError').html("Invalid module date.");
                $('#panelError').removeClass("hidden");
                return;
            }

            var batchDt = new Date(dtStr);
            var modDt = new Date($('#' + dtctl).val());
            if (modDt < batchDt) {
                $('#lblError').html("Module start date cannot be earlier than class commencement start date.");
                $('#panelError').removeClass("hidden");
                return false;
            }

            __doPostBack(btnctl, '');
            return true;
        }

        function closeErrorAlert() {
            $('#panelError').addClass("hidden");
        }

        function validateAllRemSession(oSrc, args) {
            var tmp = $('#<%=hfRemSessionLblNamingContainer.ClientID %>').val();
            var LblIds = tmp.split(";");

            tmp = $('#<%=hfRemSessionGvNamingContainer.ClientID %>').val();
            var GvIds = tmp.split(";");

            tmp = $('#<%=hfRemSessionTotalNamingContainer.ClientID %>').val(); 
            var TotalIds = tmp.split(";");

            if (LblIds.length != GvIds.length || GvIds.length != TotalIds.length) {
                args.IsValid = false;
                return false;
            }
            if (LblIds.length == 0) {
                //no new session
                args.IsValid = true;
                return true;
            }

            var lblId, gvId, totalId, i, n;
            for (i = 0; i < LblIds.length; i++) {
                lblId = LblIds[i];
                gvId = GvIds[i];
                totalId = TotalIds[i];

                if (lblId.length == 0) continue;

                //get the no of new sessions
                var sessCnt = $('#' + lblId).html();
                var total = $('#' + totalId).val();
                var sel = 0;
                for (n = 0; n < total; n++) {
                    if (document.getElementById(gvId + "_cb_" + n).checked) {
                        sel++;
                    }
                }

                if (sel != sessCnt) {
                    args.IsValid = false;
                    return false;
                }
            }

            args.IsValid = true;
            return true;
        }

        function validateAllNewSession(oSrc, args) {
            var tmp = $('#<%=hfNewSessionLblNamingContainer.ClientID %>').val();
            var LblIds = tmp.split(";");

            tmp = $('#<%=hfNewSessionRpNamingContainer.ClientID %>').val();
            var RpIds = tmp.split(";");

            tmp = $('#<%=hfNewSessionDtNamingContainer.ClientID%>').val()
            var DtIds = tmp.split(";");

            if (LblIds.length != RpIds.length || RpIds.length != DtIds.length) {
                args.IsValid = false;
                return false;
            }
            if (LblIds.length == 0) {
                //no new session
                args.IsValid = true;
                return true;
            }

            var lblId, rpId, dtId, i, n;
            var dtStart, dtEnd, dtSession;
            for (i = 0; i < LblIds.length; i++)
            {
                lblId = LblIds[i];
                rpId = RpIds[i];
                dtId = DtIds[i];

                if (lblId.length == 0) continue;

                dtStart = new Date($('#' + dtId + '_lbCommDtFrm_' + i).html());
                dtEnd = new Date($('#' + dtId + '_lbCommDtTo_' + i).html());

                //get the no of new sessions
                var sessCnt = $('#' + lblId).html();
                //for every new session check if fields are filled
                for (n = 0; n < sessCnt; n++) {
                    if ($('#' + rpId + '_tbNewSessionDt_' + n).val().length == 0 ||
                        $('#' + rpId + '_ddlNewSessionPeriod_' + n + ' option:selected').val().length == 0 ||
                        $('#' + rpId + '_tbNewSessionVenue_' + n).val().length == 0) {
                        args.IsValid = false;
                        return false;
                    }
                    
                    //check if selected venue is available
                    if ($('#' + rpId + '_lbVenueAva_' + n).val().indexOf("Not") != -1) {
                        args.IsValid = false;
                        return false;
                    }

                    if (!isValidDate($('#' + rpId + '_tbNewSessionDt_' + n).val())) {
                        args.IsValid = false;
                        return false;
                    }

                    //check if session date is within batch start and end dates
                    dtSession = new Date($('#' + rpId + '_tbNewSessionDt_' + n).val());
                    if (dtSession < dtStart || dtSession > dtEnd) {
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
                    <asp:Label ID="lblTitle" runat="server" Text="Edit Bundle Sessions"></asp:Label>
                    <asp:HiddenField ID="hfBundleId" runat="server" />
                    <asp:HiddenField ID="hfBundleCode" runat="server" />
                    <asp:HiddenField ID="hfBundleType" runat="server" />
                    <asp:HiddenField ID="hfEffectiveDate" runat="server" />
                    <asp:HiddenField ID="hfCost" runat="server" />
                </h3>

                <small>Please review the following.</small>

            </div>
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <button type="button" class="btn btn-sm btn-default" data-toggle="modal" data-target="#diagBack">Back</button>
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
        <div class="alert alert-warning" id="panelWarning" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblWaring" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger hidden" id="panelError">
            <a href="#" class="close" onClick="closeErrorAlert();">&times;</a>
            <span id="lblError" class="alert-link"></span>
        </div>
        <asp:ValidationSummary ID="vSummary1" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <%-- To display each module that has changes as a tab --%>
        <asp:HiddenField ID="hfSelModule" runat="server" />
        <asp:Repeater ID="rpModuleTabs" runat="server">
            <HeaderTemplate>
                <ul class="nav nav-tabs">
            </HeaderTemplate>
            <ItemTemplate>
                <li id="tabMod" runat="server"><a data-toggle="tab" onClick="setSelValue('<%=hfSelModule.ClientID %>', '<%# Eval("moduleId") %>');" 
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
                    <asp:HiddenField ID="hfChgType" runat="server" Value='<%# Eval("chgType") %>' />
                    <asp:HiddenField ID="hfSessionDiff" runat="server" Value='<%# Eval("sessionDiff") %>' />
                    <br />
                    <h4><b><%# Eval("moduleTitle") %></b></h4>
                    <br />
                    <asp:Repeater ID="rpProgramme" runat="server" OnItemDataBound="rpProgramme_ItemDataBound">
                        <ItemTemplate>
                            <fieldset>
                                <legend style="font-size: 18px;"><%# Eval("programmeCode") %> / <%# Eval("programmeTitle") %></legend>
                                <asp:HiddenField ID="hfProgId" runat="server" Value='<%# Eval("programmeId") %>' />
                                <asp:HiddenField ID="hfSelBatch" runat="server" />
                                <div class="col-lg-2">
                                    <asp:Repeater ID="rpBatchPills" runat="server" OnItemDataBound="rpBatchPills_ItemDataBound">
                                        <HeaderTemplate>
                                            <ul class="nav nav-pills nav-stacked">
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <li id="pillBatch" runat="server" style="border:1px solid #337ab7;border-radius:5px;"><a id="pillBatchLnk" runat="server" data-toggle="pill" ><%# Eval("batchCode") %></a></li>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </ul>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                                <div class="col-lg-10">
                                    <asp:Repeater ID="rpBatchContent" runat="server" OnItemDataBound="rpBatchContent_ItemDataBound">
                                        <HeaderTemplate>
                                            <div class="tab-content">
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <div id='<%# Eval("moduleId") %>_<%# Eval("programmeId") %>_<%# Eval("programmeBatchId") %>' class="tab-pane fade ">
                                                <asp:HiddenField ID="hfBatchId" runat="server" Value='<%# Eval("programmeBatchId") %>' />
                                                <div class="row">
                                                    <div class="col-lg-4">
                                                        <asp:Label ID="lb1" runat="server" Text="Project Code" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lbProjCode" runat="server" CssClass="form-control" ReadOnly="true"><%# Eval("projectCode") %></asp:Label>
                                                    </div>
                                                    <div class="col-lg-4">
                                                        <asp:Label ID="lb2" runat="server" Text="Mode" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lbBatchMode" runat="server" CssClass="form-control" ReadOnly="true"><%# Eval("classMode") %></asp:Label>
                                                    </div>
                                                    <div class="col-lg-4">
                                                        <asp:Label ID="lb3" runat="server" Text="Capacity" Font-Bold="true"></asp:Label>
                                                        <asp:Label ID="lbBatchCapacity" runat="server" CssClass="form-control" ReadOnly="true"><%# Eval("batchCapacity") %></asp:Label>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="row">
                                                    <div class="col-lg-6">
                                                        <asp:Label ID="lb4" runat="server" Text="Registration Date" Font-Bold="true"></asp:Label>

                                                        <div class="input-group">
                                                            <asp:Label ID="lbRegDtFrm" runat="server" CssClass="form-control" ReadOnly="true"><%# Eval("programmeRegStartDate") %></asp:Label>
                                                            <span class="input-group-addon" style="font-weight: bold;">to</span>
                                                            <asp:Label ID="lbRegDtTo" runat="server" CssClass="form-control" ReadOnly="true"><%# Eval("programmeRegEndDate") %></asp:Label>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <asp:Label ID="lb5" runat="server" Text="Class Commencement Date" Font-Bold="true"></asp:Label>
                                                        <div class="input-group">
                                                            <asp:Label ID="lbCommDtFrm" runat="server" CssClass="form-control" ReadOnly="true"><%# Eval("programmeStartDate") %></asp:Label>
                                                            <span class="input-group-addon" style="font-weight: bold;">to</span>
                                                            <asp:Label ID="lbCommDtTo" runat="server" CssClass="form-control" ReadOnly="true"><%# Eval("programmeCompletionDate") %></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <uc1:bundlemodulenewsession runat="server" ID="ns" />
                                                <uc1:bundlemoduleremsession runat="server" ID="rs" />
                                            </div>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </div>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </fieldset>
                            <br />
                            <br />
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
                <asp:Button ID="btnConfirm" runat="server" CssClass="btn btn-primary" Text="Confirm" OnClick="btnConfirm_Click" CausesValidation="true"  />
            </div>
        </div>
    </div>

    <!---------------------hidden fields to access venue dialog---------------------------->
    <asp:HiddenField ID="hfVenueSelModule" runat="server" />
    <asp:HiddenField ID="hfVenueSelProg" runat="server" />
    <asp:HiddenField ID="hfVenueSelBatch" runat="server" />
    <asp:HiddenField ID="hfVenueSelSession" runat="server" />
    <%-- this control exist in order to have a event handler for refreshing venue availability --%>
    <asp:LinkButton ID="lbtnRefreshVenue" runat="server" OnClick="lbtnRefreshVenue_Click" style="display:none;" CausesValidation="false"></asp:LinkButton>
    <!---------------------------------------------------------------------------------------------->

    <!---------------------hidden fields to access day dialog---------------------------->
    <asp:HiddenField ID="hfDaySelModule" runat="server" />
    <asp:HiddenField ID="hfDaySelProg" runat="server" />
    <asp:HiddenField ID="hfDaySelBatch" runat="server" />
    <!---------------------------------------------------------------------------------------------->

    <!---------------------aid in fields validation for new sessions---------------------------->
    <asp:TextBox ID="hfNewSessionLblNamingContainer" runat="server" style="display:none;" />
    <asp:TextBox ID="hfNewSessionRpNamingContainer" runat="server" style="display:none;" />
    <asp:TextBox ID="hfNewSessionDtNamingContainer" runat="server" style="display:none;" />
    <asp:CustomValidator ID="cvNewSessions" runat="server" Display="None" ControlToValidate="hfNewSessionRpNamingContainer" ClientValidationFunction="validateAllNewSession"
        ErrorMessage="All new session date, period, venue cannot be empty<br/>OR selected venue is not available<br/>OR session date must be within class commencement date" 
        ></asp:CustomValidator>
    <!---------------------------------------------------------------------------------------------->

    <!---------------------aid in fields validation for removed sessions---------------------------->
    <asp:TextBox ID="hfRemSessionLblNamingContainer" runat="server" style="display:none;" />
    <asp:TextBox ID="hfRemSessionTotalNamingContainer" runat="server" style="display:none;" />
    <asp:TextBox ID="hfRemSessionGvNamingContainer" runat="server" style="display:none;" />
    <asp:CustomValidator ID="cvRemSessions" runat="server" ErrorMessage="Must select the correct number of session(s) to remove." 
        Display="None" ControlToValidate="hfRemSessionGvNamingContainer" ClientValidationFunction="validateAllRemSession"></asp:CustomValidator>
    <!---------------------------------------------------------------------------------------------->

    <uc1:venuesearch runat="server" ID="venuesearch" />

    <div id="diagBack" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Back</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to discard all changes?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnBack" runat="server" CssClass="btn btn-default" Text="OK" OnClick="btnBack_Click" CausesValidation="false" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfRtnMode" runat="server" />
    <uc1:dayselect runat="server" id="dayselect" />
</asp:Content>

