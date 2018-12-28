<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="absentee-makeup-edit.aspx.cs" Inherits="ACI_TMS.absentee_makeup_edit" %>

<%@ Register Src="~/module-search.ascx" TagPrefix="uc1" TagName="modulesearch" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.collapse').on('shown.bs.collapse', function () {
                $(this).parent().find(".glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-minus");
            }).on('hidden.bs.collapse', function () {
                $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-plus");
            });
        });

        //will only work if is select one radio button in whole page
        function SelectSingleRadiobutton(rdbtnid) {
            var rdBtn = document.getElementById(rdbtnid);
            var rdBtnList = document.getElementsByTagName("input");
            for (i = 0; i < rdBtnList.length; i++) {
                if (rdBtnList[i].type == "radio" && rdBtnList[i].id != rdBtn.id) {
                    rdBtnList[i].checked = false;
                }
            }
        }

        function validateSelectedSession(oSrc, args) {
            var cnt = $('#<%=hfSessionCnt.ClientID%>').val();

            for (var i = 0; i < cnt; i++) {
                if (document.getElementById('<%=gvNewSessions.ClientID%>_rbSession_' + i).checked) {
                    args.IsValid = true;
                    return true;
                }
            }

            args.IsValid = false;
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbAbsenteeDetailsHeader" runat="server" Text="Arrange Make-up"></asp:Label>
                </h3>

                <small>Please fill up the following</small>
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
        <asp:HiddenField ID="hfSessionId" runat="server" />
        <div class="row">
            <div class="col-lg-3">
                <asp:Label ID="lb1" runat="server" Text="Trainee ID " Font-Bold="true"></asp:Label>
                <asp:Label ID="lbTraineeId" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
             <div class="col-lg-9">
                <asp:Label ID="lb2" runat="server" Text="Name " Font-Bold="true"></asp:Label>
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
                                            <asp:Label ID="lb8" runat="server" Text="Type" Font-Bold="true"></asp:Label>
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
                                            <asp:Label ID="lb5" runat="server" Text="Programme" Font-Bold="true"></asp:Label>
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
                                        <asp:Label ID="lbBatchCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                        <asp:HiddenField ID="hfprogrammeBatchId" runat="server" />
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:Label ID="lb10" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbBatchType" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:Label ID="lb11" runat="server" Text="Project Code" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbProjCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-lg-6">
                                        <asp:Label ID="lb12" runat="server" Text="Registration Date" Font-Bold="true"></asp:Label>

                                        <div class="input-group">
                                            <asp:Label ID="lbRegStartDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                            <span class="input-group-addon" style="font-weight: bold;">to</span>
                                            <asp:Label ID="lbRegEndDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <asp:Label ID="lb13" runat="server" Text="Commencement Date" Font-Bold="true"></asp:Label>

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
                                        <asp:Label ID="lb14" runat="server" Text="Capacity" Font-Bold="true"></asp:Label>
                                        <asp:Label ID="lbCapacity" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:Label ID="lb15" runat="server" Text="Mode" Font-Bold="true"></asp:Label>
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

        <br />

        <fieldset>
            <legend>Session Details</legend>
            <div class="row">
                <div class="col-lg-9">
                    <asp:Label ID="lb16" runat="server" Text="Module" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModule" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                    <asp:HiddenField ID="hfModuleId" runat="server" />
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lb17" runat="server" Text="Module Code" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModuleCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
            </div>

            <br />

            <div class="row">
            <div class="col-lg-6">
                <asp:Label ID="lb18" runat="server" Text="Session Date/Period " Font-Bold="true"></asp:Label>
                <asp:Label ID="lbSession" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                <asp:HiddenField ID="hfSessionDt" runat="server" /><asp:HiddenField ID="hfSessionPeriod" runat="server" />
            </div>

            <div class="col-lg-6">
                <asp:Label ID="lb19" runat="server" Text="Venue " Font-Bold="true"></asp:Label>
                <asp:Label ID="lbVenue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>
        </fieldset>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:Label ID="lb20" runat="server" Text="Reason " Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbReason" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:CheckBox ID="cbValid" runat="server" Text="&nbsp;Is absence valid?" OnCheckedChanged="cbValid_CheckedChanged" AutoPostBack="true" />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lbMakeupMsg" runat="server" Font-Italic="true" ForeColor="Red">**Make-up cannot be schedule with non valid absence and payment.</asp:Label>
                <asp:HiddenField ID="hfAbsPayment" runat="server" />
            </div>
        </div>

        <br />

        <fieldset id="panelMakeup" runat="server" visible="false">
            <legend>Make-up Session</legend>
            <div class="row">
                <div class="col-lg-9">
                    <asp:Label ID="lb22" runat="server" Text="Module" Font-Bold="true"></asp:Label>
                    <div class="inner-addon right-addon">
                        <i class="glyphicon glyphicon-search" data-toggle="modal" data-target="#diagSearchModule" style="cursor: pointer;" id="lnkbtnSearchModule" runat="server"></i>
                        <asp:TextBox ID="tbNewModule" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>   
                    <asp:HiddenField ID="hfNewModuleId" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvModule" runat="server" ErrorMessage="Module cannot be empty." ControlToValidate="tbNewModule" Display="None"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lb21" runat="server" Text="Module Code" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbNewModuleCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>  
            </div>

            <br />

            <div class="row">
                <div class="col-lg-12">
                    <asp:Label ID="lb24" runat="server" Text="Class/Programme" Font-Bold="true"></asp:Label>
                    <%-- consist of some programme info also --%>
                    <asp:DropDownList ID="ddlNewBatch" runat="server" CssClass="form-control" CausesValidation="false" OnSelectedIndexChanged="ddlNewBatch_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvBatch" runat="server" ErrorMessage="Class cannot be empty." ControlToValidate="ddlNewBatch" Display="None"></asp:RequiredFieldValidator>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-12">
                    <asp:GridView ID="gvNewSessions" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                        CssClass="table table-striped table-bordered dataTable no-footer hover gvv">
                        <Columns>
                            <asp:BoundField HeaderText="Session ID" DataField="sessionId" />
                            <asp:BoundField HeaderText="Batch ID" DataField="batchModuleId" />
                            <asp:BoundField HeaderText="Period" DataField="sessionPeriod" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:RadioButton ID="rbSession" name="sessions" runat="server" OnClick="javascript:SelectSingleRadiobutton(this.id)" />
                                </ItemTemplate>
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Session No." DataField="sessionNumber" ItemStyle-Width="50px" />
                            <asp:BoundField HeaderText="Date" DataField="sessionDateDisp" ItemStyle-Width="150px" />
                            <asp:BoundField HeaderText="Period" DataField="sessionPeriodDisp" ItemStyle-Width="100px" />
                            <asp:BoundField HeaderText="Venue" DataField="venueLocation"/>
                        </Columns>
                    </asp:GridView>
                    <asp:CustomValidator ID="cvSession" runat="server" Display="None" ControlToValidate="hfSessionCnt" ClientValidationFunction="validateSelectedSession"
                        ErrorMessage="Must select 1 session" ValidateEmptyText="false"></asp:CustomValidator>
                    <asp:TextBox ID="hfSessionCnt" runat="server" style="display:none;" />
                    <asp:HiddenField ID="hfOriMakeupSessionId" runat="server" />
                </div>
            </div>
        </fieldset>

        <br /><br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>

    <uc1:modulesearch runat="server" ID="modulesearch" />
</asp:Content>
