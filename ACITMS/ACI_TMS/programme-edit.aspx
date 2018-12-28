<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="programme-edit.aspx.cs" Inherits="ACI_TMS.programme_edit" %>

<%@ Register Src="~/bundle-search.ascx" TagPrefix="uc1" TagName="bundlesearch" %>
<%@ Register Src="~/enrollment-letter-legend.ascx" TagPrefix="uc1" TagName="enrollmentletterlegend" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function validateSOAModule(oSrc, args) {
            var rowCount = ($("#<%=gvModules.ClientID %> tr").length) - 1;
            var numOfSOA = $("#<%=tbNumofSOA.ClientID %>").val();

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

        function toggleImage() {
            var image = document.getElementById('<%= imgProgrammeImage.ClientID%>');
            var lbImage = document.getElementById('<%= lbProgrammeImageEmpty.ClientID%>');

            if (image.style.display = 'none') {
                image.style.display = 'block';
            }

            if (lbImage.style.display = 'block') {
                lbImage.style.display = 'none';
            }
        }

        function checkProgrammeImageSize(oSrc, args) {

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
                    <asp:Label ID="lbTitle" runat="server" Font-Bold="true" Text="Edit Programme"></asp:Label>
                </h3>
                <small>Please update the following as needed</small>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lkbtnBack_Click" CausesValidation="false"></asp:LinkButton>
            </div>
        </div>

        <hr />
        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger" id="panelSysError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSysError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <asp:ValidationSummary ID="vSummary2" ValidationGroup="validateFileSize" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <asp:HiddenField ID="hfProgrammeId" runat="server" />

        <div class="row text-left">
            <div class="col-lg-4">
                <asp:Label ID="lbProgrammeCategory" runat="server" Text="Programme Category" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="ddlProgrammeCategory" CssClass="form-control" runat="server" DataTextField="codeValueDisplay" DataValueField="codeValue">
                </asp:DropDownList>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbProgrammeCode" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgrammeCodeValue" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:Label>
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
                <asp:Label ID="lbCourseCodeValue" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:Label>
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
                <asp:Label ID="lbProgrammeVersionValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbNumOfSOA" runat="server" Text="No. Of SOA" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbNumofSOA" runat="server" placeholder="" CssClass="form-control" MaxLength="3"></asp:TextBox>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" Display="None" ForeColor="Red" ControlToValidate="tbNumofSOA" runat="server" ErrorMessage="Please fill in the No. of SOA"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvSOA" runat="server" Display="None" ForeColor="Red" ControlToValidate="tbNumOfSOA" ClientValidationFunction="validateSOAModule" ValidateEmptyText="false" ErrorMessage="Number of SOA cannot be more than number of modules"></asp:CustomValidator>
                <asp:RegularExpressionValidator ID="revSOA" runat="server" ErrorMessage="SOA must be positive whole number" Display="None" ControlToValidate="tbNumOfSOA" ValidationExpression="^[0-9]\d*$" ></asp:RegularExpressionValidator>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbSSGRefNum" runat="server" Text="SSG Reference No." Font-Bold="true"></asp:Label>
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
                    <i class="glyphicon glyphicon-search" runat="server" id="iconSearchPkg" data-toggle="modal" data-target="#diagSearchPackage" style="cursor: pointer;"></i>
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

                <asp:FileUpload ID="fileUploadProgrammeImage" runat="server" CssClass="form-control" onchange="previewFile(this); toggleImage();" />

                <asp:CustomValidator ID="cvProgrammeImage2" runat="server" Display="None" ControlToValidate="fileUploadProgrammeImage"
                    ErrorMessage="Image exceeded the size limit OR image is not in proper format." ValidateEmptyText="false" ClientValidationFunction="checkProgrammeImageSize"></asp:CustomValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbProgrammeImageEmpty" runat="server" CssClass="form-control" ReadOnly="true" Text="No existing image"></asp:Label>
            </div>

            <div class="col-lg-12">
                <asp:Image ID="imgProgrammeImage" runat="server" Style="width: 100%; height: 300px;" />
            </div>
        </div>
        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" Text="Save" />
                <button type="button" class="btn btn-default" data-toggle="modal" data-target="#diagReloadprogramme">Clear</button>
            </div>
        </div>
    </div>

    <div id="diagReloadprogramme" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Discard</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to discard all changes?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnClearProgramme" runat="server" CssClass="btn btn-default" Text="OK" CausesValidation="false" OnClick="btnClearProgramme_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hfEditNew" runat="server" />

    <uc1:enrollmentletterlegend runat="server" ID="enrollmentletterlegend" />
    <uc1:bundlesearch runat="server" ID="bundlesearch" />
</asp:Content>
