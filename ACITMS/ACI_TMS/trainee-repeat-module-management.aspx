<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="trainee-repeat-module-management.aspx.cs" Inherits="ACI_TMS.trainee_repeat_module_management" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Repeat Module Management"></asp:Label>
                </h2>
            </div>
        </div>

        <hr />
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <div class="row text-left">
            <div class="col-lg-12">
                <div class="form-group form-inline">
                    <asp:Label ID="lbSearch" runat="server" Text="Search By: "></asp:Label>
                    <asp:DropDownList ID="ddlSearch" CssClass="form-control" runat="server">
                        <asp:ListItem Text="--Select--" Value=""></asp:ListItem>
                        <asp:ListItem Text="Module" Value="M"></asp:ListItem>
                        <asp:ListItem Text="Trainee" Value="T"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSearchType" runat="server" ErrorMessage="Must select search type" Display="None" ControlToValidate="ddlSearch"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="txtSearch" runat="server"  CssClass="form-control" Width="350px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSearchValue" runat="server" ErrorMessage="Search value cannot be empty" Display="None" ControlToValidate="txtSearch"></asp:RequiredFieldValidator>
                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-info" OnClick="btnSearch_Click">
                        <span aria-hidden="true" class="fa fa-search"></span>
                    </asp:LinkButton>

                    <asp:Button ID="btnListAll" runat="server" CssClass="btn btn-default" Text="List All" CausesValidation="false" OnClick="btnListAll_Click"/>

                </div>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvRepeatMod" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvRepeatMod_PageIndexChanging" OnRowCommand="gvRepeatMod_RowCommand">
                    <EmptyDataTemplate>
                        No available records
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="Trainee ID" HeaderStyle-Width="200px">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnTraineeId" runat="server" CommandName="select" CommandArgument='<%# Container.DataItemIndex %>' CausesValidation="false">
                                    <asp:Label ID="lbgTraineeId" runat="server" Text='<%# Eval("idNumber") %>'></asp:Label>
                                </asp:LinkButton>
                                <asp:HiddenField ID="hfTrainee" runat="server" Value='<%# Eval("traineeId") %>' />
                                <asp:HiddenField ID="hfBatchModule" runat="server" Value='<%# Eval("batchModuleId") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="fullName" HeaderText="Name" /> 
                        <asp:BoundField DataField="batchCode" HeaderText="Class" HeaderStyle-Width="200px" /> 
                        <asp:TemplateField HeaderText="Programme">
                            <ItemTemplate>
                                <%# Eval("programmeTitle") %> (<%# Eval("programmeCode") %>)
                            </ItemTemplate>
                        </asp:TemplateField>  
                        <asp:TemplateField HeaderText="Module">
                            <ItemTemplate>
                                <%# Eval("moduleTitle") %> (<%# Eval("moduleCode") %>)
                            </ItemTemplate>
                        </asp:TemplateField>   
                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>

    </div>
</asp:Content>
