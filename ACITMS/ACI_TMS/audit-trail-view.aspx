<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="audit-trail-view.aspx.cs" Inherits="ACI_TMS.audit_trail_view" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function validateDate(oSrc, args) {
            if ($('#<%=txtStartDate.ClientID%>').val() == "" && $('#<%=txtEndDate.ClientID%>').val() == "") {
                args.IsValid = true;
                return true;
            }

            if ($('#<%=txtStartDate.ClientID%>').val() != "") {
                if(!isValidDate($('#<%=txtStartDate.ClientID%>').val())){
                    args.IsValid = false;
                    return false;
                }
            }

            if ($('#<%=txtEndDate.ClientID%>').val() != "") {
                if (!isValidDate($('#<%=txtEndDate.ClientID%>').val())) {
                    
                }
            }

            if ($('#<%=txtStartDate.ClientID%>').val() != "" && $('#<%=txtEndDate.ClientID%>').val() != "") {
                var start = new Date($('#<%=txtStartDate.ClientID%>').val());
                var end = new Date($('#<%=txtEndDate.ClientID%>').val());

                if (end < start) {
                    args.IsValid = false;
                    return false;
                }
            }

            args.IsValid = true;
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>Audit Trail</h3>
                <small>Please fill up the following</small>
            </div>
        </div>
        <hr />
       
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <div class="row text-left">
            <div class="col-lg-4">
                <asp:Label ID="lb1" runat="server" Text="Data Type" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="DataDDL" ValidationGroup="A" runat="server" CssClass="form-control" DataTextField="groupName" DataValueField="groupName">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvData" runat="server" ControlToValidate="DataDDL" Display="none" ErrorMessage="Data type cannot be empty"></asp:RequiredFieldValidator>
            </div>
            <div class="col-lg-6">
                <asp:Label ID="lb2" runat="server" Text="Date" Font-Bold="true"></asp:Label>
                <div class="input-group">
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                    <span class="input-group-addon" style="font-weight: bold;">to</span>
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                </div>
                <asp:CustomValidator ID="cvDate" runat="server" Display="None" ControlToValidate="txtStartDate" ClientValidationFunction="validateDate"
                            ErrorMessage="Date is not in dd MMM yyyy format<br/>OR end date cannot be earlier than start date" ValidateEmptyText="true"></asp:CustomValidator>
            </div>
            <div class="col-lg-2">
                <asp:Label ID="lb3" runat="server" Text="Action" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="actionDDL" CssClass="form-control" runat="server">
                    <asp:ListItem>All</asp:ListItem>
                    <asp:ListItem>Create</asp:ListItem>
                    <asp:ListItem>Delete</asp:ListItem>
                    <asp:ListItem>Update</asp:ListItem>
                    <asp:ListItem>View</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <br />
        <div class="row text-right">
            <div class="col-lg-12">             
                <asp:Button ID="btnPreview" class="btn btn-primary" runat="server" Text="Preview" OnClick="btnPreview_Click" />
                <asp:Button ID="btnClear" class="btn btn-default" runat="server" Text="Clear" OnClick="btnClear_Click" CausesValidation="false" />
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvPreview" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered dataTable no-footer hover gvv">
                    <Columns>
                        <asp:BoundField DataField="table" HeaderText="Table name" />
                        <asp:BoundField DataField="records" HeaderText="Number of records" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnExport" runat="server" class="btn btn-primary" Text="Export" OnClick="btnExport_Click" Visible="false" />
            </div>
        </div>

    </div>

</asp:Content>
