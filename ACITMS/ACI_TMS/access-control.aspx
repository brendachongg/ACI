<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" EnableViewState="true" CodeBehind="access-control.aspx.cs" Inherits="ACI_TMS.access_control" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .checkBoxClass tr td label {
            margin-right: 80px;
            margin-left: 8px;
        }
    </style>
    <script>
        function showSection(id) {
            var checkBoxList = document.getElementById('d' + id);
            var plusIcon = document.getElementById('p' + id);
            var minusIcon = document.getElementById('m' + id);
            var btnGrant = document.getElementById('ContentPlaceHolder1_rptGroup_btnGrant_' + id);
            var btnRevoke = document.getElementById('ContentPlaceHolder1_rptGroup_btnRevoke_' + id);

            if (checkBoxList) {
                if (checkBoxList.style.display != 'block') {
                    btnGrant.style.display = 'block';
                    btnGrant.className = "btn btn-default";
                    btnRevoke.className = "btn btn-default";
                    btnRevoke.style.display = 'block';
                    plusIcon.style.display = 'none';
                    minusIcon.style.display = 'block';
                    checkBoxList.style.display = 'block';
                } else {
                    btnGrant.style.display = 'none';
                    btnRevoke.style.display = 'none';
                    plusIcon.style.display = 'block';
                    minusIcon.style.display = 'none';
                    checkBoxList.style.display = 'none';
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-12">
                <h2>
                    <asp:Label ID="lbAccessControlHeader" runat="server" Text="Access Control"></asp:Label>
                </h2>
                <small>Please select the function that you wish to grant to the staff</small>
            </div>
        </div>
        <br />
        <div class="row text-left">
            <div class="col-lg-12">
                <div class="alert alert-success alert-dismissable" id="successMessage" role="alert" runat="server">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>
                    <asp:Label ID="lblSuccess" runat="server" Text=""></asp:Label>
                </div>
                <div class="alert alert-danger alert-dismissable" id="failMessage" role="alert" runat="server">
                    <a href="#" class="close" data-dismiss="alert" aria-label="close">×</a>
                    <asp:Label ID="lblFailure" runat="server" Text=""></asp:Label>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <div class="col-lg-6">
                    <asp:Label ID="lbStaff" runat="server" Text="Staff:" Font-Bold="True"></asp:Label>
                    <br />
                    <asp:DropDownList ID="ddlStaff" runat="server" CssClass="form-control" DataTextField="userDisplay" DataValueField="userId" AutoPostBack="true" OnSelectedIndexChanged="ddlStaff_SelectedIndexChanged"></asp:DropDownList>
                </div>
            </div>
        </div>
        <br />
        <div class="row">
            <asp:Repeater ID="rptGroup" runat="server" OnItemDataBound="rptGroup_ItemDataBound">
                <ItemTemplate>
                    <div class="col-lg-12">
                        <div class="col-lg-12">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <a id='h<%#Container.ItemIndex %>' class="abc" style="color: white; font-weight: bold; cursor: pointer;" onclick="showSection(<%#Container.ItemIndex %>)">
                                        <div class="col-lg-1">
                                            <span id='p<%#Container.ItemIndex %>' style="display: block;" class="fa glyphicon-plus"></span>
                                            <span id='m<%#Container.ItemIndex %>' style="display: none;" class="fa glyphicon-minus"></span>
                                        </div>
                                        <div class="col-lg-1">
                                            <asp:Label ID="lblCodeValueDisplay" runat="server"> <%# Eval("codeValueDisplay")%></asp:Label>
                                        </div>
                                    </a>
                                    <div class="row text-right">
                                        <asp:Button ID='btnRevoke' runat="server" class="btn btn-default" Style="color: black; margin-left: 5px; margin-right: 10px; display: none; float: right;" Text="Revoke All" OnClick="btnRevoke_Click" />
                                        <asp:Button ID='btnGrant' runat="server" class="btn btn-default" Style="color: black; display: none; float: right;" Text="Grant All" OnClick="btnGrant_Click" />
                                    </div>
                                </div>
                                <asp:HiddenField ID="hfGetGroupValue" runat="server" Value='<%#Eval("codeValue")%>' />
                                <asp:HiddenField ID="hfIndex" runat="server" Value='<%#Container.ItemIndex %>' />
                            </div>
                        </div>
                        <div class="col-lg-12">
                            <div id='d<%#Container.ItemIndex %>' class="xyz" style="display: none;">
                                <asp:CheckBoxList ID="CheckBoxListFunctions" class="checkBoxClass" DataValueField="functionId" DataTextField="functionDesc" runat="server" RepeatDirection="Horizontal" RepeatColumns="3"></asp:CheckBoxList>
                                <br />
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div class="row text-right">
            <div class="col-lg-12">
                <br />
                <asp:Button ID="saveBtn" class="btn btn-primary" runat="server" Text="Save" OnClick="saveBtn_Click" />
                
            </div>
        </div>
    </div>
</asp:Content>