<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="aci-suspended-list.aspx.cs" Inherits="ACI_TMS.aci_suspended_list" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/WebForms/MSAjax/MicrosoftAjax.js"></script>
    <script src="js/WebForms/MSAjax/MicrosoftAjaxWebForms.js"></script>

    <link href="Content/custom/pagination.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="row text-center">
                    <div class="col-lg-12">
                        <h2>
                            <asp:Label ID="lbSuspendedHeader" runat="server" Text="Suspended List"></asp:Label>
                        </h2>

                        <asp:Label ID="lbSubHeader" runat="server" Text="Suspended applicants from SSG and ACI"></asp:Label>
                        <br />
                        <br />

                    </div>
                </div>

                <hr />

                <div class="row" id="panel_operations" runat="server">

                    <div class="col-lg-9 col-md-9 col-sm-12">
                    </div>

                    <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="panel panel-default">
                            <div id="listHeader" class="panel-heading">Operations</div>
                            <div class="panel-body">
                                <p>
                                    <asp:LinkButton runat="server" ID="btnAdd" CausesValidation="false" OnClick="btnAddSuspend_Click"><i class='fa fa-plus'></i>New Suspend Case</asp:LinkButton>
                                </p>

                            </div>
                        </div>
                    </div>
                </div>

                <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link">Record removed.</asp:Label>
                </div>
                <br />

                <div class="row">
                    <div class="form-group form-inline">
                        <div class="col-lg-12">
                            <label>Search:</label>


                            <asp:TextBox ID="tbSearchApplicant" runat="server" CssClass="form-control" placeholder="NRIC or Name"></asp:TextBox>

                            <asp:Button ID="btnSearchApplicant" runat="server" class="btn btn-default" Text="Search" OnClick="btnSearchApplicant_Click" />
                        </div>

                    </div>
                </div>
                <br />

                <%-- Suspended List --%>
                <div class="row">
                    <div class="col-lg-12">


                        <asp:GridView ID="gvSuspended" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="True"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="True" PageSize="5"
                            OnPageIndexChanging="gvSuspended_PageIndexChanging">

                            <Columns>

                                <asp:BoundField DataField="idNumber" HeaderText="NRIC">
                                    <ItemStyle Width="40px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="FullName" HeaderText="Full Name">
                                    <ItemStyle Width="150px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="byOrganization" HeaderText="Suspended By">
                                    <ItemStyle Width="20px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="startDate" DataFormatString="{0:dd-MMM-yyyy}" HeaderText="Start Date">
                                    <ItemStyle Width="40px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="endDate" DataFormatString="{0:dd-MMM-yyyy}" HeaderText="Start Date">
                                    <ItemStyle Width="40px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="suspendedRemarks" HeaderText="Remarks">
                                    <ItemStyle Width="215px" />
                                </asp:BoundField>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkBtnUnsuspend" runat="server" OnCommand="lnkDelete" CommandArgument='<%# Eval("suspendId") %>' CssClass="btn btn-warning"> <i class="fa fa-times" aria-hidden="true"></i> Unsuspend </asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle Width="15px" />
                                </asp:TemplateField>



                            </Columns>



                            <PagerStyle CssClass="pagination-ys" />

                        </asp:GridView>


                    </div>
                </div>

                <%--<!-- Modal -->
                <button id="openModel" type="button" class="hidden" data-toggle="modal" data-target="#myModal"></button>

                <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <button type="button" class="close" id="closeModel" data-dismiss="modal" aria-hidden="true">&times;</button>
                                <h4 class="modal-title" id="myModalLabel">Suspension Record</h4>
                            </div>
                            <div class="modal-body">

                                <div class="row">
                                    <div class="col-lg-6">
                                    </div>

                                    <div class="col-lg-6 text-right">
                                    </div>
                                </div>

                                <br />

                                <div class="row">
                                    <div class="col-lg-3">
                                        <asp:Label ID="lbNRIC" runat="server" Text="NIRC" Font-Bold="true"> </asp:Label>
                                    </div>
                                    <div class="col-lg-9">
                                        <asp:TextBox ID="tbNRICValue" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>

                                <br />

                                <div class="row">
                                    <div class="col-lg-3">
                                        <asp:Label ID="lbName" runat="server" Text="Name" Font-Bold="true"> </asp:Label>
                                    </div>
                                    <div class="col-lg-9">
                                        <asp:TextBox ID="tbNameValue" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>

                                <br />

                                <div class="row">
                                    <div class="col-lg-3">
                                        <asp:Label ID="lbRemarks" runat="server" Text="Remarks" Font-Bold="true"> </asp:Label>
                                    </div>
                                    <div class="col-lg-9">
                                        <asp:TextBox ID="tbRemarksValue" runat="server" CssClass="form-control" Rows="5" TextMode="MultiLine" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>

                                <br />



                            </div>

                            <div class="modal-footer">

                                <asp:Button ID="btnRemoveSuspend" OnClick="btnRemoveSuspend_Click" CssClass="btn btn-primary" runat="server" Text="Unsuspend" />
                            </div>
                        </div>
                        <!-- /.modal-content -->
                    </div>
                    <!-- /.modal-dialog -->
                </div>--%>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
