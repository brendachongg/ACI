<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="venue-management.aspx.cs" Inherits="ACI_TMS.venue_management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbModuleManagementHeader" runat="server" Text="Venue Management"></asp:Label>
                </h2>

            </div>
        </div>

        <hr />

        <div class="row" id="panelNewVenue" runat="server">

            <div class="col-lg-9 col-md-9 col-sm-12">
            </div>

            <div class="col-lg-3 col-md-3 col-sm-12">
                <div class="panel panel-default">
                    <div id="listHeader" class="panel-heading">Operations</div>
                    <div class="panel-body">
                        <p>
                            <asp:LinkButton ID="lkbtnCreateVenue" OnClick="lkbtnCreateVenue_Click" runat="server"><span class="fa glyphicon-plus"></span> New Venue</asp:LinkButton>
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
                <asp:GridView ID="gvVenue" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10" OnRowCommand="gvVenue_RowCommand"
                    OnPageIndexChanging="gvVenue_PageIndexChanging">

                    <EmptyDataTemplate>
                        No available venue
                    </EmptyDataTemplate>

                    <Columns>

                        <asp:TemplateField HeaderText="ID" HeaderStyle-Width="200px">

                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnVenueID" runat="server" CommandName="viewVenueDetails" CommandArgument='<%# Eval("venueID") %>'>
                                    <asp:Label ID="lbVenueID" runat="server" Text='<%# Eval("venueID") %>'></asp:Label>
                                </asp:LinkButton>
                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:BoundField HeaderText="Location" DataField="venueLocation" HeaderStyle-Width="300px"/>

                        <asp:BoundField HeaderText="Description" DataField="venueDesc" />

                        <asp:BoundField HeaderText="Capacity" DataField="venueCapacity" ItemStyle-Width="150px" />

                        <asp:BoundField HeaderText="Effective Date" DataFormatString="{0:dd/MM/yyyy}" DataField="venueEffectDate" ItemStyle-Width="150px" />

                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>

    </div>
</asp:Content>
