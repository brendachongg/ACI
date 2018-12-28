<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="trainee-search.ascx.cs" Inherits="ACI_TMS.trainee_search" %>
<script type="text/javascript">
    function showTraineeDialog() {
        $('#diagSearchTrainee').modal('show');
    }

</script>
<div id="diagSearchTrainee" class="modal fade" role="dialog">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Search Trainee</h4>
            </div>
            <div class="modal-body">
                <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" ValidationGroup="trainee" />
                <div class="row">
                    <div class="col-lg-4" style="text-align:center">
                        <asp:RadioButton ID="rbId" runat="server" Checked="true" GroupName="searchBy" />&nbsp;&nbsp;<b>ID</b>
                    </div>
                    <div class="col-lg-4" style="text-align:center">
                        <asp:RadioButton ID="rbName" runat="server" GroupName="searchBy" />&nbsp;&nbsp;<b>Name</b>
                    </div>
                    <div class="col-lg-4" style="text-align:center">
                        <asp:RadioButton ID="rbNRIC" runat="server" GroupName="searchBy" />&nbsp;&nbsp;<b>NRIC/PIN</b>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-lg-12">
                        <div class="input-group">
                            <asp:TextBox ID="tbSearchTrainee" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                            <asp:LinkButton ID="lbtnSearchTrainee" runat="server" Font-Underline="false" OnClick="lbtnSearchTrainee_Click" CssClass="glyphicon glyphicon-search input-group-addon" ValidationGroup="trainee"></asp:LinkButton>
                        </div>
                        <asp:RequiredFieldValidator ID="rfvTraineeSearch" ControlToValidate="tbSearchTrainee" ValidationGroup="trainee" runat="server" Display="None" ErrorMessage="Search value cannot be empty"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-lg-12">
                        <asp:GridView runat="server" ID="gvTrainee" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" OnRowCommand="gvTrainee_RowCommand"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="5" OnPageIndexChanging="gvTrainee_PageIndexChanging">
                            <EmptyDataTemplate>
                                No trainees are found
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="traineeId" HeaderText="ID" />
                                <asp:TemplateField HeaderText="ID" ItemStyle-Width="200px">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnTraineeId" runat="server" CommandName="selectTrainee" CommandArgument='<%# Container.DataItemIndex %>' CausesValidation="false">
                                            <asp:Label ID="lbgTraineeId" runat="server" Text='<%# Eval("traineeId") %>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="idNumber" HeaderText="NRIC/PIN" ItemStyle-Width="200px" />
                                <asp:BoundField DataField="fullName" HeaderText="Name" />
                            </Columns>
                            <PagerStyle CssClass="pagination-ys" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
