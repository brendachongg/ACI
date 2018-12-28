<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="applicant.aspx.cs" Inherits="ACI_TMS.applicant" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Content/custom/pagination.css" rel="stylesheet" />
        <script>
            function SearchGrid(txtSearch, grd) {
      
                if ($("[id *=" + txtSearch + " ]").val() != "") {
                
                    $("[id *=" + grd + " ]").children
                    ('tbody').children('tr').each(function () {
                        $(this).show();
                    });
                    $("[id *=" + grd + " ]").children
                    ('tbody').children('tr').each(function () {
                        var match = false;
                        $(this).children('td').each(function () {
                            if ($(this).text().toUpperCase().indexOf($("[id *=" +
                        txtSearch + " ]").val().toUpperCase()) > -1) {
                                match = true;
                                return false;
                            }
                        });
                        if (match) {
                            $(this).show();
                            $(this).children('th').show();
                        }
                        else {
                            $(this).hide();
                            $(this).children('th').show();
                        }
                    });


                    $("[id *=" + grd + " ]").children('tbody').
                            children('tr').each(function (index) {
                                if (index == 0)
                                    $(this).show();
                            });
                }
                else {
                    $("[id *=" + grd + " ]").children('tbody').
                            children('tr').each(function () {
                                $(this).show();
                            });
                }
            }

            $(document).on("Keyup", function () {
                SearchGrid('<%=tbSearchApplicant.ClientID%>', '<%=gvApplicant.ClientID%>');
            });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="page-wrapper">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <%-- View all applicant --%>
                <div class="row text-center">
                    <div class="col-lg-12">
                        <h2>
                            <asp:Label ID="lbApplicantHeader" runat="server" Text="Applicants"></asp:Label>
                        </h2>

                        <asp:Label ID="lbSubHeader" runat="server" Text="Click on the applicants for more details"></asp:Label>
                        <br />
                        <br />

                        <div class="btn-group" style="margin: 0 auto;" role="group" aria-label="...">
                            <%-- View today only --%>
                            <button id="btnLast24" type="button" class="btn btn-default btn-info" runat="server" onserverclick="btnLast24_ServerClick">
                                Today
                        <span class="badge">
                            <asp:Label ID="lbTodayBadge" runat="server" Text="0"></asp:Label>
                        </span>
                            </button>
                            <%-- View all applicantion --%>
                            <button id="btnAfter24" type="button" class="btn btn-default" runat="server" onserverclick="btnAfter24_ServerClick">
                                Other Applications 
                        <span class="badge">
                            <asp:Label ID="lbOtherDatesBadge" runat="server" Text="0"></asp:Label>
                        </span>
                            </button>
                        </div>
                    </div>
                </div>

                <hr />

                <div class="row">
                    <div class="col-lg-10">
                        <div class="form-group form-inline">
                            <asp:Label ID="Label1" runat="server" Text="Search: "></asp:Label>

                            <asp:TextBox ID="tbSearchApplicant" runat="server" CssClass="form-control" placeholder="NRIC or Name or Programme Applied or Class Code" Width="600px"></asp:TextBox>

                            <asp:Button ID="btnSearchApplicant" runat="server" class="btn btn-default" Text="Search"  OnClick="btnSearchApplicant_Click"/>

                            
                        </div>
                    </div>

                    <div class="col-lg-2 text-right">
                        <div style="background-color: red; width: 10px; height: 10px; display: inline-block; margin-right: 5px;"></div><div style="display: inline-block;">Suspended</div>
                    </div>
                </div>


                <%-- List of applicants --%>
                <div class="row">
                    <div class="col-lg-12">


                        <asp:GridView ID="gvApplicant" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" 
                            CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="20" EnableSortingAndPagingCallbacks = "true"
                            OnPageIndexChanging="gvApplicant_PageIndexChanging" OnRowCommand="gvApplicant_RowCommand" OnRowDataBound="gvApplicant_RowDataBound">

                            <Columns>

                                <%-- NRIC --%>
                                <asp:TemplateField HeaderText="Identity Number">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lkbtnApplicantId" runat="server" CommandName="viewApplicantDetails" CommandArgument='<%# Eval("applicantId") %>'>
                                            <asp:Label ID="lbgvIdentity" runat="server" Text='<%# Eval("idNumber") %>'></asp:Label>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <%-- Full name --%>
                                <asp:TemplateField HeaderText="Full Name">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdfBlacklisted" runat="server" Value='<%# Eval("blacklistStatus") %>' />
                                        <asp:Label ID="lbgvFullName" runat="server" Text='<%# Eval("fullName") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- Application date and time --%>
                                <asp:TemplateField HeaderText="Date of Registration">
                                    <ItemTemplate>
                                        <asp:Label ID="lbgvApplicationDateTime" runat="server" Text='<%# Eval("applicationDate") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%-- Course applied --%>
                                <asp:TemplateField HeaderText="Programme Applied">
                                    <ItemTemplate>
                                        <asp:Label ID="lbgvCourseCode" runat="server" Text='<%# Eval("programmeTitle") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>

                            <PagerStyle CssClass="pagination-ys" />

                        </asp:GridView>


                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
