<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="aci-suspension.aspx.cs" Inherits="ACI_TMS.aci_suspension" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function validateId(oSrc, args) {
            var ic = args.Value.toUpperCase();

            if (ic.length != 9) {
                args.IsValid = false;
                return false;
            }

            var icArray = new Array(9);
            for (i = 0; i < 9; i++) {
                icArray[i] = ic.charAt(i);
            }
            icArray[1] *= 2;
            icArray[2] *= 7;
            icArray[3] *= 6;
            icArray[4] *= 5;
            icArray[5] *= 4;
            icArray[6] *= 3;
            icArray[7] *= 2;
            var weight = 0;
            for (i = 1; i < 8; i++) {
                weight += parseInt(icArray[i]);
            }
            var offset = (icArray[0] == "T" || icArray[0] == "G") ? 4 : 0;
            var temp = (offset + weight) % 11;
            var st = Array("J", "Z", "I", "H", "G", "F", "E", "D", "C", "B", "A");
            var fg = Array("X", "W", "U", "T", "R", "Q", "P", "N", "M", "L", "K");
            var theAlpha;
            if (icArray[0] == "S" || icArray[0] == "T") {
                theAlpha = st[temp];
            } else if (icArray[0] == "F" || icArray[0] == "G") {
                theAlpha = fg[temp];
            }

            if (icArray[8] != theAlpha) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validateDate(oSrc, args) {
            var start = new Date($('#<%=tbStartDateValue.ClientID%>').val());
            var end = new Date($('#<%=tbEndDateValue.ClientID%>').val());

            if (start > end) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <%--        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">--%>
        <%--         <ContentTemplate>--%>
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                <h3>
                    <asp:Label ID="lbSuspensionHeader" runat="server" Text="New Suspension"></asp:Label>
                </h3>
                <small>Please fill up the following</small>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <asp:LinkButton ID="lkbtnBack" runat="server" OnClick="lkbtnBack_Click" CssClass="btn btn-sm btn-default" Text="Back" CausesValidation="false"></asp:LinkButton>
            </div>

        </div>

        <hr />

        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link">Update successful</asp:Label>
        </div>
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link">Please correct the following:</asp:Label>
            <br />
            <asp:Label ID="lblErrorMsg" runat="server" CssClass="alert-link"></asp:Label>

        </div>

        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <br />

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbType" runat="server" Text="Suspended By" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="ddlSuspensionType" CssClass="form-control" runat="server">
                    <asp:ListItem Value="ACI">ACI</asp:ListItem>
                    <asp:ListItem Value="WDA">WDA</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvSuspensionType" runat="server" Display="None" ForeColor="Red" ControlToValidate="ddlSuspensionType" ErrorMessage="Please select the suspension type"></asp:RequiredFieldValidator>
            </div>

            <div class="col-lg-9">
                <asp:Label ID="lbName" runat="server" Text="Name" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbFullNameValue" runat="server" placeholder="Full Name" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFullName" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbFullNameValue" ErrorMessage="Please enter name"></asp:RequiredFieldValidator>
            </div>


        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-4">
                <asp:Label ID="lbNRIC" runat="server" Text="NRIC" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbNRICValue" runat="server" placeholder="Enter NRIC" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvNRIC" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbNRICValue" ErrorMessage="Please enter NRIC"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvId" runat="server" ControlToValidate="tbNRICValue" Display="None"
                    ErrorMessage="Invalid identification No." ClientValidationFunction="validateId" ValidateEmptyText="false"></asp:CustomValidator>

            </div>
            <div class="col-lg-4">
                <asp:Label ID="lbStartDate" runat="server" Text="Start Date" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbStartDateValue" CssClass="datepicker form-control" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvStatDate" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbStartDateValue" ErrorMessage="Please enter Start Date"></asp:RequiredFieldValidator>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbEndDate" runat="server" Text="End Date" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbEndDateValue" CssClass="datepicker form-control" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbEndDateValue" ErrorMessage="Please enter End Date"></asp:RequiredFieldValidator>
            </div>

            <asp:CustomValidator ID="cvCheckSusDate" runat="server" ControlToValidate="tbStartDateValue" Display="None" ClientValidationFunction="validateDate"
                ErrorMessage="Suspension End Date cannot be earlier than Suspension Start Date" ValidateEmptyText="false"></asp:CustomValidator>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbRemarks" runat="server" Text="Reason" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbRemarksValue" CssClass="form-control" TextMode="MultiLine" Rows="5" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvRemarks" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbRemarksValue" ErrorMessage="Please enter Remarks"></asp:RequiredFieldValidator>

            </div>
        </div>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnClear" runat="server" class="btn btn-default" Text="Clear" OnClick="btnClear_Click" CauseValidation="false" />
                <asp:Button ID="btnAddSuspend" runat="server" class="btn btn-info" Text="Submit" OnClick="btnAddSuspend_Click" CauseValidation="true" />
                <br />
                <asp:Label ID="lbSuccessMsg" runat="server" Text="Entry added!" Visible="false" Font-Bold="true"></asp:Label>
            </div>
        </div>

        <br />

        <%--            </ContentTemplate>
        </asp:UpdatePanel>--%>
    </div>
</asp:Content>
