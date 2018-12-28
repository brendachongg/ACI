<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="bundle-module-rem-session.ascx.cs" Inherits="ACI_TMS.bundle_module_rem_session" %>
<h4>Remove <asp:Label ID="lblNumSession" runat="server"></asp:Label> Session(s)</h4>
<asp:HiddenField ID="hfTotalSession" runat="server" />
<br />
<div class="row">
    <div class="col-lg-12">
        <asp:GridView ID="gvRemovalSession" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="false"
            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" >
            <Columns>
                <asp:BoundField DataField="sessionId" />
                <asp:BoundField DataField="sessionPeriod" />
                <asp:TemplateField HeaderText="" ItemStyle-Width="80px">
                    <ItemTemplate>
                        <div class="row text-center">
                            <asp:CheckBox ID="cb" runat="server" CssClass="text-center" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Session No" DataField="sessionNumber" ItemStyle-Width="80px" />
                <asp:BoundField HeaderText="Date" DataField="sessionDateDisp" ItemStyle-Width="150px"/>
                <asp:BoundField HeaderText="Period" DataField="sessionPeriodDisp" ItemStyle-Width="100px"/>
                <asp:BoundField HeaderText="Venue" DataField="venueLocation" />
            </Columns>

            <PagerStyle CssClass="pagination-ys" />

        </asp:GridView>
    </div>
</div>
