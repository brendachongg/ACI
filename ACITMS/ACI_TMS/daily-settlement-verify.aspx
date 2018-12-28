<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="daily-settlement-verify.aspx.cs" Inherits="ACI_TMS.daily_settlement_verify" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Daily Payments"></asp:Label>
                </h2>

            </div>
        </div>

        <hr />
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvPendingSettlementRecs" runat="server" CssClass="table table-striped table-bordered table-responsive dataTable no-footer hover gvv" AutoGenerateColumns="false" OnRowCommand="gvPendingSettlementRecs_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                Settlement Date
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbSettlementDate" runat="server" CommandArgument='<%# Eval("dailysettlementid") %>' CommandName="Select" Text='<%# Eval("settlementdate") %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="settlementdate" HeaderText="Settlement Date" />
                        <asp:BoundField DataField="username" HeaderText="Prepared By" />
                        <asp:BoundField DataField="createon" HeaderText="Submmitted On" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>


        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvConfirm" CssClass="table table-striped table-bordered dataTable no-footer hover table-responsive gvv" EnableViewState="false" runat="server" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="paymentMode" HeaderText="Payment Mode" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                        <asp:BoundField DataField="applicantname" HeaderText="Name" />
                        <asp:BoundField DataField="applicantid" HeaderText="Name" />
                        <asp:BoundField DataField="programmeName" HeaderText="Programme Name" />
                        <asp:BoundField DataField="programmestartdate" HeaderText="Programme Start Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="programmeenddate" HeaderText="Programme End Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="courseCode" HeaderText="Course Code" />
                        <asp:BoundField DataField="projectCode" HeaderText="Project Code" />
                        <asp:BoundField DataField="adminFeesWOGst" HeaderText="Admin Fees (excl of GST) S$" />
                        <asp:BoundField DataField="adminFeesGst" HeaderText="GST Amt S$" />
                        <asp:BoundField DataField="adminFeesWGst" HeaderText="Admin Fees (excl of GST) S$" />
                        <asp:BoundField DataField="courseFeesWOGst" HeaderText="Course Fees (excl of GST) S$" />
                        <asp:BoundField DataField="coursefeesgst" HeaderText="GST Amt S$" />
                        <asp:BoundField DataField="courseFeesWGst" HeaderText="Course Fees (excl of GST) S$" />
                        <asp:BoundField DataField="lessschemeamt" HeaderText="Less Scheme (WTS/MES/SFC)" />
                        <asp:BoundField DataField="totalCourseFees" HeaderText="Total Course Fees (Incl. of GST) S$" />
                        <asp:BoundField DataField="totalFeesCollected" HeaderText="Total Fees Collected (Incl. of GST) S$" />
                        <asp:BoundField DataField="remarks" HeaderText="Remarks" />

                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <label>Reject Remarks:</label>
                <asp:TextBox ID="tbRejectRemarks" CssClass="form-control" runat="server" Rows="3" TextMode="MultiLine"></asp:TextBox>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="pull-right">
                <asp:Button ID="btnVerify" runat="server" CssClass="btn btn-primary" Text="Accept" OnClick="btnVerify_Click" />
                <asp:Button ID="btnReject" runat="server" CssClass="btn btn-danger" Text="Reject" OnClick="btnReject_Click" />
                
            </div>
        </div>
</asp:Content>
