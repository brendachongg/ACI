<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="module-search.ascx.cs" Inherits="ACI_TMS.module_search" %>
<script type="text/javascript">
    function showModuleDialog() {
        $('#diagSearchModule').modal('show');
    }

    function showListModule() {
        $('.nav-tabs a[href="#listModule"]').tab('show');
    }
</script>
<div id="diagSearchModule" class="modal fade" role="dialog">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Search Module</h4>
            </div>
            <div class="modal-body">
                <ul class="nav nav-tabs">
                    <li class="active"><a data-toggle="tab" href="#recentModule">Recent</a></li>
                    <li><a data-toggle="tab" href="#listModule">Listing</a></li>
                </ul>
                <div class="tab-content">
                    <br />
                    <div id="recentModule" class="tab-pane fade in active">
                        <asp:GridView runat="server" ID="gvRecentModule" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" OnRowCommand="gvModule_RowCommand"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="false">
                            <EmptyDataTemplate>
                                No modules are found
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="moduleId" HeaderText="ID" />
                                <asp:TemplateField HeaderText="Module Code">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnModuleId" runat="server" CommandName="selectModuleRecent" CommandArgument='<%# Container.DataItemIndex %>' CausesValidation="false">
                                            <asp:Label ID="lbgvenueLocation" runat="server" Text='<%# Eval("moduleCode") %>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="moduleVersion" HeaderText="Version" ItemStyle-Width="50px"/>
                                <asp:BoundField DataField="moduleTitle" HeaderText="Title" />
                                <asp:BoundField DataField="moduleEffectDate" HeaderText="Eff. Date" ItemStyle-Width="120px" />
                            </Columns>
                        </asp:GridView>
                    </div>
                    <div id="listModule" class="tab-pane fade" style="text-align:center">
                        <asp:Button ID="btnModAD" runat="server" Text="A - D" CssClass="btn btn-primary" CommandArgument="AD" OnClick="btnMod_Click" CausesValidation="false" />
                        <asp:Button ID="btnModEH" runat="server" Text="E - H" CssClass="btn btn-default" CommandArgument="EH" OnClick="btnMod_Click" CausesValidation="false" />
                        <asp:Button ID="btnModIL" runat="server" Text="I - L" CssClass="btn btn-default" CommandArgument="IL" OnClick="btnMod_Click" CausesValidation="false" />
                        <asp:Button ID="btnModMP" runat="server" Text="M - P" CssClass="btn btn-default" CommandArgument="MP" OnClick="btnMod_Click" CausesValidation="false" />
                        <asp:Button ID="btnModQT" runat="server" Text="Q - T" CssClass="btn btn-default" CommandArgument="QT" OnClick="btnMod_Click" CausesValidation="false" />
                        <asp:Button ID="btnModUX" runat="server" Text="U - X" CssClass="btn btn-default" CommandArgument="UX" OnClick="btnMod_Click" CausesValidation="false" />
                        <asp:Button ID="btnModYZ" runat="server" Text="Y - Z, #" CssClass="btn btn-default" CommandArgument="YZ#" OnClick="btnMod_Click" CausesValidation="false" />
                        <br />
                        <br />
                        <asp:GridView runat="server" ID="gvListModule" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" OnRowCommand="gvModule_RowCommand"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="5" OnPageIndexChanging="gvListModule_PageIndexChanging">
                            <EmptyDataTemplate>
                                No modules are found
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="moduleId" HeaderText="ID" />
                                <asp:TemplateField HeaderText="Module Code">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnModuleId" runat="server" CommandName="selectModuleList" CommandArgument='<%# Container.DataItemIndex %>' CausesValidation="false">
                                            <asp:Label ID="lbgvenueLocation" runat="server" Text='<%# Eval("moduleCode") %>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="moduleVersion" HeaderText="Version" ItemStyle-Width="50px" />
                                <asp:BoundField DataField="moduleTitle" HeaderText="Title" />
                                <asp:BoundField DataField="moduleEffectDate" HeaderText="Eff. Date" ItemStyle-Width="120px" />
                            </Columns>
                            <PagerStyle CssClass="pagination-ys" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
