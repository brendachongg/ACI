<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="booking-edit.aspx.cs" Inherits="ACI_TMS.booking_edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbViewBookingHeader" runat="server" Text="Edit Booking"></asp:Label>
                </h3>
                <small>Please update as needed</small>
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

         <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <asp:HiddenField ID="hfBookingId" runat="server" />

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbDate" runat="server" Text="Date" Font-Bold="true"></asp:Label>

                <asp:Label ID="lbDateValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
            <div class="col-lg-3">
                <asp:Label ID="lbPeriod" runat="server" Text="Period" Font-Bold="true"></asp:Label>

                <asp:Label ID="lbPeriodValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
             <div class="col-lg-6">
                <asp:Label ID="lbVenue" runat="server" Text="Venue" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbVenueValue" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>
   
        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:Label ID="lbRemark" runat="server" Text="Remarks" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbRemarkValue" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvRemarks" runat="server" ErrorMessage="Remarks cannot be empty." ControlToValidate="tbRemarkValue" Display="None"></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>
</asp:Content>
