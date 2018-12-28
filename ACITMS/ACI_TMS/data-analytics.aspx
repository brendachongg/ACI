<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="data-analytics.aspx.cs" Inherits="ACI_TMS.data_analytics" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <script>
        $(document).ready(function () {
            $('[data-toggle="popover"]').popover();
        });

        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip({
                placement: 'bottom'
            });
        });
    </script>
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h2>
                    <asp:Label ID="lblDataAnalytics" runat="server" Text="Export Data"></asp:Label>
                </h2>

                <%-- <asp:LinkButton ID="LinkButton2" runat="server" data-toggle="tooltip" title="&thinsp;&thinsp;Step 1: Please select a category.  Step 2: Please select a table. &thinsp; Step 3: Generate and download the template."><span class="fa fa-info-circle fa-fw" ></span></asp:LinkButton>--%>
                <small>Please select the following to export data that you want to analyse from the database</small>

                <br />

            </div>
        </div>

        <hr />
        <div class="row">


            <div class="col-lg-4 col-md-4 col-sm-12">
            </div>


            <div class="col-lg-5 col-md-5 col-sm-12">
            </div>

            <%--                    <div class="col-lg-3 col-md-3 col-sm-12">
                        <div class="panel panel-default">
                            <div id="listHeader" class="panel-heading">Operations</div>
                            <div class="panel-body">
                                <p>
                                    <asp:LinkButton ID="lkbtnUploadFile" runat="server" PostBackUrl="~/data-migration-upload.aspx"><span class="fa glyphicon-plus"></span> Upload Data File </asp:LinkButton>
                                </p>
                            

                            </div>
                        </div>
                    </div>--%>
        </div>


        <div class="row text-left">
            <div class="col-lg-8">
                <asp:Label ID="lblCate" runat="server" Text="Department" Font-Bold="True"></asp:Label>
                <asp:DropDownList ID="DdlDept" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="DdlDept_SelectedIndexChanged">
                    <asp:ListItem Value="0">-- Select Department --</asp:ListItem>
                    <asp:ListItem Value="1">Front Desk</asp:ListItem>
                    <asp:ListItem Value="2">Programme</asp:ListItem>
                    <asp:ListItem Value="3">Finance</asp:ListItem>
                    <asp:ListItem Value="4">IT</asp:ListItem>
                    <asp:ListItem Value="5">Business Development</asp:ListItem>
                    <asp:ListItem Value="6">Directorate</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <br />

        <table>
            <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" Text="Start Period" Font-Bold="True"></asp:Label></td>
                <td></td>
                <td>
                    <asp:Label ID="Label4" runat="server" Text="End Period" Font-Bold="True"></asp:Label></td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="startYear" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="DdlDept_SelectedIndexChanged">
                        <asp:ListItem Value="2015">FY 2015 Q1</asp:ListItem>
                        <asp:ListItem Value="2016">FY 2016 Q1</asp:ListItem>
                        <asp:ListItem Value="2017">FY 2017 Q1</asp:ListItem>
                        <asp:ListItem Value="2018">FY 2018 Q1</asp:ListItem>
                        <asp:ListItem Value="2019">FY 2019 Q1</asp:ListItem>
                        <asp:ListItem Value="2020">FY 2020 Q1</asp:ListItem>
                        <asp:ListItem Value="2021">FY 2021 Q1</asp:ListItem>
                    </asp:DropDownList></td>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;</td>
                <td>
                    <asp:DropDownList ID="endYear" runat="server" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="DdlDept_SelectedIndexChanged">
                        <asp:ListItem Value="2016">FY 2015 Q4</asp:ListItem>
                        <asp:ListItem Value="2017">FY 2016 Q4</asp:ListItem>
                        <asp:ListItem Value="2018">FY 2017 Q4</asp:ListItem>
                        <asp:ListItem Value="2019">FY 2018 Q4</asp:ListItem>
                        <asp:ListItem Value="2020">FY 2019 Q4</asp:ListItem>
                        <asp:ListItem Value="2021">FY 2020 Q4</asp:ListItem>
                        <asp:ListItem Value="2022">FY 2021 Q4</asp:ListItem>
                    </asp:DropDownList></td>
                <td>&nbsp;&nbsp;&nbsp; &nbsp;</td>
                <td>
                    <asp:Label ID="validation" runat="server" Text=""></asp:Label></td>
            </tr>
        </table>

        <br />
        <div class="row text-left">
            <div class="col-lg-8">
                <div class="panel panel-default" runat="server" id="DivInfoDes">
                    <div class="panel-heading" style="background-color: #428bca; color: white;">Description</div>
                    <div class="panel-body">
                        <asp:Label ID="lblDepartment" runat="server" Text=""></asp:Label>
                    </div>
                </div>
            </div>
        </div>


        <div class="row">
            <asp:Panel ID="tablePanel" runat="server" Visible="False">
                <div class="col-lg-8">

                    <asp:GridView ID="gvTable" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered dataTable no-footer hover gvv" EmptyDataText="No Related Tables" EmptyDataRowStyle-ForeColor="Black" EmptyDataRowStyle-VerticalAlign="Middle">
                        <Columns>


                            <asp:TemplateField HeaderText="Relevant Table">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("table") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("table") %>'></asp:Label>
                                    <i data-content='<%# Eval("DataContent")%>' data-toggle="popover" data-trigger="hover" title="Description"><span class="fa fa-info-circle fa-fw" style="color: #6192b7"></span></i>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("table2") %>'></asp:TextBox>
                                </EditItemTemplate>

                                <ItemTemplate>

                                    <asp:Label ID="Lbl2" runat="server" Text='<%# Bind("table2") %>'></asp:Label>
                                    <i runat="server" id="iconDiv" visible='<%#Eval("table2") != DBNull.Value%>'>

                                        <i data-content='<%# Eval("DataContent2")%>' data-toggle="popover" data-trigger="hover" title="Description"><span class="fa fa-info-circle fa-fw" style="color: #6192b7"></span></i>
                                    </i>
                                </ItemTemplate>

                            </asp:TemplateField>
                            <asp:BoundField AccessibleHeaderText="DataContent" DataField="DataContent" HeaderText="DataContent" Visible="False" />
                        </Columns>
                        <EmptyDataRowStyle ForeColor="Black" VerticalAlign="Middle" />
                    </asp:GridView>
                </div>
            </asp:Panel>
        </div>


        <asp:Panel ID="downloadPanel" runat="server" Visible="False">
            <div class="row text-left">
                <div class=" col-lg-8">
                    <div class="alert alert-success alert-dismissable">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>
                        <br />
                        <img id="Img1" runat="server" src="../Resource/images/succeed.png" height="30" width="30" />
                        <asp:LinkButton ID="lbDownloadData" runat="server" OnClick="lbDownloadData_Click">Data Generated. Download Data.</asp:LinkButton>
                        <br />


                    </div>
                </div>
            </div>
        </asp:Panel>


        <div class="row text-right">
            <div class="col-lg-12">

                <asp:Button ID="btnGetData" runat="server" Text="Get Data" class="btn btn-info" OnClick="btnGetData_Click" />

            </div>
        </div>


    </div>

</asp:Content>
