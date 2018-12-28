<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="subsidy-management.aspx.cs" Inherits="ACI_TMS.subsidy_management" %>

<%@ Register Src="~/programme_search.ascx" TagPrefix="uc1" TagName="programme_search" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.collapse').on('shown.bs.collapse', function () {
                $(this).parent().find(".glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-minus");
            }).on('hidden.bs.collapse', function () {
                $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-plus");
            });
        });

        function validateDate(oSrc, args) {
            var str = args.Value;

            if (!isValidDate(str)) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function showDelDialog(id) {
            $("#<%=hfSelSub.ClientID%>").val(id);
            $('#diagDelSub').modal('show');
        }

        function showNew() {
            $('#newSubDetails').addClass("in");
            $('#newSubIcon').removeClass("glyphicon-plus").addClass("glyphicon-minus");
        }

        function validateValue(oSrc, args) {
            if ($('#<%=ddlType.ClientID%> option:selected').val() == "<%=GeneralLayer.SubsidyType.RATE.ToString()%>") {
                var v = parseFloat(args.Value);
                if (v < 0 || v > 1) {
                    args.IsValid = false;
                    return false;
                }
            }

            args.IsValid = true;
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Subsidy Management"></asp:Label>
                </h2>
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
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="new" />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="edit" />

        <div class="row" id="panelNewSub" runat="server">
            <div class="col-lg-12">
                <div class="panel-group" id="accordion">
                    <div class="panel panel-default">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="panel-title">
                                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#newSubDetails">
                                        <span id="newSubIcon" class="glyphicon glyphicon-plus"></span>
                                        New
                                    </a>
                                </h4>
                            </div>
                            <div id="newSubDetails" class="panel-collapse collapse">
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-3">
                                            <b>
                                                Programme Code&nbsp;
                                                <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" data-placement="top" title="If left empty, subsidy will be applicable to all programmes."></i>
                                            </b>
                                            <div class="inner-addon right-addon">
                                                <i class="glyphicon glyphicon-search" style="cursor: pointer;" data-toggle="modal" data-target="#diagSearchProg" ></i>
                                                <asp:TextBox ID="tbProgCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                                <asp:HiddenField ID="hfProgId" runat="server" />
                                            </div>
                                        </div>
                                        <div class="col-lg-9">
                                            <b>Title</b>
                                            <asp:TextBox ID="tbProgTitle" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <b>Scheme</b>
                                            <asp:TextBox ID="tbScheme" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvScheme" runat="server" ValidationGroup="new" ControlToValidate="tbScheme" ErrorMessage="Scheme cannot be empty" Display="None"></asp:RequiredFieldValidator>
                                        </div>
                                        <div class="col-lg-2">
                                            <b>Effective Date</b>
                                            <asp:TextBox ID="tbDt" runat="server" placeholder="" CssClass="datepicker form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvNewDt" runat="server" ValidationGroup="new" ControlToValidate="tbDt" ErrorMessage="Effective date cannot be empty" Display="None"></asp:RequiredFieldValidator>
                                            <asp:CustomValidator ID="cvNewDt" runat="server" Display="None" ControlToValidate="tbDt" ClientValidationFunction="validateDate"
                                                ErrorMessage="Date is not in dd MMM yyyy format" ValidateEmptyText="false" ValidationGroup="new"></asp:CustomValidator>
                                        </div>
                                        <div class="col-lg-2">
                                            <b>Type</b>
                                            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control" DataTextField="CodeValueDisplay" DataValueField="CodeValue"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvType" runat="server" ValidationGroup="new" ControlToValidate="ddlType" ErrorMessage="Type cannot be empty" Display="None"></asp:RequiredFieldValidator>
                                        </div>
                                        <div class="col-lg-2">
                                            <b>Value</b>
                                            <asp:TextBox ID="tbValue" runat="server" placeholder="" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvValue" runat="server" ValidationGroup="new" ControlToValidate="tbValue" ErrorMessage="Value cannot be empty" Display="None"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="revValue" runat="server" ErrorMessage="Value must be a non negative number, up to 2 decimal places"
                                                Display="None" ControlToValidate="tbValue" ValidationExpression="^\d+(\.\d{1,2})?$" ValidationGroup="new"></asp:RegularExpressionValidator>
                                            <asp:CustomValidator ID="cvValue" runat="server" ControlToValidate="tbValue" ValidationGroup="new" ErrorMessage="Value for percentage must be between 0 and 1" 
                                                ClientValidationFunction="validateValue" Display="None" ValidateEmptyText="false"></asp:CustomValidator>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row text-right">
                                        <div class="col-lg-12">
                                            <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="Add" ValidationGroup="new" OnClick="btnAdd_Click"/>&nbsp;
                                            <asp:Button ID="btnClear" runat="server" CssClass="btn btn-default" Text="Clear" CausesValidation="false" OnClick="btnClear_Click"/>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvSub" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" DataKeyNames="subsidyId"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10" OnRowDataBound="gvSub_RowDataBound"
                    OnPageIndexChanging="gvSub_PageIndexChanging" OnRowEditing="gvSub_RowEditing" OnRowCancelingEdit="gvSub_RowCancelingEdit" OnRowUpdating="gvSub_RowUpdating">
                    <EmptyDataTemplate>
                        No available records
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>Scheme</HeaderTemplate>
                            <ItemTemplate>
                                <%# Eval("subsidyScheme") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Programme</HeaderTemplate>
                            <ItemTemplate>
                                <%# Eval("programmeTitle") %> <%# Eval("programmeCode") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Effective Date</HeaderTemplate>
                            <ItemTemplate>
                                <%# Eval("effectiveDateDisp") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="tbDt" runat="server" placeholder="" CssClass="datepicker form-control" Text='<%# Eval("effectiveDateDisp") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvDt" runat="server" ValidationGroup="edit" ControlToValidate="tbDt" ErrorMessage="Effective date cannot be empty" Display="None"></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="cvDt" runat="server" Display="None" ControlToValidate="tbDt" ClientValidationFunction="validateDate"
                                    ErrorMessage="Date is not in dd MMM yyyy format" ValidateEmptyText="false" ValidationGroup="edit"></asp:CustomValidator>
                            </EditItemTemplate>
                            <ItemStyle Width="150px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Type</HeaderTemplate>
                            <ItemTemplate>
                                <%# Eval("subsidyTypeDisp") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control" DataTextField="CodeValueDisplay" DataValueField="CodeValue"></asp:DropDownList>
                            </EditItemTemplate>
                            <ItemStyle Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>Value</HeaderTemplate>
                            <ItemTemplate>
                                <%# Eval("subsidyValueDisp") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="tbValue" runat="server" placeholder="" CssClass="form-control" Text='<%# Eval("subsidyValue") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValue" runat="server" ValidationGroup="edit" ControlToValidate="tbValue" ErrorMessage="Value cannot be empty" Display="None"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revValue" runat="server" ErrorMessage="Value must be a non negative number, up to 2 decimal places"
                                    Display="None" ControlToValidate="tbValue" ValidationExpression="^\d+(\.\d{1,2})?$" ValidationGroup="edit"></asp:RegularExpressionValidator>
                            </EditItemTemplate>
                            <ItemStyle Width="100px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnEdit" runat="server" CommandName="Edit" CausesValidation="false" CssClass="glyphicon glyphicon-pencil" Style="font-size: 20px; text-decoration: none;"></asp:LinkButton>
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lbtnDelSub" runat="server" CssClass="glyphicon glyphicon-remove" Style="font-size: 20px;"
                                    ForeColor="Red"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton ID="lbtnEdit" runat="server" CommandName="Update" ValidationGroup="edit" CssClass="glyphicon glyphicon-ok" Style="font-size: 20px; text-decoration: none;" ForeColor="Green"></asp:LinkButton>
                                &nbsp;&nbsp;&nbsp;
                                <asp:LinkButton ID="lbtnCancel" runat="server" CommandName="Cancel" CausesValidation="false" CssClass="glyphicon glyphicon-remove" Style="font-size: 20px; text-decoration: none;" ForeColor="Red"></asp:LinkButton>
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                        </asp:TemplateField>
                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfSelSub" runat="server" />
    <div id="diagDelSub" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete Subsidy</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this subsidy?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelSub" runat="server" CssClass="btn btn-primary" Text="OK" CausesValidation="false" OnClick="btnDelSub_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <uc1:programme_search runat="server" id="programme_search" />
</asp:Content>
