<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="module-management.aspx.cs" Inherits="ACI_TMS.module_management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Content/custom/pagination.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <%-- Module management --%>
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbModuleManagementHeader" runat="server" Text="Module Management"></asp:Label>
                </h2>

            </div>
        </div>

        <hr />

        <div class="row" id="panelNewModule" runat="server">

            <div class="col-lg-9 col-md-9 col-sm-12">
            </div>

            <div class="col-lg-3 col-md-3 col-sm-12">
                <div class="panel panel-default">
                    <div id="listHeader" class="panel-heading">Operations</div>
                    <div class="panel-body">
                        <p>
                            <asp:LinkButton ID="lkbtnCreateModule" OnClick="lkbtnCreateModule_Click" runat="server" CausesValidation="false"><span class="fa glyphicon-plus"></span> New Module</asp:LinkButton>
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

                    <asp:Label ID="lbSearchModule" runat="server" Text="Search by"></asp:Label>

                    <asp:DropDownList ID="ddlSearchModule" runat="server" CssClass="form-control" Width="220px">
                        <asp:ListItem Text="--Select--" Value=""></asp:ListItem>
                        <asp:ListItem Text="Code" Value="MC"></asp:ListItem>
                        <asp:ListItem Text="Title" Value="MT"></asp:ListItem>
                        <asp:ListItem Text="WSQ Competency Code" Value="WSQ"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSearchType" runat="server" ErrorMessage="Search criteria cannot be empty." ControlToValidate="ddlSearchModule" Display="None"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="tbSearchModule" runat="server" placeholder="" CssClass="form-control" Width="350px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSearchTxt" runat="server" ErrorMessage="Search value cannot be empty." ControlToValidate="tbSearchModule" Display="None"></asp:RequiredFieldValidator>
                    <asp:LinkButton ID="btnSearchModule" runat="server" OnClick="btnSearchModule_Click" CssClass="btn btn-info">
                                <span aria-hidden="true" class="fa fa-search"></span>
                    </asp:LinkButton>

                    <asp:Button ID="btnListAll" runat="server" CssClass="btn btn-default" OnClick="btnListAll_Click" Text="List All" CausesValidation="false" />
                </div>
            </div>
        </div>

        <br />

        <%-- List of module --%>
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvModule" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    OnRowCommand="gvModule_RowCommand" runat="server" OnPageIndexChanging="gvModule_PageIndexChanging">
                    <EmptyDataTemplate>
                        No modules found.
                    </EmptyDataTemplate>
                    <Columns>

                        <%-- Module Code --%>
                        <asp:TemplateField HeaderText="Code" ItemStyle-Width="200px">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnModuleCode" runat="server" CommandName="viewModuleDetails" CommandArgument='<%# Eval("moduleId") %>' CausesValidation="false">
                                    <asp:Label ID="lbModuleCode" runat="server" Text='<%# Eval("moduleCode") %>'></asp:Label>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Module Title --%>
                        <asp:TemplateField HeaderText="Title">
                            <ItemTemplate>
                                <asp:Label ID="lbModuleTitle" runat="server" Text='<%# Eval("moduleTitle") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Module Version --%>
                        <asp:TemplateField HeaderText="Version" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <asp:Label ID="lbModuleVersion" runat="server" Text='<%# Eval("moduleVersion") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Module Credit --%>
                        <asp:TemplateField HeaderText="Credit" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <asp:Label ID="lbModuleCredit" runat="server" Text='<%# Eval("moduleCredit") %>'></asp:Label>

                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- WSQ completency code --%>
                        <asp:TemplateField HeaderText="WSQ Competency Code" ItemStyle-Width="300px">
                            <ItemTemplate>
                                <asp:Label ID="lbWSQCode" runat="server" Text='<%# Eval("WSQCompetencyCode") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
