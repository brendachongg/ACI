<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="programme-view.aspx.cs" Inherits="ACI_TMS.programme_view" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                <h3>
                    <asp:Label ID="lbProgrammeDetailHeader" runat="server" Text="Programme Details"></asp:Label>
                </h3>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" OnClick="lkbtnBack_Click" CssClass="btn btn-sm btn-default" Text="Back"></asp:LinkButton>
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

        <asp:HiddenField ID="hfProgrammeId" runat="server" />

        <div class="row text-left">
            <div class="col-lg-4">
                <asp:Label ID="lbProgrammeCategory" runat="server" Text="Category" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgrammeCategoryValue" CssClass="form-control" runat="server" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbProgrammeCode" runat="server" Text="Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgrammeCodeValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbProgrammeLevel" runat="server" Text="Level" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgrammeLevelValue" CssClass="form-control" runat="server" ReadOnly="true"></asp:Label>
            </div>

        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbCourseCode" runat="server" Text="Course Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbCourseCodeValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-9">
                <asp:Label ID="lbProgrammeTitle" runat="server" Text="Title" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgrammeTitleValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-3">
                <asp:Label ID="lbProgrammeType" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgrammeTypeValue" CssClass="form-control" runat="server" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbProgrammeVersion" runat="server" Text="Version" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbProgrammeVersionValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbNumOfSOA" runat="server" Text="No. Of SOA" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="lbNumofSOAValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbSSGRefNum" runat="server" Text="SSG Reference No." Font-Bold="true"></asp:Label>
                <asp:TextBox ID="lbSSGRefNumValue" runat="server" placeholder="" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbProgrammeDescription" runat="server" Text="Description" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="lbProgrammeDescriptionValue" runat="server" CssClass="form-control" ReadOnly="true" TextMode="MultiLine" Rows="5"></asp:TextBox>
            </div>
        </div>

        <br />
        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lblBundle" runat="server" Text="Bundle" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbBundleCodeValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                <asp:HiddenField ID="hfBundleId" runat="server" />
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <asp:GridView ID="gvModules" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="false" runat="server">
                    <Columns>
                        <asp:BoundField DataField="ModuleCode" HeaderText="Module" />
                        <asp:BoundField DataField="ModuleTitle" HeaderText="Title" />
                        <asp:BoundField DataField="ModuleNumOfSession" HeaderText="Num. of Sessions" ItemStyle-Width="100px" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbProgrammeImage" runat="server" Text="Programme Image" Font-Bold="true"></asp:Label>
            </div>

            <div class="col-lg-12">
                <asp:Label ID="lbProgrammeImageEmpty" runat="server" CssClass="form-control" ReadOnly="true" Text="No existing image"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Image ID="imgProgrammeImage" CssClass="img-responsive" runat="server" Style="width: 100%; height: 300px;" Visible="false" />
            </div>
        </div>

        <br />

        <div class="row text-right">
            <div class="col-lg-12">
                <asp:Button ID="btnUpdateCurrent" runat="server" CssClass="btn btn-primary" OnClick="btnUpdateCurrent_Click" Text="Edit" />
                <asp:Button ID="btnUpdateNew" runat="server" CssClass="btn btn-primary" OnClick="btnUpdateNew_Click" Text="Copy to new version" />
                <button id="btnConfirmDel" runat="server" type="button" class="btn btn-danger" data-toggle="modal" data-target="#diagDelete">Delete</button>
            </div>
        </div>
    </div>

    <div id="diagDelete" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete programme</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this programme?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnDelete_Click" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
