<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="module-creation.aspx.cs" Inherits="ACI_TMS.module_creation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function validateEffectiveDate(oSrc, args) {

            var startStr = $('#<%=tbModuleEffectiveDate.ClientID%>').val();

            if (startStr == "") {
                args.IsValid = false;
                return false;
            }

            if (!isValidDate(startStr)) {
                args.IsValid = false;
                return false;
            }

            //TODO: enable validation
            //module effective date cannot be earlier than current date
            //var today = new Date();
            //var startDt = new Date(startStr);

            //if (today > startDt) {
            //    args.IsValid = false;
            //    return false;
            //}

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
                    <asp:Label ID="lbModuleCreationHeader" runat="server" Text="New Module"></asp:Label>
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

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbModuleCode" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleCodeValue" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleCodeRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleCodeValue" runat="server" ErrorMessage="Please fill in the Module Code"></asp:RequiredFieldValidator>
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
                <asp:TextBox ID="tbModuleVersion" runat="server" Text="1" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbModuleLevel" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleLevelValue" runat="server" CssClass="form-control" MaxLength="3"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleLevelRequiredValidator" Display="None" ControlToValidate="tbModuleLevelValue" runat="server" ErrorMessage="Please fill in the Module Level"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revLvl" runat="server" ErrorMessage="Level must be positive whole number, greater than zero" Display="None" ControlToValidate="tbModuleLevelValue" ValidationExpression="^[1-9]\d*$" ></asp:RegularExpressionValidator>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbModuleCredit" runat="server" Text="Credit" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleCreditValue" runat="server" CssClass="form-control" MaxLength="3"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleCreditRequiredValidator" Display="None" ControlToValidate="tbModuleCreditValue" runat="server" ErrorMessage="Please fill in the Module Credit"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revCredit" runat="server" ErrorMessage="Credit must be positive whole number, greater than zero" Display="None" ControlToValidate="tbModuleCreditValue" ValidationExpression="^[1-9]\d*$" ></asp:RegularExpressionValidator>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbModuleTrgHr" runat="server" Text="Training Hour(s)" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleTrgHr" runat="server" CssClass="form-control" MaxLength="5"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleTrainingHrRequiredValidator" Display="None" ControlToValidate="tbModuleTrgHr" runat="server" ErrorMessage="Please fill in the Module Training Hour"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revTrainHr" runat="server" ErrorMessage="Training hour must be a non negative number, up to 1 decimal place" Display="None" ControlToValidate="tbModuleTrgHr" ValidationExpression="^\d+(\.\d{1})?$" ></asp:RegularExpressionValidator>
            </div>

        </div>

        <br />

        <div class="row text-left">

            <div class="col-lg-4">
                <asp:Label ID="lbModuleCost" runat="server" Text="Cost" Font-Bold="true"></asp:Label>

                <div class="inner-addon left-addon">
                    <i class="glyphicon glyphicon-usd" ></i>
                <asp:TextBox ID="tbModuleCostValue" runat="server" CssClass="form-control" MaxLength="8"></asp:TextBox>
                    </div>

                <asp:RequiredFieldValidator ID="moduleCostRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleCostValue" runat="server" ErrorMessage="Please fill in the Module Cost"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="rev" runat="server" ErrorMessage="Module cost must be a non negative number, up to 2 decimal places" Display="None" ControlToValidate="tbModuleCostValue" ValidationExpression="^\d+(\.\d{1,2})?$" ></asp:RegularExpressionValidator>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbModuleEffectiveDate" runat="server" Text="Effective Date" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbModuleEffectiveDate" runat="server" Display="None" placeholder="dd MMM yyyy" CssClass="datepicker form-control"></asp:TextBox>


                <asp:RequiredFieldValidator ID="moduleEffectDateRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleEffectiveDate" runat="server" ErrorMessage="Please fill in the Module Effective Date"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="cvModuleEffectDate" runat="server" Display="None" ControlToValidate="tbModuleEffectiveDate" ClientValidationFunction="validateEffectiveDate"
                    ErrorMessage="Date is not in dd MMM yyyy format OR cannot be earlier than today" ValidateEmptyText="false"></asp:CustomValidator>
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
                <asp:TextBox ID="tbModuleDescription" runat="server" CssClass="form-control" MaxLength="255" TextMode="MultiLine" Rows="5"></asp:TextBox>

                <asp:RequiredFieldValidator ID="moduleDescRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbModuleDescription" runat="server" ErrorMessage="Please fill in the Module Description"></asp:RequiredFieldValidator>

            </div>
        </div>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnCreate" runat="server" class="btn btn-primary" Text="Create" OnClick="btnCreate_Click" />
                <asp:Button ID="btnClear" runat="server" class="btn btn-default" Text="Clear" CausesValidation="false" OnClick="btnClear_Click" />
            </div>
        </div>
    </div>
</asp:Content>
