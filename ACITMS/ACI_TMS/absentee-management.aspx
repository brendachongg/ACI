<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="absentee-management.aspx.cs" Inherits="ACI_TMS.absentee_management" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var Page_Validators = Page_Validators || new Array();

        function formatSearchTxt() {
            if ($('#<%=ddlSearchType.ClientID%> option:selected').val() == "SD"){
                $('#<%=tbSearch.ClientID%>').datepicker({
                    dateFormat: "dd M yy",
                    changeMonth: true,
                    changeYear: true
                });
            } else
                $('#<%=tbSearch.ClientID%>').datepicker("destroy");

            if ($('#<%=ddlSearchType.ClientID%> option:selected').val() == "AVA") {
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

        function validateSessionDate(oSrc, args) {
            if ($('#<%=ddlSearchType.ClientID%> option:selected').val() != "SD") {
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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Absentee Management"></asp:Label>
                </h2>
            </div>
        </div>
        <hr />
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <div class="row text-left">
            <div class="col-lg-12">
                <div class="form-group form-inline">
                    <asp:Label ID="lbSearch" runat="server" Text="Search by"></asp:Label>
                    <asp:RequiredFieldValidator ID="rfvSearchType" runat="server" ErrorMessage="Search criteria cannot be empty." ControlToValidate="ddlSearchType" Display="None"></asp:RequiredFieldValidator>
                    <asp:DropDownList ID="ddlSearchType" runat="server" CssClass="form-control" onChange="formatSearchTxt()">
                        <asp:ListItem Value="">--Select--</asp:ListItem>
                        <asp:ListItem Value="T">Trainee</asp:ListItem>
                        <asp:ListItem Value="SD">Session Date</asp:ListItem>
                        <asp:ListItem Value="AVA">Available for makeup</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSearchTxt" runat="server" ErrorMessage="Search value cannot be empty." ControlToValidate="tbSearch" Display="None"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="tbSearch" runat="server" placeholder="" CssClass="form-control" Width="350px"></asp:TextBox>
                    <asp:CustomValidator ID="cvDate" runat="server" Display="None" ControlToValidate="tbSearch" ClientValidationFunction="validateSessionDate"
                            ErrorMessage="Invalid date" ValidateEmptyText="false"></asp:CustomValidator>
                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-info" OnClick="btnSearch_Click">
                        <span aria-hidden="true" class="fa fa-search"></span>
                    </asp:LinkButton>

                    <asp:Button ID="btnListAll" runat="server" CssClass="btn btn-default" Text="List All" CausesValidation="false" OnClick="btnListAll_Click" />

                </div>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvAbsentList" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvAbsentList_PageIndexChanging" OnRowCommand="gvAbsentList_RowCommand">
                    <EmptyDataTemplate>
                        No available records
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>Trainee ID</HeaderTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnTrainee" runat="server" CommandName="selectTrainee" CommandArgument='<%# Container.DataItemIndex %>' CausesValidation="false">
                                    <asp:Label ID="lbgvTrainee" runat="server" Text='<%# Eval("traineeId") %>'></asp:Label>
                                </asp:LinkButton>
                                <asp:HiddenField ID="hfTraineeId" runat="server" Value='<%# Eval("traineeId") %>' />
                                <asp:HiddenField ID="hfSessionId" runat="server" Value='<%# Eval("sessionId") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" Width="200px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="fullName" HeaderText="Trainee" />
                        <asp:BoundField DataField="batchCode" HeaderText="Class" ItemStyle-Width="200px" />
                        <asp:TemplateField HeaderText="Session" ItemStyle-Width="160px">
                            <ItemTemplate>
                                <%# Eval("sessionDateDisp") %> <%# Eval("sessionPeriodDisp") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    
                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>
    </div>

</asp:Content>
