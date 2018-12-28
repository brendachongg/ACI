<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="module-edit.aspx.cs" Inherits="ACI_TMS.module_edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function validateEffectiveDate(oSrc, args) {
            var startStr = $('#<%=tbModuleEffectiveDateValue.ClientID%>').val();

            if (startStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(startStr)) {
                args.IsValid = false;
                return false;
            }

            //module effective date cannot be earlier than current date
            //var today = new Date();
            //var startDt = new Date(startStr);

            //if (today > startDt) {
            //    args.IsValid = false;
            //    return false;
            //}

            //args.IsValid = true;
            //return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbTitle" runat="server" Font-Bold="true" Text="Edit Module"></asp:Label>
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
        <asp:HiddenField ID="hfModuleId" runat="server" />

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbModuleCode" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleCodeValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-9">
                <asp:Label ID="lbModuleTitle" runat="server" Text="Title" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleTitleValue" runat="server" CssClass="form-control" MaxLength="125"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleTitleRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleTitleValue" runat="server" ErrorMessage="Please fill in the Module Title"></asp:RequiredFieldValidator>

            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbModuleVersion" runat="server" Text="Version" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleVersionValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbModuleLevel" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleLevelValue" runat="server" CssClass="form-control" MaxLength="3"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleLevelRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleLevelValue" runat="server" ErrorMessage="Please fill in the Module Level"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revLvl" runat="server" ErrorMessage="Level must be positive whole number, greater than zero" Display="None" ControlToValidate="tbModuleLevelValue" ValidationExpression="^[1-9]\d*$" ></asp:RegularExpressionValidator>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbModuleCredit" runat="server" Text="Credit" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleCreditValue" runat="server" CssClass="form-control" MaxLength="3"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleCreditRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleCreditValue" runat="server" ErrorMessage="Please fill in the Module Credit"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revCredit" runat="server" ErrorMessage="Credit must be positive whole number, greater than zero" Display="None" ControlToValidate="tbModuleCreditValue" ValidationExpression="^[1-9]\d*$" ></asp:RegularExpressionValidator>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbModuleTrgHr" runat="server" Text="Training Hour(s)" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleTrgHrValue" runat="server" CssClass="form-control" MaxLength="5"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleTrainingHrRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleTrgHrValue" runat="server" ErrorMessage="Please fill in the Module Training Hour"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revTrainHr" runat="server" ErrorMessage="Training hour must be a non neagtive number, up to 1 decimal place" Display="None" ControlToValidate="tbModuleTrgHrValue" ValidationExpression="^\d+(\.\d{1})?$" ></asp:RegularExpressionValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">

            <div class="col-lg-4">
                <asp:Label ID="lbModuleCost" runat="server" Text="Cost (SGD$)" Font-Bold="true"></asp:Label>
                <div class="inner-addon left-addon">
                    <i class="glyphicon glyphicon-usd"></i>
                    <asp:TextBox ID="tbModuleCostValue" runat="server" CssClass="form-control" MaxLength="8"></asp:TextBox>
                </div>

                <asp:RequiredFieldValidator ID="moduleCostRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleCostValue" runat="server" ErrorMessage="Please fill in the Module Cost"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="rev" runat="server" ErrorMessage="Module cost must be a non neagtive number, up to 2 decimal places" Display="None" ControlToValidate="tbModuleCostValue" ValidationExpression="^\d+(\.\d{1,2})?$" ></asp:RegularExpressionValidator>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbModuleEffectiveDate" runat="server" Text="Effective Date" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleEffectiveDateValue" runat="server" CssClass="datepicker form-control"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleEffectDateRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleEffectiveDateValue" runat="server" ErrorMessage="Please fill in the Module Effective Date"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvModuleEffectDate" runat="server" Display="None" ControlToValidate="tbModuleEffectiveDateValue" ClientValidationFunction="validateEffectiveDate"
                    ErrorMessage="Date is not in dd MMM yyyy format" ValidateEmptyText="false"></asp:CustomValidator>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbWSQCompetencyCode" runat="server" Text="WSQ Competency Code" Font-Bold="true"></asp:Label>
                <i class="glyphicon glyphicon-question-sign" data-toggle="tooltip" data-placement="top" title="For Short Course, please replace this with SSG Reference Num."></i>
                <asp:TextBox ID="tbWSQCompetencyCodeValue" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>

                <asp:RequiredFieldValidator ID="WSQRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbWSQCompetencyCodeValue" runat="server" ErrorMessage="Please fill in the WSQ Competency Code"></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbModuleDescription" runat="server" Text="Description" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleDescriptionValue" runat="server" MaxLength="255" CssClass="form-control" TextMode="MultiLine" Rows="5"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleDescRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleDescriptionValue" runat="server" ErrorMessage="Please fill in the Module Description"></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" OnClick="btnSave_Click" Text="Save" />
                <button type="button" class="btn btn-default" data-toggle="modal" data-target="#diagReloadmodule">Clear</button>
            </div>
        </div>
    </div>

    <div id="diagReloadmodule" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Clear</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to discard all changes?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnClearModule" runat="server" CssClass="btn btn-default" OnClick="btnClearModule_Click" Text="OK" CausesValidation="false" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
