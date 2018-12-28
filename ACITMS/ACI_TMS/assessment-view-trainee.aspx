<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="assessment-view-trainee.aspx.cs" Inherits="ACI_TMS.assessment_view_trainee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.collapse').on('shown.bs.collapse', function () {
                $(this).parent().find(".glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-minus");
            }).on('hidden.bs.collapse', function () {
                $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-plus");
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>View Assessment <i>(By Trainee)</i></h3>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click" CausesValidation="false"></asp:LinkButton>
            </div>
        </div>
        <hr />

        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lb1" runat="server" Text="Trainee ID" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbTraineeId" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-9">
                <asp:Label ID="lb2" runat="server" Text="Trainee Name" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbTraineeName" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-lg-12">
                <div class="panel-group" id="accordion">
                    <%------------------------------------ Programme Details ------------------------------------%>
                    <div class="panel panel-default">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="panel-title">
                                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#programmeDetails">
                                        <span id="ProgrammeIcon" class="glyphicon glyphicon-plus"></span>
                                        Programme Details
                                    </a>
                                </h4>
                            </div>
                            <div id="programmeDetails" class="panel-collapse collapse">
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-3">
                                            <asp:Label ID="lb3" runat="server" Text="Category" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeCategory" runat="server" CssClass="form-control"></asp:Label>
                                        </div>

                                        <div class="col-lg-3">
                                            <asp:Label ID="lb4" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeLevel" runat="server" CssClass="form-control"></asp:Label>
                                        </div>

                                        <div class="col-lg-3">
                                            <asp:Label ID="lb5" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeType" runat="server" CssClass="form-control"></asp:Label>
                                        </div>

                                        <div class="col-lg-3">
                                            <asp:Label ID="lb6" runat="server" Text="Version" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeVersion" runat="server" CssClass="form-control"></asp:Label>
                                        </div>
                                    </div>

                                    <br />

                                    <div class="row">
                                        <div class="col-lg-3">
                                            <asp:Label ID="lb7" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeCode" runat="server" CssClass="form-control"></asp:Label>
                                        </div>

                                        <div class="col-lg-9">
                                            <asp:Label ID="lb8" runat="server" Text="Programme" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgramme" runat="server" CssClass="form-control"></asp:Label>
                                        </div>    
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%------------------------------------ END Programme Details ------------------------------------%>
                    <br />
                    <%------------------------------------ Class Details ------------------------------------%>
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#classDetails">
                                    <span id="classIcon" class="glyphicon glyphicon-plus"></span>
                                    Class Details
                                </a>
                            </h4>
                        </div>
                        <div id="classDetails" class="panel-collapse collapse">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-4">
                                        <asp:Label ID="lb9" runat="server" Text="Class" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbBatchCode" runat="server" CssClass="form-control"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:Label ID="lb10" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbBatchType" runat="server" CssClass="form-control"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:Label ID="lb11" runat="server" Text="Project Code" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbProjCode" runat="server" CssClass="form-control"></asp:Label>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-lg-6">
                                        <asp:Label ID="lb12" runat="server" Text="Registration Date" Font-Bold="true"></asp:Label>

                                        <div class="input-group">
                                            <asp:Label ID="lbRegStartDate" runat="server" CssClass="form-control"></asp:Label>
                                            <span class="input-group-addon" style="font-weight: bold;">to</span>
                                            <asp:Label ID="lbRegEndDate" runat="server" CssClass="form-control"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <asp:Label ID="lb13" runat="server" Text="Commencement Date" Font-Bold="true"></asp:Label>

                                        <div class="input-group">
                                            <asp:Label ID="lbBatchStartDate" runat="server" CssClass="form-control"></asp:Label>
                                            <span class="input-group-addon" style="font-weight: bold;">to</span>
                                            <asp:Label ID="lbBatchEndDate" runat="server" CssClass="form-control"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-lg-6">
                                        <asp:Label ID="lb14" runat="server" Text="Capacity" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbCapacity" runat="server" CssClass="form-control"></asp:Label>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:Label ID="lb15" runat="server" Text="Mode" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbClassMode" runat="server" CssClass="form-control"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%------------------------------------ END Class Details ------------------------------------%>
                </div>
            </div>
        </div>
        <br />
        <asp:HiddenField ID="hfBatchModuleId" runat="server" />
        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lb16" runat="server" Text="Module Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleCode" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-9">
                <asp:Label ID="lb17" runat="server" Text="Module" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModule" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>
        <br />
        <hr />
        <div class="row text-left">
            <div class="col-lg-6">
                <asp:Label ID="lb18" runat="server" Text="First Attempt" Font-Bold="true" Font-Underline="true"></asp:Label>
            </div>
            <div class="col-lg-6">
                <asp:Label ID="lb19" runat="server" Text="Second Attempt" Font-Bold="true" Font-Underline="true"></asp:Label>
            </div>
        </div>
        <br />
        <div class="row text-left">
            <div class="col-lg-6">
                <asp:Label ID="lb20" runat="server" Text="Assessment Date" Font-Bold="true"></asp:Label>
                <asp:Label ID="lb1stAssessDt" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-6">
                <asp:Label ID="lb21" runat="server" Text="Assessment Date" Font-Bold="true"></asp:Label>
                <asp:Label ID="lb2ndAssessDt" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>
        <br />
        <div class="row text-left">
            <div class="col-lg-6">
                <asp:Label ID="lb22" runat="server" Text="Assessor" Font-Bold="true"></asp:Label>
                <asp:Label ID="lb1stAssessor" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-6">
                <asp:Label ID="lb23" runat="server" Text="Assessor" Font-Bold="true"></asp:Label>
                <asp:Label ID="lb2ndAssessor" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>
        <br />
        <div class="row text-left">
            <div class="col-lg-6">
                <asp:Label ID="lb24" runat="server" Text="Result" Font-Bold="true"></asp:Label><br />
                <asp:Label ID="lb1stResult" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-6">
                <asp:Label ID="lb25" runat="server" Text="Result" Font-Bold="true"></asp:Label><br />
                <asp:Label ID="lb2ndResult" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>
        <br />
        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnEdit" runat="server" class="btn btn-primary" Text="Edit" OnClick="btnEdit_Click" />
            </div>
        </div>
        <br />
    </div>
</asp:Content>
