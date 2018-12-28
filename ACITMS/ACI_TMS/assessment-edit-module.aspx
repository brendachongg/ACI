<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="assessment-edit-module.aspx.cs" Inherits="ACI_TMS.assessment_edit_module" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.collapse').on('shown.bs.collapse', function () {
                $(this).parent().find(".glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-minus");
            }).on('hidden.bs.collapse', function () {
                $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-plus");
            });
        });

        function closeErrorAlert() {
            $('#panelError').addClass("hidden");
        }

        function validateInput(oSrc, args) {
            var errStr = validateGridView('<%=gv1stAssessment.ClientID%>');
            if (errStr != "") {
                $('#lblError').html(errStr);
                $('#panelError').removeClass("hidden");
                args.IsValid = false;
                return false;
            }

            errStr = validateGridView('<%=gv2ndAssessment.ClientID%>');
            if (errStr != "") {
                $('#lblError').html(errStr);
                $('#panelError').removeClass("hidden");
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function fillDefDate(asm) {
            var prefix;
            if (asm == 1) prefix = '<%=gv1stAssessment.ClientID%>';
            else prefix = '<%=gv2ndAssessment.ClientID%>';

            var defDtStr = $('#' + prefix + "_txtDefDt").val();
            if (defDtStr == "" || !isValidDate(defDtStr)) {
                $('#lblError').html("Invalid or empty default assessment date");
                $('#panelError').removeClass("hidden");
                return;
            }
             
            var defDt = new Date(defDtStr);
            if (defDt > (new Date())) {
                $('#lblError').html("Default assessment date cannot be later than today.");
                $('#panelError').removeClass("hidden");
                return;
            }

            var batchStart = new Date($('#<%=lbBatchStartDate.ClientID%>').html());
            if (defDt < batchStart) {
                $('#lblError').html("Default assessment date cannot be earlier than class commencement date.");
                $('#panelError').removeClass("hidden");
                return;
            }

            var cnt = $('#<%=hfNoOfTrainees.ClientID%>').val();
            for (i = 0; i < cnt; i++) {
                if ($('#' + prefix + '_txtAssessDt_' + i).prop('disabled')) continue;
                var dtStr = $('#' + prefix + '_txtAssessDt_' + i).val();
                if (dtStr == "") $('#' + prefix + '_txtAssessDt_' + i).val(defDtStr);
            }
        }

        function fillDefResult(asm) {
            var prefix;
            if (asm == 1) prefix = '<%=gv1stAssessment.ClientID%>';
            else prefix = '<%=gv2ndAssessment.ClientID%>';

            var defResult = $('#' + prefix + "_ddlHeadResult option:selected").val();

            var cnt = $('#<%=hfNoOfTrainees.ClientID%>').val();
            for (i = 0; i < cnt; i++) {
                if ($('#' + prefix + '_ddlResult_' + i).prop('disabled')) continue;
                var resStr = $('#' + prefix + '_ddlResult_' + i + ' option:selected').val();
                if (resStr == "") $('#' + prefix + '_ddlResult_' + i).val(defResult).change();
            }
        }

        function validateGridView(prefix) {
            var isEmptyFields = true;
            var isValidAssmentDate = true;
            var isBeforeToday = true;
            var isAfterBatchStart = true;
            var today = new Date();

            var cnt = $('#<%=hfNoOfTrainees.ClientID%>').val();
            var batchStart = new Date($('#<%=lbBatchStartDate.ClientID%>').html());
            for (i = 0; i < cnt; i++) {
                var dtStr = $('#' + prefix + '_txtAssessDt_' + i).val();
                var assessor = $('#' + prefix + '_ddlAssessor_' + i + '  option:selected').val();
                var result = $('#' + prefix + '_ddlResult_' + i + '  option:selected').val();

                if (dtStr.length > 0 && assessor.length > 0 && result.length > 0) {
                    if (!isValidDate(dtStr)) isValidAssmentDate = false;

                    var dt = new Date(dtStr);
                    if (dt > today) isBeforeToday = false;
                    if (dt < batchStart) isAfterBatchStart = false;
                } else if (dtStr.length > 0 || result.length > 0) {
                    isEmptyFields = false;
                }
            }

            var errStr = "";
            if (!isEmptyFields)  errStr += "<li>Assement date, assessor and result cannot be empty.</li>";
            if (!isValidAssmentDate) errStr += "<li>Invalid assessment date.</li>";
            if (!isBeforeToday) errStr += "<li>Assessment date cannot be after today.</li>";
            if (!isAfterBatchStart) errStr += "<li>Assessment date cannot be before class commencement date.</li>";

            return errStr;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>Edit Assessment <i>(By Module)</i></h3>
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
        <div class="alert alert-danger" id="panelSysError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSysError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger hidden" id="panelError">
            <a href="#" class="close" onClick="closeErrorAlert();">&times;</a>
            <span id="lblError" class="alert-link"></span>
        </div>
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
                                            <asp:Label ID="lb1" runat="server" Text="Category" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeCategory" runat="server" CssClass="form-control"></asp:Label>
                                        </div>

                                        <div class="col-lg-3">
                                            <asp:Label ID="lb2" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeLevel" runat="server" CssClass="form-control"></asp:Label>
                                        </div>

                                        <div class="col-lg-3">
                                            <asp:Label ID="lb3" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeType" runat="server" CssClass="form-control"></asp:Label>
                                        </div>

                                        <div class="col-lg-3">
                                            <asp:Label ID="lb4" runat="server" Text="Version" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeVersion" runat="server" CssClass="form-control"></asp:Label>
                                        </div>
                                    </div>

                                    <br />

                                    <div class="row">
                                        <div class="col-lg-3">
                                            <asp:Label ID="lb5" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeCode" runat="server" CssClass="form-control"></asp:Label>
                                        </div>

                                        <div class="col-lg-9">
                                            <asp:Label ID="lb6" runat="server" Text="Programme" Font-Bold="true"></asp:Label>
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
                                        <asp:Label ID="lb7" runat="server" Text="Class" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbBatchCode" runat="server" CssClass="form-control"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:Label ID="lb8" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbBatchType" runat="server" CssClass="form-control"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:Label ID="lb9" runat="server" Text="Project Code" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbProjCode" runat="server" CssClass="form-control"></asp:Label>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-lg-6">
                                        <asp:Label ID="lb10" runat="server" Text="Registration Date" Font-Bold="true"></asp:Label>

                                        <div class="input-group">
                                            <asp:Label ID="lbRegStartDate" runat="server" CssClass="form-control"></asp:Label>
                                            <span class="input-group-addon" style="font-weight: bold;">to</span>
                                            <asp:Label ID="lbRegEndDate" runat="server" CssClass="form-control"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <asp:Label ID="lb11" runat="server" Text="Commencement Date" Font-Bold="true"></asp:Label>

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
                                        <asp:Label ID="lb12" runat="server" Text="Capacity" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbCapacity" runat="server" CssClass="form-control"></asp:Label>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:Label ID="lb13" runat="server" Text="Mode" Font-Bold="true"></asp:Label>
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
                <asp:Label ID="lb14" runat="server" Text="Module Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
            <div class="col-lg-9">
                <asp:Label ID="lb15" runat="server" Text="Module" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModule" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>
        <br />
        <br />
        <div class="row">
            <div class="col-lg-12">
                <ul class="nav nav-tabs">
                    <li class="active"><a data-toggle="tab" href="#firstAttempt"><b>1st Attempt</b></a></li>
                    <li><a data-toggle="tab" href="#secondAttempt"><b>2nd Attempt</b></a></li>
                </ul>
                <div class="tab-content">
                    <br />
                    <div id="firstAttempt" class="tab-pane fade in active">
                        <div class="row">
                            <div class="col-lg-12">
                                <asp:GridView ID="gv1stAssessment" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" OnRowDataBound="gvAssessment_RowDataBound">
                                    <Columns>
                                        <asp:BoundField HeaderText="Trainee ID" DataField="traineeId" ItemStyle-Width="200px" />
                                        <asp:BoundField HeaderText="Trainee Name" DataField="fullName" />
                                        <asp:TemplateField ItemStyle-Width="160px">
                                            <HeaderTemplate>
                                                Assessment Date
                                                <div class="inner-addon right-addon">
                                                    <i class="glyphicon glyphicon-ok" style="cursor: pointer;" onclick="fillDefDate(1);"></i>
                                                    <asp:TextBox ID="txtDefDt" runat="server" Text="" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                                                </div>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtAssessDt" runat="server" Text="" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Assessor">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlAssessor" CssClass="form-control" runat="server" DataTextField="userName" DataValueField="userId">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-Width="230px">
                                            <HeaderTemplate>
                                                Result
                                                <div class="inner-addon right-addon">
                                                    <asp:DropDownList ID="ddlHeadResult" CssClass="form-control" runat="server" style="width:180px">
                                                        <asp:ListItem Value="">--Select--</asp:ListItem>
                                                        <asp:ListItem Value="C">Competent</asp:ListItem>
                                                        <asp:ListItem Value="NYC">Not Yet Competent</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <i class="glyphicon glyphicon-ok" style="cursor: pointer;" onclick="fillDefResult(1);"></i>
                                                </div>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlResult" CssClass="form-control" runat="server">
                                                    <asp:ListItem Value="">--Select--</asp:ListItem>
                                                    <asp:ListItem Value="C">Competent</asp:ListItem>
                                                    <asp:ListItem Value="NYC">Not Yet Competent</asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                    <div id="secondAttempt" class="tab-pane fade">
                        <div class="row">
                            <div class="col-lg-12">
                                <asp:GridView ID="gv2ndAssessment" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv"  OnRowDataBound="gvAssessment_RowDataBound">
                                    <Columns>
                                        <asp:BoundField HeaderText="Trainee ID" DataField="traineeId" ItemStyle-Width="200px" />
                                        <asp:BoundField HeaderText="Trainee Name" DataField="fullName" />
                                        <asp:TemplateField ItemStyle-Width="160px">
                                            <HeaderTemplate>
                                                Assessment Date
                                                <div class="inner-addon right-addon">
                                                    <i class="glyphicon glyphicon-ok" style="cursor: pointer;" onclick="fillDefDate(2);"></i>
                                                    <asp:TextBox ID="txtDefDt" runat="server" Text="" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                                                </div>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtAssessDt" runat="server" Text="" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Assessor">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlAssessor" CssClass="form-control" runat="server" DataTextField="userName" DataValueField="userId">
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-Width="230px">
                                            <HeaderTemplate>
                                                Result
                                                <div class="inner-addon right-addon">
                                                    <asp:DropDownList ID="ddlHeadResult" CssClass="form-control" runat="server" style="width:180px">
                                                        <asp:ListItem Value="">--Select--</asp:ListItem>
                                                        <asp:ListItem Value="C">Competent</asp:ListItem>
                                                        <asp:ListItem Value="NYC">Not Yet Competent</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <i class="glyphicon glyphicon-ok" style="cursor: pointer;" onclick="fillDefResult(2);"></i>
                                                </div>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlResult" CssClass="form-control" runat="server">
                                                    <asp:ListItem Value="">--Select--</asp:ListItem>
                                                    <asp:ListItem Value="C">Competent</asp:ListItem>
                                                    <asp:ListItem Value="NYC">Not Yet Competent</asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-lg-12">
                <span style="background-color:lightpink">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>&nbsp;<i>Trainee has not met attendance, unable to input assessment.</i>
            </div>
        </div>
        <br />
        <div class="row text-right">
            <div class="col-lg-12">
                <asp:CustomValidator ID="cv" runat="server" Display="None" ErrorMessage="Error" ControlToValidate="hfNoOfTrainees" ValidateEmptyText="false" ClientValidationFunction="validateInput"></asp:CustomValidator>
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click"/>
                <button type="button" class="btn btn-default" data-toggle="modal" data-target="#diagReload">Clear</button>
            </div>
        </div>

    </div>
    <asp:TextBox ID="hfNoOfTrainees" runat="server" style="display:none;" />

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
                    <asp:Button ID="btnClear" runat="server" CssClass="btn btn-default" Text="OK" CausesValidation="false" OnClick="btnClear_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
