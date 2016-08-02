using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DiagnosticCenterBillManagementSystem.BusinessLogic;
using DiagnosticCenterBillManagementSystem.Models;
using System.Data;
using System.Text;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace DiagnosticCenterBillManagementSystem.UserInterface
{
    public partial class TestWiseReport : System.Web.UI.Page
    {
        TestReportManager testReportManager = new TestReportManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void showButton_Click(object sender, EventArgs e)
        {
            if (fromDateInut.Value != string.Empty && toDateInput.Value != string.Empty)
            {
                LoadTypeWiseReport();
                GetTotal();
            }
            else
            {
                return;
            }
        }

        protected void pdfId_Click(object sender, EventArgs e)
        {
            PdfGenerator();
        }

        private void LoadTypeWiseReport()
        {
            DateTime fromDate = Convert.ToDateTime(fromDateInut.Value);
            DateTime toDate = Convert.ToDateTime(toDateInput.Value);
            List<TestReport> testWiseReportsList = testReportManager.GetTypeWiseReport(fromDate, toDate);

            testWiseReportGridView.DataSource = testWiseReportsList;
            testWiseReportGridView.DataBind();
        }

        private void GetTotal()
        {
            double total = 0;
            for (int i = 0; i < testWiseReportGridView.Rows.Count; i++)
            {
                string amount = (testWiseReportGridView.Rows[i].FindControl("totalAmountLabel") as Label).Text;
                total += Convert.ToDouble(amount);
            }

            totalTextBox.Text = total.ToString();
        }

        private void PdfGenerator()
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < testWiseReportGridView.Columns.Count; i++)
            {
                dt.Columns.Add(testWiseReportGridView.Columns[i].ToString());
            }
            foreach (GridViewRow row in testWiseReportGridView.Rows)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < testWiseReportGridView.Columns.Count; j++)
                {
                    if (!row.Cells[j].Text.Equals("&nbsp;"))
                        dr[testWiseReportGridView.Columns[j].ToString()] = row.Cells[j].Text;
                }
                dt.Rows.Add(dr);
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<table width = '100%' cellspacing='0' cellpadding ='2'>");
            stringBuilder.Append("<tr><td align = 'centre'>Test Wise Report</td></tr></table>");
            stringBuilder.Append("</br>");

            stringBuilder.Append("<table border='1'>");
            stringBuilder.Append("<tr>");

            foreach (DataColumn header in dt.Columns)
            {
                stringBuilder.Append("<th style= 'background-color: green'>");
                stringBuilder.Append(header.ColumnName);
                stringBuilder.Append("</th>");
            }
            stringBuilder.Append("</tr>");

            foreach (DataRow row in dt.Rows)
            {
                stringBuilder.Append("<tr>");
                foreach (DataColumn column in dt.Columns)
                {
                    stringBuilder.Append("<td>");
                    stringBuilder.Append(row[column]);
                    stringBuilder.Append("</td>");
                }
                stringBuilder.Append("</tr>");
            }
            stringBuilder.Append("</table>");

            StringReader strReader = new StringReader(stringBuilder.ToString());
            Document pdfDocument = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
            HTMLWorker htmlPerser = new HTMLWorker(pdfDocument);
            PdfWriter.GetInstance(pdfDocument, Response.OutputStream);

            pdfDocument.Open();
            htmlPerser.Parse(strReader);
            pdfDocument.Close();

            Response.ContentType = "application/pdf";
            Response.AppendHeader("content-disposition", "attachment;filename=TestWiseReport.pdf");
            Response.Write(pdfDocument);
            Response.Flush();
            Response.End();
        }

    }
}