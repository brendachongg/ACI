<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="bundle-creation.aspx.cs" Inherits="ACI_TMS.bundle_creation" %>

<%@ Register Src="~/module-search.ascx" TagPrefix="uc1" TagName="modulesearch" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function confirmDel(moduleId) {
            $('#<%=hdSelModuleId.ClientID%>').val(moduleId);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbBundleCreationHeader" runat="server" Font-Bold="true" Text="New Bundle"></asp:Label>
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
        <asp:ValidationSummary ID="vSummary1" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="module" />
        <asp:ValidationSummary ID="vSummary2" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="bundle" />

        <div class="row">
            <div class="col-lg-3">
                <asp:Label ID="lbBundleCode" runat="server" Text="Bundle Code" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbBundleCode" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvBundleCode" runat="server" ErrorMessage="Bundle code cannot be empty." Display="None" ControlToValidate="tbBundleCode" 
                    ValidationGroup="bundle"></asp:RequiredFieldValidator>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbl2" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                <asp:DropDownList ID="ddlBundleType" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvBundleType" runat="server" ErrorMessage="Bundle type cannot be empty." Display="None" ControlToValidate="ddlBundleType" 
                    ValidationGroup="bundle"></asp:RequiredFieldValidator>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbl1" runat="server" Text="Effective Date" Font-Bold="true"></asp:Label>&nbsp;
                <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" data-placement="top" title="Automatically populated by latest selected modules' effective date"></i>
                <asp:TextBox ID="lbEffectiveDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lb7" runat="server" Text="Cost" Font-Bold="true"></asp:Label>&nbsp;
                <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" data-placement="top" title="The sum of all selected module's cost"></i>
                <asp:TextBox ID="lbTotalCost" runat="server" CssClass="form-control" ReadOnly="true" Text="0"></asp:TextBox>
            </div>
        </div>

        <br />

        <fieldset>
            <legend style="font-size: 18px;">Module Details</legend>

            <div class="row">
                <div class="col-lg-3">
                    <asp:Label ID="lbModuleCode" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                    <div class="inner-addon right-addon">
                        <i class="glyphicon glyphicon-search" data-toggle="modal" data-target="#diagSearchModule" style="cursor: pointer;" id="lnkbtnSearchModule" runat="server"></i>
                        <asp:TextBox ID="tbModuleCode" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                    <asp:RequiredFieldValidator ID="rfvModuleCode" runat="server" ErrorMessage="Module cannot be empty." Display="None" ControlToValidate="tbModuleCode" ValidationGroup="module"></asp:RequiredFieldValidator>
                </div>

                <div class="col-lg-7">
                    <asp:Label ID="lb2" runat="server" Text="Module" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModule" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>

                <div class="col-lg-2">
                    <asp:Label ID="lb3" runat="server" Text="Version" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModuleVersion" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
            </div>

            <br />

            <div class="row">
                <div class="col-lg-3">
                    <asp:Label ID="lbNumOfSession" runat="server" Text="No. Of Session" Font-Bold="true"></asp:Label>&nbsp;
                    <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" data-placement="top" title="Please note that longer processing time may be required for bigger number of sessions."></i>
                    <asp:TextBox ID="tbNumOfSession" runat="server" CssClass="form-control" MaxLength="3"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNumOfSession" runat="server" ErrorMessage="Number of session cannot be empty." Display="None" ControlToValidate="tbNumOfSession" ValidationGroup="module"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revNumOfSession" runat="server" ErrorMessage="Number of session must be positive whole number, greater than zero" Display="None" ControlToValidate="tbNumOfSession" ValidationGroup="module" ValidationExpression="^[1-9]\d*$" ></asp:RegularExpressionValidator>
                </div>

                <div class="col-lg-2">
                    <asp:Label ID="lb4" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModuleLevel" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>

                <div class="col-lg-2">
                    <asp:Label ID="lb5" runat="server" Text="Training Hours" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModuleTrainingHour" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>

                <div class="col-lg-3">
                    <asp:Label ID="lb6" runat="server" Text="Effective Date" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModuleEffectiveDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>

                <div class="col-lg-2">
                    <asp:Label ID="lb8" runat="server" Text="Cost" Font-Bold="true"></asp:Label>
                    <asp:Label ID="lbModuleCost" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                </div>
            </div>

            <br />

            <div class="row text-right">
                <div class="col-lg-12">
                    <asp:Button ID="btnSaveModule" runat="server" Text="Add" CssClass="btn btn-info" OnClick="btnSaveModule_Click" ValidationGroup="module"/>
                    <asp:Button ID="btnClearModule" runat="server" Text="Clear" CssClass="btn btn-default" OnClick="btnClearModule_Click" CausesValidation="false" />
                </div>
            </div>
        </fieldset>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvModule" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" OnRowCommand="gvModule_RowCommand">

                    <EmptyDataTemplate>
                        No available modules
                    </EmptyDataTemplate>

                    <Columns>
                        <asp:BoundField DataField="moduleId" />
                        <asp:TemplateField HeaderText="Order" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lbSpace" runat="server">&nbsp;</asp:Label><asp:LinkButton ID="lbtnUp" runat="server" CssClass="glyphicon glyphicon-arrow-up" style="cursor:pointer;text-decoration:none;" CommandName="moveUp" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton>  
                                <asp:Label ID="lbNewLine" runat="server"><br /></asp:Label>
                                <asp:LinkButton ID="lbtnDown" runat="server" CssClass="glyphicon glyphicon-arrow-down" style="cursor:pointer;text-decoration:none;" CommandName="moveDown" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Module Code" DataField="moduleCode" ItemStyle-Width="200px" />
                        <asp:BoundField HeaderText="Version" DataField="moduleVersion" ItemStyle-Width="80px" />
                        <asp:BoundField HeaderText="Title" DataField="moduleTitle" />
                        <asp:BoundField HeaderText="Effective Date" DataField="moduleEffectDate" ItemStyle-Width="150px" />
                        <asp:BoundField HeaderText="Cost" DataField="moduleCost" ItemStyle-Width="100px" />
                        <asp:BoundField HeaderText="Num. Of Session" DataField="ModuleNumOfSession" ItemStyle-Width="80px" />

                        <asp:TemplateField HeaderText="" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:Button runat="server" CssClass="btn btn-info" Text="Edit" CommandName="editModule" CommandArgument='<%# Eval("moduleId") %>' CausesValidation="false" />
                                <button type="button" class="btn btn-info" onclick='confirmDel(<%# Eval("moduleId") %>)' data-toggle="modal" data-target="#diagRemModule">Remove</button>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />

                </asp:GridView>
            </div>
        </div>
        <hr />
        <br />
        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnCreateBundle" runat="server" CssClass="btn btn-primary" Text="Create" ValidationGroup="bundle" OnClick="btnCreateBundle_Click" />
                <asp:Button ID="btnClearBundle" runat="server" CssClass="btn btn-default" Text="Clear" CausesValidation="false" OnClick="btnClearBundle_Click" />
            </div>
        </div>

    </div>

    <asp:HiddenField ID="hdSelModuleId" runat="server" />
    <div id="diagRemModule" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Remove Module</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to remove selected module from bundle?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnRemMod" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnRemMod_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <uc1:modulesearch runat="server" ID="modulesearch" />
</asp:Content>
