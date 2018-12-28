<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="assessment-edit-trainee.aspx.cs" Inherits="ACI_TMS.assessment_edit_trainee" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.collapse').on('shown.bs.collapse', function () {
                $(this).parent().find(".glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-minus");
            }).on('hidden.bs.collapse', function () {
                $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-plus");
            });
        });

        function validate1stDt(oSrc, args) {
            var str = $('#<%=txt1stAssessDt.ClientID%>').val();

            if (!checkDate(str)) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validate2ndAssessment(oSrc, args) {
            if($('#<%=txt2ndAssessDt.ClientID%>').val()=="" &&
                $('#<%=ddl2ndResult.ClientID%> option:selected').val() == "") {
                args.IsValid = true;
                return true;
            }

            var str = $('#<%=txt2ndAssessDt.ClientID%>').val();
            if (str=="" || !checkDate(str)) {
                args.IsValid = false;
                return false;
            }

            //check that the 2nd assessment date cannot be later than 1st
            if ($('#<%=txt1stAssessDt.ClientID%>').val() != "") {
                var dt2 = new Date(str);
                var dt1 = new Date($('#<%=txt1stAssessDt.ClientID%>').val());
                if (dt1 >= dt2) {
                    args.IsValid = false;
                    return false;
                }
            } else {
                args.IsValid = false;
                return false;
            }

            if ($('#<%=ddl2ndAssessor.ClientID%> option:selected').val() == "" || $('#<%=ddl2ndResult.ClientID%> option:selected').val() == "") {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function checkDate(str) {
            if (!isValidDate(str)) {
                return false;
            }

            var dt = new Date(str);
            var today = new Date();
            if (dt > today) {
                return false;
            }

            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>Edit Assessment <i>(By Trainee)</i></h3>
                <small>Please fill up the following.</small>
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
            <div class="col-lg-3">
                <asp:Label ID="lb1" runat="server" Text="Trainee ID" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbTraineeId" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
            <div class="col-lg-9">
                <asp:Label ID="lb2" runat="server" Text="Trainee Name" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbTraineeName" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
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
                <asp:Label ID="lbModuleCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
            <div class="col-lg-9">
                <asp:Label ID="lb17" runat="server" Text="Module" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModule" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
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
                <asp:TextBox ID="txt1stAssessDt" runat="server" Text="31-Jun-2017" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                <asp:CustomValidator ID="cv1stDt" runat="server" Display="None" ControlToValidate="txt1stAssessDt" ClientValidationFunction="validate1stDt"
                            ErrorMessage="Invalid 1st assessment date<br>OR 1st assessment date cannot be later than current date" ValidateEmptyText="false"></asp:CustomValidator>
            </div>
            <div class="col-lg-6">
                <asp:Label ID="lb21" runat="server" Text="Assessment Date" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="txt2ndAssessDt" runat="server" Text="" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                <asp:CustomValidator ID="cv2ndDt" runat="server" Display="None" ControlToValidate="txt2ndAssessDt" ClientValidationFunction="validate2ndAssessment" ValidateEmptyText="true"
                            ErrorMessage="Invalid 2nd assessment date<br>OR 2nd assessment date cannot be later than current date<br/>OR 2nd Assessor cannot be empty<br/>OR 2nd assessment result cannot be empty" ></asp:CustomValidator>
            </div>
        </div>
        <br />
        <div class="row text-left">
            <div class="col-lg-6">
                <asp:Label ID="lb22" runat="server" Text="Assessor" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="ddl1stAssessor" CssClass="form-control" runat="server" DataTextField="userName" DataValueField="userId">
                </asp:DropDownList>
            </div>
            <div class="col-lg-6">
                <asp:Label ID="lb23" runat="server" Text="Assessor" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="ddl2ndAssessor" CssClass="form-control" runat="server" DataTextField="userName" DataValueField="userId">
                </asp:DropDownList>
            </div>
        </div>
        <br />
        <div class="row text-left">
            <div class="col-lg-6">
                <asp:Label ID="lb24" runat="server" Text="Result" Font-Bold="true"></asp:Label><br />
                <asp:DropDownList ID="ddl1stResult" CssClass="form-control" runat="server">
                    <asp:ListItem Value="">--Select--</asp:ListItem>
                    <asp:ListItem Value="C">Competent</asp:ListItem>
                    <asp:ListItem Value="NYC">Not Yet Competent</asp:ListItem>
                </asp:DropDownList>
            </div>         
            <div class="col-lg-6">
                <asp:Label ID="lb25" runat="server" Text="Result" Font-Bold="true"></asp:Label><br />
                <asp:DropDownList ID="ddl2ndResult" CssClass="form-control" runat="server">
                    <asp:ListItem Value="">--Select--</asp:ListItem>
                    <asp:ListItem Value="C">Competent</asp:ListItem>
                    <asp:ListItem Value="NYC">Not Yet Competent</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <br />
        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnSave" runat="server" class="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
                <button type="button" class="btn btn-default" data-toggle="modal" data-target="#diagReload">Clear</button>
                
            </div>
        </div>
        <br />
    </div>

    <div id="diagReload" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Discard Changes</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to discard all changes?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnClear" runat="server" class="btn btn-primary" Text="Clear" CausesValidation="false" OnClick="btnClear_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
