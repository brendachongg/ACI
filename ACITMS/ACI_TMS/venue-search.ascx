<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="venue-search.ascx.cs" Inherits="ACI_TMS.venue_search" %>
<script type="text/javascript">
    function showVDialog() {
        $('#diagSearchVenue').modal('show');
    }

    function showListVenue() {
        $('.nav-tabs a[href="#listVenue"]').tab('show');
    }
</script>
<div id="diagSearchVenue" class="modal fade" role="dialog">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Search Venue</h4>
            </div>
            <div class="modal-body">
                <ul class="nav nav-tabs">
                    <li class="active"><a data-toggle="tab" href="#recentVenue">Recent</a></li>
                    <li><a data-toggle="tab" href="#listVenue">Listing</a></li>
                </ul>
                <div class="tab-content">
                    <br />
                    <div id="recentVenue" class="tab-pane fade in active">
                        <asp:GridView runat="server" ID="gvRecentVenue" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" OnRowCommand="gvVenue_RowCommand"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="false">
                            <EmptyDataTemplate>
                                No venues are found
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="Location">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnVenueId" runat="server" CausesValidation="false" CommandName="selectVenueRecent" CommandArgument='<%# Container.DataItemIndex %>'>
                                            <asp:HiddenField ID="hfVenueId" runat="server" Value='<%# Eval("venueId") %>' />
                                            <asp:Label ID="lbgvenueLocation" runat="server" Text='<%# Eval("venueLocation") %>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="venueDesc" HeaderText="Description" />
                                <asp:BoundField DataField="venueCapacity" HeaderText="Capacity" ItemStyle-Width="100px" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    <div id="listVenue" class="tab-pane fade" style="text-align:center">
                        <br />
                        <asp:Button ID="btnVenueAD" runat="server" Text="A - D" CssClass="btn btn-primary" CommandArgument="AD" OnClick="btnVenue_Click" CausesValidation="false" />
                        <asp:Button ID="btnVenueEH" runat="server" Text="E - H" CssClass="btn btn-default" CommandArgument="EH" OnClick="btnVenue_Click" CausesValidation="false" />
                        <asp:Button ID="btnVenueIL" runat="server" Text="I - L" CssClass="btn btn-default" CommandArgument="IL" OnClick="btnVenue_Click" CausesValidation="false" />
                        <asp:Button ID="btnVenueMP" runat="server" Text="M - P" CssClass="btn btn-default" CommandArgument="MP" OnClick="btnVenue_Click" CausesValidation="false" />
                        <asp:Button ID="btnVenueQT" runat="server" Text="Q - T" CssClass="btn btn-default" CommandArgument="QT" OnClick="btnVenue_Click" CausesValidation="false" />
                        <asp:Button ID="btnVenueUX" runat="server" Text="U - X" CssClass="btn btn-default" CommandArgument="UX" OnClick="btnVenue_Click" CausesValidation="false" />
                        <asp:Button ID="btnVenueYZ" runat="server" Text="Y - Z, #" CssClass="btn btn-default" CommandArgument="YZ#" OnClick="btnVenue_Click" CausesValidation="false" />
                        <br />
                        <asp:GridView runat="server" ID="gvListVenue" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" ShowHeader="false" OnRowCommand="gvVenue_RowCommand"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10" OnPageIndexChanging="gvListVenue_PageIndexChanging">
                            <EmptyDataTemplate>
                                No venues are found
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="Location">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnVenueId" runat="server" CausesValidation="false" CommandName="selectVenueList" CommandArgument='<%# Container.DataItemIndex %>'>
                                            <asp:HiddenField ID="hfVenueId" runat="server" Value='<%# Eval("venueId") %>' />
                                            <asp:Label ID="lbgvenueLocation" runat="server" Text='<%# Eval("venueLocation") %>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="venueDesc" HeaderText="Description" />
                                <asp:BoundField DataField="venueCapacity" HeaderText="Capacity" ItemStyle-Width="100px" />
                            </Columns>
                            <PagerStyle CssClass="pagination-ys" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
