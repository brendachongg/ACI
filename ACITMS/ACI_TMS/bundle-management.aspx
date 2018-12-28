<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="bundle-management.aspx.cs" Inherits="ACI_TMS.bundle_management" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbManageBundleHeader" runat="server" Text="Bundle Management"></asp:Label>
                </h2>

            </div>
        </div>

        <hr />

        <div class="row" id="panelNewBundle" runat="server">
            <div class="col-lg-9 col-md-9 col-sm-12">
            </div>
            <div class="col-lg-3 col-md-3 col-sm-12">
                <div class="panel panel-default">
                    <div id="listHeader" class="panel-heading">Operations</div>
                    <div class="panel-body">
                        <p>
                            <asp:LinkButton ID="lkbtnCreateBundle" runat="server" CausesValidation="false" OnClick="lkbtnCreateBundle_Click"><span class="fa glyphicon-plus"></span> New Bundle</asp:LinkButton>
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
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <div class="row text-left">
            <div class="col-lg-12">
                <div class="form-group form-inline">
                    <asp:Label ID="lbSearchBundle" runat="server" Text="Search by"></asp:Label>
                    <asp:RequiredFieldValidator ID="rfvSearchType" runat="server" ErrorMessage="Search criteria cannot be empty." ControlToValidate="ddlSearchBundleType" Display="None"></asp:RequiredFieldValidator>
                    <asp:DropDownList ID="ddlSearchBundleType" runat="server" CssClass="form-control">
                        <asp:ListItem Value="">--Select--</asp:ListItem>
                        <asp:ListItem Value="BC">Bundle Code</asp:ListItem>
                        <asp:ListItem Value="MC">Module Code</asp:ListItem>
                        <asp:ListItem Value="MT">Module Title</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSearchTxt" runat="server" ErrorMessage="Search value cannot be empty." ControlToValidate="tbSearchBundle" Display="None"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="tbSearchBundle" runat="server" placeholder="" CssClass="form-control" Width="350px"></asp:TextBox>

                    <asp:LinkButton ID="btnSearchBundle" runat="server" CssClass="btn btn-info" OnClick="btnSearchBundle_Click">
                        <span aria-hidden="true" class="fa fa-search"></span>
                    </asp:LinkButton>

                    <asp:Button ID="btnListAll" runat="server" CssClass="btn btn-default" Text="List All" OnClick="btnListAll_Click" CausesValidation="false" />

                </div>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvBundle" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="false"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvBundle_PageIndexChanging" OnRowCommand="gvBundle_RowCommand">

                    <EmptyDataTemplate>
                        No available bundle
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>Bundle Code</HeaderTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnBundleCode" runat="server" CommandName="selectBundle" CommandArgument='<%# Eval("bundleId") %>' CausesValidation="false">
                                    <asp:Label ID="lbgvBundleCode" runat="server" Text='<%# Eval("bundleCode") %>'></asp:Label>
                                </asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>

                        <asp:BoundField DataField="NoOfModules" HeaderText="Num. Of Modules" />
                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>

    </div>
</asp:Content>
