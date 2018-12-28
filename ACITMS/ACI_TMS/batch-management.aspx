<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="batch-management.aspx.cs" Inherits="ACI_TMS.batch_management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var Page_Validators = Page_Validators || new Array();

        function checkValidators() {
            if ($('#<%=ddlSearchBatchType.ClientID%> option:selected').val() == "AVA") {
                $('#<%=tbSearchBatch.ClientID%>').hide();
                ValidatorEnable(document.getElementById('<%=rfvSearchTxt.ClientID%>'), false);
            } else {
                $('#<%=tbSearchBatch.ClientID%>').show();
                ValidatorEnable(document.getElementById('<%=rfvSearchTxt.ClientID%>'), true);
            }
        }

        function hideError() {
            $('#<%=vSummary.ClientID%>').hide();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Class Management"></asp:Label>
                </h2>

            </div>
        </div>

        <hr />

        <div class="row" id="panelNewBatch" runat="server">

            <div class="col-lg-9 col-md-9 col-sm-12">
            </div>

            <div class="col-lg-3 col-md-3 col-sm-12">
                <div class="panel panel-default">
                    <div id="listHeader" class="panel-heading">Operations</div>
                    <div class="panel-body">
                        <p>
                            <asp:LinkButton ID="lkbtnCreateBatch" runat="server" OnClick="lkbtnCreateBatch_Click" CausesValidation="false"><span class="fa glyphicon-plus"></span> New Class</asp:LinkButton>
                        </p>

                    </div>
                </div>
            </div>

        </div>

        <br />
        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <div class="row">
            <div class="col-lg-12">
                <div class="form-group form-inline">
                    <asp:Label ID="lbSearchBatch" runat="server" Text="Search By"></asp:Label>
                    <asp:RequiredFieldValidator ID="rfvSearchType" runat="server" ErrorMessage="Search criteria cannot be empty." ControlToValidate="ddlSearchBatchType" Display="None"></asp:RequiredFieldValidator>
                    <asp:DropDownList ID="ddlSearchBatchType" runat="server" CssClass="form-control" onChange="checkValidators()">
                        <asp:ListItem Value="">--Select--</asp:ListItem>
                        <asp:ListItem Value="BTC">Class Code</asp:ListItem>
                        <asp:ListItem Value="PJC">Project Code</asp:ListItem>
                        <asp:ListItem Value="PGC">Programme Code</asp:ListItem>
                        <asp:ListItem Value="AVA">Available Classes</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSearchTxt" runat="server" ErrorMessage="Search value cannot be empty." ControlToValidate="tbSearchBatch" Display="None"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="tbSearchBatch" runat="server" placeholder="" CssClass="form-control" Width="350px"></asp:TextBox>

                    <asp:LinkButton ID="btnSearchBatch" runat="server" CssClass="btn btn-info" OnClick="btnSearchBatch_Click">
                        <span aria-hidden="true" class="fa fa-search"></span>
                    </asp:LinkButton>

                    <asp:Button ID="btnListAll" runat="server" CssClass="btn btn-default" Text="List All" OnClick="btnListAll_Click" CausesValidation="false" />
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvBatch" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvBatch_PageIndexChanging" OnRowCommand="gvBatch_RowCommand">

                    <EmptyDataTemplate>
                        No available class
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>Class Code</HeaderTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnBatchCode" runat="server" CommandName="selectBatch" CommandArgument='<%# Eval("programmeBatchId") %>' CausesValidation="false">
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
                        <asp:BoundField DataField="batchCapacity" HeaderText="Capacity" ItemStyle-Width="100px" />

                        <asp:TemplateField HeaderText="Registration Date" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <%# Eval("programmeRegStartDateDisp") %> to <%# Eval("programmeRegEndDateDisp") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Commencement Date" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <%# Eval("programmeStartDateDisp") %> to<br /><%# Eval("programmeCompletionDateDisp") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>

    </div>
</asp:Content>
