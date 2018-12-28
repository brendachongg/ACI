<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="applicant-module-exemption.aspx.cs" Inherits="ACI_TMS.applicant_module_exemption" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="row text-left">
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                        <h3>
                            <asp:Label ID="lbApplicationHeader" runat="server" Text="Application"></asp:Label>
                        </h3>

                        <small>

                            <asp:Label ID="lbApplicantHeader" runat="server" Text="Application ID: "> </asp:Label>
                            <asp:Label ID="lbApplicantId" runat="server" Text=""></asp:Label>

                            <br />
                        </small>
                    </div>
                </div>

                <hr />

                <div class="row text-left">
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                        <h4>
                            <asp:Label ID="lbExemptedModule" runat="server" Text="Module Exemption"></asp:Label>
                        </h4>

                        <small>
                            <asp:Label ID="lbExemptedModuleMsg" runat="server" Text="Uncheck to exclude module"></asp:Label>
                            <br />

                        </small>
                    </div>
                </div>

                <br />


                <div class="row">
                    <div class="col-lg-12 col-md-6 col-sm-6 col-xs-6">
                        <table class="table">
                            <asp:Repeater ID="rptModuleExamptedSelection" runat="server"  OnItemDataBound="rptModuleExamptedSelection_ItemDataBound" OnItemCommand="rptModuleExamptedSelection_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            
                                            <asp:LinkButton ID="lkbtnExemptedSelection" CssClass="fa fa-check-circle fa-2x" CommandName='<%#Eval("moduleId") %>' CommandArgument='<%# STATUS_TAKING %>' runat="server"></asp:LinkButton>
                                        </td>

                                        <td>
                                            <asp:Label ID="lbModuleCode" runat="server" Text='<%#Eval("WSQCompetencyCode") %>'>'></asp:Label>
                                        </td>

                                        <td>
                                            <asp:Label ID="lbModuleTitle" runat="server" Text='<%#Eval("moduleTitle") %>'></asp:Label>
                                        </td>

                                        <td>
                                            <asp:Label ID="lbModuleCost" runat="server" Text='<%#Eval("moduleCost","{0:C}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </div>
                </div>

                <br />

                <div class="row text-right">
                    <div class="col-lg-12 ">
                        <asp:LinkButton ID="lkbtnBack" runat="server" OnClick="lkbtnBack_Click" CssClass="btn btn-sm btn-default" Text="Back"></asp:LinkButton>
                        <asp:LinkButton ID="lkbtnSave" runat="server" OnClick="lkbtnSave_Click" CssClass="btn btn-sm btn-info" Text="Save"></asp:LinkButton>
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
