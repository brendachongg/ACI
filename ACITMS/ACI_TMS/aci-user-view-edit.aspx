<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="aci-user-view-edit.aspx.cs" Inherits="ACI_TMS.aci_user_view_edit" %>

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

        function validateLoginID(oSrc, args) {
            if ($('#<%=ddlEmpType.ClientID%> option:selected').val() == "<%=GeneralLayer.StaffEmploymentType.FT.ToString()%>" || $('#<%=ddlEmpType.ClientID%> option:selected').val() == "<%=GeneralLayer.StaffEmploymentType.ADJ.ToString()%>") {
                if ($('#<%=tbLoginId.ClientID%>').val() == '') {
                    args.IsValid = false;
                    return false;
                } else {
                    args.IsValid = true;
                    return true;
                }

            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <%--      <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>
        <div class="row text-left">
            <%--<div class="col-lg-12">

                        <h3>
                            <asp:Label ID="lbACIStaffDetails" runat="server" Text="ACI Staff Edit"></asp:Label>
                        </h3>

                    </div>--%>

            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbACIStaffDetails" runat="server" Text="ACI Staff Edit"></asp:Label>
                </h3>

                <small>Please fill up the following</small>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnEdit" runat="server" CssClass="btn btn-sm btn-default" Text="Edit" CausesValidation="false" OnClick="lkbtnEdit_Click"></asp:LinkButton>
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click" CausesValidation="false"></asp:LinkButton>
            </div>
        </div>

        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link">Update successful</asp:Label>
        </div>
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblErrorMsg" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <br />

        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <asp:Panel ID="panelParticular" Enabled="false" runat="server">
            <div class="row text-left">

                <div class="col-lg-2">
                    <asp:Label ID="lbSalutation" runat="server" Text="Salutation" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlSalutations" runat="server" CssClass="form-control"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="salutationsRequiredValidators" Display="None" ForeColor="Red" ControlToValidate="ddlSalutations" runat="server" ErrorMessage="Please select the Salutations"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-5">
                    <asp:Label ID="lbFullName" runat="server" Text="Full Name" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbFullName" runat="server" CssClass="form-control"></asp:TextBox>

                    <asp:RequiredFieldValidator ID="fullNameRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbFullName" runat="server" ErrorMessage="Please fill in the Full Name"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-5">
                    <asp:Label ID="lbIdentificationNo" runat="server" Text="Identification Number" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbIdentificationNo" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:CustomValidator ID="cvId" runat="server" ControlToValidate="tbIdentificationNo" Display="None"
                        ErrorMessage="Invalid identification No." ClientValidationFunction="validateId" ValidateEmptyText="false"></asp:CustomValidator>
                    <asp:RequiredFieldValidator ID="identificationNoRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbIdentificationNo" runat="server" ErrorMessage="Please fill in the Identfication No"></asp:RequiredFieldValidator>
                </div>


            </div>
            <br />
            <div class="row text-left">


                <div class="col-lg-6">
                    <asp:Label ID="lbEmail" runat="server" Text="Email" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbEmail" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="emailRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbEmail" runat="server" ErrorMessage="Please fill in the Email"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="rexEmail" Display="None" ValidationExpression="^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" runat="server" ErrorMessage="Invalid email." ControlToValidate="tbEmail"></asp:RegularExpressionValidator>
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lbEmpType" runat="server" Text="Employment Type" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlEmpType" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlEmpType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="empTypeRF" Display="None" ForeColor="Red" ControlToValidate="ddlEmpType" runat="server" ErrorMessage="Please select the Employment Type"></asp:RequiredFieldValidator>
                </div>


                <div class="col-lg-3">
                    <asp:Label ID="lblLoginId" runat="server" Text="Login ID" Font-Bold="true" Visible="false"></asp:Label>
                    <asp:TextBox ID="tbLoginId" runat="server" Visible="false" CssClass="form-control"></asp:TextBox>
                    <asp:CustomValidator ID="cvLoginId" runat="server" ErrorMessage="Please fill in the Login ID" Display="None" ClientValidationFunction="validateLoginID" ValidateEmptyText="false"></asp:CustomValidator>
                </div>

            </div>
            <br />
            <div class="row text-left">
                <div class="col-lg-3">
                    <asp:Label ID="lbAuthorisedToTMS" runat="server" Text="Authorised to Access TMS?" Font-Bold="true"></asp:Label>
                    <asp:CheckBox ID="cbTMS" runat="server" CssClass="control-label" OnCheckedChanged="cbTMS_CheckedChanged" />
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lbAccessor" runat="server" Text="Is Accessor?" Font-Bold="true"></asp:Label>
                    <asp:CheckBox ID="cbAccessor" runat="server" CssClass="control-label" OnCheckedChanged="cbAccessor_CheckedChanged" />
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lbTrainer" runat="server" Text="Is Trainer?" Font-Bold="true"></asp:Label>
                    <asp:CheckBox ID="cbTrainer" runat="server" CssClass="control-label" OnCheckedChanged="cbTrainer_CheckedChanged" />
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lbInterviewer" runat="server" Text="Is Interviewer?" Font-Bold="true"></asp:Label>
                    <asp:CheckBox ID="cbInterviewer" runat="server" CssClass="control-label" OnCheckedChanged="cbInterviewer_CheckedChanged" />
                </div>
            </div>
            <br />
            <div class="row text-left">
                <div class="col-lg-6">
                    <asp:Label ID="lbContactNumber1" runat="server" Text="Contact Number 1" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbContactNumber1" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revContact1" Display="None" ValidationExpression="^\+?\d+$" runat="server" ErrorMessage="Contact number can only contain numbers."
                        ControlToValidate="tbContactNumber1"></asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator ID="contactNumber1RF" Display="None" ForeColor="Red" ControlToValidate="tbContactNumber1" runat="server" ErrorMessage="Please fill in Contact Number 1"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lnContactNumber2" runat="server" Text="Contact Number 2" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbContactNumber2" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="revContact2" Display="None" ValidationExpression="^\+?\d+$" runat="server" ErrorMessage="Alternative contact no. can only contain numbers."
                        ControlToValidate="tbContactNumber2"></asp:RegularExpressionValidator>
                </div>


            </div>

            <br />
            <div class="row text-left">
                <div class="col-lg-9">
                    <asp:Label ID="lbAddress" runat="server" Text="Address" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbAddress" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="addressRF" Display="None" ForeColor="Red" ControlToValidate="tbAddress" runat="server" ErrorMessage="Please fill in Address"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lbPostalCode" runat="server" Text="Postal Code" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbPostalCode" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="postalCodeRF" Display="None" ForeColor="Red" ControlToValidate="tbPostalCode" runat="server" ErrorMessage="Please fill in Postal Code"></asp:RequiredFieldValidator>

                </div>


            </div>
            <br />

            <div class="row text-right">
                <asp:Button ID="btnCancel" runat="server" class="btn btn-default" Text="Cancel" OnClick="btnCancel_Click" />
                <asp:Button ID="btnUpdate" runat="server" class="btn btn-warning" Text="Update" OnClick="btnUpdate_Click" />
            </div>
        </asp:Panel>
        <%--           </ContentTemplate>
        </asp:UpdatePanel>--%>
    </div>
</asp:Content>
