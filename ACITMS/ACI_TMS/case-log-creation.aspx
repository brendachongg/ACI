<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="case-log-creation.aspx.cs" Inherits="ACI_TMS.case_log_creation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        
        function validateIncidentDate(oSrc, args) {
            var startStr = $('#<%=tbIncidentDate.ClientID%>').val();
            if (startStr == "" ) {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(startStr)) {
                args.IsValid = false;
                return false;
            }

            var startDt = new Date(startStr);

            args.IsValid = true;
            return true;
            }

        function checkCaseLogFileSize(oSrc, args) {
            var allowedExtension = ['JPG', 'JPEG', 'PNG', 'PDF', 'DOC', 'DOCX', 'XLS', 'XLSX', 'CSV'];
            var uploadControl = document.getElementById('<%= fileUploadCaseLogAttachment.ClientID %>');
            var fileExtension = document.getElementById('<%= fileUploadCaseLogAttachment.ClientID %>').value.substring(document.getElementById('<%= fileUploadCaseLogAttachment.ClientID %>').value.lastIndexOf('.') + 1).toUpperCase();
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
                    <asp:Label ID="lbCaseLogCreationCreationHeader" runat="server" Text="New Case Log"></asp:Label>
                </h3>
                <small>Please fill up the following
                </small>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click" CausesValidation="false"></asp:LinkButton>
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

        <div class="row text-left">
            <div class="col-lg-6">
                <asp:Label ID="lbCaseLogCategory" runat="server" Text="Category" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="ddlCaseLogCategory" CssClass="form-control" runat="server">
                </asp:DropDownList>

                <asp:RequiredFieldValidator ID="rfvCaseLogCategory" Display="None" ForeColor="Red" ControlToValidate="ddlCaseLogCategory" runat="server" ErrorMessage="Category cannot be empty"></asp:RequiredFieldValidator>
            </div>


            <div class="col-lg-6">
                <asp:Label ID="lbSubject" runat="server" Text="Subject" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbSubject" runat="server" placeholder="" CssClass="form-control" MaxLength="100"></asp:TextBox>

                <asp:RequiredFieldValidator ID="subjectRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbSubject" runat="server" ErrorMessage="Subject cannot be empty"></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-6">
                <asp:Label ID="lbIncidentDate" runat="server" Text="Incident Date:" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbIncidentDate" runat="server" CssClass="datepicker form-control"></asp:TextBox>
                <asp:CustomValidator ID="cvIncidentDate" runat="server" Display="None" ControlToValidate="tbIncidentDate" ClientValidationFunction="validateIncidentDate"
                            ErrorMessage="Date is not in dd MMM yyyy format" ValidateEmptyText="false"></asp:CustomValidator>
                <asp:RequiredFieldValidator ID="rfvIncidentDate" Display="None" ControlToValidate="tbIncidentDate" runat="server" ErrorMessage="Incident date cannot be empty."></asp:RequiredFieldValidator>
            </div>

            <div class="col-lg-6">
                <asp:Label ID="lbTimeOccured" runat="server" Text="Incident Time:" Font-Bold="true"></asp:Label>
                <div class="input-group">
                    <asp:DropDownList ID="ddlIncidentHour" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                    <span class="input-group-addon" style="font-weight: bold;">:</span>
                    <asp:DropDownList ID="ddlIncidentMinutes" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
        </div>
        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbCaseLogMessage" runat="server" Text="Message" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbCaseLogMessageValue" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvMessage" Display="None" ControlToValidate="tbCaseLogMessageValue" runat="server" ErrorMessage="Message cannot be empty."></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbCaseLogAttachment" runat="server" Text="Case Log Attachment" Font-Bold="true"></asp:Label>

                <i class="glyphicon glyphicon-question-sign" data-toggle="tooltip" data-placement="top" title="Only .JPG, .JPEG, .PNG, .PDF, .DOC, .DOCX with maximum size of 2MB is allowed. For cases of multiple images, please upload it as a .DOC, .DOCX or .PDF"></i>

                <asp:FileUpload ID="fileUploadCaseLogAttachment" runat="server" CssClass="form-control" AllowMultiple ="true"/>

                <asp:CustomValidator ID="cvCaseLogAttachment" runat="server" Display="None" ControlToValidate="fileUploadCaseLogAttachment"
                    ErrorMessage="File exceeded the size limit OR File is not in proper format." ValidateEmptyText="false" ClientValidationFunction="checkCaseLogFileSize"></asp:CustomValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Image ID="imgCaseLogImage" runat="server" Style="width: 100%; height: 300px; display: none;" />
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
</asp:Content>
