<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="access-functions.aspx.cs" Inherits="ACI_TMS.access_functions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="Scripts/jquery-3.1.1.min.js"></script>
    <script>
        function confirmDel(functionId) {
            var hidden = document.getElementById("<%=HiddenFunctionId.ClientID%>");
            if (hidden != null) {
                hidden.value = functionId;
            } else {
                alert("null");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbFunctionHeader" runat="server" Text="Function Management"></asp:Label>
                </h2>
            </div>
        </div>
        <hr />
        <div class="alert alert-success alert-dismissable" id="successDiv" role="alert" runat="server">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>
            <asp:Label ID="lblSuccessMsg" runat="server" Text=""></asp:Label>
        </div>
        <div class="alert alert-danger alert-dismissable" id="failureDiv" role="alert" runat="server">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>
            <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="new" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="edit" />

        <div class="row">
            <div class="col-lg-12">
                <div class="col-lg-16">
                    <div class="panel panel-primary">
                        <div class="panel-heading"><b>New function</b></div>
                    </div>
                </div>
                <div id="newFunctionForm">
                    <div class="row text-left">
                        <div class="col-lg-6">
                            <asp:Label ID="lblGroup" runat="server" Text="Group" Font-Bold="true"></asp:Label>
                            <asp:DropDownList ID="ddlGrp" DataValueField="codeValue" DataTextField="codeValueDisplay" ValidationGroup="new" runat="server" CssClass="form-control"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvGrp" runat="server" ValidationGroup="new" ControlToValidate="ddlGrp" ErrorMessage="Function group cannot be empty" Display="None"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-lg-6">
                            <asp:Label ID="lblFunction" runat="server" Text="Function" Font-Bold="true"></asp:Label>
                            <asp:TextBox ID="tbNewFunction" placeholder="E.g. View Registration" CssClass="form-control" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFunction" runat="server" ErrorMessage="Function name cannot be empty" ControlToValidate="tbNewFunction" ValidationGroup="new" Display="None"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <br />
                    <div class="row text-left">
                        <div class="col-lg-12">
                            <asp:Label ID="lblDesc" runat="server" Text="Description" Font-Bold="true"></asp:Label>
                            <asp:TextBox ID="tbNewDesc" runat="server" placeholder="E.g. To be able to view all registration records" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ValidationGroup="new" ControlToValidate="tbNewDesc" ErrorMessage="Function description cannot be empty" Display="None"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row text-right">
                        <div class="col-lg-12">
                            <br />
                            <asp:Button ID="btnAdd" class="btn btn-info" runat="server" Text="Add" CausesValidation="true" ValidationGroup="new" OnClick="btnAdd_Click" />
                            <asp:Button ID="btnClear" class="btn btn-default" runat="server" Text="Clear" OnClick="btnClear_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-lg-12">
                <div class="col-lg-16">
                    <div class="panel panel-primary">
                        <div class="panel-heading"><b>Manage function</b></div>
                    </div>
                </div>
                <div id="manageFunctionForm">
                    <%-- if you use abbr for the id for the rest of the controls, how come it does not follow the same?! --%>
                    <asp:GridView ID="FunctionsGridView" runat="server" CssClass="table table-striped table-bordered dataTable no-footer hover gvv" 
                        AllowPaging="True" OnPageIndexChanging="FunctionsGridView_PageIndexChanging"
                        PageSize="10" AutoGenerateColumns="False" OnRowCancelingEdit="FunctionsGridView_RowCancelingEdit"
                        OnRowEditing="FunctionsGridView_RowEditing" OnRowUpdating="FunctionsGridView_RowUpdating"
                        DataKeyNames="functionId,codeValue,codeValueDisplay,functionName,functionDesc" OnRowDataBound="FunctionsGridView_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="functionId" HeaderText="functionId" Visible="False" />
                            <asp:BoundField DataField="codeValue" HeaderText="codeValue" Visible="False" />
                            <asp:TemplateField HeaderText="Group" SortExpression="codeValueDisplay">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlEdit" DataValueField="codeValue" DataTextField="codeValueDisplay" runat="server" CssClass="form-control"></asp:DropDownList>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:HiddenField ID="hfGrp" runat="server" Value='<%# Eval("codeValue") %>' />
                                    <%# Eval("codeValueDisplay") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="List Function" SortExpression="functionName">
                                <EditItemTemplate>
                                    <asp:Label ID="lbFunction" runat="server" Text='<%# Bind("functionName") %>' CssClass="form-control" ReadOnly="true"></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <%# Eval("functionName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description" SortExpression="functionDesc">
                                <EditItemTemplate>
                                    <asp:TextBox ID="tbDescription" runat="server" CssClass="form-control" Text='<%# Bind("functionDesc") %>' ></asp:TextBox>
                                     <asp:RequiredFieldValidator ID="rfvEditDesc" runat="server" ErrorMessage="Function description cannot be empty" ValidationGroup="edit" Display="None" ControlToValidate="tbDescription"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <%# Eval("functionDesc") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="False">
                                <EditItemTemplate>
                                    <asp:Button ID="btnUpdate" runat="server" class="btn btn-info" CausesValidation="true" CommandName="Update" Text="Update" ValidationGroup="edit" />&nbsp;
                                    <asp:Button ID="btnCancel" runat="server" class="btn btn-default" CausesValidation="False" CommandName="Cancel" Text="Cancel" />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Button ID="btnEdit" runat="server" class="btn btn-info" CausesValidation="False" CommandName="Edit" Text="Edit" />&nbsp;     
                                     <button type="button" id="openModel" class="btn btn-info" data-toggle="modal" data-target="#confirmDeleteModal" onclick="confirmDel(<%#DataBinder.Eval(Container.DataItem, "functionId") %>)">Remove</button>
                                </ItemTemplate>
                                <HeaderStyle Width="185px" />
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-ys" />
                    </asp:GridView>
                    <asp:HiddenField ID="HiddenFunctionId" runat="server" Value="" />
                </div>


                <!-- Modal -->
                <div class="modal fade" id="confirmDeleteModal" tabindex="-1" role="dialog" aria-hidden="true">
                    <div class="modal-dialog">
                        <!-- Modal content-->
                        <div class="modal-content">
                            <div class="modal-header">
                                <button type="button" class="close" id="closeModel" data-dismiss="modal" aria-hidden="true">&times;</button>
                                <h4 class="modal-title">Remove?</h4>
                            </div>
                            <div class="modal-body">
                                <p>Are you sure you want to remove this function?</p>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnRemove" class="btn btn-danger" runat="server" Text="Remove" OnClick="btnRemove_Click" />
                                <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
