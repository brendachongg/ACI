<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="venue-edit.aspx.cs" Inherits="ACI_TMS.venue_edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        function validateEffectiveDate(oSrc, args) {

            var startStr = $('#<%=tbEffectiveDate.ClientID%>').val();

            if (startStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(startStr)) {
                args.IsValid = false;
                return false;
            }

            //module effective date cannot be earlier than current date
            //var today = new Date();
            //var startDt = new Date(startStr);

            //if (today > startDt) {
            //    args.IsValid = false;
            //    return false;
            //}

            args.IsValid = true;
            return true;
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbTitle" runat="server" Font-Bold="true" Text="Edit Module"></asp:Label>
                </h3>
                <small>Please update the following as needed</small>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lkbtnBack_Click" CausesValidation="false"></asp:LinkButton>
            </div>
        </div>

        <hr />
        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger" id="panelSysError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSysError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <asp:HiddenField ID="hfVenueId" runat="server" />

        <div class="row">
            <div class="col-lg-3">
                <asp:Label ID="lbVenueID" runat="server" Text="ID: " Font-Bold="true"></asp:Label>
                <asp:Label ID="lbVenueIDValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-9">
                <asp:Label ID="lbLocation" runat="server" Text="Location: " Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbLocation" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>

                <asp:RequiredFieldValidator ID="locationRV" Display="None" ForeColor="Red" ControlToValidate="tbLocation" runat="server" ErrorMessage="Please fill in the Venue Location."></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:Label ID="lbDescription" runat="server" Text="Description: " Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" MaxLength="255"></asp:TextBox>

                <asp:RequiredFieldValidator ID="descriptionRV" Display="None" ForeColor="Red" ControlToValidate="tbDescription" runat="server" ErrorMessage="Please fill in the Venue Description."></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-6">
                <asp:Label ID="lbCapacity" runat="server" Text="Capacity: " Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbCapacity" runat="server" CssClass="form-control" MaxLength="4"></asp:TextBox>

                <asp:RequiredFieldValidator ID="capacityRV" Display="None" ForeColor="Red" ControlToValidate="tbCapacity" runat="server" ErrorMessage="Please fill in the Venue Capacity."></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revCapacity" runat="server" ErrorMessage="Capacity must be positive whole number, greater than zero" Display="None" ControlToValidate="tbCapacity" ValidationExpression="^[1-9]\d*$" ></asp:RegularExpressionValidator>
            </div>

            <div class="col-lg-6">
                <asp:Label ID="lbEffectiveDate" runat="server" Text="Effective Date: " Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbEffectiveDate" runat="server" CssClass="form-control datepicker"></asp:TextBox>

                <asp:RequiredFieldValidator ID="effectDateRV" Display="None" ForeColor="Red" ControlToValidate="tbEffectiveDate" runat="server" ErrorMessage="Please fill in the Effective Date of Venue."></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvEffectDate" runat="server" Display="None" ControlToValidate="tbEffectiveDate" ClientValidationFunction="validateEffectiveDate"
                    ErrorMessage="Date is not in dd MMM yyyy format" ValidateEmptyText="false"></asp:CustomValidator>
            </div>
        </div>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" Text="Save" />
                <button type="button" class="btn btn-default" data-toggle="modal" data-target="#diagReloadmodule">Clear</button>
            </div>
        </div>

        <div id="diagReloadmodule" class="modal fade" role="dialog">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <h4 class="modal-title">Discard</h4>
                    </div>
                    <div class="modal-body">
                        Are you sure you want to discard all changes?
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnClearVenue" runat="server" CssClass="btn btn-default" Text="OK" OnClick="btnClearVenue_Click" CausesValidation="false" />
                        <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
