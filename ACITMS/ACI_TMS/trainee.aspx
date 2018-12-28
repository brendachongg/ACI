<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="trainee.aspx.cs" Inherits="ACI_TMS.trainee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <%-- View all applicant --%>
        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbApplicantHeader" runat="server" Text="Trainee"></asp:Label>
                </h2>

                <asp:Label ID="lbSubHeader" runat="server" Text="Click on the trainees for more details"></asp:Label>
                <br />
                <br />

            </div>
        </div>

        <hr />

        <div class="row">


            <div class="col-lg-12">
                <div class="form-group form-inline">
                    <asp:Label ID="Label1" runat="server" Text="Search: "></asp:Label>
                    <asp:TextBox ID="tbSearchApplicant" runat="server" CssClass="form-control" placeholder="NRIC/Name/Trainee ID/ Programme/Class Code" Width="600px"></asp:TextBox>
                    <asp:Button ID="btnSearchApplicant" runat="server" class="btn btn-default" Text="Search" OnClick="btnSearchApplicant_Click" />

                </div>
            </div>


        </div>


        <%-- List of applicants --%>
        <div class="row">
            <div class="col-lg-12">

                <asp:GridView ID="gvTrainee" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvTrainee_PageIndexChanging" OnRowCommand="gvTrainee_RowCommand" AllowSorting="False" AllowCustomPaging="True">


                    <Columns>

                        <%-- Trainee ID --%>
                        <asp:TemplateField HeaderText="Trainee ID">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnTraineeID" runat="server" CommandName="viewTraineeDetail" CommandArgument='<%# Eval("traineeID") %>'>
                                    <asp:Label ID="lbgvTraineeID" runat="server" Text='<%# Eval("traineeId") %>'></asp:Label>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Full name --%>
                        <asp:TemplateField HeaderText="Full Name">
                            <ItemTemplate>
                                <asp:Label ID="lbgvFullName" runat="server" Text='<%# Eval("fullName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Course Enrolled --%>
                        <asp:TemplateField HeaderText="Course Enrolled">
                            <ItemTemplate>
                                <asp:Label ID="lbgvCourseEnrolled" runat="server" Text='<%# Eval("programmeTitle") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Enrolled Date --%>
                        <asp:TemplateField HeaderText="Date of Enrolment">
                            <ItemTemplate>
                                <asp:Label ID="lbgvEnrolDate" runat="server" Text='<%# Eval("enrolDate", "{0:dd MMM yyyy}")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />

                </asp:GridView>

            </div>
        </div>
    </div>
</asp:Content>
