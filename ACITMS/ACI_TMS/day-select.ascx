<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="day-select.ascx.cs" Inherits="ACI_TMS.day_select" %>
<style>
    .list{
        margin-top:-10px;
        margin-bottom:15px;
        margin-left:10px;
        margin-right:10px;
    }
</style>
<div id="diagSelectDay" class="modal fade" role="dialog">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Select Day</h4>
            </div>
            <div class="modal-body">
                <fieldset>
                    <legend style="font-size: 18px;">Monday</legend>
                    <div class="list">
                    <asp:CheckBoxList ID="cbListMon" runat="server" RepeatLayout="Flow" DataTextField="codeValueDisplay" DataValueField="codeValue" RepeatDirection="Horizontal" ></asp:CheckBoxList>
                    </div>
                </fieldset>
                <fieldset>
                    <legend style="font-size: 18px;">Tuesday</legend>
                    <div class="list">
                    <asp:CheckBoxList ID="cbListTue" runat="server" RepeatLayout="Flow" DataTextField="codeValueDisplay" DataValueField="codeValue" RepeatDirection="Horizontal" ></asp:CheckBoxList>
                    </div>
                </fieldset>
                 <fieldset>
                    <legend style="font-size: 18px;">Wednesday</legend>
                    <div class="list">
                    <asp:CheckBoxList ID="cbListWed" runat="server" RepeatLayout="Flow" CssClass="nobold" DataTextField="codeValueDisplay" DataValueField="codeValue" RepeatDirection="Horizontal" ></asp:CheckBoxList>
                    </div>
                </fieldset>
                 <fieldset>
                    <legend style="font-size: 18px;">Thursday</legend>
                    <div class="list">
                    <asp:CheckBoxList ID="cbListThu" runat="server" RepeatLayout="Flow" CssClass="nobold" DataTextField="codeValueDisplay" DataValueField="codeValue" RepeatDirection="Horizontal" ></asp:CheckBoxList>
                    </div>
                </fieldset>
                 <fieldset>
                    <legend style="font-size: 18px;">Friday</legend>
                    <div class="list">
                    <asp:CheckBoxList ID="cbListFri" runat="server" RepeatLayout="Flow" CssClass="nobold" DataTextField="codeValueDisplay" DataValueField="codeValue" RepeatDirection="Horizontal" ></asp:CheckBoxList>
                    </div>
                </fieldset>
                 <fieldset>
                    <legend style="font-size: 18px;">Saturday</legend>
                    <div class="list">
                    <asp:CheckBoxList ID="cbListSat" runat="server" RepeatLayout="Flow" CssClass="nobold" DataTextField="codeValueDisplay" DataValueField="codeValue" RepeatDirection="Horizontal" ></asp:CheckBoxList>
                    </div>
                </fieldset>
                <fieldset>
                    <legend style="font-size: 18px;">Sunday</legend>
                    <div class="list">
                    <asp:CheckBoxList ID="cbListSun" runat="server" RepeatLayout="Flow" CssClass="nobold" DataTextField="codeValueDisplay" DataValueField="codeValue" RepeatDirection="Horizontal" ></asp:CheckBoxList>
                    </div>
                </fieldset>
                <div class="row text-right">
                    <div class="col-lg-12">
                        <asp:Button ID="btnSelDay" runat="server" CssClass="btn btn-primary" Text="OK" CausesValidation="false" OnClick="btnSelDay_Click"  />
                        <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
