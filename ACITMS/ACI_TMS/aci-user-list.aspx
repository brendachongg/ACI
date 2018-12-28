<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="aci-user-list.aspx.cs" Inherits="ACI_TMS.aci_user_list" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function showDelDialog(id) {
            $("#<%=hfSelSub.ClientID%>").val(id);
            $('#diagDelSub').modal('show');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="page-wrapper">
                <%-- <div class="row text-left">
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                        <h2>
                            <asp:Label ID="lbACIUsers" runat="server" Text="ACI Staffs"></asp:Label>
                        </h2>
                    </div>



                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                        <br />
                        <asp:LinkButton ID="lkbAdd" runat="server" CssClass="btn btn-sm btn-primary" CausesValidation="false" OnClick="lkbAdd_Click"><i class='fa fa-plus'></i> Add</asp:LinkButton>
                    </div>


                </div>--%>


                <div class="row text-center">
                    <div class="col-lg-12">
                        <h2>
                            <asp:Label ID="lbACIUsers" runat="server" Text="ACI Users Management"></asp:Label>
                        </h2>

                    </div>
                </div>

                <hr />

                <div class="row" id="panelNewUser" runat="server">

                    <div class="col-lg-9 col-md-9 col-sm-12">
                    </div>

                    <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="panel panel-default">
                            <div id="listHeader" class="panel-heading">Operations</div>
                            <div class="panel-body">
                                <p>
                                    <asp:LinkButton ID="lkbAdd" OnClick="lkbAdd_Click" runat="server" CausesValidation="false"><span class="fa glyphicon-plus"></span>New User</asp:LinkButton>
                                </p>

                            </div>
                        </div>
                    </div>
                </div>

                <br />
                <div class="row text-left">
                    <div class="col-lg-12">

                        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
                            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
                        </div>
                        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
                            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
                        </div>
                    </div>
                </div>
                <asp:Label ID="lbSubHeader" runat="server" Text="Click on the staff for more details"></asp:Label>
                <br />
                <br />

                <div class="row text-left">
                    <div class="col-lg-12">
                        <div class="form-group form-inline">
                            <asp:Label ID="Label1" runat="server" Text="Search: "></asp:Label>

                            <asp:TextBox ID="tbSearchStaff" runat="server" CssClass="form-control" placeholder="NRIC/Name/Email"></asp:TextBox>

                            <asp:Button ID="btnSearchStaff" runat="server" class="btn btn-default" Text="Search" OnClick="btnSearchStaff_Click" />
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12">


                        <asp:GridView ID="gvStaffs" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="True"
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="True" OnRowCommand="gvStaffs_RowCommand" OnRowDataBound="gvStaffs_RowDataBound" OnPageIndexChanging="gvStaffs_PageIndexChanging" OnRowCreated="gvStaffs_RowCreated">
                            <Columns>
                                <asp:TemplateField HeaderText="Identification No.">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnStaffNo" runat="server" CommandName="viewStaffDetails" CommandArgument='<%# Eval("userid") %>'>
                                            <asp:Label ID="lbgvTraineeID" runat="server" Text='<%# Eval("idNumber") %>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="username" HeaderText="Full Name" />
                                <asp:BoundField DataField="UserEmail" HeaderText="Email" />
                                <asp:BoundField DataField="CodeValueDisplay" HeaderText="Employment Type" />
                                <asp:BoundField DataField="contactnumber1" HeaderText="Contact Number" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Label ID="lbtnDelSub" runat="server" CssClass="glyphicon glyphicon-remove" Style="font-size: 20px;"
                                            ForeColor="Red"></asp:Label>

                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle CssClass="pagination-ys" />
                        </asp:GridView>
                    </div>
                </div>

            </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:HiddenField ID="hfSelSub" runat="server" />
    <div id="diagDelSub" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete ACI Staff</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this ACI Staff?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelSub" runat="server" CssClass="btn btn-primary" Text="OK" CausesValidation="false" OnClick="btnDelSub_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
