<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="bundle-search.ascx.cs" Inherits="ACI_TMS.bundle_search" %>
<script type="text/javascript">
    function showPackageDialog() {
        $('#diagSearchPackage').modal('show');
    }

    function showListPackage() {
        $('.nav-tabs a[href="#listing"]').tab('show');
    }
</script>
<div id="diagSearchPackage" class="modal fade" role="dialog">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Search Bundle</h4>
            </div>
            <div class="modal-body">
                <ul class="nav nav-tabs">
                    <li class="active"><a data-toggle="tab" href="#recent">Recent</a></li>
                    <li><a data-toggle="tab" href="#listing">Listing</a></li>
                </ul>
                <div class="tab-content">
                    <div id="recent" class="tab-pane fade in active">
                        <asp:GridView runat="server" ID="gvRecentBundle" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" ShowHeader="false"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="false" OnRowCommand="gvBundle_RowCommand">
                            <EmptyDataTemplate>
                                No bundles are found
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnBundleCode" runat="server" CausesValidation="false" CommandName="selectBundle" Text='<%# Eval("bundleCode") %>'> 
                                        </asp:LinkButton>
                                        <asp:HiddenField ID="hfBundleCode" runat="server" Value='<%# Eval("bundleCode") %>'></asp:HiddenField>
                                        <asp:HiddenField ID="hfBundleId" runat="server" Value='<%# Eval("bundleId") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <div id="listing" class="tab-pane fade" style="text-align:center">
                        <br />
                        <asp:Button ID="btnPkgAD" runat="server" Text="A - D" CssClass="btn btn-primary" OnClick="btnPkg_Click" CommandArgument="AD" CausesValidation="false" />
                        <asp:Button ID="btnPkgEH" runat="server" Text="E - H" CssClass="btn btn-default" OnClick="btnPkg_Click" CommandArgument="EH" CausesValidation="false" />
                        <asp:Button ID="btnPkgIL" runat="server" Text="I - L" CssClass="btn btn-default" OnClick="btnPkg_Click" CommandArgument="IL" CausesValidation="false" />
                        <asp:Button ID="btnPkgMP" runat="server" Text="M - P" CssClass="btn btn-default" OnClick="btnPkg_Click" CommandArgument="MP" CausesValidation="false" />
                        <asp:Button ID="btnPkgQT" runat="server" Text="Q - T" CssClass="btn btn-default" OnClick="btnPkg_Click" CommandArgument="QT" CausesValidation="false" />
                        <asp:Button ID="btnPkgUX" runat="server" Text="U - X" CssClass="btn btn-default" OnClick="btnPkg_Click" CommandArgument="UX" CausesValidation="false" />
                        <asp:Button ID="btnPkgYZ" runat="server" Text="Y - Z, #" CssClass="btn btn-default" OnClick="btnPkg_Click" CommandArgument="YZ#" CausesValidation="false" />
                        
                        <asp:GridView runat="server" ID="gvListBundle" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" ShowHeader="false"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10" 
                            OnPageIndexChanging="gvListBundle_PageIndexChanging" OnRowCommand="gvBundle_RowCommand">
                            <EmptyDataTemplate>
                                No bundles are found
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnBundleCode" runat="server" CausesValidation="false" CommandName="selectBundle" Text='<%# Eval("bundleCode") %>'></asp:LinkButton>
                                        <asp:HiddenField ID="hfBundleCode" runat="server" Value='<%# Eval("bundleCode") %>'></asp:HiddenField>
                                        <asp:HiddenField ID="hfBundleId" runat="server" Value='<%# Eval("bundleId") %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle CssClass="pagination-ys" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
