<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="payment-management.aspx.cs" Inherits="ACI_TMS.payment_management" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var Page_Validators = Page_Validators || new Array();

        function formatSearchTxt() {
            if ($('#<%=ddlSearchType.ClientID%> option:selected').val() == "PD") {
                $('#<%=tbSearch.ClientID%>').datepicker({
                    dateFormat: "dd M yy",
                    changeMonth: true,
                    changeYear: true
                });
            } else
                $('#<%=tbSearch.ClientID%>').datepicker("destroy");

            if ($('#<%=ddlSearchType.ClientID%> option:selected').val() == "OUT") {
                $('#<%=tbSearch.ClientID%>').hide();               
                ValidatorEnable(document.getElementById('<%=rfvSearchTxt.ClientID%>'), false);
                ValidatorEnable(document.getElementById('<%=cvDate.ClientID%>'), false);
            } else {
                $('#<%=tbSearch.ClientID%>').show();
                ValidatorEnable(document.getElementById('<%=rfvSearchTxt.ClientID%>'), true);
                ValidatorEnable(document.getElementById('<%=cvDate.ClientID%>'), true);
            }
        }

        function hideError() {
            $('#<%=vSummary.ClientID%>').hide();
        }

        function validatePaymentDate(oSrc, args) {
            if ($('#<%=ddlSearchType.ClientID%> option:selected').val() != "PD") {
                args.IsValid = true;
                return true;
            }

            var str = $('#<%=tbSearch.ClientID%>').val();
            if (!isValidDate(str)) {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function showDelDialog(id, type, user) {
            $("#<%=hfSelPayId.ClientID%>").val(id);
            $("#<%=hfSelPayType.ClientID%>").val(type);
            $("#<%=hfSelPayUser.ClientID%>").val(user);
            $('#diagDelPayment').modal('show');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Payment Management"></asp:Label>
                </h2>
            </div>
        </div>
        <hr />
        <div class="row" id="panelOperations" runat="server">
            <div class="col-lg-9 col-md-9 col-sm-12">
            </div>

            <div class="col-lg-3 col-md-3 col-sm-12">
                <div class="panel panel-default">
                    <div id="listHeader" class="panel-heading">Operations</div>
                    <div class="panel-body">
                        <asp:LinkButton ID="lkbtnMakeupPayment" runat="server" CausesValidation="false" OnClick="lkbtnMakeupPayment_Click"><span class="glyphicon glyphicon-calendar"></span> Make-up Fee Payment</asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <div class="row text-left">
            <div class="col-lg-12">
                <div class="form-group form-inline">
                    <asp:Label ID="lbSearch" runat="server" Text="Search by"></asp:Label>
                    <asp:RequiredFieldValidator ID="rfvSearchType" runat="server" ErrorMessage="Search criteria cannot be empty." ControlToValidate="ddlSearchType" Display="None"></asp:RequiredFieldValidator>
                    <asp:DropDownList ID="ddlSearchType" runat="server" CssClass="form-control" onChange="formatSearchTxt()">
                        <asp:ListItem Value="PD">Payment Date</asp:ListItem>
                        <asp:ListItem Value="REF">Reference Number</asp:ListItem>
                        <asp:ListItem Value="APP">Applicant</asp:ListItem>
                        <asp:ListItem Value="TR">Trainee</asp:ListItem>
                        <asp:ListItem Value="OUT">Outstanding Payment</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSearchTxt" runat="server" ErrorMessage="Search value cannot be empty." ControlToValidate="tbSearch" Display="None"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="tbSearch" runat="server" placeholder="" CssClass="form-control" Width="350px"></asp:TextBox>
                    <asp:CustomValidator ID="cvDate" runat="server" Display="None" ControlToValidate="tbSearch" ClientValidationFunction="validatePaymentDate"
                            ErrorMessage="Invalid date" ValidateEmptyText="false"></asp:CustomValidator>
                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-info" OnClick="btnSearch_Click">
                        <span aria-hidden="true" class="fa fa-search"></span>
                    </asp:LinkButton>
                </div>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvPayment" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvPayment_PageIndexChanging" OnRowCommand="gvPayment_RowCommand" OnRowDataBound="gvPayment_RowDataBound">
                    <EmptyDataTemplate>
                        No available records
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField DataField="paymentTypeDisp" HeaderText="Payment Type" ItemStyle-Width="180px" />
                        <asp:TemplateField>
                            <HeaderTemplate>Applicant/Trainee ID</HeaderTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnUser" runat="server" CommandName="select" CommandArgument='<%# Container.DataItemIndex %>' CausesValidation="false">
                                    <asp:Label ID="lbgvUser" runat="server" Text='<%# Eval("userId") %>'></asp:Label>
                                </asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" Width="200px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="fullName" HeaderText="Name" />
                        <asp:BoundField DataField="batchCode" HeaderText="Class" ItemStyle-Width="200px" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label ID="lbtnDelPayment" runat="server" CssClass="glyphicon glyphicon-remove" style="font-size: 20px;"
                                    ForeColor="Red" data-toggle="tooltip" data-placement="top" title="Delete payment"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="center" Width="50px" />
                        </asp:TemplateField>
                    </Columns>
                    
                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hfSelPayId" runat="server" />
    <asp:HiddenField ID="hfSelPayType" runat="server" />
    <asp:HiddenField ID="hfSelPayUser" runat="server" />
    <div id="diagDelPayment" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete Payment</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this payment? The action cannot be reversed.
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelPayment" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnDelPayment_Click"/>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
