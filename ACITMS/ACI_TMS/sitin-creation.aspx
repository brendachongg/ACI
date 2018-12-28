<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="sitin-creation.aspx.cs" Inherits="ACI_TMS.sitin_creation" %>

<%@ Register Src="~/module-search.ascx" TagPrefix="uc1" TagName="modulesearch" %>
<%@ Register Src="~/trainee-search.ascx" TagPrefix="uc1" TagName="traineesearch" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbHeader" runat="server" Text="New No SOA"></asp:Label>
                </h3>
                <small>Please fill up the following</small>
            </div>
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click" CausesValidation="false"></asp:LinkButton>
            </div>
        </div>
        <hr />
        <div class="alert alert-success" id="panelSuccess" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblSuccess" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <div class="alert alert-danger" id="panelError" runat="server" visible="false">
            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
            <asp:Label ID="lblError" runat="server" CssClass="alert-link"></asp:Label>
        </div>
        <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
        <div class="row">
            <div class="col-lg-3">
                <asp:Label ID="lb1" runat="server" Text="Trainee ID " Font-Bold="true"></asp:Label>
                <div class="inner-addon right-addon">
                    <i class="glyphicon glyphicon-search" data-toggle="modal" data-target="#diagSearchTrainee" style="cursor: pointer;"></i>
                    <asp:TextBox ID="tbTraineeId" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
                <asp:RequiredFieldValidator ID="rfvTrainee" runat="server" ErrorMessage="Trainee cannot be empty." ControlToValidate="tbTraineeId" Display="None"></asp:RequiredFieldValidator>
            </div>
            <div class="col-lg-9">
                <asp:Label ID="lb2" runat="server" Text="Name " Font-Bold="true"></asp:Label>
                <asp:Label ID="lbTraineeName" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-lg-9">
                <asp:Label ID="lb22" runat="server" Text="Module" Font-Bold="true"></asp:Label>
                <div class="inner-addon right-addon">
                    <i class="glyphicon glyphicon-search" data-toggle="modal" data-target="#diagSearchModule" style="cursor: pointer;" id="lnkbtnSearchModule" runat="server"></i>
                    <asp:TextBox ID="tbModule" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
                <asp:HiddenField ID="hfModuleId" runat="server" />
                <asp:RequiredFieldValidator ID="rfvModule" runat="server" ErrorMessage="Module cannot be empty." ControlToValidate="tbModule" Display="None"></asp:RequiredFieldValidator>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lb21" runat="server" Text="Module Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbModuleCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:Label ID="lb24" runat="server" Text="Class/Programme" Font-Bold="true"></asp:Label>
                <%-- consist of some programme info also --%>
                <asp:DropDownList ID="ddlBatch" runat="server" CssClass="form-control" CausesValidation="false" OnSelectedIndexChanged="ddlBatch_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvBatch" runat="server" ErrorMessage="Class cannot be empty." ControlToValidate="ddlBatch" Display="None"></asp:RequiredFieldValidator>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv">
                    <Columns>
                        <asp:BoundField HeaderText="Session ID" DataField="sessionId" />
                        <asp:BoundField HeaderText="Batch ID" DataField="batchModuleId" />
                        <asp:BoundField HeaderText="Period" DataField="sessionPeriod" />
                        <asp:BoundField HeaderText="Session No." DataField="sessionNumber" ItemStyle-Width="50px" />
                        <asp:BoundField HeaderText="Date" DataField="sessionDateDisp" ItemStyle-Width="150px" />
                        <asp:BoundField HeaderText="Period" DataField="sessionPeriodDisp" ItemStyle-Width="100px" />
                        <asp:BoundField HeaderText="Venue" DataField="venueLocation" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <br />
        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="Add" OnClick="btnAdd_Click" />
                <asp:Button ID="btnClear" runat="server" CssClass="btn btn-default" Text="Clear" OnClick="btnClear_Click" />
            </div>
        </div>
    </div>

    <uc1:modulesearch runat="server" ID="modulesearch" />
    <uc1:traineesearch runat="server" id="traineesearch" />
</asp:Content>
