<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="module-view.aspx.cs" Inherits="ACI_TMS.module_view" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(function () {
                $('[data-toggle="tooltip"]').tooltip()
            })
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                <h3>
                    <asp:Label ID="lbModuleDetailHeader" runat="server" Text="View Module"></asp:Label>
                </h3>
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
        <asp:HiddenField ID="hfModuleId" runat="server" />

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbModuleCode" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleCodeValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>

            </div>

            <div class="col-lg-9">
                <asp:Label ID="lbModuleTitle" runat="server" Text="Title" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleTitleValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>

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
                <asp:Label ID="lbModuleLevelValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>

            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbModuleCredit" runat="server" Text="Credit" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleCreditValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>

            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbModuleTrgHr" runat="server" Text="Training Hour" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleTrgHrValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row text-left">

            <div class="col-lg-4">
                <asp:Label ID="lbModuleCost" runat="server" Text="Cost" Font-Bold="true"></asp:Label>
                <div class="inner-addon left-addon">
                    <i class="glyphicon glyphicon-usd"></i>
                    <asp:TextBox ID="lbModuleCostValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbModuleEffectiveDate" runat="server" Text="Effective Date" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleEffectiveDateValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>

            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbWSQCompetencyCode" runat="server" Text="WSQ Competency Code" Font-Bold="true"></asp:Label>
                <i class="glyphicon glyphicon-question-sign" data-toggle="tooltip" data-placement="top" title="For Short Course, this is replaced with SSG Reference Num."></i>
                <asp:Label ID="lbWSQCompetencyCodeValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>

            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbModuleDescription" runat="server" Text="Description" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="lbModuleDescriptionValue" runat="server" CssClass="form-control" ReadOnly="true" TextMode="MultiLine" Rows="5"></asp:TextBox>
            </div>
        </div>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnUpdateCurrent" runat="server" CssClass="btn btn-primary" OnClick="btnUpdateCurrent_Click" Text="Edit" />
                <asp:Button ID="btnUpdateNew" runat="server" CssClass="btn btn-primary" OnClick="btnUpdateNew_Click" Text="Copy to new version" />
                <button id="btnConfirmDel" runat="server" type="button" class="btn btn-danger" data-toggle="modal" data-target="#diagDelete">Delete</button>
            </div>
        </div>
    </div>

    <div id="diagDelete" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete module</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this module?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnDelete_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

