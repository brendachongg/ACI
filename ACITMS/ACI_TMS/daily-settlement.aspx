<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="daily-settlement.aspx.cs" Inherits="ACI_TMS.daily_payment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function precise_round(num, dec) {

            if ((typeof num !== 'number') || (typeof dec !== 'number'))
                return false;

            var num_sign = num >= 0 ? 1 : -1;

            return (Math.round((num * Math.pow(10, dec)) + (num_sign * 0.0001)) / Math.pow(10, dec)).toFixed(dec);
        }


        $(document).ready(function () {

            // use keyup event with attribute equals selector
            $("[id*=tbCourseFeesWOGST]").keyup(function () {
                var quantity = parseFloat($.trim($(this).val()));
                var row = $(this).closest("tr");
                $("[id*=tbCourseFeesWGST]", row).val(parseFloat($("[id*=lblCourseFeesGST]", row).html()) + parseFloat($(this).val()));
     
                $("[id*=tbScheme]", row).val(precise_round(parseFloat($("[id*=lblTotalCourseFees]", row).html()) - parseFloat($("[id*=tbCourseFeesWGST]", row).val()), 2));

            });
        });

    </script>
    <style type="text/css">
        .hiddencol {
            display: none;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>--%>
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Daily Payments"></asp:Label>
                </h2>

            </div>
        </div>

        <hr />

        <%--                <div class="row" id="pnOperations" runat="server">

                    <div class="col-lg-9 col-md-9 col-sm-12">
                    </div>

                    <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="panel panel-default">
                            <div id="listHeader" class="panel-heading">Operations</div>
                            <div class="panel-body">
                                <p>
                                
                                </p>

                            </div>
                        </div>
                    </div>
                </div>--%>

        <div class="row text-left">
            <div class="col-lg-12">

                <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
                </div>
                <div class="alert alert-danger" id="panelError" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
                </div>
            </div>
        </div>

        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <div class="row">
            <div class="col-lg-4">
                <label>Date of Payment:</label>
                <asp:TextBox ID="tbDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>

            </div>

        </div>

        <div class="row">
            <div class="col-lg-2">
                <asp:Button ID="btnGet" Text="Get" runat="server" CssClass="btn btn-default" OnClick="btnGet_Click" />
            </div>
        </div>

        <!-- HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" -->

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvDailyPayments" CssClass="table table-striped table-bordered table-responsive dataTable no-footer hover gvv" runat="server" AutoGenerateColumns="False">
                    <Columns>

                        <asp:TemplateField>
                            <HeaderTemplate>Payment Mode</HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblPaymentMode" Text='<%# Eval("paymentMode") %>' runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Applicants / Trainee</HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblName" Text='<%# Eval("fullname") %>' runat="server"></asp:Label>
                                (<asp:Label ID="lblNRIC" Text='<%# Eval("idNumberMasked") %>' runat="server"></asp:Label>)
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Programme Start / End Date</HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblProgStartDate" runat="server" Text='<%# Eval("StartDate") %>'></asp:Label>
                                to
                                        <asp:Label ID="lblProgEndDate" runat="server" Text='<%# Eval("EndDate") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Programme Title</HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblProgrammeName" Text='<%# Eval("programmeTitle") %>' runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Course Code / Project Code</HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCourseCode" runat="server" Text='<%# Eval("courseCode") %>'></asp:Label>
                                /
                                        <asp:Label ID="lblProjectCode" runat="server" Text='<%# Eval("projectcode") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                that 
                                        Admin Fees
                                        <br />
                                (excl. of GST)
                                        <br />
                                S$
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAdminFeesWOGST" runat="server" Text='<%# Eval("registrationFee") %>'></asp:Label>
                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>
                                GST Amt
                                        <br />
                                S$
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAdminFeesGST" runat="server" Text='<%# Eval("regGST") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>



                        <asp:TemplateField>
                            <HeaderTemplate>
                                Admin Fee (Incl. of GST)
                                        <br />
                                S$
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAdminFeesWGST" runat="server" Text='<%# Eval("totalReg") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>
                                Course Fees
                                        <br />
                                (excl. of GST)
                                        <br />
                                S$
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%--  <asp:Label ID="lblCourseFeesWOGST" runat="server" Text='<%# Eval("programmePayableAmount") %>'></asp:Label>--%>
                                <asp:TextBox ID="tbCourseFeesWOGST" runat="server" Text='<%# Eval("afterFees") %>' CssClass="form-control"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>
                                GST Amt
                                        <br />
                                S$
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCourseFeesGST" runat="server" Text='<%# Eval("GSTPayableAmount") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>
                                Course Fee (Incl. of GST)
                                        <br />
                                S$
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%--<asp:Label ID="lblCourseFeesWGST" runat="server" Text='<%# Eval("totalProgrammeAmt") %>'></asp:Label>--%>
                                <asp:TextBox ID="tbCourseFeesWGST" runat="server" Text='<%# Eval("afterFeesS") %>' CssClass="form-control"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>
                                Less Scheme
                                        <br />
                                (WTS/MES/SFC)
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="tbScheme" runat="server" CssClass="form-control" Text="0.00"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>
                                Total Course Fee (Incl. of GST)
                                        <br />
                                S$
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTotalCourseFees" runat="server" Text='<%# Eval("afterSubsidyFees") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>
                                Total Fee Collected (Incl. of GST)
                                        <br />
                                S$
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTotalFeesCollected" runat="server" Text='<%# Eval("payment") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <HeaderTemplate>Remarks</HeaderTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="tbRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--  <asp:TemplateField>
                                    <HeaderTemplate>Payment Mode</HeaderTemplate>
                                    <ItemTemplate><%# Eval("paymentModeDisplay") %></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>Subsidy</HeaderTemplate>
                                    <ItemTemplate><%# decimal.Parse(Eval("totalSubsidy").ToString()) != 0 ? "-" + Eval("totalSubsidy").ToString() : "0"  %></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>Total Fees Collected (Admin & Course Fees)</HeaderTemplate>
                                    <ItemTemplate><%# Eval("totalPaymentFee") %></ItemTemplate>
                                </asp:TemplateField>--%>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <%--              <div class="row">
                    <div class="col-lg-12">
                        <label>Upload Receipts Documents:</label>
                        <i class="glyphicon glyphicon-question-sign" data-toggle="tooltip" data-placement="top" title="Only .PDF, .DOC, .DOCX with maximum size of 2MB is allowed."></i>
                        <asp:FileUpload ID="fuDocs" runat="server" CssClass="form-control" />
                        <asp:CustomValidator ID="cvReceiptDocuments" runat="server" Display="None" ControlToValidate="fuDocs"
                            ErrorMessage="File exceeded the size limit OR File is not in proper format." ValidateEmptyText="false" ClientValidationFunction="checkReceiptAttachment"></asp:CustomValidator>
                    </div>

                </div>--%>

        <br />

        <div class="row">
            <div class="pull-right">
                <asp:Button ID="btnVerify" CssClass="btn btn-primary" runat="server" Text="Proceed" OnClick="btnVerify_Click" />
            </div>
        </div>
        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvConfirm" CssClass="table table-striped table-bordered dataTable no-footer hover table-responsive gvv" runat="server" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="paymentMode" HeaderText="Payment Mode" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                        <asp:BoundField DataField="applicantname" HeaderText="Name" />
                        <asp:BoundField DataField="applicantnric" HeaderText="Name" />
                        <asp:BoundField DataField="programmeName" HeaderText="Programme Name" />
                        <asp:BoundField DataField="progStartDate" HeaderText="Programme Start Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="progEndDate" HeaderText="Programme End Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="courseCode" HeaderText="Course Code" />
                        <asp:BoundField DataField="projectCode" HeaderText="Project Code" />
                        <asp:BoundField DataField="adminFeesWOGst" HeaderText="Admin Fees (excl of GST) S$" />
                        <asp:BoundField DataField="adminFeesGst" HeaderText="GST Amt S$" />
                        <asp:BoundField DataField="adminFeesWGst" HeaderText="Admin Fees (excl of GST) S$" />
                        <asp:BoundField DataField="courseFeesWOGst" HeaderText="Course Fees (excl of GST) S$" />
                        <asp:BoundField DataField="courseFessGst" HeaderText="GST Amt S$" />
                        <asp:BoundField DataField="courseFeesWGst" HeaderText="Course Fees (excl of GST) S$" />
                        <asp:BoundField DataField="lessScheme" HeaderText="Less Scheme (WTS/MES/SFC)" />
                        <asp:BoundField DataField="totalCourseFees" HeaderText="Total Course Fees (Incl. of GST) S$" />
                        <asp:BoundField DataField="totalFeesCollected" HeaderText="Total Fees Collected (Incl. of GST) S$" />
                        <asp:BoundField DataField="remarks" HeaderText="Remarks" />

                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div class="row">
            <div class="pull-right">
                <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-danger" />
                <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CssClass="btn btn-success" OnClick="btnConfirm_Click" />
            </div>

        </div>

    </div>

    <%--        </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
