<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="booking-creation.aspx.cs" Inherits="ACI_TMS.booking_creation" %>

<%@ Register Src="~/venue-search.ascx" TagPrefix="uc1" TagName="venuesearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function validateDate(oSrc, args) {
            if ($('#<%=tbDate.ClientID%>').val().length == 0) {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate($('#<%=tbDate.ClientID%>').val())) {
                args.IsValid = false;
                return false;
            }

            var dt = new Date($('#<%=tbDate.ClientID%>').val());
            var today = new Date();
            if (dt < today) {
                args.IsValid = false;
                return false;
            }

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
                    <asp:Label ID="lbNewBookingHeader" runat="server" Text="New Booking"></asp:Label>
                </h3>

                <small>Please fill up the following
                </small>

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
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="ava" />
        <asp:ValidationSummary ID="vSummary1" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        
        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbVenue" runat="server" Text="Venue" Font-Bold="true"></asp:Label>
                <div class="inner-addon right-addon">
                    <i class="glyphicon glyphicon-search" data-toggle="modal" data-target="#diagSearchVenue" style="cursor: pointer;"></i>
                    <asp:TextBox ID="tbVenue" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    <asp:HiddenField ID="hfVenueId" runat="server" />
                </div>
                <asp:RequiredFieldValidator ID="rfvVenue" runat="server" ErrorMessage="Venue cannot be empty." Display="None" ValidationGroup="ava" ControlToValidate="tbVenue"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="rfvVenue1" runat="server" ErrorMessage="Venue cannot be empty." Display="None" ControlToValidate="tbVenue"></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbDate" runat="server" Text="Date" Font-Bold="true"></asp:Label>
                <div class="input-group">
                    <asp:TextBox ID="tbDate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                    <span class="input-group-btn">
                        <asp:Button ID="btnAvailable" runat="server" CssClass="btn btn-default" Text="View Availability" OnClick="btnAvailable_Click" ValidationGroup="ava" />
                    </span>
                </div>
                <asp:CustomValidator ID="cvDate" runat="server" ErrorMessage="Date cannot be empty OR invalid date OR date cannot be earlier than today." 
                    Display="None" ControlToValidate="tbDate" ClientValidationFunction="validateDate" ValidateEmptyText="true" ValidationGroup="ava"></asp:CustomValidator>
                <asp:CustomValidator ID="cvDate1" runat="server" ErrorMessage="Date cannot be empty OR invalid date OR date cannot be earlier than today." 
                    Display="None" ControlToValidate="tbDate" ClientValidationFunction="validateDate" ValidateEmptyText="true" ></asp:CustomValidator>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:Label ID="lbRemark" runat="server" Text="Remarks" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbRemark" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvRemarks" runat="server" ErrorMessage="Remarks cannot be empty." ControlToValidate="tbRemark" Display="None"></asp:RequiredFieldValidator>
            </div>
        </div>


        <br />

        <div class="row" id="panelBooking" runat="server" visible="false">
            <div class="col-lg-12">
                <asp:Label ID="lbBookings" runat="server" Text="Availability" Font-Bold="true"></asp:Label>
                <asp:GridView ID="gvNewBooking" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" OnRowDataBound="gvNewBooking_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox ID="cbNewBooking" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="bookingPeriod" />
                        <asp:BoundField HeaderText="Period" DataField="codeValueDisplay" ItemStyle-Width="80px" />
                        <asp:TemplateField HeaderText="Details">
                            <ItemTemplate>
                                <asp:Label ID="lbBookingDetail" runat="server" Text="Period available for booking." Visible="false"></asp:Label>
                                <asp:Label ID="lbBookingClassCode" runat="server" Visible="false"></asp:Label>
                                <asp:Label ID="lbBookingModuleId" runat="server" Visible="false"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
                <asp:Button ID="btnClear" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="btnClear_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <uc1:venuesearch runat="server" ID="venuesearch" />
</asp:Content>
