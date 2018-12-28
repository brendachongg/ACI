<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="programme_search.ascx.cs" Inherits="ACI_TMS.programme_search" %>
<script type="text/javascript">
    function showProgDialog() {
        $('#diagSearchProg').modal('show');
    }

</script>
<div id="diagSearchProg" class="modal fade" role="dialog">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Search Programme</h4>
            </div>
            <div class="modal-body">
                <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" ValidationGroup="prog" />
                <div class="row">
                    <div class="col-lg-4" style="text-align:center">
                        <asp:RadioButton ID="rbCode" runat="server" Checked="true" GroupName="searchBy" />&nbsp;&nbsp;<b>Code</b>
                    </div>
                    <div class="col-lg-4" style="text-align:center">
                        <asp:RadioButton ID="rbTitle" runat="server" GroupName="searchBy" />&nbsp;&nbsp;<b>Title</b>
                    </div>
                    <div class="col-lg-4" style="text-align:center">
                        <asp:RadioButton ID="rbCseCode" runat="server" GroupName="searchBy" />&nbsp;&nbsp;<b>Course Code</b>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-lg-12">
                        <div class="input-group">
                            <asp:TextBox ID="tbSearchProg" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                            <asp:LinkButton ID="lbtnSearchProg" runat="server" Font-Underline="false" OnClick="lbtnSearchProg_Click" CssClass="glyphicon glyphicon-search input-group-addon" ValidationGroup="prog"></asp:LinkButton>
                        </div>
                        <asp:RequiredFieldValidator ID="rfvProgSearch" ControlToValidate="tbSearchProg" ValidationGroup="prog" runat="server" Display="None" ErrorMessage="Search value cannot be empty"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-lg-12">
                        <asp:GridView runat="server" ID="gvProg" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" OnRowCommand="gvProg_RowCommand"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="5" OnPageIndexChanging="gvProg_PageIndexChanging">
                            <EmptyDataTemplate>
                                No programme are found
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="programmeId" HeaderText="ID" />
                                <asp:TemplateField HeaderText="Code" ItemStyle-Width="200px">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnProg" runat="server" CommandName="selectProg" CommandArgument='<%# Container.DataItemIndex %>' CausesValidation="false">
                                            <asp:Label ID="lbgProgCode" runat="server" Text='<%# Eval("programmeCode") %>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="courseCode" HeaderText="Course Code" ItemStyle-Width="200px" />
                                <asp:BoundField DataField="programmeVersion" HeaderText="Version" ItemStyle-Width="60px" />
                                <asp:BoundField DataField="programmeTitle" HeaderText="Title" />
                            </Columns>
                            <PagerStyle CssClass="pagination-ys" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
