<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="booking-view.aspx.cs" Inherits="ACI_TMS.booking_view" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbViewBookingHeader" runat="server" Text="View Booking"></asp:Label>
                </h3>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click" CausesValidation="false"></asp:LinkButton>
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

        <asp:HiddenField ID="hfBookingId" runat="server" />

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbDate" runat="server" Text="Date" Font-Bold="true"></asp:Label>

                <asp:Label ID="lbDateValue" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-3">
                <asp:Label ID="lbPeriod" runat="server" Text="Period" Font-Bold="true"></asp:Label>

                <asp:Label ID="lbPeriodValue" runat="server" CssClass="form-control" ></asp:Label>
            </div>
             <div class="col-lg-6">
                <asp:Label ID="lbVenue" runat="server" Text="Venue" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbVenueValue" runat="server" placeholder="" CssClass="form-control"></asp:Label>
            </div>
        </div>
   
        <br />

        <asp:Panel ID="panelRemark" runat="server">
            <div class="row">
                <div class="col-lg-12">
                    <asp:Label ID="lbRemark" runat="server" Text="Remarks" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="lbRemarkValue" runat="server" CssClass="form-control" ReadOnly="true" TextMode="MultiLine" Rows="5"></asp:TextBox>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="panelSession" runat="server" Visible="false">
            <div class="row">
                <div class="col-lg-3">
                    <asp:Label ID="lbClassCode" runat="server" Text="Class Code" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbClassCodeValue" runat="server" CssClass="form-control"></asp:Label>
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lbModuleCode" runat="server" Text="Class Code" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModuleCodeValue" runat="server" CssClass="form-control"></asp:Label>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lbModule" runat="server" Text="Module" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModuleValue" runat="server" CssClass="form-control"></asp:Label>
                </div>
            </div>
        </asp:Panel>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnEdit" runat="server" CssClass="btn btn-primary" Text="Edit" OnClick="btnEdit_Click" />
                <button id="btnConfirmDel" runat="server" type="button" class="btn btn-danger" data-toggle="modal" data-target="#diagDelete">Delete</button>
            </div>
        </div>
    </div>

    <div id="diagDelete" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete booking</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this booking?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnDelete_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
