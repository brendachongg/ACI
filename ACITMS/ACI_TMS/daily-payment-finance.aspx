<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="daily-payment-finance.aspx.cs" Inherits="ACI_TMS.daily_payment_finance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="page-wrapper">
                <div class="row text-center">
                    <div class="col-lg-12">
                        <h2>
                            <asp:Label ID="lbHeader" runat="server" Text="Verify Daily Payments"></asp:Label>
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
                    <div class="col-lg-12">
                        <a href="Files/12_Sep_2018.pdf">View uploaded receipts</a>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-6">
                        <label>Batch No.:</label>
                        <asp:TextBox ID="tbBatchNo" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>


                <div class="row">
                    <div class="col-lg-12">
                        <table class="table table-responsive"></table>
                    </div>
                </div>

                <div class="row">
                    <div class="pull-right">
                        <asp:Button ID="btnReject" CssClass="btn btn-danger" runat="server" Text="Reject" />
                        <asp:Button ID="btnAccept" CssClass="btn btn-primary" runat="server" Text="Accept" />
                    </div>
                </div>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
