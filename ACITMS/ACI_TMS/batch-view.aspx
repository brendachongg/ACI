<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="batch-view.aspx.cs" Inherits="ACI_TMS.batch_view" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function setSelValue(hf, val) {
            $('#' + hf).val(val);
        }

        function showModule(tabId) {
            $("#" + tabId).attr("class", "tab-pane fade in active");
            $('.nav-tabs a[href="#' + tabId + '"]').tab('show');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbBatchViewHeader" runat="server" Font-Bold="true" Text="View Class"></asp:Label>
                </h3>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click"></asp:LinkButton>
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
        <asp:HiddenField ID="hfBatchId" runat="server" />

        <fieldset>
            <legend style="font-size: 18px;">Programme Details</legend>
            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb1" runat="server" Text="Category" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgrammeCategory" runat="server" CssClass="form-control"></asp:Label>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lb2" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgrammeLevel" runat="server" CssClass="form-control"></asp:Label>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-10">
                    <asp:Label ID="lb3" runat="server" Text="Programme" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgramme" runat="server" CssClass="form-control"></asp:Label>
                </div>

                <div class="col-lg-2">
                    <asp:Label ID="lb4" runat="server" Text="Version" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgrammeVersion" runat="server" CssClass="form-control"></asp:Label>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-6">
                    <asp:Label ID="lb5" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgrammeCode" runat="server" CssClass="form-control"></asp:Label>
                </div>

                <div class="col-lg-6">
                    <asp:Label ID="lb6" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbProgrammeType" runat="server" CssClass="form-control"></asp:Label>
                </div>
            </div>
        </fieldset>

        <br />

        <fieldset>
            <legend style="font-size: 18px;">Class Details</legend>

            <div class="row">
                <div class="col-lg-9">
                    <asp:Label ID="lb7" runat="server" Text="Class Code" Font-Bold="true"></asp:Label>
                    <div class="input-group">
                        <asp:Label ID="lbBatchCode" runat="server" CssClass="form-control"></asp:Label>
                        <span class="input-group-addon" style="font-weight: bold;">-</span>
                        <asp:Label ID="lbClsType" runat="server" CssClass="form-control"></asp:Label>
                    </div>
                </div>
                <div class="col-lg-3">
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
                    <asp:Label ID="lbMode" runat="server" CssClass="form-control"></asp:Label>
                </div>
            </div>
        </fieldset>

        <br />

        <fieldset>
            <legend style="font-size: 18px;">Session Details</legend>

            <asp:HiddenField ID="hfSelModule" runat="server" />
            <asp:Repeater ID="rpModuleTabs" runat="server">
                <HeaderTemplate>
                    <ul class="nav nav-tabs">
                </HeaderTemplate>
                <ItemTemplate>
                    <li id="tabMod" runat="server"><a data-toggle="tab" onclick="setSelValue('<%=hfSelModule.ClientID %>', '<%# Eval("moduleId") %>');"
                        href='#<%# Eval("moduleId") %>'><%# Eval("moduleCode") %></a></li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>

            <asp:Repeater ID="rpModuleContent" runat="server" OnItemDataBound="rpModuleContent_ItemDataBound">
                <HeaderTemplate>
                    <div class="tab-content">
                </HeaderTemplate>
                <ItemTemplate>
                    <div id='<%# Eval("moduleId") %>' class="tab-pane fade">
                        <asp:HiddenField ID="hfModuleId" runat="server" Value='<%# Eval("moduleId") %>' />
                        <br />
                        <h4><b>
                            <asp:Label ID="lbModuleTitle" runat="server"><%# Eval("moduleTitle") %></asp:Label></b></h4>
                        <br />
                        <div class="row">
                            <div class="col-lg-6">
                                <asp:Label ID="lb1" runat="server" Text="Day" Font-Bold="true"></asp:Label>
                                <asp:Label ID="lbDay" runat="server" CssClass="form-control"><%# Eval("dayDisp") %></asp:Label>
                            </div>
                            <div class="col-lg-6">
                                <asp:Label ID="lb2" runat="server" Text="Date" Font-Bold="true"></asp:Label>

                                <div class="input-group">
                                    <asp:Label ID="lbDtFrm" runat="server" CssClass="form-control"><%# Eval("startDateDisp") %></asp:Label>
                                    <span class="input-group-addon" style="font-weight: bold;">to</span>
                                    <asp:Label ID="lbDtTo" runat="server" CssClass="form-control"><%# Eval("endDateDisp") %></asp:Label>
                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-lg-4">
                                <asp:Label ID="lb3" runat="server" Text="Trainer 1" Font-Bold="true"></asp:Label>
                                <asp:Label ID="lbTrainer1" runat="server" CssClass="form-control"><%# Eval("trainerUserName1") %></asp:Label>
                            </div>
                            <div class="col-lg-4">
                                <asp:Label ID="lb4" runat="server" Text="Trainer 2" Font-Bold="true"></asp:Label>
                                <asp:Label ID="lbTrainer2" runat="server" CssClass="form-control"><%# Eval("trainerUserName2") %></asp:Label>
                            </div>
                            <div class="col-lg-4">
                                <asp:Label ID="lb5" runat="server" Text="Assessor" Font-Bold="true"></asp:Label>
                                <asp:Label ID="lbAssessor" runat="server" CssClass="form-control"><%# Eval("assessorUserName") %></asp:Label>
                            </div>
                        </div>
                        <br />
                        <h4>Sessions</h4>
                        <asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="false" CssClass="table table-striped table-bordered dataTable no-footer hover gvv">
                            <Columns>
                                <asp:BoundField HeaderText="Session No" DataField="sessionNo" ItemStyle-Width="80px" />
                                <asp:BoundField HeaderText="Date" DataField="sessionDateDisp" ItemStyle-Width="150px" />
                                <asp:BoundField HeaderText="Period" DataField="sessionPeriodDisp" ItemStyle-Width="100px" />
                                <asp:BoundField HeaderText="Venue" DataField="venueLocation" />
                            </Columns>

                            <PagerStyle CssClass="pagination-ys" />

                        </asp:GridView>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </fieldset>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnEdit" runat="server" CssClass="btn btn-primary" Text="Edit" OnClick="btnEdit_Click" />
                <button id="btnConfirmDel" runat="server" type="button" class="btn btn-danger" data-toggle="modal" data-target="#diagDelete">Delete</button>
            </div>
        </div>

    </div>

    <div id="diagDelete" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete Class</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this class? All related information such as venue bookings etc will be removed as well.
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnDelete_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
