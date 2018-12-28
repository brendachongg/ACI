<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="daily-settlement-finance.aspx.cs" Inherits="ACI_TMS.daily_settlement_finance" %>

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

        <div class="row">
            <div class="col-lg-12">
                <label>Search By:</label>
                <div class="row">
                    <div class="col-lg-2">
                        <asp:DropDownList ID="ddlSearchBy" runat="server" AutoPostBack="false" CssClass="form-control ">
                            <asp:ListItem>Date</asp:ListItem>
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-lg-6">
                        <asp:TextBox ID="tbSearch" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <label>Enter Batch Number: </label>
                <asp:TextBox ID="tbBatchNo" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

        </div>
    </div>

</asp:Content>
