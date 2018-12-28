<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="session-edit.aspx.cs" Inherits="ACI_TMS.session_edit" %>

<%@ Register Src="~/venue-search.ascx" TagPrefix="uc1" TagName="venuesearch" %>

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

        function checkSessionDate() {
            if ($('#<%=tbDate.ClientID%>').val().length == 0 || !isValidDate($('#<%=tbDate.ClientID%>').val())) return false;

            if (!isValidDate($('#<%=tbDate.ClientID%>').val())) return false;

            var startDt = new Date('<%=lbBatchStartDate.Text%>');
            var endDt = new Date('<%=lbBatchEndDate.Text%>');
            var sessDt = new Date($('#<%=tbDate.ClientID%>').val());
            if (sessDt < startDt || sessDt > endDt) {
                return false;
            }

            if (sessDt < new Date()) {
                return false;
            }

            return true;
        }

        function validateSession(oSrc, args) {
            if (checkSessionDate()) {
                args.IsValid = true;
                return true;
            } else {
                args.IsValid = false;
                return false;
            }
        }

        function checkSessionInput() {
            if ($('#<%=tbDate.ClientID%>').val().length == 0 || $('#<%=ddlPeriod.ClientID%> option:selected').val().length == 0) {
                $('#lblError').html("Session date and/or period cannot be empty.");
                $('#panelError').removeClass("hidden");
                return;
            }

            if (!isValidDate($('#<%=tbDate.ClientID%>').val())) {
                $('#lblError').html("Invalid session date.");
                $('#panelError').removeClass("hidden");
                return;
            }

            //check if within batch dates and after current date
            if (!checkSessionDate()) {
                $('#lblError').html("Invalid session date OR Session date cannot be earlier or later than commencement date or earlier than today.");
                $('#panelError').removeClass("hidden");
                return;
            }

            $('#diagSearchVenue').modal('show');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbEditSingleSession" runat="server" Text="Edit Session"></asp:Label>
                </h3>
                <small>Please update the following as needed</small>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click" CausesValidation="false"></asp:LinkButton>
            </div>
        </div>
        <asp:HiddenField ID="hfSessionId" runat="server" />
        <hr />

        <a name="session" />
        <div class="alert alert-danger hidden" id="panelError">
            <a href="#session" class="close" onclick="closeErrorAlert();">&times;</a>
            <span id="lblError" class="alert-link"></span>
        </div>
        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger" id="panelSysError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSysError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />

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
                                        <div class="col-lg-6">
                                            <asp:Label ID="lb1" runat="server" Text="Category" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeCategory" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                        </div>

                                        <div class="col-lg-6">
                                            <asp:Label ID="lb2" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeLevel" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                        </div>
                                    </div>

                                    <br />

                                    <div class="row">
                                        <div class="col-lg-10">
                                            <asp:Label ID="lb3" runat="server" Text="Programme" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgramme" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                        </div>

                                        <div class="col-lg-2">
                                            <asp:Label ID="lb4" runat="server" Text="Version" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeVersion" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                        </div>
                                    </div>

                                    <br />

                                    <div class="row">
                                        <div class="col-lg-6">
                                            <asp:Label ID="lb5" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                        </div>

                                        <div class="col-lg-6">
                                            <asp:Label ID="lb6" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                                            <asp:Label ID="lbProgrammeType" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
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
                                        <asp:Label ID="lbBatchCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:Label ID="lb8" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbBatchType" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:Label ID="lb9" runat="server" Text="Project Code" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbProjCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-lg-6">
                                        <asp:Label ID="lb10" runat="server" Text="Registration Date" Font-Bold="true"></asp:Label>

                                        <div class="input-group">
                                            <asp:Label ID="lbRegStartDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                            <span class="input-group-addon" style="font-weight: bold;">to</span>
                                            <asp:Label ID="lbRegEndDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <asp:Label ID="lb11" runat="server" Text="Commencement Date" Font-Bold="true"></asp:Label>

                                        <div class="input-group">
                                            <asp:Label ID="lbBatchStartDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                            <span class="input-group-addon" style="font-weight: bold;">to</span>
                                            <asp:Label ID="lbBatchEndDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-lg-6">
                                        <asp:Label ID="lb12" runat="server" Text="Capacity" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbCapacity" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:Label ID="lb13" runat="server" Text="Mode" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbClassMode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%------------------------------------ END Class Details ------------------------------------%>
                </div>
            </div>
        </div>

        <fieldset>
            <legend>Module Details</legend>
            <div class="row">
                <div class="col-lg-9">
                    <asp:Label ID="lb14" runat="server" Text="Module" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModule" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lb15" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModuleCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb16" runat="server" Text="Day" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbDay" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
                <div class="col-lg-6">
                    <asp:Label ID="lb17" runat="server" Text="Date" Font-Bold="true"></asp:Label>&nbsp;
                    <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" data-placement="top" title="Date will be automatically adjusted by system depending on the session dates."></i>

                    <div class="input-group">
                        <asp:Label ID="lbModDtFrm" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                        <span class="input-group-addon" style="font-weight: bold;">to</span>
                        <asp:Label ID="lbModDtTo" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                        <asp:HiddenField ID="hfModDtFrm" runat="server" />
                        <asp:HiddenField ID="hfModDtTo" runat="server" />
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-lg-4">
                    <asp:Label ID="lb18" runat="server" Text="Trainer 1" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbTrainer1" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
                <div class="col-lg-4">
                    <asp:Label ID="lb19" runat="server" Text="Trainer 2" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbTrainer2" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
                <div class="col-lg-4">
                    <asp:Label ID="lb20" runat="server" Text="Assessor" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbAssessor" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
            </div>
        </fieldset>

        <br />

        <fieldset>
            <legend>Session Details</legend>
            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb21" runat="server" Text="Date" Font-Bold="true"></asp:Label>
                    <asp:TextBox ID="tbDate" runat="server" CssClass="datepicker form-control" placeholder="dd MMM yyyy"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="tfvDate" runat="server" Display="None" ControlToValidate="tbDate" ErrorMessage="Date cannot be empty."></asp:RequiredFieldValidator>
                </div>
                <div class="col-lg-6">
                    <asp:Label ID="lb22" runat="server" Text="Period" Font-Bold="true"></asp:Label>
                    <asp:DropDownList ID="ddlPeriod" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvPeriod" runat="server" Display="None" ControlToValidate="ddlPeriod" ErrorMessage="Period cannot be empty."></asp:RequiredFieldValidator>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-12">
                    <asp:Label ID="lb23" runat="server" Text="Venue" Font-Bold="true"></asp:Label>&nbsp;<asp:Label ID="lbVenueAva" runat="server"></asp:Label>
                    <div class="inner-addon right-addon">
                        <i class="glyphicon glyphicon-search" style="cursor: pointer;" onclick="checkSessionInput();"></i>
                        <asp:TextBox ID="tbVenue" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        <asp:HiddenField ID="hfVenueId" runat="server" />
                    </div>
                    <asp:RequiredFieldValidator ID="rfvVenue" runat="server" Display="None" ControlToValidate="tbVenue" ErrorMessage="Venue cannot be empty."></asp:RequiredFieldValidator>
                </div>
            </div>
        </fieldset>

        <br />
        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" CausesValidation="true" OnClick="btnSave_Click" />
                <asp:CustomValidator ID="cv" Display="None" ControlToValidate="tbDate" ClientValidationFunction="validateSession" runat="server" 
                    ErrorMessage="Invalid session date OR Session date is not within commencement date or earlier than today."></asp:CustomValidator>
            </div>
        </div>

    </div>

    <uc1:venuesearch runat="server" ID="venuesearch" />
</asp:Content>
