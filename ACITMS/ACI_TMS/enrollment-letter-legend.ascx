<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="enrollment-letter-legend.ascx.cs" Inherits="ACI_TMS.enrollment_letter_legend" %>
<div id="diagELengend" class="modal fade" role="dialog">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Enrollment Template/Letter Legend</h4>
            </div>
            <div class="modal-body">
                <div class="row text-left">
                    <div class="col-lg-4">
                        <b>Label</b>
                    </div>
                    <div class="col-lg-8">
                        <b>Value</b> <i>(will be replace with actual details when letter is generated)</i>
                    </div>
                </div>
                <div class="row text-left">
                    <div class="col-lg-4">
                        @name
                    </div>
                    <div class="col-lg-8">
                        Applicant's Name
                    </div>
                </div>
                <div class="row text-left">
                    <div class="col-lg-4">
                        @idNumber
                    </div>
                    <div class="col-lg-8">
                        Applicant's ID Number
                    </div>
                </div>
                <div class="row text-left">
                    <div class="col-lg-4">
                        @programme_title
                    </div>
                    <div class="col-lg-8">
                        Programe Name
                    </div>
                </div>
                <div class="row text-left">
                    <div class="col-lg-4">
                        @current_date
                    </div>
                    <div class="col-lg-8">
                        Today Date
                    </div>
                </div>
                <div class="row text-left">
                    <div class="col-lg-4">
                        @class_start_date
                    </div>
                    <div class="col-lg-8">
                        Class Commencement Date
                    </div>
                </div>
                <div class="row text-left">
                    <div class="col-lg-4">
                        @class_end_date
                    </div>
                    <div class="col-lg-8">
                        Class End Date
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>