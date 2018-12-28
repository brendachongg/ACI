<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="programme-management.aspx.cs" Inherits="ACI_TMS.programme_management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbProgrammeManagementHeader" runat="server" Text="Programme Management"></asp:Label>
                </h2>

            </div>
        </div>

        <hr />

        <div class="row" id="panelNewProgramme" runat="server">
            <div class="col-lg-9 col-md-9 col-sm-12">
            </div>

            <div class="col-lg-3 col-md-3 col-sm-12">
                <div class="panel panel-default">
                    <div id="listHeader" class="panel-heading">Operations</div>
                    <div class="panel-body">
                        <p>
                            <asp:LinkButton ID="lkbtnCreateProgramme" OnClick="lkbtnCreateProgramme_Click" runat="server"><span class="fa glyphicon-plus"></span> New Programme</asp:LinkButton>
                        </p>

                    </div>
                </div>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-6">
                <div class="form-group form-inline">
                    <asp:Label ID="lbProgrammeCategory" runat="server" Text="Category: "></asp:Label>

                    <asp:DropDownList ID="ddlProgrammeCategory" DataTextField="codeValueDisplay" DataValueField="codeValue" CssClass="form-control" runat="server" OnSelectedIndexChanged="ddlProgrammeCategory_SelectedIndexChanged" AutoPostBack="true">
                    </asp:DropDownList>
                </div>

            </div>
        </div>

        <%-- List of course by category --%>
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvProgramme" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    OnRowCommand="gvProgramme_RowCommand" runat="server" OnPageIndexChanging="gvProgramme_PageIndexChanging">

                    <EmptyDataTemplate>
                        No available programme
                    </EmptyDataTemplate>

                    <Columns>

                        <%-- Programme Code --%>
                        <asp:TemplateField HeaderText="Code" ItemStyle-Width="200px">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnProgrammeCode" runat="server" CommandName="viewProgrammeDetails" CommandArgument='<%# Eval("programmeId") %>'>
                                    <asp:Label ID="lbProgrammeCode" runat="server" Text='<%# Eval("programmeCode") %>'></asp:Label>
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
</asp:Content>
