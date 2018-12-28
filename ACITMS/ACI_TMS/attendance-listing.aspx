<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="attendance-listing.aspx.cs" Inherits="ACI_TMS.attendance_listing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function showOverwrite() {
            $('#diagOverwrite').modal('show');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbAttendanceHeader" runat="server" Text="Attendance"></asp:Label>
                </h3>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click" CausesValidation="false"></asp:LinkButton>
            </div>
        </div>
        <asp:HiddenField ID="hfSessionId" runat="server" />
        <asp:HiddenField ID="hfBatchModuleId" runat="server" />

        <hr />
        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>

        <div class="row">
            <div class="col-lg-3">
                <asp:Label ID="lb1" runat="server" Text="Programme Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgrammeCode" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-9">
                <asp:Label ID="lb2" runat="server" Text="Programme Title" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgramme" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-lg-4">
                <asp:Label ID="lb3" runat="server" Text="Class Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbBatchCode" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-4">
                <asp:Label ID="lb4" runat="server" Text="Class Type" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbBatchType" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-4">
                <asp:Label ID="lb5" runat="server" Text="Class Capacity" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbCapacity" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-3">
                <asp:Label ID="lb6" runat="server" Text="Module Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleCode" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-9">
                <asp:Label ID="lb7" runat="server" Text="Module Title" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModule" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-4">
                <asp:Label ID="lb8" runat="server" Text="Trainer 1" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbTrainer1" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-4">
                <asp:Label ID="lb9" runat="server" Text="Trainer 2" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbTrainer2" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-4">
                <asp:Label ID="lb10" runat="server" Text="Assessor" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbAssessor" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-6">
                <asp:Label ID="lb11" runat="server" Text="Session Date" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbSessionDate" runat="server" CssClass="form-control"></asp:Label>
            </div>
            <div class="col-lg-6">
                <asp:Label ID="lb12" runat="server" Text="Session Period" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbSessionPeriod" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:Label ID="lb13" runat="server" Text="Session Venue" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbVenue" runat="server" CssClass="form-control"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvOri" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" OnRowDataBound="gv_RowDataBound">
                    <Columns>
                        <asp:BoundField HeaderText="Trainee ID" DataField="traineeId" ItemStyle-Width="200px" />
                        <asp:BoundField HeaderText="Name" DataField="fullName" />
                        <asp:TemplateField HeaderText="Absent?" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox ID="cbAbsent" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div id="panelMakeup" runat="server">
            <fieldset>
                <legend>Make up/No SOA</legend>
                <div class="row">
                    <div class="col-lg-12">
                        <asp:GridView ID="gvInserted" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" OnRowDataBound="gv_RowDataBound">
                            <Columns>
                                <asp:BoundField HeaderText="Trainee ID" DataField="traineeId" ItemStyle-Width="200px" />
                                <asp:BoundField HeaderText="Name" DataField="fullName" />
                                <asp:TemplateField HeaderText="Absent?" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbAbsent" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </fieldset>
        </div>

        <br />
        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click"  />
                <asp:Button ID="btnExport" runat="server" CssClass="btn btn-primary" Text="Export" OnClick="btnExport_Click"  />
            </div>
        </div>
    </div>

    <div id="diagOverwrite" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Overwrite</h4>
                </div>
                <div class="modal-body">
                    The following trainee(s) has been schedule for makeup class, makeup class records (including payment records if applicable) will be removed if proceed.
                    <br /><br />
                    <ul>
                        <asp:Label ID="lbTrainees" runat="server" Text="Label"></asp:Label>
                    </ul>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnOverwrite" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnOverwrite_Click"  />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
