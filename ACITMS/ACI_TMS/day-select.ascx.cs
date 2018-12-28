using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class day_select : System.Web.UI.UserControl
    {
        public delegate void SelectDay(string strValue, string strDisp);
        public event SelectDay selectDay;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    DataTable dtPeriods = (new Venue_Management()).getPeriods();

                    //add spacing between labels
                    foreach (DataRow dr in dtPeriods.Rows)
                    {
                        dr["codeValueDisplay"] = "&nbsp;" + dr["codeValueDisplay"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    }

                    cbListMon.DataSource = dtPeriods;
                    cbListMon.DataBind();
                    cbListTue.DataSource = dtPeriods;
                    cbListTue.DataBind();
                    cbListWed.DataSource = dtPeriods;
                    cbListWed.DataBind();
                    cbListThu.DataSource = dtPeriods;
                    cbListThu.DataBind();
                    cbListFri.DataSource = dtPeriods;
                    cbListFri.DataBind();
                    cbListSat.DataSource = dtPeriods;
                    cbListSat.DataBind();
                    cbListSun.DataSource = dtPeriods;
                    cbListSun.DataBind();
                }
                catch (Exception ex)
                {
                    string n=ex.StackTrace;
                }
            }
        }

        protected void btnSelDay_Click(object sender, EventArgs e)
        {
            //get what the user has selected
            string selValue = "", selDisp = "";

            Tuple<string, string> t = getSelectedPeriod(cbListMon, "Mon", 1);
            selValue += t.Item1;
            selDisp += t.Item2;

            t = getSelectedPeriod(cbListTue, "Tue", 2);
            if (t.Item1 != "")
            {
                selValue += (selValue == "" ? "" : ";") + t.Item1;
                selDisp += (selDisp == "" ? "" : ", ") + t.Item2;
            }
            
            t = getSelectedPeriod(cbListWed, "Wed", 3);
            if (t.Item1 != "")
            {
                selValue += (selValue == "" ? "" : ";") + t.Item1;
                selDisp += (selDisp == "" ? "" : ", ") + t.Item2;
            }

            t = getSelectedPeriod(cbListThu, "Thu", 4);
            if (t.Item1 != "")
            {
                selValue += (selValue == "" ? "" : ";") + t.Item1;
                selDisp += (selDisp == "" ? "" : ", ") + t.Item2;
            }

            t = getSelectedPeriod(cbListFri, "Fri", 5);
            if (t.Item1 != "")
            {
                selValue += (selValue == "" ? "" : ";") + t.Item1;
                selDisp += (selDisp == "" ? "" : ", ") + t.Item2;
            }

            t = getSelectedPeriod(cbListSat, "Sat", 6);
            if (t.Item1 != "")
            {
                selValue += (selValue == "" ? "" : ";") + t.Item1;
                selDisp += (selDisp == "" ? "" : ", ") + t.Item2;
            }

            t = getSelectedPeriod(cbListSun, "Sun", 7);
            if (t.Item1 != "")
            {
                selValue += (selValue == "" ? "" : ";") + t.Item1;
                selDisp += (selDisp == "" ? "" : ", ") + t.Item2;
            }

            selectDay(selValue, selDisp);
        }

        private Tuple<string, string> getSelectedPeriod(CheckBoxList cbl, string dayStr, int day)
        {
            string value = "", disp = "";
            foreach (ListItem i in cbl.Items)
            {
                if (i.Selected)
                {
                    value += day + "/" + i.Value + ";";
                    disp += dayStr + "(" + i.Text.Replace("&nbsp;", "") + "), ";
                    //reset it
                    i.Selected = false;
                } 
            }

            if (value != "")
            {
                value = value.Substring(0, value.Length - 1);
                disp = disp.Substring(0, disp.Length - 2);
            }

            return new Tuple<string, string>(value, disp);
        }
    }
}