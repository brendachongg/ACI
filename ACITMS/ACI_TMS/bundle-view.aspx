<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="bundle-view.aspx.cs" Inherits="ACI_TMS.bundle_view" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">

        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbBundleCreationHeader" runat="server" Font-Bold="true" Text="View Bundle"></asp:Label>
                </h3>
            </div>

            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 text-right">
                <br />
                <asp:LinkButton ID="lkbtnBack" runat="server" CssClass="btn btn-sm btn-default" Text="Back" OnClick="lbtnBack_Click"></asp:LinkButton>
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

        <div class="row">
            <div class="col-lg-3">
                <asp:Label ID="lb3" runat="server" Text="Bundle Code" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbBundleCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
                <asp:HiddenField ID="hfBundleId" runat="server" />
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbl2" runat="server" Text="Type" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbBundleType" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lbl1" runat="server" Text="Effective Date" Font-Bold="true"></asp:Label>&nbsp;
                <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" data-placement="top" title="Automatically populated by latest selected modules' effective date"></i>
                <asp:TextBox ID="lbEffectiveDate" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="col-lg-3">
                <asp:Label ID="lb4" runat="server" Text="Cost" Font-Bold="true"></asp:Label>&nbsp;
                <i class="glyphicon glyphicon-info-sign" data-toggle="tooltip" data-placement="top" title="The sum of all selected module's cost"></i>
                <asp:TextBox ID="lbBundleCost" runat="server" CssClass="form-control" ReadOnly="true" Text="0"></asp:TextBox>
            </div>
        </div>

        <br />

        <fieldset>
            <legend style="font-size: 18px;">Module Details</legend>
            <div class="row">
                <div class="col-lg-12">
                    <asp:GridView ID="gvModule" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                        CssClass="table table-striped table-bordered dataTable no-footer hover gvv" >

                        <Columns>
                            <asp:BoundField HeaderText="Module Code" DataField="moduleCode" ItemStyle-Width="200px" />
                            <asp:BoundField HeaderText="Version" DataField="moduleVersion" ItemStyle-Width="80px" />
                            <asp:BoundField HeaderText="Level" DataField="moduleLevel" ItemStyle-Width="80px"/>
                            <asp:BoundField HeaderText="Title" DataField="moduleTitle" />
                            <asp:BoundField HeaderText="Effective Date" DataField="moduleEffectDate" ItemStyle-Width="150px" />
                            <asp:BoundField HeaderText="Cost" DataField="moduleCost" ItemStyle-Width="100px" />
                            <asp:BoundField HeaderText="Num. Of Session" DataField="ModuleNumOfSession" ItemStyle-Width="80px" />
                        </Columns>

                        <PagerStyle CssClass="pagination-ys" />

                    </asp:GridView>
                </div>
            </div>
            <hr />
        </fieldset>
        
        <div class="row text-right">
            <div class="col-lg-12">
               <asp:Button ID="btnEditBundle" runat="server" CssClass="btn btn-primary" Text="Edit" OnClick="btnEditBundle_Click" />
               <button type="button" class="btn btn-danger" data-toggle="modal" data-target="#diagRemBundle" id="btnRemBundle" runat="server">Delete</button>
            </div>
        </div>

    </div>

    <div id="diagRemBundle" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Delete Bundle</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this bundle? It will not be available for selection during programme creation.
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnOK" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnRemBundle_Click" CausesValidation="false" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
