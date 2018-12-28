<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="soa-view.aspx.cs" Inherits="ACI_TMS.soa_view" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function showSearch2() {
            if ($('#<%=ddlSearch.ClientID%> option:selected').val() == "CM") {
                $('#<%=txtSearch2.ClientID%>').show();
                $('#<%=txtSearch.ClientID%>').attr('placeholder', 'Class/project code');
            } else {
                $('#<%=txtSearch2.ClientID%>').hide();
                $('#<%=txtSearch.ClientID%>').attr('placeholder', '');
            }
        }

        function validateSearch2(oSrc, args) {
            if ($('#<%=ddlSearch.ClientID%> option:selected').val() == "CM" && $('#<%=txtSearch2.ClientID%>').val()=="") {
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
                    <asp:Label ID="lbHeader" runat="server" Text="View Statement of Attainment (SOA) Status"></asp:Label>
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
                        <p id="panelProcSOA" runat="server">
                            <asp:LinkButton ID="lkbtnProcSOA" runat="server" CausesValidation="false" OnClick="lkbtnProcSOA_Click" ><span class="glyphicon glyphicon-download"></span> Process SOA</asp:LinkButton>
                        </p>
                        <p id="panelRecevSOA" runat="server">
                            <asp:LinkButton ID="lkbtnRecevSOA" runat="server" CausesValidation="false" OnClick="lkbtnRecevSOA_Click" ><span class="glyphicon glyphicon-upload"></span> Receive SOA</asp:LinkButton>
                        </p>
                    </div>
                </div>
            </div>
        </div>

        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <div class="row">
            <div class="col-lg-12">
                <div class="form-group form-inline">
                    <asp:Label ID="lbSearch" runat="server" Text="Search By: "></asp:Label>
                    <asp:DropDownList ID="ddlSearch" CssClass="form-control" runat="server" onChange="showSearch2()">
                        <asp:ListItem Text="--Select--" Value=""></asp:ListItem>
                        <asp:ListItem Text="Module" Value="M"></asp:ListItem>
                        <asp:ListItem Text="Trainee" Value="T"></asp:ListItem>
                        <asp:ListItem Text="Class & Module" Value="CM"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvSearchType" runat="server" ErrorMessage="Must select search type" Display="None" ControlToValidate="ddlSearch"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" Width="350px"></asp:TextBox>
                    <asp:TextBox ID="txtSearch2" runat="server" CssClass="form-control" Width="350px" placeholder="Module code/name"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSearchValue" runat="server" ErrorMessage="Search value cannot be empty" Display="None" ControlToValidate="txtSearch"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cvSearch" runat="server" ErrorMessage="Module code/name cannot be empty" 
                        Display="None" ControlToValidate="ddlSearch" ClientValidationFunction="validateSearch2" ValidateEmptyText="false"></asp:CustomValidator>
                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-info" OnClick="btnSearch_Click">
                        <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                    </asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvTrainee" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" runat="server" AllowPaging="true" PageSize="30"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" OnPageIndexChanging="gvTrainee_PageIndexChanging">
                    <EmptyDataTemplate>
                        No available records
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField DataField="idNumber" HeaderText="NRIC/FIN/Passport" HeaderStyle-Width="200px" />
                        <asp:TemplateField HeaderText="Trainee">
                            <ItemTemplate>
                                [<%# Eval("traineeId") %>] <%# Eval("fullName") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="batchCode" HeaderText="Class" HeaderStyle-Width="200px" />
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
                        <asp:BoundField DataField="SOAStatusDisp" HeaderText="Status" HeaderStyle-Width="150px" />
                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>
        
    </div>
</asp:Content>
