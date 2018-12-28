<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="session-management.aspx.cs" Inherits="ACI_TMS.session_management" %>

<%@ Register Src="~/module-search.ascx" TagPrefix="uc1" TagName="modulesearch" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.collapse').on('shown.bs.collapse', function () {
                $(this).parent().find(".glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-minus");
            }).on('hidden.bs.collapse', function () {
                $(this).parent().find(".glyphicon-minus").removeClass("glyphicon-minus").addClass("glyphicon-plus");
            });
        });

        function showAdvSearch() {
            $('#advSearch').addClass("in");
            $('#advSearchIcon').removeClass("glyphicon-plus").addClass("glyphicon-minus");
        }

        function showBasicSearch() {
            $('#basicSearch').addClass("in");
            $('#basicSearchIcon').removeClass("glyphicon-plus").addClass("glyphicon-minus");
        }

        function validateAdvSearch(oSrc, args) {
            var title = $('#<%=ddlProgrammeTitle.ClientID%> option:selected').val();
            if ($('#<%=ddlProgrammeCategory.ClientID%> option:selected').val() == "" && $('#<%=ddlProgrammeLevel.ClientID%> option:selected').val() == ""
                && (title==null || title == "") && $('#<%=tbBatchCode.ClientID%>').val() == ""
                && $('#<%=tbModule.ClientID%>').val() == "") {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }

        function validateBasicSearch(oSrc, args) {
            if ($('#<%=ddlBasicSearchType.ClientID%> option:selected').val() == "" || $('#<%=tbBasicSearch.ClientID%>').val() == "") {
                args.IsValid = false;
                return false;
            }

            args.IsValid = true;
            return true;
        }
    </script>

    <style>
        .accordion-toggle:hover {
            text-decoration: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-center">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbHeader" runat="server" Text="Session Management"></asp:Label>
                </h2>

            </div>
        </div>

        <hr />
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummaryAvdSearch" runat="server" CssClass="alert alert-danger alert-link"  ValidationGroup="advSearch"/>
        <asp:ValidationSummary ID="vSummaryBasicSearch" runat="server" CssClass="alert alert-danger alert-link"  ValidationGroup="basicSearch"/>

        <div class="row">
            <div class="col-lg-12">
                <div class="panel-group" id="accordion">
                    <%------------------------------------ Basic search ------------------------------------%>
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#basicSearch">
                                    <span id="basicSearchIcon" class="glyphicon glyphicon-plus"></span>
                                    Basic Search
                                </a>
                            </h4>
                        </div>
                        <div id="basicSearch" class="panel-collapse collapse">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-group form-inline">
                                            <asp:Label ID="lb6" runat="server" Text="Search By"></asp:Label>
                                            <asp:DropDownList ID="ddlBasicSearchType" runat="server" CssClass="form-control">
                                                <asp:ListItem Value="">--Select--</asp:ListItem>
                                                <asp:ListItem Value="BC">Class Code</asp:ListItem>
                                                <asp:ListItem Value="PC">Programme Code</asp:ListItem>
                                                <asp:ListItem Value="PJC">Project Code</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:TextBox ID="tbBasicSearch" runat="server" placeholder="" CssClass="form-control" Width="350px"></asp:TextBox>
                                            <asp:Button ID="btnBasicSearch" runat="server" Text="Search" CssClass="btn btn-info" ValidationGroup="basicSearch" OnClick="btnBasicSearch_Click" />

                                            <asp:CustomValidator ID="cvBasicSearch" runat="server" Display="None" ControlToValidate="tbBasicSearch" ClientValidationFunction="validateBasicSearch" ValidateEmptyText="true"
                                            ErrorMessage="Must select and enter 1 search criteria" ValidationGroup="basicSearch"></asp:CustomValidator>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%------------------------------------ END Basic search ------------------------------------%>
                    <br />
                    <%------------------------------------ Adv search ------------------------------------%>
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#advSearch">
                                    <span id="advSearchIcon" class="glyphicon glyphicon-plus"></span>
                                    Advanced Search
                                </a>
                            </h4>
                        </div>
                        <div id="advSearch" class="panel-collapse collapse">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-4">
                                        <asp:Label ID="lb1" runat="server" Text="Programme Category" Font-Bold="true"></asp:Label>
                                        <asp:DropDownList ID="ddlProgrammeCategory" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue" OnSelectedIndexChanged="ddlProgrammeCategory_SelectedIndexChanged" AutoPostBack="true" CausesValidation="false"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-4">
                                        <asp:Label ID="lb2" runat="server" Text="Programme Level" Font-Bold="true"></asp:Label>
                                        <asp:DropDownList ID="ddlProgrammeLevel" runat="server" CssClass="form-control" DataTextField="codeValueDisplay" DataValueField="codeValue" OnSelectedIndexChanged="ddlProgrammeLevel_SelectedIndexChanged" AutoPostBack="true" CausesValidation="false"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-4">
                                        <asp:Label ID="lb3" runat="server" Text="Class Code" Font-Bold="true"></asp:Label>
                                        <asp:TextBox ID="tbBatchCode" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>

                                <br />

                                <div class="row">
                                    <div class="col-lg-12">
                                        <asp:Label ID="lb4" runat="server" Text="Programme Title" Font-Bold="true"></asp:Label>
                                        <asp:DropDownList ID="ddlProgrammeTitle" runat="server" CssClass="form-control" DataTextField="programmeTitle" DataValueField="programmeTitle"></asp:DropDownList>
                                    </div>
                                </div>

                                <br />

                                <div class="row">
                                    <div class="col-lg-12">
                                        <asp:Label ID="lb5" runat="server" Text="Module" Font-Bold="true"></asp:Label>

                                        <div class="inner-addon right-addon">
                                            <i class="glyphicon glyphicon-search" data-toggle="modal" data-target="#diagSearchModule" style="cursor: pointer;"></i>
                                            <asp:TextBox ID="tbModule" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                            <asp:HiddenField ID="hfModuleId" runat="server" />
                                        </div>
                                    </div>
                                </div>

                                <br />

                                <div class="row text-right">
                                    <div class="col-lg-12">
                                        <asp:Button ID="btnAdvSearch" runat="server" Text="Search" CssClass="btn btn-info" OnClick="btnAdvSearch_Click" ValidationGroup="advSearch" />
                                        <asp:Button ID="btnAdvClear" runat="server" Text="Clear" CssClass="btn btn-default" OnClick="btnAdvClear_Click" CausesValidation="false" />
                                        <asp:CustomValidator ID="cvAdvSearch" runat="server" Display="None" ControlToValidate="tbBatchCode" ClientValidationFunction="validateAdvSearch" ValidateEmptyText="true"
                                            ErrorMessage="Must have at least 1 search criteria" ValidationGroup="advSearch"></asp:CustomValidator>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <%------------------------------------ END Adv search ------------------------------------%>
                </div>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvSession" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvSession_PageIndexChanging" OnRowCommand="gvSession_RowCommand">
                    <EmptyDataTemplate>
                        No available session
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField HeaderText="Class" ItemStyle-Width="150px" DataField="batchCode" />
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
                        <asp:TemplateField HeaderText="Session" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:LinkButton ID="lkbtnSession" runat="server" CommandName="selectSession" CommandArgument='<%# Eval("sessionId") %>' CausesValidation="false">
                                    <asp:Label ID="lbgvSession" runat="server" Text='<%# Eval("sessionDateDisp") + " " + Eval("sessionPeriodDisp") %> '></asp:Label>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Venue" ItemStyle-Width="150px" DataField="venueLocation" />
                    </Columns>
                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>

    </div>
    <asp:HiddenField ID="hfSelSearchType" runat="server" />
    <uc1:modulesearch runat="server" ID="modulesearch" />
</asp:Content>
