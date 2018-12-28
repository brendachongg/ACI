<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="case-log-management.aspx.cs" Inherits="ACI_TMS.case_log_management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function selectAll() {
            $(":checkbox").prop('checked', $('#<%= gvCaseLog.ClientID%>_cbAll').is(':checked'));
        }

        function formatSearchTxt() {
            if ($('#<%=ddlSearchCaseLog.ClientID%> option:selected').val() == "D")
                $('#<%=tbSearchCaseLog.ClientID%>').datepicker({
                    dateFormat: "dd M yy",
                    changeMonth: true,
                    changeYear: true
                });
            else
                $('#<%=tbSearchCaseLog.ClientID%>').datepicker("destroy");
        }

        function validateCaseLogDate(oSrc, args) {
            if ($('#<%=ddlSearchCaseLog.ClientID%> option:selected').val() != "D") {
                args.IsValid = true;
                return true;
            }

            var str = $('#<%=tbSearchCaseLog.ClientID%>').val();
            if (!isValidDate(str)) {
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
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Case Log Management"></asp:Label>
                </h2>
            </div>
        </div>
        <hr />
        <div class="row" id="panelNewCaseLog" runat="server">
            <div class="col-lg-9 col-md-9 col-sm-12">
            </div>

            <div class="col-lg-3 col-md-3 col-sm-12">
                <div class="panel panel-default">
                    <div id="listHeader" class="panel-heading">Operations</div>
                    <div class="panel-body">
                        <asp:LinkButton ID="lkbtnCreateCaseLog" runat="server" CausesValidation="false" OnClick="lkbtnCreateCaseLog_Click">
                            <span class="fa glyphicon-plus"></span> New Case Log</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <div class="row">
            <div class="col-lg-3 col-md-3 col-sm-12">
                <asp:HiddenField ID="hdfStatus" runat="server" />
            </div>
            <div class="col-lg-6 col-md-6 col-sm-12">
                <div class="btn-group" style="margin: 0 auto;" role="group" aria-label="...">
                    <%-- View Unattended --%>
                    <button id="btnUnattendedLogs" type="button" class="btn btn-default" runat="server" onserverclick="btnUnattendedLogs_ServerClick" causesvalidation="false">
                        Unattended
                        <span class="badge">
                            <asp:Label ID="lblUnattendedLogs" runat="server" Text=""></asp:Label>
                        </span>
                    </button>
                    <%-- View New --%>
                    <button id="btnNewLogs" type="button" class="btn btn-default btn-info" runat="server" onserverclick="btnNewLogs_ServerClick" causesvalidation="false">
                        New
                        <span class="badge">
                            <asp:Label ID="lblNewLogs" runat="server" Text=""></asp:Label>
                        </span>
                    </button>
                    <%-- View Resolving --%>
                    <button id="btnResolvingLogs" type="button" class="btn btn-default" runat="server" onserverclick="btnResolvingLogs_ServerClick" causesvalidation="false">
                        Resolving
                        <span class="badge">
                            <asp:Label ID="lblResolvingLogs" runat="server" Text=""></asp:Label>
                        </span>
                    </button>
                    <%-- View Closed --%>
                    <button id="btnClosedLogs" type="button" class="btn btn-default" runat="server" onserverclick="btnClosedLogs_ServerClick" causesvalidation="false">
                        Closed
                        <span class="badge">
                            <asp:Label ID="lblClosedLogs" runat="server" Text=""></asp:Label>
                        </span>
                    </button>
                </div>
            </div>
            <div class="col-lg-3 col-md-3 col-sm-12">
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <div class="form-group form-inline">
                    <asp:Label ID="lbCaseLogCategory" runat="server" Text="Category: "></asp:Label>
                    &nbsp
                    <asp:DropDownList ID="ddlCaseLogCategory" CssClass="form-control" runat="server" OnSelectedIndexChanged="ddlCaseLogCategory_SelectedIndexChanged" AutoPostBack="true" DataTextField="codeValueDisplay" DataValueField="codeValue">
                    </asp:DropDownList>
                </div>
            </div>
        </div>


        <div class="row">
            <div class="col-lg-12">
                <div class="form-group form-inline">
                    <asp:Label ID="lbSearchCaseLog" runat="server" Text="Search By:"></asp:Label>
                    <asp:RequiredFieldValidator ID="rfvSearchType" runat="server" ErrorMessage="Search criteria cannot be empty." ControlToValidate="ddlSearchCaseLog" Display="None"></asp:RequiredFieldValidator>

                    <asp:DropDownList ID="ddlSearchCaseLog" runat="server" CssClass="form-control" onChange="formatSearchTxt()">
                        <asp:ListItem Value="">--Select--</asp:ListItem>
                        <asp:ListItem Value="D">Date</asp:ListItem>
                        <asp:ListItem Value="RB">Reported By</asp:ListItem>
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="rfvSearchTxt" runat="server" ErrorMessage="Search value cannot be empty." ControlToValidate="tbSearchCaseLog" Display="None"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="tbSearchCaseLog" runat="server" placeholder="" CssClass="datepicker form-control" Width="350px"></asp:TextBox>

                    <asp:LinkButton ID="btnSearchCaseLog" runat="server" CssClass="btn btn-info" OnClick="btnSearchCaseLog_Click">
                        <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                    </asp:LinkButton>

                    <asp:CustomValidator ID="cvDate" runat="server" Display="None" ControlToValidate="tbSearchCaseLog" ClientValidationFunction="validateCaseLogDate"
                        ErrorMessage="Invalid date" ValidateEmptyText="false"></asp:CustomValidator>

                    <asp:Button ID="btnListAll" runat="server" CssClass="btn btn-default" Text="List All" OnClick="btnListAll_Click" CausesValidation="false" />
                </div>
            </div>
        </div>

        <br />

        <%-- List of course by category --%>
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvCaseLog" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="20"
                    OnRowCommand="gvCaseLog_RowCommand" runat="server" OnPageIndexChanging="gvCaseLog_PageIndexChanging">

                    <EmptyDataTemplate>
                        No available Case Logs.
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                             <HeaderTemplate>
                                &nbsp;<asp:CheckBox ID="cbAll" runat="server" onchange="selectAll()" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="cb" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Case Log ID --%>
                        <asp:TemplateField HeaderText="Case Log ID" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnCaseLog" runat="server" CommandName="viewCaseLogDetails" CommandArgument='<%# Eval("caseLogId") %>' CausesValidation="false">
                                    <asp:Label ID="lbCaseLogId" runat="server" Text='<%# Eval("caseLogId") %>'></asp:Label>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Subject" DataField="subject" />
                        <asp:BoundField HeaderText="Reported By" DataField="userName" ItemStyle-Width="150px" />
                        <asp:BoundField HeaderText="Submitted On" DataField="submittedOn" ItemStyle-Width="200px" DataFormatString="{0:dd MMM yyyy HH:mm}" />

                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>

        <div class="row text-right">
            <div class="col-lg-12">
                <button id="btnConfirmDel" runat="server" type="button" class="btn btn-danger" data-toggle="modal" data-target="#diagDelete" visible="false">Delete Case Log</button>
            </div>
        </div>
    </div>

    <div id="diagDelete" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Confirm Delete</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete selected case log(s)?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDel" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnDel_Click" CausesValidation="false" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
