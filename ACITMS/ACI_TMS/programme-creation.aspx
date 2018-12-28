<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="programme-creation.aspx.cs" Inherits="ACI_TMS.programme_creation" %>

<%@ Register Src="~/bundle-search.ascx" TagPrefix="uc1" TagName="bundlesearch" %>
<%@ Register Src="~/enrollment-letter-legend.ascx" TagPrefix="uc1" TagName="enrollmentletterlegend" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        function validateSOAModule(oSrc, args) {
            var rowCount = ($("#<%=gvModules.ClientID %> tr").length) - 1;
            var numOfSOA = $("#<%=tbNumofSOA.ClientID %>").val();

            if (isNaN(numOfSOA)) {
                //return true first as the expression validator will be fired and show the appropriate message
                args.IsValid = true;
                return;
            }

            if (rowCount >= numOfSOA) {
                args.IsValid = true;
            }
            else {
                args.IsValid = false;
            }
        }

        function previewFile() {
            var preview = document.querySelector('#<%=imgProgrammeImage.ClientID %>');
            var file = document.querySelector('#<%=fileUploadProgrammeImage.ClientID %>').files[0];
            var reader = new FileReader();

            reader.onloadend = function () {
                preview.src = reader.result;
            }

            if (file) {
                reader.readAsDataURL(file);
            } else {
                preview.src = "";
            }
        }

        function checkProgrammeImageSize(oSrc, args) {

            document.getElementById('<%=imgProgrammeImage.ClientID %>').style.display = "block";

            var allowedExtension = ['JPEG', 'JPG', 'PNG'];
            var uploadControl = document.getElementById('<%= fileUploadProgrammeImage.ClientID %>');
            var fileExtension = document.getElementById('<%= fileUploadProgrammeImage.ClientID %>').value.substring(document.getElementById('<%= fileUploadProgrammeImage.ClientID %>').value.lastIndexOf('.') + 1).toUpperCase();
            var checkSize = true;
            var checkExtension = false;

            var i = 0;

            for (i = 0; i < allowedExtension.length ; i++) {
                if (fileExtension == allowedExtension[i]) {
                    checkExtension = true;
                }
            }

            if (uploadControl.files[0].size > 2000000) {
                checkSize = false;
            }

            if (checkSize == false || checkExtension == false) {
                args.IsValid = false;
                return false;
            }
            else {
                args.IsValid = true;
                return true;
            }
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                <h3>
                    <asp:Label ID="lbProgrammeCreationHeader" runat="server" Text="New Programme"></asp:Label>
                </h3>
                <small>Please fill up the following
                </small>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" OnClick="lkbtnBack_Click" CssClass="btn btn-sm btn-default" Text="Back" CausesValidation="false"></asp:LinkButton>

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
        <asp:ValidationSummary ID="vSummary2" ValidationGroup="validateFileSize" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

        <div class="row text-left">
            <div class="col-lg-4">
                <asp:Label ID="lbProgrammeCategory" runat="server" Text="Category" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="ddlProgrammeCategory" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                </asp:DropDownList>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbProgrammeCode" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbProgrammeCode" runat="server" placeholder="" CssClass="form-control" MaxLength="20"></asp:TextBox>

                <asp:RequiredFieldValidator ID="programmeCodeRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbProgrammeCode" runat="server" ErrorMessage="Please fill in the Programme Code"></asp:RequiredFieldValidator>

            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbProgrammeLevel" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="ddlProgrammeLevel" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                </asp:DropDownList>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbCourseCode" runat="server" Text="Course Code" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbCourseCode" runat="server" placeholder="" CssClass="form-control" MaxLength="20"></asp:TextBox>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" Display="None" ForeColor="Red" ControlToValidate="tbCourseCode" runat="server" ErrorMessage="Please fill in the Course Code"></asp:RequiredFieldValidator>
            </div>

            <div class="col-lg-9">
                <asp:Label ID="lbProgrammeTitle" runat="server" Text="Title" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbProgrammeTitle" runat="server" placeholder="" CssClass="form-control" MaxLength="100"></asp:TextBox>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" Display="None" ForeColor="Red" ControlToValidate="tbProgrammeTitle" runat="server" ErrorMessage="Please fill in the Programme Title"></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbProgrammeType" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="ddlProgrammeType" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                </asp:DropDownList>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbProgrammeVersion" runat="server" Text="Version" Font-Bold="true"></asp:Label>
                <asp:Label ID="tbProgrammeVersion" runat="server" Text="1" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbNumOfSOA" runat="server" Text="Num. Of SOA" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbNumofSOA" runat="server" placeholder="" CssClass="form-control" MaxLength="3"></asp:TextBox>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" Display="None" ControlToValidate="tbNumofSOA" runat="server" ErrorMessage="Please fill in the No. of SOA"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revSOA" runat="server" ErrorMessage="SOA must be positive whole number" Display="None" ControlToValidate="tbNumOfSOA" ValidationExpression="^[0-9]\d*$" ></asp:RegularExpressionValidator>
                <asp:CustomValidator ID="cvSOA" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbNumOfSOA" ClientValidationFunction="validateSOAModule" ValidateEmptyText="false" ErrorMessage="Number of SOA cannot be more than number of modules"></asp:CustomValidator>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbSSGRefNum" runat="server" Text="SSG Reference Num." Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbSSGRefNum" runat="server" placeholder="" CssClass="form-control" MaxLength="255"></asp:TextBox>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" Display="None" ForeColor="Red" ControlToValidate="tbSSGRefNum" runat="server" ErrorMessage="Please fill in the SSG Reference No."></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbProgrammeDescription" runat="server" Text="Description" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbProgrammeDescription" runat="server" placeholder="" CssClass="form-control" TextMode="MultiLine" Rows="5" MaxLength="255"></asp:TextBox>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" Display="None" ForeColor="Red" ControlToValidate="tbProgrammeDescription" runat="server" ErrorMessage="Please fill in the Programme Description"></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lblBundle" runat="server" Text="Bundle" Font-Bold="true"></asp:Label>
                <div class="inner-addon right-addon">
                    <i class="glyphicon glyphicon-search" style="cursor: pointer;" data-target="#diagSearchPackage" data-toggle="modal"></i>
                    <asp:TextBox ID="txtBundle" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    <asp:HiddenField ID="hfBundleId" runat="server" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" Display="None" ForeColor="Red" ControlToValidate="txtBundle" runat="server" ErrorMessage="Please choose a Bundle"></asp:RequiredFieldValidator>
                </div>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvModules" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="false" runat="server">
                    <EmptyDataTemplate>
                        No available module
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:BoundField DataField="ModuleCode" HeaderText="Module" />
                        <asp:BoundField DataField="ModuleTitle" HeaderText="Title" />
                        <asp:BoundField DataField="ModuleNumOfSession" HeaderText="Num. of Sessions" ItemStyle-Width="100px" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbProgrammeImage" runat="server" Text="Programme Image" Font-Bold="true"></asp:Label>

                <i class="glyphicon glyphicon-question-sign" data-toggle="tooltip" data-placement="top" title="Only .JPG, .JPEG, .PNG image with maximum size of 2MB is allowed."></i>

                <asp:FileUpload ID="fileUploadProgrammeImage" runat="server" CssClass="form-control" onchange="previewFile(this);" />

                <asp:CustomValidator ID="cvProgrammeImage1" runat="server" Display="None" ControlToValidate="fileUploadProgrammeImage"
                    ErrorMessage="Image exceeded the size limit OR image is not in proper format." ValidateEmptyText="false" ClientValidationFunction="checkProgrammeImageSize"></asp:CustomValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Image ID="imgProgrammeImage" runat="server" Style="width: 100%; height: 300px; display: none;" />
            </div>
        </div>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnCreate" runat="server" class="btn btn-info" Text="Create" OnClick="btnCreate_Click" />
                <asp:Button ID="btnClear" runat="server" class="btn btn-default" Text="Clear" CausesValidation="false" OnClick="btnClear_Click" />
            </div>
        </div>

    </div>

    <uc1:enrollmentletterlegend runat="server" id="enrollmentletterlegend" />
    <uc1:bundlesearch runat="server" ID="bundlesearch" />
</asp:Content>
