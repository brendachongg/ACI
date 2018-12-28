<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="applicant-programme-payment.aspx.cs" Inherits="ACI_TMS.applicant_programme_payment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function calculateNetTotal(frm) {
            var regCtrl = $("#<%=tbRegFee.ClientID%>");
            var gstCtrl = $("#<%=tbGST.ClientID%>");
            var gsthdCtrl = $("#<%=hfGST.ClientID%>");
            var feeCtrl = $("#<%=lbProgFee.ClientID%>");
            var subCtrl = $("#<%=hfSubsidyAmt.ClientID%>");
            var totallbCtrl = $("#<%=lbNetTotalValue.ClientID%>");
            var outlbCtrl = $("#<%=lbAmtOutstanding.ClientID%>");
            var outCtrl = $("#<%=hfAmtOutstanding.ClientID%>");
            var paidCtrl = $("#<%=hfAmtPaid.ClientID%>");

            var total = 0;
            if (regCtrl.length != 0) {
                var reg = regCtrl.val().replace(",", "");
                if (isNaN(reg)) {
                    totallbCtrl.html("N/A");
                    return;
                } else total += parseFloat(reg);
            }

            if (feeCtrl.length != 0) {
                var fee = feeCtrl.html().replace(",", "");
                total += parseFloat(fee);
            }
            if (subCtrl.length != 0) total -= parseFloat(subCtrl.val());

            var gst = 0;
            if (frm == "GST") {
                //set gst
                gst = gstCtrl.val().replace(",", "");
                if (isNaN(gst)) {
                    totallbCtrl.html("N/A");
                    return;
                } else gst = parseFloat(gst); 
            } else {
                //calculate gst
                gst = total * <%=GeneralLayer.General_Constance.GST_RATE%>;
                gstCtrl.val(toFixed2dp(gst).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));
            }

            total += gst;
            gsthdCtrl.val(toFixed2dp(gst).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));
            //alert(gsthdCtrl.val());
            totallbCtrl.html(toFixed2dp(total).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));

            var outstanding = total - parseFloat(paidCtrl.val())
            outCtrl.val(outstanding);
            outlbCtrl.html("S$" + toFixed2dp(outstanding).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));
        }


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
            if(validateDate($("#<%=tbVoidDt.ClientID%>").val())){
                args.IsValid = true;
                return true;
            }

            args.IsValid = false;
            return false;
        }

        function validateBankInDate(oSrc, args) {
            if(validateDate($("#<%=tbBankDt.ClientID%>").val())){
                args.IsValid = true;
                return true;
            }

            args.IsValid = false;
            return false;
        }

        function showReceipt(appln, trainee, type, pid) {
            if (trainee == "") {
                window.open('<%= ACI_TMS.applicant_programme_receipt.PAGE_NAME %>?<%= ACI_TMS.applicant_programme_receipt.APPLICANT_QUERY %>=' + encodeURI(appln)
                    + '&<%= ACI_TMS.applicant_programme_receipt.TYPE_QUERY %>=' + encodeURI(type) + '&<%= ACI_TMS.applicant_programme_receipt.PAYMENT_QUERY %>=' + encodeURI(pid), '_blank', 'menubar=no,location=no,scrollbars=yes,resizable=yes');
            } else {
                window.open('<%= ACI_TMS.applicant_programme_receipt.PAGE_NAME %>?<%= ACI_TMS.applicant_programme_receipt.TRAINEE_QUERY %>=' + encodeURI(trainee)
                   + '&<%= ACI_TMS.applicant_programme_receipt.TYPE_QUERY %>=' + encodeURI(type) + '&<%= ACI_TMS.applicant_programme_receipt.PAYMENT_QUERY %>=' + encodeURI(pid), '_blank', 'menubar=no,location=no,scrollbars=yes,resizable=yes');
            }
        }

        function showVoidDialog1() {
            $('#diagVoidPayment').modal('show');
        }

        function showDelDialog() {
            $('#diagDelPayment').modal('show');
        }

        function showClearDialog(index) {
            $("#<%=hfSelPayment.ClientID%>").val(index);
            $('#diagClearPayment').modal('show');
        }

        function showClearChqDialog(index) {
            $("#<%=hfSelPayment.ClientID%>").val(index);
            $('#diagClearChqPayment').modal('show');
        }

        function showVoidDelDialog(index) {
            $("#<%=hfSelPayment.ClientID%>").val(index);
            $('#diagDelVoidPayment').modal('show');
        }

        function showVoidDialog2(index) {           
            $("#<%=hfSelPayment.ClientID%>").val(index);
            $('#diagVoidPayment').modal('show');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbHeader" runat="server"></asp:Label>&nbsp;Fee Payment</h3>
                <small>ID:&nbsp;<asp:Label ID="lbApplicantId" runat="server" Text=""></asp:Label><asp:Label ID="lbTraineeId" runat="server" Text=""></asp:Label>
                    <asp:HiddenField ID="hfNric" runat="server" />
                    <br />
                    Name:&nbsp;<asp:Label ID="lbApplName" runat="server" Text=""></asp:Label>&nbsp;<asp:Label ID="lbSelfSponsored" runat="server" Text=""></asp:Label>
                    <br />
                    Application submitted on:&nbsp;<asp:Label ID="lbApplDate" runat="server" Text=""></asp:Label>
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
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <asp:ValidationSummary ID="vSummary1" ValidationGroup="addPymt" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <asp:ValidationSummary ID="vSummary4" ValidationGroup="editPaymt" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <asp:HiddenField ID="hfRtn" runat="server" />
        <asp:HiddenField ID="hfType" runat="server" />
        <div class="row text-left">
            <div class="col-lg-9 col-md-9 col-sm-9 col-xs-9">
                <h4>
                    <asp:Label ID="lbProgTitle" runat="server" Text=""></asp:Label>
                </h4>
                <div class="text-muted">
                    <small>Course Code: <u>
                        <asp:Label ID="lbProgCseCode" runat="server" Text=""></asp:Label></u>&nbsp;|
                        Project Code: <u>
                            <asp:Label ID="lbBatchProjCode" runat="server" Text=""></asp:Label></u>&nbsp;|
                        Class Code: <u>
                            <asp:Label ID="lbBatchCode" runat="server" Text=""></asp:Label></u>
                    </small>
                    <asp:HiddenField ID="hfBundleId" runat="server" />
                    <asp:HiddenField ID="hfBatchId" runat="server" />
                    <asp:HiddenField ID="hfProgrammeId" runat="server" />
                </div>
            </div>
        </div>
        <br />
        <br />

        <div class="row text-left">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <table class="table table-responsive">
                    <thead>
                        <tr>
                            <th>Item</th>
                            <th class="text-right">Price (S$)</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr id="trReg" runat="server">
                            <td>Registration Fee</td>
                            <td class="text-right" style="width: 200px">
                                <div class="inner-addon right-addon">
                                    <i class="glyphicon glyphicon-ok" style="cursor: pointer;" onclick="calculateNetTotal('REG');" id="lbtnSetRegFee" runat="server"></i>
                                    <asp:TextBox ID="tbRegFee" runat="server" CssClass="form-control text-right" MaxLength="6"></asp:TextBox>
                                </div>
                                <asp:RequiredFieldValidator ID="rfvRegFee" runat="server" ErrorMessage="Registration fee cannot be empty" Display="None"
                                    ControlToValidate="tbRegFee"></asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator ID="rfvRegFee1" runat="server" ErrorMessage="Registration fee cannot be empty" Display="None"
                                    ControlToValidate="tbRegFee" ValidationGroup="addPymt"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revRegFee" runat="server" ErrorMessage="Registration fee must be a non negative number, up to 2 decimal places"
                                    Display="None" ControlToValidate="tbRegFee" ValidationExpression="^\d+(\.\d{1,2})?$"></asp:RegularExpressionValidator>
                                <asp:RegularExpressionValidator ID="revRegFee1" runat="server" ErrorMessage="Registration fee must be a non negative number, up to 2 decimal places"
                                    Display="None" ControlToValidate="tbRegFee" ValidationExpression="^\d+(\.\d{1,2})?$" ValidationGroup="addPymt"></asp:RegularExpressionValidator>
                            </td>
                        </tr>

                        <tr id="trProg" runat="server">
                            <td>Programme Fee</td>
                            <td class="text-right">
                                <asp:Label ID="lbProgFee" runat="server"></asp:Label></td>
                        </tr>

                        <asp:Repeater ID="rpModules" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-&nbsp;<%# Eval("isExempted") %><%# Eval("moduleTitle") %> (<%# Eval("moduleCode") %>)</td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                        <tr id="trSubsidy" runat="server">
                            <td>
                                <div class="input-group">
                                    <span class="input-group-addon">Subsidy</span>
                                    <asp:DropDownList ID="ddlSubsidy" CssClass="form-control" runat="server" Width="95%"
                                        OnSelectedIndexChanged="ddlSubsidy_SelectedIndexChanged" AutoPostBack="true" CausesValidation="false" DataTextField="subsidyScheme" DataValueField="subsidyId">
                                    </asp:DropDownList>
                                </div>
                            </td>
                            <td class="text-right">
                                <asp:Label ID="lbSubsidyAmt" runat="server" Text="(0.00)"></asp:Label>
                                <asp:HiddenField ID="hfSubsidyAmt" runat="server" Value="0" />
                            </td>
                        </tr>

                        <tr id="trGST" runat="server">
                            <td>Goods and Service Tax (GST)</td>
                            <td class="text-right" style="width: 200px">
                                <div class="inner-addon right-addon">
                                    <i class="glyphicon glyphicon-ok" style="cursor: pointer; right: 23px;" onclick="calculateNetTotal('GST');" id="lbtnSetGST" runat="server"></i>
                                    <i class="glyphicon glyphicon-repeat" style="cursor: pointer;" onclick="calculateNetTotal('SUB');" id="lbtnCalGST" runat="server"
                                        data-toggle="tooltip" data-placement="top" title="Re-calculate GST"></i>
                                    <asp:TextBox ID="tbGST" runat="server" CssClass="form-control text-right" MaxLength="6"></asp:TextBox>
                                </div>
                                <asp:HiddenField ID="hfGST" runat="server" Value="0" />
                                <asp:RequiredFieldValidator ID="rfvGST" runat="server" ErrorMessage="GST cannot be empty" Display="None"
                                    ControlToValidate="tbGST"></asp:RequiredFieldValidator>
                                <asp:RequiredFieldValidator ID="rfvGST1" runat="server" ErrorMessage="GST cannot be empty" Display="None"
                                    ControlToValidate="tbGST" ValidationGroup="addPymt"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revGST" runat="server" ErrorMessage="GST must be a non negative number, up to 2 decimal places"
                                    Display="None" ControlToValidate="tbGST" ValidationExpression="^\d+(\.\d{1,2})?$"></asp:RegularExpressionValidator>
                                <asp:RegularExpressionValidator ID="revGST1" runat="server" ErrorMessage="GST must be a non negative number, up to 2 decimal places"
                                    Display="None" ControlToValidate="tbGST" ValidationExpression="^\d+(\.\d{1,2})?$" ValidationGroup="addPymt"></asp:RegularExpressionValidator>
                            </td>
                        </tr>

                        <tr style="font-weight: bold;">
                            <td class="text-right"Net Total</td>
                            <td class="text-right">
                                <asp:Label ID="lbNetTotalValue" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <div class="row text-right" runat="server" id="Div1">
            <br />
            <asp:Button ID="btnUpdateApplSusbidy" runat="server" CssClass="btn btn-info" Text="Save" OnClick="btnUpdateApplSusbidy_Click" />
        </div>

        <div class="row text-right" runat="server" id="panelShowPayment">
            <br />
            <asp:Button ID="btnShowPayment" runat="server" CssClass="btn btn-info" Text="Show Payment" OnClick="btnShowPayment_Click" CausesValidation="true" />
        </div>

        <div runat="server" id="panelPayment" visible="false">
            <hr />
            <h3>Paymentt</h3>
            <br />
            <div id="panelNewPayment" runat="server">
                <div class="row text-left">
                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                        <b>Date</b>
                        <asp:TextBox ID="tbPaymentDt" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPaymentDt" Display="None" ControlToValidate="tbPaymentDt" runat="server"
                            ErrorMessage="Payment date cannot be empty" ValidationGroup="addPymt"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cvPaymentDt" runat="server" Display="None" ControlToValidate="tbPaymentDt" ClientValidationFunction="validatePaymentDate"
                            ErrorMessage="Invalid date OR date is not in dd MMM yyyy format OR cannot be later than today" ValidateEmptyText="false" ValidationGroup="addPymt"></asp:CustomValidator>
                    </div>

                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                        <b>Mode</b>
                        <asp:DropDownList ID="ddlPaymentMode" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvPaymentMode" Display="None" ControlToValidate="ddlPaymentMode" runat="server"
                            ErrorMessage="Payment mode cannot be empty" ValidationGroup="addPymt"></asp:RequiredFieldValidator>
                    </div>

                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                        <b>Reference Num.</b>
                        <asp:TextBox ID="tbPaymentRef" CssClass="form-control" runat="server" MaxLength="20"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPaymentRef" Display="None" ControlToValidate="tbPaymentRef" runat="server"
                            ErrorMessage="Reference number cannot be empty" ValidationGroup="addPymt"></asp:RequiredFieldValidator>
                    </div>

                    <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                        <b>Amount (S$)</b>
                        <asp:TextBox ID="tbPaymentAmt" CssClass="form-control" runat="server" MaxLength="8"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPaymentAmt" Display="None" ControlToValidate="tbPaymentAmt" runat="server"
                            ErrorMessage="Payment amount cannot be empty" ValidationGroup="addPymt"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revPaymentAmt" runat="server" ErrorMessage="Payment amount must be a non negative number, up to 2 decimal places"
                            Display="None" ControlToValidate="tbPaymentAmt" ValidationExpression="^\d+(\.\d{1,2})?$" ValidationGroup="addPymt"></asp:RegularExpressionValidator>
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
                        <asp:Button ID="btnAddPayment" runat="server" CssClass="btn btn-info" Text="Add" OnClick="btnAddPayment_Click" ValidationGroup="addPymt" CausesValidation="true" />&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnClearPayment" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="btnClearPayment_Click" CausesValidation="false" />
                    </div>
                </div>

                <br />
            </div>

            <div class="row">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <asp:GridView ID="gvPayment" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered dataTable no-footer hover gvv"
                        ShowHeaderWhenEmpty="true" OnRowDataBound="gvPayment_RowDataBound" OnRowEditing="gvPayment_RowEditing" OnRowCancelingEdit="gvPayment_RowCancelingEdit" OnRowUpdating="gvPayment_RowUpdating">
                        <EmptyDataTemplate>
                            No available records
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="paymentId" HeaderText="Payment ID" />
                            <asp:BoundField DataField="paymentDate" HeaderText="Date" ItemStyle-Width="150px" ReadOnly="true" />
                            <asp:BoundField DataField="paymentModeDisp" HeaderText="Mode" ItemStyle-Width="100px" ReadOnly="true" />
                            <asp:TemplateField HeaderText="Reference Num." ItemStyle-Width="200px">
                                <ItemTemplate>
                                    <%# Eval("referenceNumber") %>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbEditRef" CssClass="form-control" runat="server" MaxLength="20" Text='<%# Eval("referenceNumber") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEditRef" Display="None" ControlToValidate="tbEditRef" runat="server"
                                        ErrorMessage="Reference number cannot be empty" ValidationGroup="editPaymt"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="paymentAmount" HeaderText="Amount (S$)" ItemStyle-Width="100px" DataFormatString="{0:#,##0.00}" ReadOnly="true" />
                            <asp:TemplateField HeaderText="Remarks">
                                <ItemTemplate>
                                    <%# Eval("paymentRemarks") %>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbEditRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" Text='<%# Eval("paymentRemarks") %>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>Status</HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("paymentStatusDisp") %>&nbsp;&nbsp;
                                <asp:Label ID="lbVoidDetails" runat="server" CssClass="glyphicon glyphicon-info-sign" Style="font-size: 20px;" data-toggle="tooltip" data-placement="top" data-html="true"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Width="120px" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtnEdit" runat="server" CommandName="Edit" CausesValidation="false" CssClass="glyphicon glyphicon-pencil" Style="font-size: 20px; text-decoration: none;" data-toggle="tooltip" data-placement="top" title="Edit Payment"></asp:LinkButton>
                                    <asp:Label ID="lbSpace1" runat="server" Text="&nbsp;&nbsp;"></asp:Label>
                                    <asp:Label ID="lbtnClearPayment" runat="server" CssClass="glyphicon glyphicon-ok" Style="font-size: 20px;"
                                        ForeColor="green" data-toggle="tooltip" data-placement="top" title="Clear payment"></asp:Label>
                                    <asp:Label ID="lbtnReceipt" runat="server" CssClass="glyphicon glyphicon-file" Style="font-size: 20px;"
                                        ForeColor="green" data-toggle="tooltip" data-placement="top" title="View Receipt"></asp:Label>
                                    <asp:Label ID="lbSpace" runat="server" Text="&nbsp;&nbsp;"></asp:Label>
                                    <asp:Label ID="lbtnVoidPayment" runat="server" CssClass="glyphicon glyphicon-remove" Style="font-size: 20px;"
                                        ForeColor="Red" data-toggle="tooltip" data-placement="top" title="Void payment"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:LinkButton ID="lbtnEdit" runat="server" CommandName="Update" ValidationGroup="editPaymt" CssClass="glyphicon glyphicon-ok" Style="font-size: 20px; text-decoration: none;" ForeColor="Green"></asp:LinkButton>
                                    &nbsp;&nbsp;&nbsp;
                                    <asp:LinkButton ID="lbtnCancel" runat="server" CommandName="Cancel" CausesValidation="false" CssClass="glyphicon glyphicon-remove" Style="font-size: 20px; text-decoration: none;" ForeColor="Red"></asp:LinkButton>
                                </EditItemTemplate>
                                <ItemStyle Width="110px" />
                            </asp:TemplateField>
                        </Columns>

                    </asp:GridView>
                </div>
            </div>
            <div class="row" id="panelNewPaymentLegend" runat="server">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <span style="background-color: Khaki">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;<i>New payment or payment with changes.</i>
                </div>
            </div>

            <div class="row text-right">
                <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                </div>
                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                    <h4>Amount Paid</h4>
                    <h3>
                        <asp:Label ID="lbAmtPaid" runat="server" Text="$0.00"></asp:Label></h3>
                    <asp:HiddenField ID="hfAmtPaid" runat="server" Value="0" />
                </div>
                <div class="col-lg-3 col-md-3 col-sm-3 col-xs-3">
                    <h4>Amount Outstanding</h4>
                    <h3>
                        <asp:Label ID="lbAmtOutstanding" runat="server" Text="$0.00"></asp:Label></h3>
                    <asp:HiddenField ID="hfAmtOutstanding" runat="server" Value="0" />
                </div>
            </div>

            <br />
            <br />
            <div class="row text-right">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <asp:Button ID="btnProcessPayment" runat="server" CssClass="btn btn-primary" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Process / Save&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                        OnClick="btnProcessPayment_Click" />
                    <button type="button" class="btn btn-default" data-toggle="modal" data-target="#diagReloadPayment">Discard Changes</button>
                </div>
            </div>
        </div>
    </div>

    <div id="diagReloadPayment" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Discard Changes</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to discard all payment changes?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnRevert" runat="server" CssClass="btn btn-primary" Text="OK" CausesValidation="false" OnClick="btnRevert_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
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
                    <asp:Button ID="btnClearChq" runat="server" CssClass="btn btn-primary" Text="OK" ValidationGroup="bankPymt" OnClick="btnClearChq_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="diagDelPayment" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete Payment</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this payment?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelPayment" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnDelPayment_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div id="diagDelVoidPayment" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete/Void Payment</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete or void this payment?
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal" onclick="showDelDialog();">Delete</button>
                    <button type="button" class="btn btn-primary" data-dismiss="modal" onclick="showVoidDialog1();">Void</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
