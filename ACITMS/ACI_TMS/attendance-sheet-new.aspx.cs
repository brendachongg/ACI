using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ACI_TMS
{
    public partial class attendance_sheet : BasePage
    {
        public const string PAGE_NAME = "attendance-sheet-new.aspx";
        public const string BATCH_QUERY = "bid";
        public const string MODE_QUERY = "m";

        private int maxCol = Int32.Parse(ConfigurationManager.AppSettings["maxCol"]);

        private Attendance_Management am = new Attendance_Management();

        public attendance_sheet()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.ATTENDANCE_VIEW, AccessRight_Constance.ATTENDANCE_VIEW_ALL }, attendance_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[BATCH_QUERY] == null || Request.QueryString[BATCH_QUERY] == "" || Request.QueryString[MODE_QUERY] == null || (Request.QueryString[MODE_QUERY] != "M" && Request.QueryString[MODE_QUERY] != "I"))
                {
                    redirectToErrorPg("Missing information.");
                    return;
                }

                if (!checkAccessRights(AccessRight_Constance.ATTENDANCE_VIEW_ALL) && !(new Attendance_Management()).isOwnBatchModule(LoginID, int.Parse(HttpUtility.UrlDecode(Request.QueryString[BATCH_QUERY]))))
                {
                    redirectToErrorPg("You are not authorized to download this module attendance list.");
                    return;
                }

                exportAttendance(int.Parse(HttpUtility.UrlDecode(Request.QueryString[BATCH_QUERY])), HttpUtility.UrlDecode(Request.QueryString[MODE_QUERY]));
            }
        }

        private void exportAttendance(int batchModuleId, string mode)
        {
            // if normal attendance, get main enrollment trainees, else get inserted trainees for all sessions of the batch module
            Tuple<DataTable, DataTable> batchTable = null;
            DataTable dtInsertedTrainees = null;

            if (mode == "M")
            {
                batchTable = am.getSessionDetails(batchModuleId);
            }
            else
            {
                batchTable = am.getInsertedSessionDetails(batchModuleId);
                dtInsertedTrainees = am.getInsertedSessionTrainees(batchModuleId);
            }

            if (batchTable == null || (mode == "I" && dtInsertedTrainees == null))
            {
                redirectToErrorPg("Unable to retrieve trainee details.");
                return;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    Document doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    DataRow drModInfo = batchTable.Item1.Rows[0];

                    string classCode = drModInfo["batchCode"].ToString();
                    string moduleCode = drModInfo["moduleCode"].ToString();

                    PdfPTable logoHeader = new PdfPTable(1);
                    logoHeader.WidthPercentage = 15;
                    logoHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                    iTextSharp.text.Image aci_logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~\\Resource\\images\\ACI_logo_small.png"));
                    PdfPCell logocell = new PdfPCell(aci_logo, true); //  **PdfPCell(Image,Boolean Fit)**


                    logocell.Border = Rectangle.NO_BORDER;
                    logoHeader.AddCell(logocell);
                   
                    Font dateTitle = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, Font.NORMAL | Font.ITALIC , BaseColor.BLACK);
                    Paragraph dateGenerated = new Paragraph("Generated on: " + DateTime.Now.ToString("dd/MMM/yyyy HH:MM:ss"), dateTitle);
                    dateGenerated.Alignment = Element.ALIGN_RIGHT & Element.ALIGN_BOTTOM;
                    dateGenerated.SpacingBefore = 100;
                   

                    //create the header content
                    PdfPTable tblHeader = new PdfPTable(new float[] { 2, 4, 1, 1 });
                    tblHeader.WidthPercentage = 100;
                    tblHeader.DefaultCell.Border = Rectangle.NO_BORDER;
                    Font fLabel = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11f, Font.BOLD, BaseColor.BLACK);
                    Font fValue = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11f, Font.NORMAL, BaseColor.BLACK);

                    //iTextSharp.text.Image aci_logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~\\Resource\\images\\ACI_logo_small.png"));
                    //PdfPCell logocell = new PdfPCell(aci_logo, true); //  **PdfPCell(Image,Boolean Fit)** 
         

                    //logocell.Border = Rectangle.NO_BORDER;
                    //tblHeader.AddCell(logocell);
                    //tblHeader.AddCell(new Phrase());
                    //tblHeader.AddCell(new Phrase());
                    //tblHeader.AddCell(new Phrase());

                    tblHeader.AddCell(new Phrase("Module Title:", fLabel));
                    tblHeader.AddCell(new Phrase(drModInfo["moduleTitle"].ToString(), fLabel));
                    tblHeader.AddCell(new Phrase("Class Code:", fLabel));
                    tblHeader.AddCell(new Phrase(classCode, fValue));

                    tblHeader.AddCell(new Phrase("Module Start Date:", fLabel));
                    tblHeader.AddCell(new Phrase(batchTable.Item1.Rows[0]["sessionDateDisp"].ToString(), fValue));
                    tblHeader.AddCell(new Phrase("Course Code:", fLabel));
                    tblHeader.AddCell(new Phrase(drModInfo["courseCode"].ToString(), fValue));

                    tblHeader.AddCell(new Phrase("Module End Date:", fLabel));
                    tblHeader.AddCell(new Phrase(batchTable.Item1.Rows[batchTable.Item1.Rows.Count - 1]["sessionDateDisp"].ToString(), fValue));
                    tblHeader.AddCell(new Phrase("Project Code: ", fLabel));
                    tblHeader.AddCell(new Phrase(drModInfo["projectCode"].ToString(), fValue));

                    tblHeader.AddCell(new Phrase("Total Duration (In Hours):", fLabel));
                    tblHeader.AddCell(new Phrase(drModInfo["moduleTrainingHour"].ToString(), fValue));
                    tblHeader.AddCell(" ");
                    tblHeader.AddCell(" ");

                    tblHeader.AddCell(new Phrase("Trainer:", fLabel));
                    tblHeader.AddCell(new Phrase(drModInfo["trainerUserName1"].ToString(), fValue));
                    tblHeader.AddCell(" ");
                    tblHeader.AddCell(" ");

                    Font fTitle = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14f, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                    Paragraph title = new Paragraph("Attendance List", fTitle);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingAfter = 20;

                    List<float> tblWidths = new List<float>();
                    tblWidths.Add(1);
                    tblWidths.Add(6);
                    tblWidths.Add(4);
                    for (int i = 0; i < maxCol + 3; i++) tblWidths.Add(3);


                    PdfPCell cell = null;
                    Font fContentLabel = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, Font.BOLD, BaseColor.BLACK);
                    Font fContentValue = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, Font.NORMAL, BaseColor.BLACK);
                    int noOfPages = (int)Math.Ceiling(batchTable.Item1.Rows.Count / 6d);
                    for (int pg = 0; pg < noOfPages; pg++)
                    {
                        if (pg > 0) doc.NewPage();

                        doc.Add(logoHeader);
                        doc.Add(title);
                        doc.Add(tblHeader);
                        doc.Add(new Paragraph(" "));

                        PdfPTable tblContent = new PdfPTable(tblWidths.ToArray());
                        tblContent.WidthPercentage = 100;

                        addHeader(tblContent, batchTable.Item1, pg, drModInfo["assessorUserName"].ToString());

                         //+ " " + dr["batchCode"] + " " + dr["courseCode"] + " " + dr["projectCode"]

                        #region CommentedOFF
                        //#region header1stRow
                        //tblContent.AddCell(new PdfPCell(new Phrase("S/N", fContentLabel)) { Rowspan = 3, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                        //tblContent.AddCell(new PdfPCell(new Phrase("Name", fContentLabel)) { Rowspan = 3, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                        //tblContent.AddCell(new PdfPCell(new Phrase("NRIC/PP No.", fContentLabel)) { Rowspan = 3, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });

                        ////create the session date cell and merge when necessary
                        //cell = new PdfPCell(new Phrase(batchTable.Item1.Rows[pg * maxCol]["sessionDateDisp"].ToString(), fContentValue)) { HorizontalAlignment = Element.ALIGN_CENTER };
                        //string prevSessionDate = batchTable.Item1.Rows[pg * maxCol]["sessionDateDisp"].ToString();
                        //int mergeCol = 0;
                        //for (int cnt = pg * maxCol; cnt < pg * maxCol + maxCol; cnt++)
                        //{
                        //    //for the last page (or first page if no of session less than max col) when the session is less than max col
                        //    if (batchTable.Item1.Rows.Count <= cnt)
                        //    {
                        //        if (cell != null)
                        //        {
                        //            //add the previous session date cell
                        //            if (mergeCol > 1) cell.Colspan = mergeCol;
                        //            tblContent.AddCell(cell);
                        //            cell = null;
                        //        }

                        //        tblContent.AddCell(" ");
                        //        continue;
                        //    }

                        //    if (prevSessionDate != batchTable.Item1.Rows[cnt]["sessionDateDisp"].ToString())
                        //    {
                        //        //add the previous session date cell
                        //        if (mergeCol > 1) cell.Colspan = mergeCol;
                        //        tblContent.AddCell(cell);

                        //        //create new cell and reset values
                        //        cell = new PdfPCell(new Phrase(batchTable.Item1.Rows[cnt]["sessionDateDisp"].ToString(), fContentValue)) { HorizontalAlignment = Element.ALIGN_CENTER};
                        //        prevSessionDate = batchTable.Item1.Rows[cnt]["sessionDateDisp"].ToString();
                        //        mergeCol = 1;
                        //    }
                        //    else mergeCol++;
                        //}

                        //if (cell != null)
                        //{
                        //    //add the previous session date cell
                        //    if (mergeCol > 1) cell.Colspan = mergeCol;
                        //    tblContent.AddCell(cell);
                        //}

                        //tblContent.AddCell(new PdfPCell(new Phrase("Assessor: " + drModInfo["assessorUserName"].ToString(), fContentLabel)) { Colspan = 3, HorizontalAlignment = Element.ALIGN_CENTER });
                        //#endregion

                        //#region header2ndRow
                        ////create the session period column
                        //for (int cnt = pg * maxCol; cnt < pg * maxCol + maxCol; cnt++)
                        //{
                        //    //for the last page (or first page if no of session less than max col) when the session is less than max col
                        //    if (batchTable.Item1.Rows.Count <= cnt)
                        //    {
                        //        tblContent.AddCell(" ");
                        //        continue;
                        //    }

                        //    tblContent.AddCell(new PdfPCell(new Phrase(batchTable.Item1.Rows[cnt]["sessionPeriodDisp"].ToString(), fContentLabel)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        //}

                        //tblContent.AddCell(new PdfPCell(new Phrase("Assessment Date", fContentLabel)) { Rowspan = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                        //tblContent.AddCell(new PdfPCell(new Phrase("Results (C/NYC)", fContentLabel)) { Rowspan = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                        //tblContent.AddCell(new PdfPCell(new Phrase("Assessor's Signature", fContentLabel)) { Rowspan = 2, HorizontalAlignment = Element.ALIGN_CENTER });
                        //#endregion

                        //#region header3rdRow
                        //for (int cnt = pg * maxCol; cnt < pg * maxCol + maxCol; cnt++)
                        //{
                        //    //for the last page (or first page if no of session less than max col) when the session is less than max col
                        //    if (batchTable.Item1.Rows.Count <= cnt)
                        //    {
                        //        tblContent.AddCell(" ");
                        //        continue;
                        //    }

                        //    tblContent.AddCell(new PdfPCell(new Phrase(batchTable.Item1.Rows[cnt]["venueLocation"].ToString(), fContentLabel)) 
                        //        { HorizontalAlignment=Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_TOP });
                        //}
                        //#endregion
                        #endregion

                        #region traineeRows
                        int n = 1, n_pg = n;
                        foreach (DataRow dr in batchTable.Item2.Rows)
                        {
                            //if trainee exceeds more than 15, then create new page with header to continue writing
                            if (n_pg > 15)
                            {
                                doc.Add(tblContent);
                                doc.NewPage();

                                tblContent = new PdfPTable(tblWidths.ToArray());
                                tblContent.WidthPercentage = 100;

                                addHeader(tblContent, batchTable.Item1, pg, drModInfo["assessorUserName"].ToString());

                                n_pg = 1;
                            }

                            tblContent.AddCell(new PdfPCell(new Phrase(n.ToString(), fContentValue)) { VerticalAlignment = Element.ALIGN_TOP });

                            string name = mode == "I" ? dr["fullName"].ToString() + "/ " + dr["batchCode"] + "/ " + dr["courseCode"] + "/ " + dr["projectCode"] : dr["fullName"].ToString();
                            tblContent.AddCell(new PdfPCell(new Phrase(name, fContentValue)));
                            tblContent.AddCell(new PdfPCell(new Phrase(dr["idNumberMasked"].ToString(), fContentValue)));
                            n++;
                            n_pg++;

                            for (int c = 0; c < maxCol + 3; c++)
                            {
                                //for inserted trainees, check if should gray out the cell(s)
                                cell = new PdfPCell(new Phrase(" "));
                                if (mode == "I" && dr["type"].ToString() == "SI")
                                {
                                    //for sit in trainees only gray out the assments cells
                                    if (c >= maxCol)
                                    {
                                        cell.BackgroundColor = BaseColor.GRAY;
                                        Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                                        cell.AddElement(p);
                                    }
                                }
                                else if (mode == "I" && c < maxCol && batchTable.Item1.Rows.Count > pg * maxCol + c)
                                {
                                    //only if generating inserted attendance sheet and
                                    //is not at the assessment columns and is within the available sessions
                                    if (dtInsertedTrainees.Select("sessionDate='" + batchTable.Item1.Rows[pg * maxCol + c]["sessionDate"].ToString() + "' "
                                        + "and sessionPeriod='" + batchTable.Item1.Rows[pg * maxCol + c]["sessionPeriod"].ToString() + "' "
                                        + "and idNumber = '" + dr["idNumber"].ToString() + "'").Length == 0)
                                    {
                                        cell.BackgroundColor = BaseColor.GRAY;
                                        Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                                        cell.AddElement(p);
                                    }
                                }

                                tblContent.AddCell(cell);
                            }
                        }
                        #endregion

                        #region endRows
                        tblContent.AddCell(new PdfPCell(new Phrase("Total No. Of Participants", fContentLabel)) { Colspan = 3, HorizontalAlignment = Element.ALIGN_RIGHT });
                        for (int c = 0; c < maxCol + 3; c++) tblContent.AddCell(" ");

                        tblContent.AddCell(new PdfPCell(new Phrase("Signature of Trainer", fContentLabel)) { Colspan = 3, HorizontalAlignment = Element.ALIGN_RIGHT });
                        for (int c = 0; c < maxCol + 3; c++) tblContent.AddCell(" ");

                        tblContent.AddCell(new PdfPCell(new Phrase("Name of Trainer", fContentLabel)) { Colspan = 3, HorizontalAlignment = Element.ALIGN_RIGHT });
                        for (int c = 0; c < maxCol + 3; c++) tblContent.AddCell(" ");
                        #endregion

                        doc.Add(tblContent);
                        doc.Add(dateGenerated);
                    }

                    doc.Close();
                    byte[] data = ms.ToArray();
                    ms.Close();

                    string fileHeaderName = classCode.Replace(@"/", "-") + "_" + moduleCode + "_" + (mode == "M" ? "Enrolled" : "Inserted");
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileHeaderName + ".pdf;");
                    Response.Buffer = true;
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(data);

                    //use the following instead of response.end to prevent thread abort exception error
                    HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
                    HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                    HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, PAGE_NAME, "exportAttendance()", ex.Message, -1);

                    redirectToErrorPg("Error generating attendance sheet.");
                }
                finally
                {
                    if (ms != null) ms.Close();
                }
            }
        }


        private void addHeader(PdfPTable tblContent, DataTable dtSessions, int pg, string accessorName)
        {
            PdfPCell cell = null;
            Font fContentLabel = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, Font.BOLD, BaseColor.BLACK);
            Font fContentValue = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, Font.NORMAL, BaseColor.BLACK);

            #region header1stRow
            tblContent.AddCell(new PdfPCell(new Phrase("S/N", fContentLabel)) { Rowspan = 3, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            tblContent.AddCell(new PdfPCell(new Phrase("Name", fContentLabel)) { Rowspan = 3, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            tblContent.AddCell(new PdfPCell(new Phrase("NRIC/PP No.", fContentLabel)) { Rowspan = 3, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });

            //create the session date cell and merge when necessary
            cell = new PdfPCell(new Phrase(dtSessions.Rows[pg * maxCol]["sessionDateDisp"].ToString(), fContentValue)) { HorizontalAlignment = Element.ALIGN_CENTER };
            string prevSessionDate = dtSessions.Rows[pg * maxCol]["sessionDateDisp"].ToString();
            int mergeCol = 0;
            for (int cnt = pg * maxCol; cnt < pg * maxCol + maxCol; cnt++)
            {
                //for the last page (or first page if no of session less than max col) when the session is less than max col
                if (dtSessions.Rows.Count <= cnt)
                {
                    if (cell != null)
                    {
                        //add the previous session date cell
                        if (mergeCol > 1) cell.Colspan = mergeCol;
                        tblContent.AddCell(cell);
                        cell = null;
                    }

                    tblContent.AddCell(" ");
                    continue;
                }

                if (prevSessionDate != dtSessions.Rows[cnt]["sessionDateDisp"].ToString())
                {
                    //add the previous session date cell
                    if (mergeCol > 1) cell.Colspan = mergeCol;
                    tblContent.AddCell(cell);

                    //create new cell and reset values
                    cell = new PdfPCell(new Phrase(dtSessions.Rows[cnt]["sessionDateDisp"].ToString(), fContentValue)) { HorizontalAlignment = Element.ALIGN_CENTER };
                    prevSessionDate = dtSessions.Rows[cnt]["sessionDateDisp"].ToString();
                    mergeCol = 1;
                }
                else mergeCol++;
            }

            if (cell != null)
            {
                //add the previous session date cell
                if (mergeCol > 1) cell.Colspan = mergeCol;
                tblContent.AddCell(cell);
            }

            tblContent.AddCell(new PdfPCell(new Phrase("Assessor: " + accessorName, fContentLabel)) { Colspan = 3, HorizontalAlignment = Element.ALIGN_CENTER });
            #endregion

            #region header2ndRow
            //create the session period column
            for (int cnt = pg * maxCol; cnt < pg * maxCol + maxCol; cnt++)
            {
                //for the last page (or first page if no of session less than max col) when the session is less than max col
                if (dtSessions.Rows.Count <= cnt)
                {
                    tblContent.AddCell(" ");
                    continue;
                }

                tblContent.AddCell(new PdfPCell(new Phrase(dtSessions.Rows[cnt]["sessionPeriodDisp"].ToString(), fContentLabel)) { HorizontalAlignment = Element.ALIGN_CENTER });
            }

            tblContent.AddCell(new PdfPCell(new Phrase("Assessment Date", fContentLabel)) { Rowspan = 2, HorizontalAlignment = Element.ALIGN_CENTER });
            tblContent.AddCell(new PdfPCell(new Phrase("Results (C/NYC)", fContentLabel)) { Rowspan = 2, HorizontalAlignment = Element.ALIGN_CENTER });
            tblContent.AddCell(new PdfPCell(new Phrase("Assessor's Signature", fContentLabel)) { Rowspan = 2, HorizontalAlignment = Element.ALIGN_CENTER });
            #endregion

            #region header3rdRow
            for (int cnt = pg * maxCol; cnt < pg * maxCol + maxCol; cnt++)
            {
                //for the last page (or first page if no of session less than max col) when the session is less than max col
                if (dtSessions.Rows.Count <= cnt)
                {
                    tblContent.AddCell(" ");
                    continue;
                }

                tblContent.AddCell(new PdfPCell(new Phrase(dtSessions.Rows[cnt]["venueLocation"].ToString(), fContentLabel)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_TOP });
            }
            #endregion
        }
    }
}