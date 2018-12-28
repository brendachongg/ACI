<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="soa-process.aspx.cs" Inherits="ACI_TMS.soa_process" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function selectAll() {
            $(":checkbox").prop('checked', $('#<%= gvTrainee.ClientID%>_cbAll').is(':checked'));
        }

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
            if ($('#<%=ddlSearch.ClientID%> option:selected').val() == "CM" && $('#<%=txtSearch2.ClientID%>').val() == "") {
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
                    <asp:Label ID="lbHeader" runat="server" Text="Process Statement of Attainment (SOA)"></asp:Label>
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
                        <p id="panelViewSOA" runat="server">
                            <asp:LinkButton ID="lkbtnViewSOA" runat="server" CausesValidation="false" OnClick="lkbtnViewSOA_Click" ><span class="glyphicon glyphicon-eye-open"></span> View SOA</asp:LinkButton>
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
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" ValidationGroup="process" />
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
                    <asp:RequiredFieldValidator ID="rfvSearchType1" runat="server" ErrorMessage="Must select search type" Display="None" ControlToValidate="ddlSearch" ValidationGroup="process"></asp:RequiredFieldValidator>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" Width="350px"></asp:TextBox>
                    <asp:TextBox ID="txtSearch2" runat="server" CssClass="form-control" Width="350px" placeholder="Module code/name"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSearchValue" runat="server" ErrorMessage="Search value cannot be empty" Display="None" ControlToValidate="txtSearch"></asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="rfvSearchValue1" runat="server" ErrorMessage="Search value cannot be empty" Display="None" ControlToValidate="txtSearch" ValidationGroup="process"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cvSearch" runat="server" ErrorMessage="Module code/name cannot be empty" 
                        Display="None" ControlToValidate="ddlSearch" ClientValidationFunction="validateSearch2" ValidateEmptyText="false"></asp:CustomValidator>
                    <asp:CustomValidator ID="cvSearch2" runat="server" ErrorMessage="Module code/name cannot be empty" ValidationGroup="process"
                        Display="None" ControlToValidate="ddlSearch" ClientValidationFunction="validateSearch2" ValidateEmptyText="false"></asp:CustomValidator>
                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-info" OnClick="btnSearch_Click">
                        <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                    </asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvTrainee" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" runat="server"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv">
                    <EmptyDataTemplate>
                        No available records
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField DataField="batchModuleId" HeaderText="BatchModule" />
                        <asp:BoundField DataField="traineeId" HeaderText="traineeId" />
                        <asp:TemplateField HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                &nbsp;<asp:CheckBox ID="cbAll" runat="server" onchange="selectAll()" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="cb" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
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
                        <asp:BoundField DataField="SOAStatusDisp" HeaderText="Status" HeaderStyle-Width="100px" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div id="panelProcess" runat="server" visible="false">
            <br />
            <div class="row text-right">
                <div class="col-lg-12">
                    <div class="form-group form-inline">
                        <b>Cert Code: </b>
                        <asp:DropDownList ID="ddlCertCode" CssClass="form-control" runat="server">
                            <asp:ListItem value=""></asp:ListItem>
                            <asp:ListItem value="SOA-001">Industry and Generic Skills SOAs</asp:ListItem>
                            <asp:ListItem value="SOA-FB-001">WSQ Food Hygiene Course SOAs</asp:ListItem>
                            <asp:ListItem value="SF_SOA_001">Skills Future (SF) Food Services Framework</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvCertCode" runat="server" ErrorMessage="Must select cert code" Display="None" ControlToValidate="ddlCertCode" ValidationGroup="process"></asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>
            <div class="row text-right">
                <div class="col-lg-12">
                    <div class="form-group form-inline">
                        <b>ATO Contact Person: </b>
                        <asp:DropDownList ID="ddlContact" CssClass="form-control" runat="server" DataValueField="userId" DataTextField="userDisp">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvContact" runat="server" ErrorMessage="Must select contact person" Display="None" ControlToValidate="ddlContact" ValidationGroup="process"></asp:RequiredFieldValidator>
                    </div>
                    <asp:HiddenField ID="hfCount" runat="server" />
                </div>
            </div>
            <br />
            <div class="row text-right">
                <div class="col-lg-12">

                    <asp:Button ID="btnProcess" runat="server" CssClass="btn btn-primary" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Process&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                        OnClick="btnProcess_Click" ValidationGroup="process" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
