<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="data-migration.aspx.cs" Inherits="ACI_TMS.data_migration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lb1" runat="server" Text="ACI TMS Data Migration"></asp:Label>
                </h2>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <b>Data To Migrate:</b>
                <asp:DropDownList ID="ddlModules" runat="server" CssClass="form-control" AutoPostBack="true">
                    <asp:ListItem Value="Modules">Modules</asp:ListItem>
                    <asp:ListItem Value="Bundle">Bundle</asp:ListItem>
                    <asp:ListItem Value="Programme">Programme</asp:ListItem>
                    <asp:ListItem Value="Class">Class</asp:ListItem>
                    <asp:ListItem Value="Subsidy">Subsidy</asp:ListItem>
                    <asp:ListItem Value="Applicant">Applicant</asp:ListItem>
                    <asp:ListItem Value="Payment">Payment</asp:ListItem>
                    <asp:ListItem Value="SOA">SOA</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12">
                <b>File:</b>
                <asp:FileUpload ID="fuExcelFile" runat="server" CssClass="form-" />
                <asp:RequiredFieldValidator ID="rfvFileNeeded" runat="server" ErrorMessage="Please select a file" ControlToValidate="fuExcelFile" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revFileFormate" runat="server" ErrorMessage="Only excel format(.xlsx/.xls) file is allowed" ForeColor="Red" ValidationExpression="^.*\.(xls|xlsx)$" ControlToValidate="fuExcelFile" Display="Dynamic"></asp:RegularExpressionValidator>

            </div>
        </div>

      

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv">
                </asp:GridView>
            </div>
        </div>

        <div class="row">
            <div class="pull-right">
                <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btn btn-primary" OnClick="btnUpload_Click" />
                <asp:Button ID="btnEnrollTrainee" runat="server" Text="Enroll Trainees" CssClass="btn btn-primary" OnClick="btnEnrollTrainee_Click" />
            </div>
         
        </div>

        <div class="row">
            <asp:Label ID="lbResult" runat="server" ></asp:Label>
        </div>
    </div>

</asp:Content>
