<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="booking-management.aspx.cs" Inherits="ACI_TMS.booking_management" %>

<%@ Register Src="~/venue-search.ascx" TagPrefix="uc1" TagName="venuesearch" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function validateSearch(oSrc, args) {
            if ($('#<%=tbVenue.ClientID%>').val().length == 0 && $('#<%=tbStartDate.ClientID%>').val().length == 0 && $('#<%=tbEndDate.ClientID%>').val().length == 0) {
                args.IsValid = false;
                return false;
            }

            if ($('#<%=tbStartDate.ClientID%>').val().length != 0 && !isValidDate($('#<%=tbStartDate.ClientID%>').val())) {
                args.IsValid = false;
                return false;
            }

            if ($('#<%=tbEndDate.ClientID%>').val().length != 0 && !isValidDate($('#<%=tbEndDate.ClientID%>').val())) {
                args.IsValid = false;
                return false;
            }

            if($('#<%=tbStartDate.ClientID%>').val().length != 0 && $('#<%=tbEndDate.ClientID%>').val().length != 0){
                var d1 = new Date($('#<%=tbStartDate.ClientID%>').val());
                var d2 = new Date($('#<%=tbEndDate.ClientID%>').val());

                if (d1 > d2) {
                    args.IsValid = false;
                    return false;
                }
            }

            args.IsValid = true;
            return true;
        }

        function clearVenue() {
            $('#<%=tbVenue.ClientID%>').val("");
            $('#<%=hfVenueId.ClientID%>').val("");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbBookingManagementHeader" runat="server" Text="Booking Management"></asp:Label>
                </h2>

            </div>
        </div>

        <hr />
        
        <div class="row" id="panelNewBooking" runat="server">
            <div class="col-lg-9 col-md-9 col-sm-12">
            </div>

            <div class="col-lg-3 col-md-3 col-sm-12">
                <div class="panel panel-default">
                    <div id="listHeader" class="panel-heading">Operations</div>
                    <div class="panel-body">
                        <p>
                            <asp:LinkButton ID="lkbtnCreateBooking" OnClick="lkbtnCreateBooking_Click" runat="server" CausesValidation="false"><span class="fa glyphicon-plus"></span> New Booking</asp:LinkButton>
                        </p>

                    </div>
                </div>
            </div>
        </div>

        <br />
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link"/>
        <div class="row text-left">
            <div class="col-lg-5">
                <asp:Label ID="lbVenue" runat="server" Text="Venue: " Font-Bold="true"></asp:Label>
                <div class="inner-addon right-addon">
                    <i class="glyphicon glyphicon-search" style="cursor: pointer;right: 23px;" data-toggle="modal" data-target="#diagSearchVenue"></i>
                    <i class="glyphicon glyphicon-remove" style="cursor: pointer;" onClick="clearVenue();"></i>
                    <asp:TextBox ID="tbVenue" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    <asp:HiddenField ID="hfVenueId" runat="server" />
                </div>
            </div>
            <div class="col-lg-6">
                <asp:Label ID="lbDate" runat="server" Text="Date: " Font-Bold="true"></asp:Label>
                <div class="input-group">
                    <asp:TextBox ID="tbStartDate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                    <span class="input-group-addon" style="font-weight: bold;">to</span>
                    <asp:TextBox ID="tbEndDate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                </div>
            </div>
            <div class="col-lg-1">
                <br />
                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-info" OnClick="btnSearch_Click">
                    <span aria-hidden="true" class="fa fa-search"></span>
                </asp:LinkButton>
                <asp:CustomValidator ID="cvSearch" runat="server" ErrorMessage="Must enter either Venue or Date OR invalid start date and/or end date OR End date cannot be earlier than start date." 
                    Display="None" ControlToValidate="tbVenue" ClientValidationFunction="validateSearch" ValidateEmptyText="true"></asp:CustomValidator>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvBooking" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvBooking_PageIndexChanging" OnRowDataBound="gvBooking_RowDataBound" OnRowCommand="gvBooking_RowCommand">

                    <EmptyDataTemplate>
                        No available booking record.
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:TemplateField HeaderText="Date" ItemStyle-Width="130px">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnBookingDate" runat="server" CommandName="viewBooking" CommandArgument='<%# Eval("bookingId") %>' CausesValidation="false">
                                    <asp:Label ID="lbBookingDate" runat="server" Text='<%# Eval("bookingDate") %>'></asp:Label>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField HeaderText="Period" DataField="codeValueDisplay" ItemStyle-Width="80px" />

                        <asp:BoundField HeaderText="Location" DataField="venueLocation"/>

                        <asp:TemplateField HeaderText="Details">
                            <ItemTemplate>
                                <asp:Label ID="lbBookingDetail" runat="server" Visible="false"></asp:Label>
                                <asp:Label ID="lbBookingClassCode" runat="server" Visible="false"></asp:Label>
                                <asp:Label ID="lbBookingModuleId" runat="server" Visible="false"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <uc1:venuesearch runat="server" ID="venuesearch" />
</asp:Content>
