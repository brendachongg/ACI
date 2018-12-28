<%@ Page Title="" Language="C#" MasterPageFile="~/kiosk/aci-kiosk.Master" AutoEventWireup="true" CodeBehind="registration-success.aspx.cs" Inherits="ACI_TMS.kiosk.registration_success" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <div class="row text-center" style="margin-right:0px;">
        <asp:Image ID="img" runat="server" ImageUrl="~/Resource/images/success.jpg" Height="250" Width="250" border="0" />
        <h3 style="color:darkgreen;s">Your application has been submitted successfully.</h3>
        <h4>Your applicant ID is <asp:Label ID="lbAppId" runat="server"></asp:Label>. You may wish to process to the next counter.</h4>
        <h4>Thank you.</h4>
        <br />
        <asp:HyperLink ID="btnNew" runat="server" Text="New Registration" CssClass="btn btn-primary" NavigateUrl="~/kiosk/registration.aspx"></asp:HyperLink>
    </div>
</asp:Content>
