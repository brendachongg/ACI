<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="absentee-payment.aspx.cs" Inherits="ACI_TMS.absentee_payment" %>

<%@ Register Src="~/trainee-search.ascx" TagPrefix="uc1" TagName="traineesearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function validatePaymentDate(oSrc, args) {
            var str = $("#<%=tbPaymentDt.ClientID%>").val();

            if (!isValidDate(str)) {
                args.IsValid = false;
                return false;
            }

            var payDt = new Date(str);
            var today = new Date();
            if (payDt > today) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validateDate(str) {
            if (!isValidDate(str)) {
                return false;
            }

            var dt = new Date(str);
            var today = new Date();
            if (dt > today) {
                return false;
            }

            return true;
        }

        function validateVoidDate(oSrc, args) {
            if (validateDate($("#<%=tbVoidDt.ClientID%>").val())) {
                args.IsValid = true;
                return true;
            }

            args.IsValid = false;
            return false;
        }

        function validateBankInDate(oSrc, args) {
            if (validateDate($("#<%=tbBankDt.ClientID%>").val())) {
                args.IsValid = true;
                return true;
            }

            args.IsValid = false;
            return false;
        }

        function showReceipt(pid) {
            window.open('<%= ACI_TMS.absentee_payment_receipt.PAGE_NAME %>?<%= ACI_TMS.absentee_payment_receipt.PAYMENT_QUERY %>=' + encodeURI(pid)
                , '_blank', 'menubar=no,location=no,scrollbars=yes,resizable=yes');
        }

        function showClearDialog(id) {
            $("#<%=hfSelPayment.ClientID%>").val(id);
            $('#diagClearPayment').modal('show');
        }

        function showClearChqDialog(id) {
            $("#<%=hfSelPayment.ClientID%>").val(id);
            $('#diagClearChqPayment').modal('show');
        }

        function showVoidDialog(id) {
            $("#<%=hfSelPayment.ClientID%>").val(id);
            $('#diagVoidPayment').modal('show');
        }

        function showPaymentHistory(id) {
            $("#<%=hfSelSession.ClientID%>").val(id);
            __doPostBack('ctl00$ContentPlaceHolder1$lbtnViewPaymentHistory', '');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>Make-up Fee Payment</h3>

                <small>Please fill up the following</small>
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
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <asp:ValidationSummary ID="vSummary1" ValidationGroup="addPymt" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <div class="row">
            <div class="col-lg-3">
                <asp:Label ID="lb1" runat="server" Text="Trainee ID " Font-Bold="true"></asp:Label>
                <div class="inner-addon right-addon">
                    <i class="glyphicon glyphicon-search" data-toggle="modal" data-target="#diagSearchTrainee" style="cursor: pointer;"></i>
                    <asp:TextBox ID="tbTraineeId" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvTrainee" runat="server" ErrorMessage="Trainee cannot be empty." ControlToValidate="tbTraineeId" Display="None"></asp:RequiredFieldValidator>
            </div>
            <div class="col-lg-9">
                <asp:Label ID="lb2" runat="server" Text="Name " Font-Bold="true"></asp:Label>
                <asp:Label ID="lbTraineeName" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-lg-2">
                <asp:Label ID="lb3" runat="server" Text="Class Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbBatchCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
            <div class="col-lg-2">
                <asp:Label ID="lb4" runat="server" Text="Programme Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
            <div class="col-lg-8">
                <asp:Label ID="lb5" runat="server" Text="Programme" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgTitle" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-lg-12">
                <asp:HiddenField ID="hfBatchId" runat="server" />
                <asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" OnRowDataBound="gvSessions_RowDataBound">
                    <EmptyDataTemplate>
                        No absent records found.
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField HeaderText="Absent ID" DataField="absentId" />
                        <asp:BoundField HeaderText="Session ID" DataField="sessionId" />
                        <asp:BoundField HeaderText="Batch ID" DataField="batchModuleId" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="cb" runat="server" />
                            </ItemTemplate>
                            <ItemStyle Width="50px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Module">
                            <ItemTemplate>
                                <%# Eval("moduleTitle") %>&nbsp;(<%# Eval("moduleCode") %>)
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Session">
                            <ItemTemplate>
                                <%# Eval("sessionDateDisp") %>&nbsp;<%# Eval("sessionPeriodDisp") %>
                            </ItemTemplate>
                            <ItemStyle Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Status</HeaderTemplate>
                            <ItemTemplate>
                                <%# Eval("paymentStatusDisp") %>&nbsp;&nbsp;
                                <asp:Label ID="lbPaymentDetails" runat="server" CssClass="glyphicon glyphicon-info-sign" Style="font-size: 20px;" data-toggle="tooltip" data-placement="top" data-html="true"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="120px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label ID="lbtnClearPayment" runat="server" CssClass="glyphicon glyphicon-ok" Style="font-size: 20px;"
                                    ForeColor="green" data-toggle="tooltip" data-placement="top" title="Clear payment"></asp:Label>
                                <asp:Label ID="lbtnReceipt" runat="server" CssClass="glyphicon glyphicon-file" Style="font-size: 20px;"
                                    ForeColor="green" data-toggle="tooltip" data-placement="top" title="View Receipt"></asp:Label>
                                <asp:Label ID="lbSpace1" runat="server" Text="&nbsp;&nbsp;"></asp:Label>
                                <asp:Label ID="lbtnVoidPayment" runat="server" CssClass="glyphicon glyphicon-remove" Style="font-size: 20px;"
                                    ForeColor="Red" data-toggle="tooltip" data-placement="top" title="Void payment"></asp:Label>
                                <asp:Label ID="lbSpace2" runat="server" Text="&nbsp;&nbsp;"></asp:Label>
                                <asp:Label ID="lbtnHistoryPayment" runat="server" CssClass="glyphicon glyphicon-tasks" Style="font-size: 20px;"
                                    ForeColor="Blue" data-toggle="tooltip" data-placement="top" title="View History"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="120px" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:LinkButton ID="lbtnViewPaymentHistory" runat="server" OnClick="lbtnViewPaymentHistory_Click" CausesValidation="false" Style="display: none;"></asp:LinkButton>
                <asp:HiddenField ID="hfSelSession" runat="server" />
            </div>
        </div>

        <div class="row text-right" runat="server" id="panelMakePayment">
            <br />
            <asp:Button ID="btnMakePayment" runat="server" CssClass="btn btn-info" Text="Make Payment" OnClick="btnMakePayment_Click" CausesValidation="false" />
        </div>

        <div runat="server" id="panelPayment" visible="false">
            <hr />
            <h3>Payment Details</h3>
            <br />
            <div class="row text-left">
                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                    <b>Date</b>
                    <asp:TextBox ID="tbPaymentDt" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPaymentDt" Display="None" ControlToValidate="tbPaymentDt" runat="server"
                        ErrorMessage="Payment date cannot be empty"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cvPaymentDt" runat="server" Display="None" ControlToValidate="tbPaymentDt" ClientValidationFunction="validatePaymentDate"
                        ErrorMessage="Date is not in dd MMM yyyy format OR cannot be later than today" ValidateEmptyText="false"></asp:CustomValidator>
                </div>

                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                    <b>Mode</b>
                    <asp:DropDownList ID="ddlPaymentMode" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvPaymentMode" Display="None" ControlToValidate="ddlPaymentMode" runat="server"
                        ErrorMessage="Payment mode cannot be empty"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                    <b>Reference Num.</b>
                    <asp:TextBox ID="tbPaymentRef" CssClass="form-control" runat="server" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPaymentRef" Display="None" ControlToValidate="tbPaymentRef" runat="server"
                        ErrorMessage="Reference number cannot be empty"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                    <b>Amount (S$)</b>
                    <asp:TextBox ID="tbPaymentAmt" CssClass="form-control" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPaymentAmt" Display="None" ControlToValidate="tbPaymentAmt" runat="server"
                        ErrorMessage="Payment amount cannot be empty"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revPaymentAmt" runat="server" ErrorMessage="Payment amount must be a non negative number, up to 2 decimal places"
                        Display="None" ControlToValidate="tbPaymentAmt" ValidationExpression="^\d+(\.\d{1,2})?$"></asp:RegularExpressionValidator>
                </div>
            </div>

            <br />

            <div class="row text-left">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <b>Remarks</b>
                    <asp:TextBox ID="tbRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                </div>
            </div>

            <br />
            <div class="row text-right">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <asp:Button ID="btnProcessPayment" runat="server" CssClass="btn btn-primary" Text="Process/Save Payment" OnClick="btnProcessPayment_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfSelPayment" runat="server" />
    <div id="diagClearPayment" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Clear Payment</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to mark this payment as paid?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnPaidPayment" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnPaidPayment_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="diagVoidPayment" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Void Payment</h4>
                    <small><i>Enter the following to void the selected payment.</i></small>
                </div>
                <div class="modal-body">
                    <asp:ValidationSummary ID="vSummary2" ValidationGroup="voidPymt" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <b>Date:</b>
                            <asp:TextBox ID="tbVoidDt" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvVoidDt" Display="None" ControlToValidate="tbVoidDt" runat="server"
                                ErrorMessage="Date cannot be empty" ValidationGroup="voidPymt"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="cvVoidDt" runat="server" Display="None" ControlToValidate="tbVoidDt" ClientValidationFunction="validateVoidDate"
                                ErrorMessage="Date is not in dd MMM yyyy format OR cannot be later than today" ValidateEmptyText="false" ValidationGroup="voidPymt"></asp:CustomValidator>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <b>Reason:</b>
                            <asp:TextBox ID="tbVoidReason" runat="server" CssClass="form-control" Rows="2" TextMode="MultiLine"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvVoidReason" Display="None" ControlToValidate="tbVoidReason" runat="server"
                                ErrorMessage="Reason cannot be empty" ValidationGroup="voidPymt"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnVoidPayment" runat="server" CssClass="btn btn-primary" Text="OK" ValidationGroup="voidPymt" OnClick="btnVoidPayment_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="diagClearChqPayment" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Clear Cheque Payment</h4>
                </div>
                <div class="modal-body">
                    <asp:ValidationSummary ID="vsummary3" ValidationGroup="bankPymt" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <b>Bank In Date:</b>
                            <asp:TextBox ID="tbBankDt" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvBankDt" Display="None" ControlToValidate="tbBankDt" runat="server" 
                                ErrorMessage="Date cannot be empty" ValidationGroup="bankPymt"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="cvBankDt" runat="server" Display="None" ControlToValidate="tbBankDt" ClientValidationFunction="validateBankInDate"
                                ErrorMessage="Date is not in dd MMM yyyy format OR cannot be later than today" ValidateEmptyText="false" ValidationGroup="bankPymt"></asp:CustomValidator>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnClearChq" runat="server" CssClass="btn btn-primary" Text="OK" ValidationGroup="bankPymt" OnClick="btnPaidPayment_Click"/>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="diagPaymentHist" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Payment History</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <asp:GridView ID="gvPayment" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered dataTable no-footer hover gvv"
                                ShowHeaderWhenEmpty="true" OnRowDataBound="gvPayment_RowDataBound">
                                <EmptyDataTemplate>
                                    No available records
                                </EmptyDataTemplate>
                                <Columns>
                                    <asp:BoundField DataField="paymentDateDisp" HeaderText="Date" ItemStyle-Width="150px" />
                                    <asp:BoundField DataField="paymentModeDisp" HeaderText="Mode" ItemStyle-Width="100px" />
                                    <asp:BoundField DataField="referenceNumber" HeaderText="Reference Num." ItemStyle-Width="200px" />
                                    <asp:BoundField DataField="paymentAmount" HeaderText="Amount (S$)" ItemStyle-Width="100px" DataFormatString="{0:#,##0.00}" />
                                    <asp:BoundField DataField="paymentRemarks" HeaderText="Remarks" />
                                    <asp:TemplateField>
                                        <HeaderTemplate>Status</HeaderTemplate>
                                        <ItemTemplate>
                                            <%# Eval("paymentStatusDisp") %>&nbsp;&nbsp;
                                            <asp:Label ID="lbVoidDetails" runat="server" CssClass="glyphicon glyphicon-info-sign" Style="font-size: 20px;" data-toggle="tooltip" data-placement="top" data-html="true"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="120px" />
                                    </asp:TemplateField>
                                </Columns>

                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <uc1:traineesearch runat="server" ID="traineesearch" />
</asp:Content>
