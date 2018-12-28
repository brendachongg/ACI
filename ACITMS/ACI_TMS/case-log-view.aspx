<%@ Page Title="" Language="C#" MasterPageFile="~/aci-dashboard.Master" AutoEventWireup="true" CodeBehind="case-log-view.aspx.cs" Inherits="ACI_TMS.case_log_view" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function checkFollowUpAttachmentSize(oSrc, args) {
            var allowedExtension = ['JPG', 'JPEG', 'PNG', 'PDF', 'DOC', 'DOCX', 'XLS', 'XLSX', 'CSV'];
            var uploadControl = document.getElementById('<%= fileUploadFollowUpAttachment.ClientID %>');
            var fileExtension = document.getElementById('<%= fileUploadFollowUpAttachment.ClientID %>').value.substring(document.getElementById('<%= fileUploadFollowUpAttachment.ClientID %>').value.lastIndexOf('.') + 1).toUpperCase();
            var checkSize = true;
            var checkExtension = false;

            var i = 0;

            for (i = 0; i < allowedExtension.length ; i++) {
                if (fileExtension == allowedExtension[i]) {
                    checkExtension = true;
                }
            }

            if (uploadControl.files[0].size > 2000000) {
                checkSize = false;
            }

            if (checkSize == false || checkExtension == false) {
                args.IsValid = false;
                return false;
            }
            else {
                args.IsValid = true;
                return true;
            }
        }

        var showAttachment = {
            GetFollowUpId: function (input) {   
                var convert_element = $(input);
                var element_btn = convert_element["0"].id;
                if (element_btn == "ContentPlaceHolder1_lbtnAttachment") {
                    return;
                }
                else {
                    var rowIndex = element_btn.split('_')[3];
                    var hiddenFieldID = '#ContentPlaceHolder1_gvFollowUp_hdfFollowUpId_' + rowIndex;
                    var element_field = $('#ContentPlaceHolder1_gvFollowUp').find('tbody').find('tr').find('td').find(hiddenFieldID);
                    var id = element_field["0"].value;
                    return id;
                }
            },

            Start: function (id, type, result) {
                var caseLogId = document.getElementById('<%=hdfCaseLogId.ClientID%>').value;
                var followUpId = showAttachment.GetFollowUpId(result);
                if (type == 'CASELOG') {
                    window.open('<%= ACI_TMS.case_log_view_attachment.PAGE_NAME %>?<%= ACI_TMS.case_log_view_attachment.CASELOG_QUERY %>=' + encodeURI(id)
                    + '&<%= ACI_TMS.case_log_view_attachment.TYPE_QUERY %>=' + encodeURI(type), '_blank', 'menubar=no,location=no,scrollbars=yes,resizable=yes');
                } else if (type == 'FOLLOWUP') {
                    window.open('<%= ACI_TMS.case_log_view_attachment.PAGE_NAME %>?<%= ACI_TMS.case_log_view_attachment.FOLLOWUP_QUERY %>=' + encodeURI(id)
                        + '&<%= ACI_TMS.case_log_view_attachment.TYPE_QUERY %>=' + encodeURI(type), '_blank', 'menubar=no,location=no,scrollbars=yes,resizable=yes');
                }
            }
        };

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="page-wrapper">
        <div class="row text-left">
            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                <h3>
                    <asp:Label ID="lbCaseLogDetailHeader" runat="server" Text="View/Reply Case Log Details"></asp:Label>
                </h3>
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


        <asp:HiddenField ID="hdfCaseLogId" runat="server" />
        <asp:HiddenField ID="hdfStatus" runat="server" />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbCaseLogStatus" runat="server" Text="Status:" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbCaseLogStatusValue" CssClass="form-control" runat="server" ReadOnly="true"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-6">
                <asp:Label ID="lbCaseLogCategory" runat="server" Text="Category:" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbCaseLogCategoryValue" CssClass="form-control" runat="server" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-6">
                <asp:Label ID="lbCaseLogSubject" runat="server" Text="Subject:" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbCaseLogSubjectValue" runat="server" CssClass="form-control" ReadOnly="true"></asp:Label>
            </div>

        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-4">
                <asp:Label ID="lbCaseLogIncidentDate" runat="server" Text="Incident Date:" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbCaseLogIncidentDateValue" CssClass="form-control" runat="server" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbCaseLogIncidentTime" runat="server" Text="Incident Time:" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbCaseLogIncidentTimeValue" CssClass="form-control" runat="server" ReadOnly="true"></asp:Label>
            </div>

            <div class="col-lg-4">
                <asp:Label ID="lbReportedBy" runat="server" Text="Reported By:" Font-Bold="true"></asp:Label>
                <asp:Label ID="lbReportedByValue" CssClass="form-control" runat="server" ReadOnly="true"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbCaseLogMessage" runat="server" Text="Description:" Font-Bold="true"></asp:Label>
                <asp:TextBox ID="tbCaseLogMessageValue" runat="server" CssClass="form-control" ReadOnly="true" TextMode="MultiLine" Rows="5"></asp:TextBox>
            </div>
        </div>

        <br />

        <div class="row text-left">
            <div class="col-lg-12">
                <asp:Label ID="lbCaseLogAttachment" runat="server" Text="Attachments:" Font-Bold="true"></asp:Label>
            </div>

            <div class="col-lg-12">
                <asp:Label ID="lbCaseLogAttachmentEmpty" runat="server" CssClass="form-control" ReadOnly="true" Text="N/A"></asp:Label>
                <asp:Label ID="lbtnAttachment" runat="server" CssClass="glyphicon glyphicon-file" Style="font-size: 20px;" ForeColor="green" data-toggle="tooltip" data-placement="top" title="View Attachment"></asp:Label>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-lg-12">
                <h3>
                    <asp:Label ID="lblReplies" runat="server" Text="Replies"></asp:Label>
                </h3>

                <asp:GridView ID="gvFollowUp" AutoGenerateColumns="False" ShowHeaderWhenEmpty="true"
                    CssClass="table table-striped table-bordered dataTable no-footer hover gvv" AllowPaging="True" PageSize="10" runat="server" OnRowDataBound="gvFollowUp_RowDataBound" OnPageIndexChanging="gvFollowUp_PageIndexChanging">
                    <EmptyDataTemplate>
                        No Follow Up Records.
                    </EmptyDataTemplate>

                    <Columns>

                        <%-- Follow Up Reply Date --%>
                        <asp:TemplateField HeaderText="Replied Date" ItemStyle-Width="200px">
                            <ItemTemplate>

                                <asp:Label ID="lbCaseLogStatus" runat="server" Text='<%# Eval("repliedOn") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Follow Up Replied By --%>
                        <asp:TemplateField HeaderText="Replied By" ItemStyle-Width="100px">
                            <ItemTemplate>

                                <asp:Label ID="lbReportedBy" runat="server" Text='<%# Eval("userName") %>'></asp:Label>

                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Follow Up Message--%>
                        <asp:TemplateField HeaderText="Message">
                            <ItemTemplate>
                                <asp:Label ID="lbCaseLogMessage" runat="server" Text='<%# Eval("message") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%-- Follow Up Attachment --%>
                        <asp:TemplateField HeaderText="Attachment">
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfFollowUpId" runat="server" />
                                <asp:Label ID="lbtnAttachment" runat="server" CssClass="glyphicon glyphicon-file" Style="font-size: 20px;" ForeColor="green" data-toggle="tooltip" data-placement="top" title="View Attachment"></asp:Label>
                                <asp:Label ID="lbFollowUpAttachmentEmpty" runat="server" CssClass="form-control" ReadOnly="true" Text="No existing attachments"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <PagerStyle CssClass="pagination-ys" />
                </asp:GridView>
            </div>
        </div>

        <div class="row text-right">
            <div class="col-lg-12">
                <button id="btnReply" runat="server" type="button" class="btn btn-primary" data-toggle="modal" data-target="#diagReply" visible="false">Reply Case</button>
                <button id="btnConfirmDel" runat="server" type="button" class="btn btn-danger" data-toggle="modal" data-target="#diagClose" visible="false">Close Case</button>
                <button id="btnReopen" runat="server" type="button" class="btn btn-danger" data-toggle="modal" data-target="#diagReopen" visible="false">Reopen Case</button>
            </div>
        </div>

    </div>

    <!--Reply Modal-->

    <div id="diagReply" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <asp:ValidationSummary ID="vSummary" runat="server" CssClass="alert alert-danger alert-link" HeaderText="Please correct the following:" />
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Reply Caselog</h4>
                </div>

                <div class="modal-body">
                    <div class="row text-left">
                        <div class="col-lg-12">
                            <asp:Label ID="lbReplyBox" runat="server" Text="Message:" Font-Bold="true"></asp:Label>
                            <asp:TextBox ID="tbReplyBoxValue" runat="server" MaxLength="255" CssClass="form-control" TextMode="MultiLine" Rows="5"></asp:TextBox>

                            <asp:RequiredFieldValidator ID="replyBoxRequiredValidator" Display="None" ForeColor="Red" ControlToValidate="tbReplyBoxValue" runat="server" ErrorMessage="Please fill in the follow up message"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <br />

                    <div class="row text-left">
                        <div class="col-lg-12">
                            <asp:Label ID="lbAttachment" runat="server" Text="Attachment:" Font-Bold="true"></asp:Label>

                            <i class="glyphicon glyphicon-question-sign" data-toggle="tooltip" data-placement="top" title="Only .JPG, .JPEG, .PNG, .PDF, .DOC, .DOCX with maximum size of 2MB is allowed. For cases of multiple images, please upload it as a .DOC, .DOCX or .PDF"></i>

                            <asp:FileUpload ID="fileUploadFollowUpAttachment" runat="server" CssClass="form-control" />

                            <asp:CustomValidator ID="cvCaseLogReplyAttachment1" runat="server" Display="None" ControlToValidate="fileUploadFollowUpAttachment"
                                ErrorMessage="File exceeded the size limit OR file is not in proper format." ValidateEmptyText="false" ClientValidationFunction="checkFollowUpAttachmentSize"></asp:CustomValidator>
                        </div>
                    </div>
                </div>

                <br />

                <div class="row text-left">
                    <div class="col-lg-12">
                        <asp:Image ID="imgCaseLogReplyAttachment" runat="server" Style="width: 100%; height: 300px; display: none;" />
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmReply" runat="server" CssClass="btn btn-primary" Text="Reply" OnClick="btnConfirmReply_Click" />
                    <asp:Button ID="btnClear" runat="server" class="btn btn-default" Text="Clear" CausesValidation="false" OnClick="btnClear_Click" />
                </div>
            </div>
        </div>
    </div>


    <!--Close case Modal-->
    <div id="diagClose" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Close Caselog</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to close this case log?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnClose" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnClose_Click" CausesValidation="false" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <!--Reopen case Modal-->
    <div id="diagReopen" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Reopen Caselog</h4>
                </div>
                <div class="modal-body">
                    Are you sure you want to reopen this case log?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnReopenCase" runat="server" CssClass="btn btn-primary" Text="OK" OnClick="btnReopenCase_Click" CausesValidation="false" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
