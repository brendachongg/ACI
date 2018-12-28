<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="attendance-management.aspx.cs" Inherits="ACI_TMS.attendance_management" %>

<%@ Import Namespace="ACI_TMS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function formatSearchTxt() {
            if ($('#<%=ddlSearch.ClientID%> option:selected').val() == "D")
                $('#<%=txtSearch.ClientID%>').datepicker({
                    dateFormat: "dd M yy",
                    changeMonth: true,
                    changeYear: true
                });
            else
                $('#<%=txtSearch.ClientID%>').datepicker("destroy");
        }

        function validateSessionDate(oSrc, args) {
            if ($('#<%=ddlSearch.ClientID%> option:selected').val() != "D") {
                args.IsValid = true;
                return true;
            }

            var str = $('#<%=txtSearch.ClientID%>').val();
            if (!isValidDate(str)){
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function exportAttendance(bid, mode) {
            window.open("<%= attendance_sheet.PAGE_NAME%>?<%=attendance_sheet.BATCH_QUERY%>=" + encodeURI(bid) + "&<%=attendance_sheet.MODE_QUERY%>=" + encodeURI(mode), "_blank", "menubar=no,location=no");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Attendance Management"></asp:Label>
                </h2>
            </div>
        </div>
        <hr />
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <div class="row">
            <div class="col-lg-12">
                <div class="form-group form-inline">
                    <asp:Label ID="lbSearch" runat="server" Text="Search By: "></asp:Label>
                    <asp:DropDownList ID="ddlSearch" CssClass="form-control" runat="server" onChange="formatSearchTxt()">
                        <asp:ListItem Text="--Select--" Value=""></asp:ListItem>
                        <asp:ListItem Text="Module" Value="M"></asp:ListItem>
                        <asp:ListItem Text="Programme" Value="P"></asp:ListItem>
                        <asp:ListItem Text="Date" Value="D" Selected></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSearchType" runat="server" ErrorMessage="Must select search type" Display="None" ControlToValidate="ddlSearch"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="datepicker form-control" Width="350px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSearchValue" runat="server" ErrorMessage="Search value cannot be empty" Display="None" ControlToValidate="txtSearch"></asp:RequiredFieldValidator>
                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-info" OnClick="btnSearch_Click">  
                        <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                    </asp:LinkButton>
                    <asp:CustomValidator ID="cvDate" runat="server" Display="None" ControlToValidate="txtSearch" ClientValidationFunction="validateSessionDate"
                            ErrorMessage="Invalid date" ValidateEmptyText="false"></asp:CustomValidator>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvAttendance" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" runat="server"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    onPageIndexChanging="gvAttendance_PageIndexChanging" OnRowDataBound="gvAttendance_RowDataBound" OnRowCommand="gvAttendance_RowCommand">
                    <EmptyDataTemplate>
                        No session available
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField DataField="batchCode" HeaderText="Class" HeaderStyle-Width="200px"/>
                        <asp:TemplateField HeaderText="Session" HeaderStyle-Width="160px">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnSession" runat="server" CommandName="View" CommandArgument='<%# Eval("sessionId") %>'>
                                    <%# Eval("sessionDateDisp") %>&nbsp;<%# Eval("sessionPeriodDisp") %>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>     
                        <asp:TemplateField HeaderText="Programme">
                            <ItemTemplate>
                                <%# Eval("programmeTitle") %> (<%# Eval("programmeCode") %>)
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Module">
                            <ItemTemplate>
                                <%# Eval("moduleTitle") %> (<%# Eval("moduleCode") %>)
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Attendance" HeaderStyle-Width="130px">
                            <ItemTemplate>
                                <i class="btn btn-lg glyphicon glyphicon-list-alt" onclick='exportAttendance(<%# Eval("batchModuleId") %>, "M")'></i>
                                <i ID="btnInserted" runat="server" class="btn btn-lg glyphicon glyphicon-list-alt" style="color:red" ></i>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
