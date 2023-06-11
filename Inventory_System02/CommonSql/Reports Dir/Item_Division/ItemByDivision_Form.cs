﻿using Inventory_System02.Includes;
using Inventory_System02.Reports_Dir;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CheckBox = System.Windows.Forms.CheckBox;
using DataTable = System.Data.DataTable;

namespace Inventory_System02.CommonSql.Reports_Dir.Item_Division
{
    public partial class ItemByDivision_Form : Form
    {
        SQLConfig config;
        DataSet ds;
        ReportDataSource rs;
        ReportParameterCollection reportParameters;
        Report_Viewer frm;
        usableFunction func;
        List<Class_Item_Var> list2;

        string sql;
        string company_name = string.Empty;
        string company_address = string.Empty;
        string db_table = string.Empty;
        string datef = string.Empty;
        string FileName = string.Empty;

        public ItemByDivision_Form()
        {
            InitializeComponent();
        }
        bool isWorkerBusy1 = false;
        bool isWorkerBusy2 = false;
        private void ItemByDivision_Form_Load(object sender, EventArgs e)
        {
            chk_Cust_ID.Visible = false;
            chk_Cust_Name.Visible = false;
            chk_Cust_Address.Visible = false;
            chk_Entry_Date.Checked = true;
            chk_Item_ID.Checked = true;
            chk_Item_Name.Checked = true;
            chk_Quantity.Checked = true;
            chk_Price.Checked = true;
            chk_total.Checked = true;
            chk_Cust_Name.Checked = false;
            chk_Sup_Name.Checked = true;

            cbo_report_type.DropDownStyle = ComboBoxStyle.DropDownList;

            if (!isWorkerBusy2)
            {
                progressBar1.Visible = true;
                isWorkerBusy2 = true;
                backgroundWorker2.RunWorkerAsync();
            }

            if (!isWorkerBusy1)
            {
                progressBar1.Visible = true;
                isWorkerBusy1 = true;
                load_sup = true;
                backgroundWorker1.RunWorkerAsync();
            }

            // Calculate_Filtering("load", cbo_report_type.Text);

        }

        private void Group_Filtering_MustNotEmpty()
        {
            int count = 0;
            foreach (Control c in grp_filters.Controls)
            {
                if (c is System.Windows.Forms.CheckBox && ((System.Windows.Forms.CheckBox)c).Checked)
                {
                    count++;
                }
            }
            if (count == 0)
            {
                MessageBox.Show("Please select at least one filter.");
                chk_Entry_Date.Checked = true;
                chk_Item_ID.Checked = true;
                chk_Item_Name.Checked = true;
                chk_Quantity.Checked = true;
                chk_Price.Checked = true;
                chk_Trans_Ref.Checked = true;
                chk_total.Checked = true;
            }
        }

        private void WhatTable_To_Select()
        {
            if (cbo_report_type.Text == "Stock In")
            {
                db_table = "`Stocks`";
            }
            else if (cbo_report_type.Text == "Stock Out")
            {
                db_table = "`Stock Out`";
            }
            else
            {
                db_table = "`Stock Returned`";
            }
        }

        private void chk_Unselect_CheckedChanged(object sender, EventArgs e)
        {

            if (chk_Unselect.Checked == true)
            {
                foreach (CheckBox ch in grp_filters.Controls)
                {
                    if (ch is CheckBox)
                    {
                        ch.Checked = false;
                        chk_Select_All.Checked = false;

                    }
                }
            }
        }

        private void cbo_report_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbo_report_type.Text == "Stock In")
                {
                    chk_Cust_ID.Visible = false;
                    chk_Cust_Name.Visible = false;
                    chk_Cust_Address.Visible = false;
                    chk_Cust_ID.Checked = false;
                    chk_Cust_Name.Checked = false;
                    chk_Cust_Address.Checked = false;

                    chk_Sup_ID.Visible = true;
                    chk_Sup_Name.Visible = true;
                    chk_Sup_Name.Checked = true;
                    lbl_sup_div.Text = "SUPPLIER";

                    if (!isWorkerBusy1)
                    {
                        progressBar1.Visible = true;
                        isWorkerBusy1 = true;
                        load_sup = true;
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
                else
                {
                    lbl_sup_div.Text = "DIVISION";
                    chk_Cust_ID.Visible = true;
                    chk_Cust_Name.Visible = true;
                    chk_Cust_Address.Visible = true;
                    chk_Cust_Name.Checked = true;

                    chk_Sup_ID.Visible = false;
                    chk_Sup_Name.Visible = false;
                    chk_Sup_ID.Checked = false;
                    chk_Sup_Name.Checked = false;

                    if (!isWorkerBusy2)
                    {
                        progressBar1.Visible = true;
                        isWorkerBusy2 = true;
                        load_sup = false;
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
                calculate_Total();
                Calculate_Filtering("load_todtg", cbo_report_type.Text);
            }
            catch (Exception ex)
            {
                lbl_exception.Text = "Error: " + ex.Message;
            }
        }
        decimal total_val;
        int total_qty;
        private void calculate_Total()
        {
            try
            {
                if (dtg_PreviewPage.Rows.Count > 0)
                {
                    decimal amount = 0;
                    int quantity = 0;
                    total_val = 0;
                    total_qty = 0;
                    if (cbo_report_type.Text == "Stock In")
                    {
                        foreach (DataGridViewRow rw in dtg_PreviewPage.Rows)
                        {
                            quantity = 0;
                            amount = 0;
                            int.TryParse(rw.Cells[6].Value.ToString(), out quantity);
                            decimal.TryParse(rw.Cells[8].Value.ToString(), out amount);

                            total_qty += quantity;
                            total_val += amount;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow rw in dtg_PreviewPage.Rows)
                        {
                            quantity = 0;
                            amount = 0;
                            int.TryParse(rw.Cells[9].Value.ToString(), out quantity);
                            decimal.TryParse(rw.Cells[11].Value.ToString(), out amount);

                            total_qty += quantity;
                            total_val += amount;
                        }
                    }

                    lbl_total_items.Text = dtg_PreviewPage.Rows.Count.ToString();
                    lbl_total_quantity.Text = total_qty.ToString();
                    lbl_total_value.Text = total_val.ToString("#,##0.00");
                }
                else
                {
                    lbl_total_items.Text = "0";
                    lbl_total_quantity.Text = "0";
                    lbl_total_value.Text = "0";

                }
            }
            catch (InvalidOperationException ex)
            {
                lbl_exception.Text = "Error: " + ex.Message;
            }
        }

        private void chk_Select_All_CheckedChanged(object sender, EventArgs e)
        {

            if (chk_Select_All.Checked == true)
            {
                foreach (CheckBox ch in grp_filters.Controls)
                {
                    if (ch is CheckBox)
                    {
                        ch.Checked = true;
                        chk_Unselect.Checked = false;
                        if (cbo_report_type.Text == "Stock In")
                        {
                            chk_Cust_ID.Checked = false;
                            chk_Cust_Name.Checked = false;
                            chk_Cust_Address.Checked = false;
                        }
                        else
                        {
                            chk_Sup_ID.Checked = false;
                            chk_Sup_Name.Checked = false;
                        }
                    }
                }
            }
            else
            {
                chk_Unselect.Checked = true;
                chk_Unselect.Checked = false;
            }
        }

        private void btn_Print_Click(object sender, EventArgs e)
        {
            Calculate_Filtering("print", cbo_report_type.Text);
        }

        private void dtp_date_from_ValueChanged(object sender, EventArgs e)
        {
            Calculate_Filtering("load", cbo_report_type.Text);
        }

        private void dtp_date_to_ValueChanged(object sender, EventArgs e)
        {
            Calculate_Filtering("load", cbo_report_type.Text);
        }

        private void btn_Batch_Click(object sender, EventArgs e)
        {
            Calculate_Filtering("batch", cbo_report_type.Text);
        }

        private void btn_Print_Preview_Click(object sender, EventArgs e)
        {
            Calculate_Filtering("preview", cbo_report_type.Text);
        }

        public void Calculate_Filtering(string preview_or_print, string report_type)
        {
            try
            {
                rs = new ReportDataSource();
                reportParameters = new ReportParameterCollection();
                frm = new Report_Viewer();
                func = new usableFunction();
                config = new SQLConfig();
                ds = new DataSet();

                list2 = new List<Class_Item_Var>();

                if (string.IsNullOrWhiteSpace(dtp_date_to.Text))
                {
                    func.Error_Message1 = "Report \'Date To\'";
                    func.Error_Message();
                    dtp_date_to.Focus();
                    return;
                }
                else if (string.IsNullOrWhiteSpace(dtp_date_from.Text))
                {
                    func.Error_Message1 = "Report \'Date From\'";
                    func.Error_Message();
                    dtp_date_from.Focus();
                    return;
                }
                Group_Filtering_MustNotEmpty();
                WhatTable_To_Select();
                sql = string.Empty;
                if (cbo_report_type.Text == "Stock In")
                {

                    sql = "SELECT * FROM " + db_table + " WHERE  DATE(`Entry Date`) >= '" + dtp_date_from.Text + "' AND DATE(`Entry Date`) <= '" + dtp_date_to.Text + "' AND `Supplier Name` = '" + cbo_sup_divi.Text + "' ORDER BY `Entry Date` DESC";
                }
                else
                {
                    sql = "SELECT * FROM " + db_table + " WHERE  DATE(`Entry Date`) >= '" + dtp_date_from.Text + "' AND DATE(`Entry Date`) <= '" + dtp_date_to.Text + "' AND `Customer Name` = '" + cbo_sup_divi.Text + "' ORDER BY `Entry Date` DESC";
                }
                config.Load_DTG(sql, dtg_PreviewPage);
                Dtg_Properties();
                calculate_Total();
                if (preview_or_print != "load")
                {
                    if (cbo_report_type.Text == "Stock In")
                    {
                        try
                        {
                            if (dtg_PreviewPage.DataSource != null)
                            {
                                list2 = ((DataTable)dtg_PreviewPage.DataSource).AsEnumerable().Select(
                                dataRow => new Class_Item_Var
                                {
                                    Entry_Date = dataRow["Entry Date"].ToString(),
                                    Item_ID = dataRow["Stock ID"].ToString(),
                                    Item_Name = dataRow["Item Name"].ToString(),
                                    Brand = dataRow["Brand"].ToString(),
                                    Description = dataRow["Description"].ToString(),
                                    Quantity = dataRow["Quantity"].ToString(),
                                    Price = dataRow["Price"].ToString(),
                                    Amount = dataRow["Total"].ToString(),
                                    Supplier_ID = dataRow["Supplier ID"].ToString(),
                                    Supplier_Name = dataRow["Supplier Name"].ToString(),
                                    Use_ID = dataRow["User ID"].ToString(),
                                    Staff_Name = dataRow["Warehouse Staff Name"].ToString(),
                                    Job_Role = dataRow["Job Role"].ToString(),
                                    Trans_Ref = dataRow["Transaction Reference"].ToString(),
                                }).ToList();
                                rs.Value = list2;
                            }
                            else
                            {
                                lbl_exception.Text = "Error: The data source is empty!";
                            }
                        }
                        catch (NullReferenceException ex)
                        {
                            lbl_exception.Text = "Error: " + ex.Message;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (dtg_PreviewPage.DataSource != null)
                            {
                                list2 = ((DataTable)dtg_PreviewPage.DataSource).AsEnumerable().Select(
                               dataRow => new Class_Item_Var
                               {
                                   Entry_Date = dataRow["Entry Date"].ToString(),
                                   Item_ID = dataRow["Stock ID"].ToString(),
                                   Item_Name = dataRow["Item Name"].ToString(),
                                   Brand = dataRow["Brand"].ToString(),
                                   Description = dataRow["Description"].ToString(),
                                   Quantity = dataRow["Quantity"].ToString(),
                                   Price = dataRow["Price"].ToString(),
                                   Amount = dataRow["Total"].ToString(),
                                   Customer_ID = dataRow["Customer ID"].ToString(),
                                   Customer_Name = dataRow["Customer Name"].ToString(),
                                   Customer_Address = dataRow["Customer Address"].ToString(),
                                   Use_ID = dataRow["User ID"].ToString(),
                                   Staff_Name = dataRow["Warehouse Staff Name"].ToString(),
                                   Job_Role = dataRow["Job Role"].ToString(),
                                   Trans_Ref = dataRow["Transaction Reference"].ToString(),

                               }).ToList();
                                rs.Value = list2;
                            }
                        }
                        catch (NullReferenceException ex)
                        {
                            lbl_exception.Text = "Error: " + ex.Message;
                        }
                    }

                    rs.Name = "DataSet1";
                    frm.reportViewer1.LocalReport.DataSources.Clear();
                    frm.reportViewer1.LocalReport.DataSources.Add(rs);
                    frm.reportViewer1.ProcessingMode = ProcessingMode.Local;


                    frm.reportViewer1.LocalReport.ReportPath = (Includes.AppSettings.Invoice_BY_SupDivi);

                    //Load Text to RDLC TextBox
                    reportParameters.Add(new ReportParameter("param_report_date", DateTime.Now.ToString(Includes.AppSettings.DateFormatRetrieve)));
                    reportParameters.Add(new ReportParameter("DateStart", dtp_date_from.Text));
                    reportParameters.Add(new ReportParameter("DateEnd", dtp_date_to.Text));

                    //Get Company Info
                    sql = "Select * from Settings ";
                    config.singleResult(sql);
                    if (config.dt.Rows.Count > 0)
                    {
                        company_name = config.dt.Rows[0].Field<string>("Company_Name");
                        company_address = config.dt.Rows[0].Field<string>("Company Address");
                        // Includes.AppSettings.Company_DIR = config.dt.Rows[0].Field<string>("Company_Image");
                    }
                    if (company_name == "" || company_name == null)
                    {
                        company_name = "No Company Name";
                    }
                    if (company_address == "" || company_address == null)
                    {
                        company_address = "No Company Address";
                    }

                    // reportParameters.Add(new ReportParameter("Includes.AppSettings.Item_RDLC_DIR", @"C:\Users\eugen\Desktop\Inventory_System02\Inventory_System02\bin\Debug\CommonSql\Pictures\Company\Company_Logo1.PNG"));
                    reportParameters.Add(new ReportParameter("CompanyName", company_name));
                    reportParameters.Add(new ReportParameter("CompanyAddress", company_address));

                    if (dtg_PreviewPage.Rows.Count >= 1)
                    {
                        reportParameters.Add(new ReportParameter("Total_Items", lbl_total_items.Text));
                        calculate_Total();
                        reportParameters.Add(new ReportParameter("Total_Quantity", lbl_total_quantity.Text));
                        reportParameters.Add(new ReportParameter("Total_Value", lbl_total_value.Text));

                    }
                    else if (dtg_PreviewPage.Rows.Count <= 0)
                    {
                        reportParameters.Add(new ReportParameter("Total_Items", 0.ToString()));
                        reportParameters.Add(new ReportParameter("Total_Quantity", 0.ToString()));
                        reportParameters.Add(new ReportParameter("Total_Value", 0.ToString()));
                    }
                    foreach (CheckBox chk2 in grp_filters.Controls)
                    {
                        if (chk2.Checked == false)
                        {
                            reportParameters.Add(new ReportParameter("Total_Items", 0.ToString()));
                        }
                    }

                    //HIDING COLUMNS
                    reportParameters.Add(new ReportParameter("Hide_Entry_Date", (!chk_Entry_Date.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Item_ID", (!chk_Item_ID.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Item_Name", (!chk_Item_Name.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Brand", (!chk_Brand.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Description", (!chk_Description.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Quantity", (!chk_Quantity.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Price", (!chk_Price.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Amount", (!chk_total.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Supplier_ID", (!chk_Sup_ID.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Supplier_Name", (!chk_Sup_Name.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Customer_ID", (!chk_Cust_ID.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Customer_Name", (!chk_Cust_Name.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Customer_Address", (!chk_Cust_Address.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_User_ID", (!chk_User_ID.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Staff_Name", (!chk_Staff_Name.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Job_Role", (!chk_Job.Checked).ToString()));
                    reportParameters.Add(new ReportParameter("Hide_Trans_Ref", (!chk_Trans_Ref.Checked).ToString()));

                    frm.reportViewer1.LocalReport.SetParameters(reportParameters);
                    frm.reportViewer1.RefreshReport();

                    if (preview_or_print == "preview")
                    {
                        frm.ShowDialog();
                    }
                    else if (preview_or_print == "print")
                    {
                        Print_To_The_Printer prt = new Print_To_The_Printer();
                        prt.PrintToPrinter(frm.reportViewer1.LocalReport);
                    }
                    else if (preview_or_print == "batch")
                    {

                        if (report_type == "Stock Out")
                        {
                            FileName = "Outbound Report BY DIVISION " + DateTime.Now.ToString("hhmmss") + ".pdf";
                        }
                        else if (report_type == "Stock Return")
                        {
                            FileName = "Return Report BY DIVISION " + DateTime.Now.ToString("hhmmss") + ".pdf";
                        }
                        else if (report_type == "Stock In")
                        {
                            FileName = "Inbound Report BY SUPPLIER " + DateTime.Now.ToString("hhmmss") + ".pdf";
                        }
                        if (!string.IsNullOrWhiteSpace(FileName))
                        {
                            string extension;
                            string encoding;
                            string mimeType;
                            string[] streams;
                            Warning[] warnings;

                            Byte[] mybytes = frm.reportViewer1.LocalReport.Render("PDF", null,
                                            out extension, out encoding,
                                            out mimeType, out streams, out warnings); //for exporting to PDF  
                                                                                      //using (FileStream fs = File.Create(Server.MapPath("~/Report/") + FileName))
                            using (FileStream fs = File.Create((Includes.AppSettings.Doc_DIR + "\\") + FileName))
                            {
                                fs.Write(mybytes, 0, mybytes.Length);

                                MessageBox.Show("Batched!", "Send to Document Center", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            return;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Dtg_Properties()
        {
            try
            {
                dtg_PreviewPage.Columns["Entry Date"].DisplayIndex = 0;
                dtg_PreviewPage.Columns["Stock ID"].DisplayIndex = 1;
                dtg_PreviewPage.Columns["Item Name"].DisplayIndex = 2;
                dtg_PreviewPage.Columns["Brand"].DisplayIndex = 3;
                dtg_PreviewPage.Columns["Description"].DisplayIndex = 4;
                dtg_PreviewPage.Columns["Quantity"].DisplayIndex = 5;
                dtg_PreviewPage.Columns["Price"].DisplayIndex = 6;
                dtg_PreviewPage.Columns["Total"].DisplayIndex = 7;

                if (cbo_report_type.Text == "Stock In")
                {
                    dtg_PreviewPage.Columns["Supplier ID"].DisplayIndex = 8;
                    dtg_PreviewPage.Columns["Supplier Name"].DisplayIndex = 9;
                    dtg_PreviewPage.Columns["Image Path"].Visible = false;
                }

                dtg_PreviewPage.Columns["count"].Visible = false;

                dtg_PreviewPage.Columns["User ID"].DisplayIndex = 10;
                dtg_PreviewPage.Columns["Warehouse Staff Name"].DisplayIndex = 11;
                dtg_PreviewPage.Columns["Job Role"].DisplayIndex = 12;
                dtg_PreviewPage.Columns["Transaction Reference"].DisplayIndex = 13;
            }
            catch (InvalidOperationException)
            {
                // Handle the exception by waiting for a short period of time and then trying the operation again
                System.Threading.Thread.Sleep(500);
                Dtg_Properties();
            }
        }

        private void cbo_sup_divi_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Filtering("load_todtg", cbo_report_type.Text);
        }

        private void lbl_total_value_TextChanged(object sender, EventArgs e)
        {
            func.Label_Two_Decimal_Places(sender, e, lbl_total_value);
        }

        private void cbo_sup_divi_TextChanged(object sender, EventArgs e)
        {
            Calculate_Filtering("load", cbo_report_type.Text);
        }

        bool load_sup = true;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SQLConfig config = new SQLConfig();
                string sql;
                if (load_sup)
                {
                    sql = string.Empty;
                    sql = "Select `Company Name` from `Supplier` ORDER BY `Company Name` ASC";
                }
                else
                {
                    sql = string.Empty;
                    sql = "SELECT Name FROM Customer ORDER BY Name ASC";

                }

                this.Invoke(new MethodInvoker(delegate { config.fiil_CBO(sql, cbo_sup_divi); }));

                Console.WriteLine("Test");

                for (int i = 0; i < config.dt.Rows.Count; i++)
                {
                    // Perform your background task here
                    // Report progress back to the UI thread
                    // Update the progress
                    int progress = (int)((i / (double)config.dt.Rows.Count) * 100);
                    backgroundWorker1.ReportProgress(progress);
                }
                e.Result = "Successfully loaded names";
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    lbl_error_message.Text = e.Error.Message;
                    lbl_error_message.ForeColor = Color.Red;
                    timer_error.Enabled = true;
                }));
            }
            else
            {
                progressBar1.Value = 100;
                isWorkerBusy1 = false;
                lbl_error_message.Text = e.Result.ToString();
                lbl_error_message.ForeColor = Color.Green;
                timer_error.Enabled = true;
            }
            load_sup = false;
            isWorkerBusy1 = false;
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update the UI with the progress value
            progressBar1.Value = e.ProgressPercentage;
        }

        private void timer_error_Tick(object sender, EventArgs e)
        {
            lbl_error_message.Text = "";
            timer_error.Enabled = false;
            progressBar1.Visible = false;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            // Set the date range in the background worker
            dtp_date_to.Invoke(new Action(() =>
            {
                dtp_date_to.Text = DateTime.Now.ToString(Includes.AppSettings.DateFormatRetrieve);
            }));

            dtp_date_from.Invoke(new Action(() =>
            {
                dtp_date_from.Text = DateTime.Now.ToString(Includes.AppSettings.DateFormatRetrieve);
            }));
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isWorkerBusy2 = false;
        }
    }
}
