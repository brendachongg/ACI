<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="applicant-quick-registration.aspx.cs" Inherits="ACI_TMS.applicant_quick_registration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            $(".datepickerDOB").datepicker({
                dateFormat: "dd M yy",
                changeMonth: true,
                changeYear: true,
                yearRange: '1930:+0'
            });
        });

        function validateBirthDate(oSrc, args) {
            var dtStr = $('#<%=tbBirthDate.ClientID%>').val();

            if (dtStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(dtStr)) {
                args.IsValid = false;
                return false;
            }

            //DOB cannot be later than current date
            var today = new Date();
            var dt = new Date(dtStr);

            if (today < dt) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validateIC(oSrc, args) {
            var ic = $('#<%=ddlIdFirstLetter.ClientID%> option:selected').val() + args.Value + $('#<%=ddlIdLastLetter.ClientID%> option:selected').val();

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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbHeader" runat="server" Text="Quick Registration Form"></asp:Label>
                </h3>

                <small>Please fill up the following</small>
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
        <div class="row">
            <div class="col-lg-8">
                <asp:Label ID="lbFullName" runat="server" Font-Bold="true">Full Name</asp:Label>
                <asp:TextBox ID="tbFullName" CssClass="form-control" placeholder="Full Name" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rvFullName" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbFullName" ErrorMessage="Full name cannot be empty."></asp:RequiredFieldValidator>
            </div>
            <div class="col-lg-4">
                <asp:Label ID="lbBirthDate" runat="server" Font-Bold="true">Date of Birth</asp:Label>
                <asp:TextBox ID="tbBirthDate" runat="server" CssClass="datepickerDOB form-control"></asp:TextBox>

                <asp:RequiredFieldValidator ID="rvBirthDate" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbBirthDate" ErrorMessage="Date of birth cannot be empty."></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvBirthDate" runat="server" ControlToValidate="tbBirthDate" Display="None" ForeColor="Red" ClientValidationFunction="validateBirthDate"
                    ErrorMessage="Date of birth is not in dd MMM yyyy format OR cannot be earlier than today." ValidateEmptyText="false"></asp:CustomValidator>
            </div>
        </div>
        <br />
        <div class="form-group row">
            <div class="col-lg-12">
                <asp:Label ID="lbIdentificationNo" runat="server" Font-Bold="true">Identification Type/No.</asp:Label>
            </div>
            <div class="col-lg-3">
                <asp:DropDownList ID="ddlIdType" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue" CausesValidation="false" AutoPostBack="true" OnSelectedIndexChanged="ddlIdType_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            <div id="panelPP" runat="server" visible="false">
                <div class="col-lg-9">
                    <asp:TextBox ID="tbPPIdentification" Style="clear: both;" CssClass="form-control" placeholder="Passport No" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPPIdentification" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbPPIdentification" ErrorMessage="Identification no. cannot be empty."></asp:RequiredFieldValidator>
                </div>
            </div>
            <div id="panelLocalId" runat="server">
                <div class="col-lg-9">
                    <div class="input-group">
                        <asp:DropDownList ID="ddlIdFirstLetter" runat="server" CssClass="form-control">
                            <asp:ListItem Text="S" Value="S"></asp:ListItem>
                            <asp:ListItem Text="T" Value="T"></asp:ListItem>
                            <asp:ListItem Text="F" Value="F"></asp:ListItem>
                            <asp:ListItem Text="G" Value="G"></asp:ListItem>
                        </asp:DropDownList>
                        <span class="input-group-addon" style="font-weight: bold;">-</span>
                        <asp:TextBox ID="tbLocalIdentification" Style="clear: both;" CssClass="form-control" placeholder="NRIC No / FIN No" runat="server"></asp:TextBox>
                        <span class="input-group-addon" style="font-weight: bold;">-</span>
                        <asp:DropDownList ID="ddlIdLastLetter" runat="server" CssClass="form-control">
                            <asp:ListItem Text="A" Value="A"></asp:ListItem>
                            <asp:ListItem Text="B" Value="B"></asp:ListItem>
                            <asp:ListItem Text="C" Value="C"></asp:ListItem>
                            <asp:ListItem Text="D" Value="D"></asp:ListItem>
                            <asp:ListItem Text="E" Value="E"></asp:ListItem>
                            <asp:ListItem Text="F" Value="F"></asp:ListItem>
                            <asp:ListItem Text="G" Value="G"></asp:ListItem>
                            <asp:ListItem Text="H" Value="H"></asp:ListItem>
                            <asp:ListItem Text="I" Value="I"></asp:ListItem>
                            <asp:ListItem Text="J" Value="J"></asp:ListItem>
                            <asp:ListItem Text="K" Value="K"></asp:ListItem>
                            <asp:ListItem Text="L" Value="L"></asp:ListItem>
                            <asp:ListItem Text="M" Value="M"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="P" Value="P"></asp:ListItem>
                            <asp:ListItem Text="Q" Value="Q"></asp:ListItem>
                            <asp:ListItem Text="R" Value="R"></asp:ListItem>
                            <asp:ListItem Text="T" Value="T"></asp:ListItem>
                            <asp:ListItem Text="U" Value="U"></asp:ListItem>
                            <asp:ListItem Text="W" Value="W"></asp:ListItem>
                            <asp:ListItem Text="X" Value="X"></asp:ListItem>
                            <asp:ListItem Text="Z" Value="Z"></asp:ListItem>
                        </asp:DropDownList>
                    </div>                 
                </div>
                <asp:RequiredFieldValidator ID="rvIdentificationNo" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbLocalIdentification" ErrorMessage="Identification no. cannot be empty."></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvIdentificationNo" runat="server" ControlToValidate="tbLocalIdentification" Display="None" ForeColor="Red"
                    ErrorMessage="Invalid identification no." ClientValidationFunction="validateIC" ValidateEmptyText="false"></asp:CustomValidator>
            </div>
        </div>
        <br />
        <div class="row text-left">
            <div class="col-lg-6">
                <asp:Label ID="lbProgrammeCategory" runat="server" Font-Bold="true" Text="Programme Category"></asp:Label>
                <asp:DropDownList ID="ddlProgrammeCategory" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue" OnSelectedIndexChanged="ddlProgrammeCategory_SelectedIndexChanged" AutoPostBack="true" CausesValidation="false"></asp:DropDownList>
            </div>
            <div class="col-lg-6">
                <asp:Label ID="lbSponsorship" runat="server" Font-Bold="true" Text="Type of Sponsorship"></asp:Label>
                <asp:DropDownList ID="ddlSponsorship" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvSponsorship" runat="server" Display="None" ForeColor="Red" ControlToValidate="ddlSponsorship" ErrorMessage="Type of sponsorship cannot be empty."></asp:RequiredFieldValidator>
            </div>
        </div>
        <br />
        <div class="row text-left">
            <div class="col-lg-9">
                <asp:Label ID="lbProgramme" runat="server" Font-Bold="true" Text="Available Programme"></asp:Label>
                <asp:DropDownList runat="server" CssClass="form-control" ID="ddlProgramme" AutoPostBack="true" DataTextField="programmeTitle" DataValueField="programmeId" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged" CausesValidation="false" Enabled="false"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvProgramme" runat="server" Display="None" ForeColor="Red" ControlToValidate="ddlProgramme" ErrorMessage="Programme cannot be empty."></asp:RequiredFieldValidator>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbStartDate" runat="server" Font-Bold="true" Text="Class Start Date"></asp:Label>
                <asp:DropDownList runat="server" CssClass="form-control" ID="ddlStartDate" DataTextField="programmeStartDate" DataValueField="programmeBatchId" Enabled="false"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" Display="None" ForeColor="Red" ControlToValidate="ddlStartDate" ErrorMessage="Class start date cannot be empty."></asp:RequiredFieldValidator>
            </div>
        </div>
        <br />
        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnRegister" CssClass="btn btn-info" runat="server" Text="Register" OnClick="btnRegister_Click"/>
                <asp:Button ID="btnClear" CssClass="btn btn-default" runat="server" Text="Clear" OnClick="btnClear_Click" CausesValidation="false"/>
            </div>
        </div>
    </div>
</asp:Content>
