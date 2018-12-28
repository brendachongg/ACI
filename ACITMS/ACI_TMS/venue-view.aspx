<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="venue-view.aspx.cs" Inherits="ACI_TMS.venue_view" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                <h3>
                    <asp:Label ID="lbVenueDetailHeader" runat="server" Text="View Venue"></asp:Label>
                </h3>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" OnClick="lkbtnBack_Click" CssClass="btn btn-sm btn-default" Text="Back" CausesValidation="false"></asp:LinkButton>
            </div>

        </div>

        <hr />

        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:HiddenField ID="hfVenueId" runat="server" />

        <div class="row">
            <div class="col-lg-3">
                <asp:Label ID="lbVenueId" runat="server" Text="ID: " Font-Bold="true"></asp:Label>
                <asp:Label ID="lbVenueIdValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-9">
                <asp:Label ID="lbLocation" runat="server" Text="Location: " Font-Bold="true"></asp:Label>
                <asp:Label ID="lbLocationValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:Label ID="lbDescription" runat="server" Text="Description: " Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" ReadOnly="true"></asp:TextBox>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-6">
                <asp:Label ID="lbCapacity" runat="server" Text="Capacity: " Font-Bold="true"></asp:Label>
                <asp:Label ID="lbCapacityValue" TextMode="Number" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-6">
                <asp:Label ID="lbEffectiveDate" runat="server" Text="Effective Date: " Font-Bold="true"></asp:Label>
                <asp:Label ID="lbEffectiveDateValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnUpdate" runat="server" CssClass="btn btn-primary" OnClick="btnUpdate_Click" Text="Edit" />
                <button id="btnConfirmDel" runat="server" type="button" class="btn btn-danger" data-toggle="modal" data-target="#diagDelete">Delete</button>
            </div>
        </div>
    </div>

    <div id="diagDelete" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete venue</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this venue?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnDelete_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
