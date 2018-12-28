<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="venue-creation.aspx.cs" Inherits="ACI_TMS.venue_creation" %>

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

            //TODO: enable validation
            //effective date cannot be earlier than current date
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
                    <asp:Label ID="lbVenueHeader" runat="server" Text="New Venue"></asp:Label>
                </h3>

                <small>Please fill up the following
                </small>
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
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <div class="row">
            <div class="col-lg-3">
                <asp:Label ID="lbVenueID" runat="server" Text="ID: " Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbVenueID" runat="server" CssClass="form-control" MaxLength="5"></asp:TextBox>

                <asp:RequiredFieldValidator ID="venueIdRV" Display="None" ForeColor="Red" ControlToValidate="tbVenueID" runat="server" ErrorMessage="Please fill in the Venue ID."></asp:RequiredFieldValidator>
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

                <asp:RequiredFieldValidator ID="capacityRV" Display="None" ControlToValidate="tbCapacity" runat="server" ErrorMessage="Please fill in the Venue Capacity."></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revCapacity" runat="server" ErrorMessage="Capacity must be positive whole number, greater than zero" Display="None" ControlToValidate="tbCapacity" ValidationExpression="^[1-9]\d*$" ></asp:RegularExpressionValidator>
            </div>

            <div class="col-lg-6">
                <asp:Label ID="lbEffectiveDate" runat="server" Text="Effective Date: " Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbEffectiveDate" runat="server" CssClass="form-control datepicker"></asp:TextBox>

                <asp:RequiredFieldValidator ID="effectDateRV" Display="None" ForeColor="Red" ControlToValidate="tbEffectiveDate" runat="server" ErrorMessage="Please fill in the Effective Date of Venue."></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvEffectDate" runat="server" Display="None" ControlToValidate="tbEffectiveDate" ClientValidationFunction="validateEffectiveDate"
                    ErrorMessage="Date is not in dd MMM yyyy format OR cannot be earlier than today" ValidateEmptyText="false"></asp:CustomValidator>
            </div>
        </div>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnCreate" runat="server" CssClass="btn btn-info" Text="Create" OnClick="btnCreate_Click" />
                <asp:Button ID="btnClear" runat="server" CssClass="btn btn-default" Text="Clear" CausesValidation="false" OnClick="btnClear_Click" />
            </div>
        </div>

    </div>
</asp:Content>
