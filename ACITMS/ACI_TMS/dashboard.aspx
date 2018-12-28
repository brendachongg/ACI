<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="ACI_TMS.dashboard" %>

<%@ Import Namespace="ACI_TMS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.collapse').on('shown.bs.collapse', function () {
                $(this).parent().find(".glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-minus");
            }).on('hidden.bs.collapse', function () {
                $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-plus");
            });
        });

        function showApplnTrSearch() {
            $('#panelApplnTrSearch').addClass("in");
            $('#iconApplnTr').removeClass("glyphicon-plus").addClass("glyphicon-minus");
        }

        function showClasses() {
            $('#panelClasses').addClass("in");
            $('#iconClasses').removeClass("glyphicon-plus").addClass("glyphicon-minus");
        }

        function SetPlaceHolderText() {
            var SelectedValue = $('#<%=ddlSearchApplicantTraineeType.ClientID%> option:selected').val();
            if (SelectedValue == 1) {
                $('#<%=tbSearchApplicantTrainee.ClientID%>').prop('placeholder', 'NRIC or Name');
            }
            else {
                $('#<%=tbSearchApplicantTrainee.ClientID%>').prop('placeholder', 'Trainee ID or Name');
            }
        }

        function exportAttendance(bid, mode) {
            window.open("<%= attendance_sheet.PAGE_NAME%>?<%=attendance_sheet.BATCH_QUERY%>=" + encodeURI(bid) + "&<%=attendance_sheet.MODE_QUERY%>=" + encodeURI(mode), "_blank", "menubar=no,location=no");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div id="accordion">
            <h3 style="padding-top: 20px;">
                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#panelStats" style="text-decoration: none;">
                    <span id="iconStats" class="glyphicon glyphicon-minus"></span>
                    Statistics
                </a>
            </h3>
            <div id="panelStats" class="panel-collapse collapse in" >
                <asp:ListView runat="server" ID="lvStats" GroupItemCount="3">
                    <LayoutTemplate>
                        <div id="groupPlaceholder" runat="server">
                        </div>
                    </LayoutTemplate>
                    <GroupTemplate>
                        <div class="row" style="padding-bottom: 10px;">
                            <div class="col-lg-4 col-md-6" id="itemPlaceholder" runat="server">
                            </div>
                        </div>
                    </GroupTemplate>
                    <ItemTemplate>
                        <div class="col-lg-4 col-md-6">
                            <div class="panel panel-primary" id="panelAppln" runat="server">
                                <a href='<%# Eval("link") %>'>
                                    <div class="panel-heading">
                                        <div class="row">
                                            <div class="col-xs-3">
                                                <i class='<%# Eval("icon") %>' style="font-size: 45px;"></i>
                                            </div>
                                            <div class="col-xs-9 text-right">
                                                <div class="huge"><%# Eval("cnt") %></div>
                                                <div><%# Eval("label") %></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel-footer">
                                        <span class="pull-left">View Details</span>
                                        <span class="pull-right"><i class="fa fa-arrow-circle-right"></i></span>
                                        <div class="clearfix"></div>
                                    </div>
                                </a>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:ListView>
            </div>

            <hr />

            <div id="divAttendance" runat="server" style="padding-bottom:10px;">
                <h3>
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#panelAttendance" style="text-decoration: none;">
                        <span id="iconAttendance" class="glyphicon glyphicon-plus"></span>
                        Attendance for Today's Class
                    </a>
                </h3>
                <div id="panelAttendance" class="panel-collapse collapse" style="padding-bottom:5px;">
                    <asp:GridView ID="gvAttendance" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" runat="server"
                        CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                        OnPageIndexChanging="gvAttendance_PageIndexChanging" OnRowDataBound="gvAttendance_RowDataBound" OnRowCommand="gvAttendance_RowCommand">
                        <EmptyDataTemplate>
                            No session available
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="batchCode" HeaderText="Class" HeaderStyle-Width="200px" />
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
                                    <i id="btnInserted" runat="server" class="btn btn-lg glyphicon glyphicon-list-alt" style="color: red"></i>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <PagerStyle CssClass="pagination-ys" />
                    </asp:GridView>
                </div>
            </div>

            <div id="divApplnTrainee" runat="server" style="padding-bottom:10px;">
                <h3>
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#panelApplnTrSearch" style="text-decoration: none;">
                        <span id="iconApplnTr" class="glyphicon glyphicon-plus"></span>
                        Quick Applicant/Trainee Search
                    </a>
                </h3>
                <div id="panelApplnTrSearch" class="panel-collapse collapse" style="padding-bottom:5px;">
                    <div class="row">
                        <div class="col-lg-2">
                            <asp:DropDownList ID="ddlSearchApplicantTraineeType" CssClass="form-control" runat="server" onchange="SetPlaceHolderText();">
                                <asp:ListItem Value="a">Applicant</asp:ListItem>
                                <asp:ListItem Value="t">Trainee</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-lg-9">
                            <asp:TextBox ID="tbSearchApplicantTrainee" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-lg-1">
                            <asp:Button ID="btnSearchApplicantTrainee" CssClass="btnSearch btn btn-info" runat="server" Text="Search" OnClick="btnSearchApplicantTrainee_Click" />
                        </div>
                        <div style="clear: both;"></div>
                    </div>
                    <br />
                    <%-- Result of search --%>
                    <div class="row">
                        <div class="col-lg-12">
                            <asp:GridView ID="gvSearchApplicantTrainee" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" OnPageIndexChanging="gvSearchApplicantTrainee_PageIndexChanging"
                                CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10" OnRowCommand="gvSearchApplicantTrainee_RowCommand">
                                <EmptyDataTemplate>
                                    No records found.
                                </EmptyDataTemplate>

                                <Columns>
                                    <asp:BoundField HeaderText="ID" DataField="ID" />
                                    <asp:TemplateField HeaderText="NRIC/Passport">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lkbtnApplicantId" runat="server" CommandName="viewDetails" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>'>
                                                <asp:Label ID="lbgvIdentity" runat="server" Text='<%# Eval("idNumber") %>'></asp:Label>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Name" DataField="fullName" />
                                    <asp:BoundField HeaderText="Contact" DataField="contactNumber1" />
                                    <asp:BoundField HeaderText="Programme" DataField="programmeTitle" />
                                </Columns>
                                <PagerStyle CssClass="pagination-ys" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <div id="divClasses" runat="server" style="padding-bottom:10px;">
                <h3>
                    <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#panelClasses" style="text-decoration: none;">
                        <span id="iconClasses" class="glyphicon glyphicon-plus"></span>
                        Classes
                    </a>
                </h3>
                <div id="panelClasses" class="panel-collapse collapse" style="padding-bottom:5px;">
                    <div class="row">
                        <div class="col-lg-3">
                            <asp:DropDownList ID="ddlProgCategory" CssClass="form-control" runat="server" OnSelectedIndexChanged="class_SelectedIndexChanged" AutoPostBack="true" DataTextField="codeValueDisplay" DataValueField="codeValue">
                            </asp:DropDownList>
                        </div>
                        <div class="col-lg-3">
                            <asp:DropDownList ID="ddlProgType" CssClass="form-control" runat="server" OnSelectedIndexChanged="class_SelectedIndexChanged" AutoPostBack="true" DataTextField="codeValueDisplay" DataValueField="codeValue">
                            </asp:DropDownList>
                        </div>
                        <div class="col-lg-3">
                            <asp:DropDownList ID="ddlProgStatus" CssClass="form-control" runat="server" OnSelectedIndexChanged="class_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Value="AVA" Selected>Available</asp:ListItem>
                                <asp:ListItem Value="OS">Opening Soon</asp:ListItem>
                                <asp:ListItem Value="CL">Closed</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-lg-12">
                            <asp:GridView ID="gvBatch" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                                 OnPageIndexChanging="gvBatch_PageIndexChanging" OnRowCommand="gvBatch_RowCommand">
                                <EmptyDataTemplate>
                                    No records found.
                                </EmptyDataTemplate>
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>Class Code</HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lkbtnBatchCode" runat="server" CommandName="selectBatch" CommandArgument='<%# Eval("programmeBatchId") %>' CausesValidation="false">
                                                <asp:Label ID="lbgvBatchCode" runat="server" Text='<%# Eval("batchCode") %>'></asp:Label>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="100px" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Programme">
                                        <ItemTemplate>
                                            <%# Eval("programmeCode") %> (<%# Eval("programmeTitle") %>)
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="batchCapacity" HeaderText="Capacity" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderText="Registration Date" ItemStyle-Width="120px">
                                        <ItemTemplate>
                                            <%# Eval("programmeRegStartDateDisp") %> to <%# Eval("programmeRegEndDateDisp") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Commencment Date" ItemStyle-Width="120px">
                                        <ItemTemplate>
                                            <%# Eval("programmeStartDateDisp") %> to <%# Eval("programmeCompletionDateDisp") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="enrolledCnt" HeaderText="Enrolled" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="appliedCnt" HeaderText="Registered" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" />
                                </Columns>

                                <PagerStyle CssClass="pagination-ys" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
