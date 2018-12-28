<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="enrollment-letter-managment.aspx.cs" Inherits="ACI_TMS.enrollment_letter_managment" %>
<%@ Import Namespace="ACI_TMS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var Page_Validators = Page_Validators || new Array();

        function checkValidators() {
            if ($('#<%=ddlSearchBatch.ClientID%> option:selected').val() == "AVA") {
                $('#<%=tbSearchBatch.ClientID%>').hide();
                ValidatorEnable(document.getElementById('<%=rfvSearchBatchValue.ClientID%>'), false);
            } else {
                $('#<%=tbSearchBatch.ClientID%>').show();
                ValidatorEnable(document.getElementById('<%=rfvSearchBatchValue.ClientID%>'), true);
            }
        }

        function hideError() {
            $('#<%=vSummaryLetter.ClientID%>').hide();
        }

        function showTemplateDialog() {
            $('#diagTemplate').modal({
                backdrop: 'static',
                keyboard: false
            });
        }

        function showLetterDialog() {
            $('#diagLetter').modal({
                backdrop: 'static',
                keyboard: false
            });
        }

        function showTab(tabId) {
            $("#" + tabId).attr("class", "tab-pane fade in active");
            $('.nav-tabs a[href="#' + tabId + '"]').tab('show');
        }

        function viewLetter(tid) {
            window.open("<%= enrollment_letter_print.PAGE_NAME%>?<%=enrollment_letter_print.TRAINEE_QUERY%>=" + encodeURI(tid), '_blank', 'menubar=no,location=no,scrollbars=yes,resizable=yes');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>Enrollment Letter Management
                </h2>
            </div>
        </div>
        <br />
        <ul class="nav nav-tabs" style="font-weight:bold;">
            <li id="tabGen" runat="server"><a data-toggle="tab" href="#<%=panelGen.ClientID %>">Generate</a></li>
            <li id="tabLetter" runat="server"><a data-toggle="tab" href="#<%=panelLetter.ClientID %>">Letter</a></li>
            <li id="tabTemplate" runat="server"><a data-toggle="tab" href="#<%=panelTemplate.ClientID %>">Template</a></li>
        </ul>

        <div class="tab-content">
            <%-------------------------------------------------------------------------------------------------------------------%>
            <%--------------------------------------------- Generate trainee letter ---------------------------------------------%>
            <%-------------------------------------------------------------------------------------------------------------------%>
            <div id="panelGen" runat="server" class="tab-pane fade" style="padding:30px 10px;">
                <div class="alert alert-success" id="panelGenSuccess" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblGenSuccess" runat="server" CssClass="alert-link"></asp:Label>
                </div>
                <div class="alert alert-danger" id="panelGenError" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblGenError" runat="server" CssClass="alert-link"></asp:Label>
                </div>
                <asp:ValidationSummary ID="vSummaryGen" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="gen" />
                <div class="row">
                    <div class="col-lg-12">
                        <div class="form-group form-inline">
                            <asp:Label ID="lbSearchGen" runat="server" Text="Search by"></asp:Label>
                            <asp:DropDownList ID="ddlSearchGen" runat="server" CssClass="form-control">
                                <asp:ListItem Text="--Select--" Value=""></asp:ListItem>
                                <asp:ListItem Value="BTC">Class Code</asp:ListItem>
                                <asp:ListItem Value="PJC">Project Code</asp:ListItem>
                                <asp:ListItem Value="ID">Trainee ID</asp:ListItem>
                                <asp:ListItem Value="NAME">Trainee Name</asp:ListItem>
                                <asp:ListItem Value="NRICPIN">Trainee NRIC</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvSearchGenType" runat="server" ErrorMessage="Search criteria cannot be empty." ControlToValidate="ddlSearchGen" Display="None" ValidationGroup="gen"></asp:RequiredFieldValidator>
                            <asp:TextBox ID="tbSearchGen" runat="server" placeholder="" CssClass="form-control" Width="350px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvSearchGenValue" runat="server" ErrorMessage="Search value cannot be empty." ControlToValidate="tbSearchGen" Display="None" ValidationGroup="gen"></asp:RequiredFieldValidator>
                            <asp:LinkButton ID="btnSearchGen" runat="server" CssClass="btn btn-info" ValidationGroup="gen" OnClick="btnSearchGen_Click">
                                <span aria-hidden="true" class="fa fa-search"></span>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12">
                        <asp:GridView ID="gvTrainee" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                            OnPageIndexChanging="gvTrainee_PageIndexChanging">
                            <emptydatatemplate>
                                No trainee found.
                            </emptydatatemplate>
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbTr" runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>Trainee ID</HeaderTemplate>
                                    <ItemTemplate>
                                        <a href="#" onclick="viewLetter('<%# Eval("traineeID") %>');"><%# Eval("traineeID") %></a>
                                        <asp:HiddenField ID="hfTr" runat="server" Value='<%# Eval("traineeID") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" Width="100px" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="idNumber" HeaderText="ID" />
                                <asp:BoundField DataField="fullName" HeaderText="Full Name" />
                                <asp:BoundField DataField="batchCode" HeaderText="Class Code" />
                                <asp:BoundField DataField="programmeTitle" HeaderText="Programme" />
                                <asp:BoundField DataField="enrolDateDisp" HeaderText="Date of Enrolment" ItemStyle-Width="150px" />
                                <asp:TemplateField>
                                    <HeaderTemplate>Letter Status</HeaderTemplate>
                                    <ItemTemplate>
                                        <i>On:</i> <%# Eval("enrolLetterSendOnDisp") %><br />
                                        <i>By:</i> <%# Eval("userName") %>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" Width="150px" />
                                </asp:TemplateField>
                            </Columns>

                            <PagerStyle CssClass="pagination-ys" />
                        </asp:GridView>
                    </div>
                </div>
                <div class="row text-right" id="panelGenBtn" runat="server">
                    <div class="col-lg-12">
                        <asp:Button ID="btnGen" runat="server" CssClass="btn btn-primary" Text="Email" OnClick="btnGen_Click" ValidationGroup="gen" />
                    </div>
                </div>
            </div>

            <%-------------------------------------------------------------------------------------------------------------------%>
            <%-------------------------------------------------- Manage letter --------------------------------------------------%>
            <%-------------------------------------------------------------------------------------------------------------------%>
            <div id="panelLetter" runat="server" class="tab-pane fade" style="padding:30px 10px;">
                <div class="alert alert-success" id="panelLetterSuccess" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblLetterSuccess" runat="server" CssClass="alert-link"></asp:Label>
                </div>
                <div class="alert alert-danger" id="panelLetterError" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblLetterError" runat="server" CssClass="alert-link"></asp:Label>
                </div>
                <asp:ValidationSummary ID="vSummaryLetter" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="letter" />
                <div class="row">
                    <div class="col-lg-12">
                        <div class="form-group form-inline">
                            <asp:Label ID="lbSearchBatch" runat="server" Text="Search by"></asp:Label>
                            <asp:DropDownList ID="ddlSearchBatch" runat="server" CssClass="form-control" onChange="checkValidators()">
                                <asp:ListItem Text="--Select--" Value=""></asp:ListItem>
                                <asp:ListItem Value="BTC">Class Code</asp:ListItem>
                                <asp:ListItem Value="PJC">Project Code</asp:ListItem>
                                <asp:ListItem Value="PGC">Programme Code</asp:ListItem>
                                <asp:ListItem Value="AVA">Available Classes</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvSearchBatchType" runat="server" ErrorMessage="Search criteria cannot be empty." ControlToValidate="ddlSearchBatch" Display="None" ValidationGroup="letter"></asp:RequiredFieldValidator>
                            <asp:TextBox ID="tbSearchBatch" runat="server" placeholder="" CssClass="form-control" Width="350px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvSearchBatchValue" runat="server" ErrorMessage="Search value cannot be empty." ControlToValidate="tbSearchBatch" Display="None" ValidationGroup="letter"></asp:RequiredFieldValidator>
                            <asp:LinkButton ID="btnSearchBatch" runat="server" CssClass="btn btn-info" OnClick="btnSearchBatch_Click" ValidationGroup="letter">
                                <span aria-hidden="true" class="fa fa-search"></span>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-12">
                        <asp:GridView ID="gvBatch" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" runat="server"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                            OnRowCommand="gvBatch_RowCommand" OnPageIndexChanging="gvBatch_PageIndexChanging">
                            <EmptyDataTemplate>
                                No class found.
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>Class Code</HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnBatchCode" runat="server" CommandName="selectBatch" CommandArgument='<%# Container.DataItemIndex %>' CausesValidation="false">
                                            <asp:Label ID="lbgvBatchCode" runat="server" Text='<%# Eval("batchCode") %>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="projectCode" HeaderText="Project Code" ItemStyle-Width="200px" />
                                <asp:BoundField DataField="classModeDisp" HeaderText="Mode" ItemStyle-Width="120px" />
                                <asp:TemplateField HeaderText="Programme">
                                    <ItemTemplate>
                                        <%# Eval("programmeCode") %> / <%# Eval("programmeTitle") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Registration Date" ItemStyle-Width="120px">
                                    <ItemTemplate>
                                        <%# Eval("programmeRegStartDateDisp") %> to <%# Eval("programmeRegEndDateDisp") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Commencement Date" ItemStyle-Width="120px">
                                    <ItemTemplate>
                                        <%# Eval("programmeStartDateDisp") %> to<br />
                                        <%# Eval("programmeCompletionDateDisp") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>

                            <PagerStyle CssClass="pagination-ys" />
                        </asp:GridView>
                    </div>
                </div>
                <br />
            </div>

            <%-------------------------------------------------------------------------------------------------------------------%>
            <%------------------------------------------------- Manage Template -------------------------------------------------%>
            <%-------------------------------------------------------------------------------------------------------------------%>
            <div id="panelTemplate" runat="server" class="tab-pane fade" style="padding:30px 10px;">
                <div class="alert alert-success" id="panelTemplateSuccess" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblTemplateSuccess" runat="server" CssClass="alert-link"></asp:Label>
                </div>
                <div class="alert alert-danger" id="panelTemplateError" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblTemplateError" runat="server" CssClass="alert-link"></asp:Label>
                </div>
                <div class="row">
                    <div class="col-lg-6">
                        <div class="form-group form-inline">
                            <asp:Label ID="lbProgrammeCategory" runat="server" Text="Category: "></asp:Label>

                            <asp:DropDownList ID="ddlProgrammeCategory" DataTextField="codeValueDisplay" DataValueField="codeValue" CssClass="form-control" runat="server" OnSelectedIndexChanged="ddlProgrammeCategory_SelectedIndexChanged" AutoPostBack="true">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12">
                        <asp:GridView ID="gvProgramme" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" runat="server"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                             OnRowCommand="gvProgramme_RowCommand" OnPageIndexChanging="gvProgramme_PageIndexChanging">

                            <EmptyDataTemplate>
                                No available programme
                            </EmptyDataTemplate>

                            <Columns>
                                <%-- Programme Code --%>
                                <asp:TemplateField HeaderText="Code" ItemStyle-Width="200px">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnProgrammeCode" runat="server" CommandName="selectProgramme" CommandArgument='<%# Container.DataItemIndex %>' CausesValidation="false">
                                            <%# Eval("programmeCode") %>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- Programme Title --%>
                                <asp:TemplateField HeaderText="Title">
                                    <ItemTemplate>
                                        <asp:Label ID="lbProgrammeTitle" runat="server" Text='<%# Eval("programmeTitle") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- Programme Version --%>
                                <asp:TemplateField HeaderText="Version" ItemStyle-Width="100px">
                                    <ItemTemplate>
                                        <asp:Label ID="lbProgrammeVersion" runat="server" Text='<%# Eval("programmeVersion") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- SSG Reference No. --%>
                                <asp:TemplateField HeaderText="SSG Reference No." ItemStyle-Width="300px">
                                    <ItemTemplate>
                                        <asp:Label ID="lbSSGRefNo" runat="server" Text='<%# Eval("SSGRefNum") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>

                            <PagerStyle CssClass="pagination-ys" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfSelId" runat="server" />

    <div id="diagTemplate" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Enrollment Template for <asp:Label ID="lblProgCode" runat="server"></asp:Label></h4>               
                </div>
                <div class="modal-body">
                    <asp:ValidationSummary ID="vSummaryEditTemplate" runat="server" CssClass="alert alert-danger alert-link" ValidationGroup="editTemplate" />
                    <div class="row text-right">
                        <div class="col-lg-12">
                            <i>Legend</i>&nbsp;<i class="glyphicon glyphicon-question-sign" data-toggle="modal" data-target="#diagELengend"></i>
                            <asp:TextBox ID="tbEnrolTemplate" runat="server" TextMode="MultiLine" Height="300" CssClass="tinymce" ValidateRequestMode="Disabled"></asp:TextBox>                         
                            <asp:RequiredFieldValidator ID="rfvEnrolTemplate" Display="None" ControlToValidate="tbEnrolTemplate" runat="server" ValidationGroup="editTemplate" 
                                ErrorMessage="Enrollment template cannot be empty."></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSaveTemplate" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSaveTemplate_Click" ValidationGroup="editTemplate" OnClientClick="tinyMCE.triggerSave(false,true);"/>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="diagLetter" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Enrollment Letter for <asp:Label ID="lblBatchCode" runat="server"></asp:Label></h4>               
                </div>
                <div class="modal-body">
                    <asp:ValidationSummary ID="vSummaryEditLetter" runat="server" CssClass="alert alert-danger alert-link" ValidationGroup="editLetter" />
                    <div class="row text-right">
                        <div class="col-lg-12">
                            <i>Legend</i>&nbsp;<i class="glyphicon glyphicon-question-sign" data-toggle="modal" data-target="#diagELengend"></i>
                            <asp:TextBox ID="tbEnrolLetter" runat="server" TextMode="MultiLine" Height="300" CssClass="tinymce" ValidateRequestMode="Disabled"></asp:TextBox>                         
                            <asp:RequiredFieldValidator ID="rfvEnrolLetter" Display="None" ControlToValidate="tbEnrolLetter" runat="server" ValidationGroup="editLetter" 
                                ErrorMessage="Enrollment leter cannot be empty."></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnSaveLetter" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSaveLetter_Click" ValidationGroup="editLetter" OnClientClick="tinyMCE.triggerSave(false,true);" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="diagELengend" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Enrollment Template/Letter Legend</h4>
                </div>
                <div class="modal-body">
                    <div class="row text-left">
                        <div class="col-lg-4">
                            <b>Label</b>
                        </div>
                        <div class="col-lg-8">
                            <b>Value</b> <i>(will be replace with actual details when letter is generated)</i>
                        </div>
                    </div>
                    <div class="row text-left">
                        <div class="col-lg-4">
                            @name
                        </div>
                        <div class="col-lg-8">
                            Applicant's Name
                        </div>
                    </div>
                    <div class="row text-left">
                        <div class="col-lg-4">
                            @idNumber
                        </div>
                        <div class="col-lg-8">
                            Applicant's ID Number
                        </div>
                    </div>
                    <div class="row text-left">
                        <div class="col-lg-4">
                            @programme_title
                        </div>
                        <div class="col-lg-8">
                            Programe Name
                        </div>
                    </div>
                    <div class="row text-left">
                        <div class="col-lg-4">
                            @current_date
                        </div>
                        <div class="col-lg-8">
                            Today Date
                        </div>
                    </div>
                    <div class="row text-left">
                        <div class="col-lg-4">
                            @class_start_date
                        </div>
                        <div class="col-lg-8">
                            Class Commencement Date
                        </div>
                    </div>
                    <div class="row text-left">
                        <div class="col-lg-4">
                            @class_end_date
                        </div>
                        <div class="col-lg-8">
                            Class End Date
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
