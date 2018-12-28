<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="registration-fee-management.aspx.cs" Inherits="ACI_TMS.registration_fee_management" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.collapse').on('shown.bs.collapse', function () {
                $(this).parent().find(".glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-minus");
            }).on('hidden.bs.collapse', function () {
                $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-plus");
            });
        });

        function validateDate(oSrc, args) {
            var str = args.Value;

            if (!isValidDate(str)) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function showDelDialog(id) {
            $("#<%=hfSelFee.ClientID%>").val(id);
            $('#diagDelReg').modal('show');
        }

        function showNew() {
            $('#newRegFeeDetails').addClass("in");
            $('#newRegFeeIcon').removeClass("glyphicon-plus").addClass("glyphicon-minus");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Registration Fee Management"></asp:Label>
                </h2>
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
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="new" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="edit" />

        <div class="row" id="panelNewReg" runat="server">
            <div class="col-lg-12">
                <div class="panel-group" id="accordion">
                    <div class="panel panel-default">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="panel-title">
                                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#newRegFeeDetails">
                                        <span id="newRegFeeIcon" class="glyphicon glyphicon-plus"></span>
                                        New
                                    </a>
                                </h4>
                            </div>
                            <div id="newRegFeeDetails" class="panel-collapse collapse">
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <div class="form-group form-inline">
                                                <b>Effective Date:</b>
                                                <asp:TextBox ID="tbDt" runat="server" placeholder="" CssClass="datepicker form-control" Width="200px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvDt" runat="server" ValidationGroup="new" ControlToValidate="tbDt" ErrorMessage="Effective date cannot be empty" Display="None"></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cvDt" runat="server" Display="None" ControlToValidate="tbDt" ClientValidationFunction="validateDate"
                                                    ErrorMessage="Invalid date or Date is not in dd MMM yyyy format" ValidateEmptyText="false" ValidationGroup="new"></asp:CustomValidator>
                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                <b>Fee (S$):</b>
                                                <asp:TextBox ID="tbFee" runat="server" placeholder="" CssClass="form-control" Width="150px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvFee" runat="server" ValidationGroup="new" ControlToValidate="tbFee" ErrorMessage="Fee cannot be empty" Display="None"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revFee" runat="server" ErrorMessage="Fee must be a non negative number, up to 2 decimal places" 
                                                    Display="None" ControlToValidate="tbFee" ValidationExpression="^\d+(\.\d{1,2})?$" ValidationGroup="new"></asp:RegularExpressionValidator>
                                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="Add" ValidationGroup="new" OnClick="btnAdd_Click" />&nbsp;
                                                <asp:Button ID="btnClear" runat="server" CssClass="btn btn-default" Text="Clear" CausesValidation="false" OnClick="btnClear_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <br />
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvReg" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" DataKeyNames="feeId"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10" OnRowDataBound="gvReg_RowDataBound"
                    OnPageIndexChanging="gvReg_PageIndexChanging" OnRowEditing="gvReg_RowEditing" OnRowCancelingEdit="gvReg_RowCancelingEdit" OnRowUpdating="gvReg_RowUpdating">
                    <EmptyDataTemplate>
                        No available records
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>Effective Date</HeaderTemplate>
                            <ItemTemplate>
                                <%# Eval("effectiveDateDisp") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Fee (S$)</HeaderTemplate>
                            <ItemTemplate>
                                <%# Eval("registrationFeeDisp") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="tbFee" runat="server" placeholder="" CssClass="form-control" Text='<%# Eval("registrationFeeDisp") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvFee" runat="server" ValidationGroup="edit" ControlToValidate="tbFee" ErrorMessage="Fee cannot be empty" Display="None"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revFee" runat="server" ErrorMessage="Fee must be a non negative number, up to 2 decimal places" 
                                                    Display="None" ControlToValidate="tbFee" ValidationExpression="^\d+(\.\d{1,2})?$" ValidationGroup="edit"></asp:RegularExpressionValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnEdit" runat="server" CommandName="Edit" CausesValidation="false" CssClass="glyphicon glyphicon-pencil" style="font-size: 20px;text-decoration:none;"></asp:LinkButton>
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lbtnDelReg" runat="server" CssClass="glyphicon glyphicon-remove" style="font-size: 20px;"
                                    ForeColor="Red" ></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton ID="lbtnEdit" runat="server" CommandName="Update" ValidationGroup="edit" CssClass="glyphicon glyphicon-ok" style="font-size: 20px;text-decoration:none;" ForeColor="Green"></asp:LinkButton>
                                &nbsp;&nbsp;&nbsp;
                                <asp:LinkButton ID="lbtnCancel" runat="server" CommandName="Cancel" CausesValidation="false" CssClass="glyphicon glyphicon-remove" style="font-size: 20px;text-decoration:none;" ForeColor="Red"></asp:LinkButton>
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                        </asp:TemplateField>
                    </Columns>
                    
                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfSelFee" runat="server" />
    <div id="diagDelReg" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete Registration Fee</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this registration fee? The action cannot be reversed.
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelReg" runat="server" CssClass="btn btn-primary" Text="OK" CausesValidation="false" OnClick="btnDelReg_Click"/>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
