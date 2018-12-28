<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeFile="Registrationtest.aspx.cs" Inherits="Registrationtest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <div id="page-wrapper">
            <div class="col-lg-12">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="fullName" HeaderText="Full Name" />
            <asp:BoundField DataField="birthDate" HeaderText="Date Of Birth" />
            <asp:BoundField DataField="idType" HeaderText="Identification Type" />
            <asp:BoundField DataField="idNumber" HeaderText="Identification Number" />
            <asp:BoundField DataField="Programme_Category" HeaderText="Programme Category" />
            <asp:BoundField DataField="Available_Programme" HeaderText="Available Programme" />
            <asp:BoundField DataField="Class_Start_Date" HeaderText="Class Start Date" />
            <asp:BoundField DataField="selfSponsored" HeaderText="Sponsorship" />
            <asp:CommandField ShowEditButton="True" />
            <asp:CommandField ShowDeleteButton="True" />
        </Columns>

    </asp:GridView>
    <br />
    <asp:FileUpload ID="FileUpload2" runat="server" />
                <br />
                <asp:Label ID="lblError" runat="server" BackColor="Red"></asp:Label>
                <br />
                <asp:Label ID="lblSucc" runat="server" BackColor="Green"></asp:Label>
    <br />
                <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Load" />
&nbsp;
    <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="Button1_Click" />
    <br />
               </div>
            </div>
    </asp:Content>

