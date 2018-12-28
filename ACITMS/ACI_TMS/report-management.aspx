<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="report-management.aspx.cs" Inherits="ACI_TMS.report_management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function validateInputDate(oSrc, args) {
            if (args.Value == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(args.Value)) {
                args.IsValid = false;
                return false;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Report Management"></asp:Label>
                </h2>
            </div>
        </div>
        <hr />
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <div class="row text-left">
            <div class="col-lg-6">
                <b>Category</b>
                <asp:DropDownList ID="ddlRepCat" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRepCat_SelectedIndexChanged">
                    <asp:ListItem Value="">--Select--</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-lg-6">
                <b>Report</b>
                <asp:DropDownList ID="ddlRep" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRep_SelectedIndexChanged" Enabled="false">
                </asp:DropDownList>
            </div>
        </div>
        <br />
        <div class="row text-left" id="divFYRange" runat="server" visible="false">
            <div class="col-lg-6">
                <b>Begin Financial Year</b>
                <asp:TextBox ID="tbFYRangeStart" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFYRangeStart" runat="server" ErrorMessage="Begin year cannot be empty." ControlToValidate="tbFYRangeStart" Display="None"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revFYRangeStart" runat="server" ErrorMessage="Year must be in yyyy format" ControlToValidate="tbFYRangeStart" Display="None" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
            </div>
            <div class="col-lg-6">
                <b>End Financial Year</b>
                <asp:TextBox ID="tbFYRangeEnd" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFYRangeEnd" runat="server" ErrorMessage="End year cannot be empty." ControlToValidate="tbFYRangeEnd" Display="None"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revFYRangeEnd" runat="server" ErrorMessage="Year must be in yyyy format" ControlToValidate="tbFYRangeEnd" Display="None" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="row text-left" id="divDtRange" runat="server" visible="false">
            <div class="col-lg-6">
                <b>Start Date</b>
                <asp:TextBox ID="tbDtRangeStart" runat="server" placeholder="" CssClass="datepicker form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDtRangeStart" runat="server" ErrorMessage="Start date cannot be empty." ControlToValidate="tbDtRangeStart" Display="None"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvDtRangeStart" runat="server" Display="None" ControlToValidate="tbDtRangeStart" ClientValidationFunction="validateInputDate" ErrorMessage="Start date is not in dd MMM yyyy format" ValidateEmptyText="false"></asp:CustomValidator>
            </div>
            <div class="col-lg-6">
                <b>End Date</b>
                <asp:TextBox ID="tbDtRangeEnd" runat="server" placeholder="" CssClass="datepicker form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDtRangeEnd" runat="server" ErrorMessage="End date cannot be empty." ControlToValidate="tbDtRangeEnd" Display="None"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvDtRangeEnd" runat="server" Display="None" ControlToValidate="tbDtRangeEnd" ClientValidationFunction="validateInputDate" ErrorMessage="End date is not in dd MMM yyyy format" ValidateEmptyText="false"></asp:CustomValidator>
            </div>
        </div>
        <div class="row text-left" id="divMthYrSub" runat="server" visible="false">
            <div class="col-lg-4">
                <b>Month</b>
                <asp:DropDownList ID="ddlMthYrSubMth" runat="server" CssClass="form-control">
                    <asp:ListItem Value="1">January</asp:ListItem>
                    <asp:ListItem Value="2">February</asp:ListItem>
                    <asp:ListItem Value="3">March</asp:ListItem>
                    <asp:ListItem Value="4">April</asp:ListItem>
                    <asp:ListItem Value="5">May</asp:ListItem>
                    <asp:ListItem Value="6">June</asp:ListItem>
                    <asp:ListItem Value="7">July</asp:ListItem>
                    <asp:ListItem Value="8">August</asp:ListItem>
                    <asp:ListItem Value="9">September</asp:ListItem>
                    <asp:ListItem Value="10">October</asp:ListItem>
                    <asp:ListItem Value="11">November</asp:ListItem>
                    <asp:ListItem Value="12">December</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvMthYrSubMth" runat="server" ErrorMessage="Month cannot be empty." ControlToValidate="ddlMthYrSubMth" Display="None"></asp:RequiredFieldValidator>
            </div>
            <div class="col-lg-4">
                <b>Year</b>
                <asp:TextBox ID="tbMthYrSubYr" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvMthYrSubYr" runat="server" ErrorMessage="Year cannot be empty." ControlToValidate="tbMthYrSubYr" Display="None"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revMthYrSubYr" runat="server" ErrorMessage="Year must be in yyyy format" ControlToValidate="tbMthYrSubYr" Display="None" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
            </div>
            <div class="col-lg-4">
                <b>Subsidy Scheme</b>
                <asp:TextBox ID="tbMthYrSubSub" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvMthYrSubSub" runat="server" ErrorMessage="Subsidy cannot be empty." ControlToValidate="tbMthYrSubSub" Display="None"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="row text-left" id="divYrSub" runat="server" visible="false">
            <div class="col-lg-6">
                <b>Year</b>
                <asp:TextBox ID="tbYrSubYr" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvbYrSubYr" runat="server" ErrorMessage="Year cannot be empty." ControlToValidate="tbYrSubYr" Display="None"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revbYrSubYr" runat="server" ErrorMessage="Year must be in yyyy format" ControlToValidate="tbYrSubYr" Display="None" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
            </div>
            <div class="col-lg-6">
                <b>Subsidy Scheme</b>
                <asp:TextBox ID="tbYrSubSub" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvYrSubSub" runat="server" ErrorMessage="Subsidy cannot be empty." ControlToValidate="tbYrSubSub" Display="None"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="row text-left" id="divMthYr" runat="server" visible="false">
            <div class="col-lg-6">
                <b>Month</b>
                <asp:DropDownList ID="ddlMthYrMth" runat="server" CssClass="form-control">
                    <asp:ListItem Value="1">January</asp:ListItem>
                    <asp:ListItem Value="2">Feburary</asp:ListItem>
                    <asp:ListItem Value="3">March</asp:ListItem>
                    <asp:ListItem Value="4">April</asp:ListItem>
                    <asp:ListItem Value="5">May</asp:ListItem>
                    <asp:ListItem Value="6">June</asp:ListItem>
                    <asp:ListItem Value="7">July</asp:ListItem>
                    <asp:ListItem Value="8">August</asp:ListItem>
                    <asp:ListItem Value="9">September</asp:ListItem>
                    <asp:ListItem Value="10">October</asp:ListItem>
                    <asp:ListItem Value="11">November</asp:ListItem>
                    <asp:ListItem Value="12">December</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvlMthYrMth" runat="server" ErrorMessage="Month cannot be empty." ControlToValidate="ddlMthYrMth" Display="None"></asp:RequiredFieldValidator>
            </div>
            <div class="col-lg-6">
                <b>Year</b>
                <asp:TextBox ID="tbMthYrYr" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvMthYrYr" runat="server" ErrorMessage="Year cannot be empty." ControlToValidate="tbMthYrYr" Display="None"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revMthYrYr" runat="server" ErrorMessage="Year must be in yyyy format" ControlToValidate="tbMthYrYr" Display="None" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="row text-left" id="divYr" runat="server" visible="false">
            <div class="col-lg-12">
                <b><asp:Label ID="lbYr" runat="server"></asp:Label>Year</b>
                <asp:TextBox ID="tbYr" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvYr" runat="server" ErrorMessage="Year cannot be empty." ControlToValidate="tbYr" Display="None"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revYr" runat="server" ErrorMessage="Year must be in yyyy format" ControlToValidate="tbYr" Display="None" ValidationExpression="^\d{4}$"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="row text-left" id="divDt" runat="server" visible="false">
            <div class="col-lg-12">
                <b><asp:Label ID="lbDt" runat="server"></asp:Label></b>
                <asp:TextBox ID="tbDt" runat="server" placeholder="" CssClass="datepicker form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDt" runat="server" ErrorMessage="Date cannot be empty." ControlToValidate="tbDt" Display="None"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvDt" runat="server" Display="None" ControlToValidate="tbDt" ClientValidationFunction="validateInputDate" ErrorMessage="Date is not in dd MMM yyyy format" ValidateEmptyText="false"></asp:CustomValidator>
            </div>
        </div>
        <div class="row text-left" id="divDtPaymentMode" runat="server" visible="false">
            <div class="col-lg-6">
                <b>
                    <asp:Label ID="lbDt1" runat="server"></asp:Label></b>
                <asp:TextBox ID="tbDt1" runat="server" placeholder="" CssClass="datepicker form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDt1" runat="server" ErrorMessage="Date cannot be empty." ControlToValidate="tbDt1" Display="None"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvDt1" runat="server" Display="None" ControlToValidate="tbDt1" ClientValidationFunction="validateInputDate" ErrorMessage="Date is not in dd MMM yyyy format" ValidateEmptyText="false"></asp:CustomValidator>
            </div>

            <div class="col-lg-6">
                <b><asp:Label ID="lbSettlementMode" runat="server"></asp:Label></b>
                <asp:DropDownList ID="ddlSettlementMode" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>
        </div>
        <div class="row text-left" id="divDtDdl" runat="server" visible="false">
            <div class="col-lg-4">
                <b>Start Date</b>
                <asp:TextBox ID="tbDtDdlStart" runat="server" placeholder="" CssClass="datepicker form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDtDdlStart" runat="server" ErrorMessage="Start date cannot be empty." ControlToValidate="tbDtDdlStart" Display="None"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cDtDdlStart" runat="server" Display="None" ControlToValidate="tbDtDdlStart" ClientValidationFunction="validateInputDate" ErrorMessage="Start date is not in dd MMM yyyy format" ValidateEmptyText="false"></asp:CustomValidator>
            </div>
            <div class="col-lg-4">
                <b>End Date</b>
                <asp:TextBox ID="tbDtDdlEnd" runat="server" placeholder="" CssClass="datepicker form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDtDdlEnd" runat="server" ErrorMessage="End date cannot be empty." ControlToValidate="tbDtDdlEnd" Display="None"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvDtDdlEnd" runat="server" Display="None" ControlToValidate="tbDtDdlEnd" ClientValidationFunction="validateInputDate" ErrorMessage="End date is not in dd MMM yyyy format" ValidateEmptyText="false"></asp:CustomValidator>
            </div>
            <div class="col-lg-4">
                <b><asp:Label ID="lbDtDdl" runat="server"></asp:Label></b>
                <asp:DropDownList ID="ddlDtDdl" runat="server" CssClass="form-control">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvDtDdl" runat="server" ErrorMessage="Selection cannot be empty." ControlToValidate="ddlDtDdl" Display="None"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="row text-left" id="divTrainee" runat="server" visible="false">
            <div class="col-lg-12">
                <br />
                <b>Class Code</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="tbClassCode" runat="server" CssClass="form-control"></asp:TextBox>
<%--                <asp:RequiredFieldValidator ID="rfvClassCode" runat="server" ErrorMessage="Class Code cannot be empty" Display="None" ControlToValidate="tbClassCode"></asp:RequiredFieldValidator>--%>
            </div>
            <div class="col-lg-12">
                 <br />
                <b>Type of Data</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:CheckBox ID="cbTrParticulars" runat="server" />&nbsp;Personal Particulars&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:CheckBox ID="cbTrModResult" runat="server" />&nbsp;Module Results
            </div>
        </div>
        <br />
        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnGen" runat="server" CssClass="btn btn-primary" Text="Generate Report" OnClick="btnGen_Click"/>
                <asp:Button ID="btnClear" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="btnClear_Click"/>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gv" runat="server" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv">
                </asp:GridView>
                <asp:Label ID="lblMsg" style="font-size:smaller;color:red;font-style:italic;" runat="server" Text=""></asp:Label>
            </div>
        </div>

    </div>
</asp:Content>
