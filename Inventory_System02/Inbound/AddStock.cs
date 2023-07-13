﻿using Inventory_System02.Includes;
using Inventory_System02.Items;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ToolTip = System.Windows.Forms.ToolTip;

namespace Inventory_System02
{
    public partial class AddStock : Form
    {
        Invoice_Code.Invoice_Code rdlc = new Invoice_Code.Invoice_Code();
        SQLConfig config;
        usableFunction func;
        Calculations cal = new Calculations();
        ID_Generator gen = new ID_Generator();
        string sql, Item_ID1, Global_ID, Fullname, JobRole, item_image_location = string.Empty, img_loc = string.Empty;
        int quantity;
        bool isWorkerBusy = false;
        bool isWarrantyBusy = false;

        public AddStock(string global_id, string fullname, string jobrole)
        {
            InitializeComponent();
            Global_ID = global_id;
            Fullname = fullname;
            JobRole = jobrole;

            item_image_location = Includes.AppSettings.Image_DIR;

        }

        private void definitionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings frm = new Settings(Global_ID, Fullname, JobRole);
            frm.ShowDialog();
        }

        public async Task LoadFormAsync()
        {
            // Your asynchronous loading logic goes here
            // For example, loading data from a database or performing other async operations

            await Task.Delay(1000); // Placeholder delay to simulate an asynchronous operation
                                    // Perform any additional initialization or data loading

            // You can also update any UI components if needed after the form has finished loading asynchronously
            // For example:
            // lbl_Status.Text = "Form loaded successfully!";
        }

        private async void AddStock_Load(object sender, EventArgs e)
        {
            try
            {
                await Task.Delay(1000);
                await LoadResources();

                button1_Click(sender, e);
            }
            catch ( Exception ex)
            {
                lbl_error_message.Text = ex.Message;
            }
           
        }
        double totalrows = 0;
        private void DTG_Property()
        {
            int attempts = 0;
            bool success = false;

            while (attempts < 3)
            {
                try
                {
                    if (dtg_Items.Columns.Count > 1)
                    {
                        // Format to add comma separator and two decimal places in dtg
                        dtg_Items.Columns["Price"].DefaultCellStyle.Format = "#,##0.00";
                        dtg_Items.Columns["Total"].DefaultCellStyle.Format = "#,##0.00";
                        // Hide some columns
                        dtg_Items.Columns["count"].Visible = false;
                        dtg_Items.Columns["Stock ID"].Visible = false;
                        dtg_Items.Columns["Image Path"].Visible = false;

                        dtg_Items.Columns["Entry Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        dtg_Items.Columns["Item Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        dtg_Items.Columns["Brand"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        dtg_Items.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                        dtg_Items.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dtg_Items.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dtg_Items.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        for (int i = 0; i < dtg_Items.Rows.Count; i++)
                        {
                            totalrows = i;
                        }
                        totalrows += 1;
                        lbl_items_count.Text = totalrows.ToString();

                        Calculator_Timer.Start();

                        success = true; // If no exception is thrown, mark the attempt as successful
                        break; // Break out of the while loop since we don't need further attempts
                    }
                }
                catch (InvalidOperationException)
                {
                    attempts++;
                    // Handle the exception by waiting for a short period of time and then trying the operation again
                    System.Threading.Thread.Sleep(500);
                    DTG_Property();
                }
            }

            if (!success)
            {
                lbl_error_message.Text = "Error occurred after three attempts.";
                // Throw an error here or perform additional error handling if needed
            }

        }
        private void ProcessStockLow()
        {
            quantity = 0;
            //STOCK LOW DETECTION
            sql = "Select Low_Detection from Settings";
            config.singleResult(sql);
            if (config.dt.Rows.Count >= 1)
            {
                foreach (DataGridViewRow rw in dtg_Items.Rows)
                {
                    try
                    {
                        int.TryParse(Convert.ToString(config.dt.Rows[0]["Low_Detection"]), out quantity);
                        int cellValue;
                        if (int.TryParse(Convert.ToString(rw.Cells["Quantity"].Value), out cellValue) && cellValue <= quantity)
                        {
                            //STOCK LOW DETECT CHANGE FONT COLOR
                            rw.DefaultCellStyle.ForeColor = Color.Red;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Handle the exception here. For example, you could log it, show an error message to the user, or take other appropriate action.
                        lbl_error_message.Text = $"An error occured: {ex.Message}";
                    }
                }
            }
        }

        private void Calculator_Timer_Tick(object sender, EventArgs e)
        {
            config = new SQLConfig();
            
            Calculations();
            ProcessStockLow();
            Calculator_Timer.Stop();
        }

        public void Calculations()
        {
            try
            {
                if (dtg_Items.Rows.Count > 0)
                {   
                    Calculations cal = new Calculations();
                   
                    cal.CalculateTodayTotals("Stocks", lbl_Today_Qty, lbl_Today_Amt, lbl_error_message);
                    cal.CalculateOverallTotals("Stocks", lbl_TotalQty, lbl_TotalAmt, lbl_error_message);
                    
                    lbl_items_count.Text = dtg_Items.Rows.Count.ToString();
                    return;
                }
            }
            catch (Exception ex)
            {
                lbl_error_message.Text = ex.Message;
            }
        }

        private void txt_Price_Leave(object sender, EventArgs e)
        {
            txt_Qty_ValueChanged(sender, e);
            func.Two_Decimal_Places(sender, e, txt_Price);
        }

        private void btn_Gen_Click(object sender, EventArgs e)
        {
            sql = string.Empty;
            config = new SQLConfig();

            string id = string.Empty;
            bool hasDuplicate = true;
            while (hasDuplicate)
            {
                gen.Generate_Transaction();
                sql = "Select `Transaction Reference` from `ID_Generated` ";
                config.singleResult(sql);
                if (config.dt.Rows.Count > 0)
                {
                    id = Convert.ToString(config.dt.Rows[0]["Transaction Reference"]);

                    sql = "SELECT COUNT(*) FROM `Stocks` WHERE `Transaction Reference` = '" + id + "'";
                    config.singleResult(sql);
                    if (Convert.ToInt32(config.dt.Rows[0][0]) > 0)
                    {
                        gen.Generate_Transaction();
                    }
                    else
                    {
                        txt_TransRef.Text = id;
                        hasDuplicate = false;
                        SupplierChangeDisabler();
                    }
                }

            }
        }
        private void WarrantySelection()
        {
            ///This method is for getting the warranty date by id and transaction reference.
            bool success;
            success = config.CreateOrUpdateInboundWarranty(dtp_warranty.Value, txt_Barcode.Text, txt_TransRef.Text, "select");
            if (success)
            {
                string barcodeId = txt_Barcode.Text;
                string transRef = txt_TransRef.Text;

                // Construct the SQL query
                string sql = $"SELECT warranty FROM Inbound_Warranty WHERE barcode_id = '"+barcodeId+"' AND trans_ref = '"+transRef+"'";
                config.singleResult(sql);
                if (config.dt.Rows.Count == 1)
                {
                    dtp_warranty.Value = Convert.ToDateTime(config.dt.Rows[0]["warranty"].ToString());
                    DateTime selectedDate = dtp_warranty.Value;
                    DateTime currentDate = DateTime.Now;

                    if (selectedDate <= currentDate)
                    {
                        lbl_error_message.Text = "Warranty has already expired!";
                        lbl_error_message.ForeColor = Color.Red;
                    }
                    else
                    {
                        lbl_error_message.Text = "Warranty is still valid.";
                        lbl_error_message.ForeColor = Color.Blue;

                    }
                }
            }

        }

        bool dtg_was_clicked = false;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ///Clicking the Table will populate the data to the fields.
            try
            {
                if (enable_them == true)
                {
                    if (dtg_Items.Rows.Count > 0)
                    {
                        txt_Barcode.Text = dtg_Items.CurrentRow.Cells["Stock ID"].Value.ToString();
                        txt_ItemName.Text = dtg_Items.CurrentRow.Cells[3].Value.ToString();
                        cbo_brand.Text = dtg_Items.CurrentRow.Cells[4].Value.ToString();
                        cbo_desc.Text = dtg_Items.CurrentRow.Cells[5].Value.ToString();
                        txt_Qty.Text = dtg_Items.CurrentRow.Cells[6].Value.ToString();
                        txt_Price.Text = dtg_Items.CurrentRow.Cells[7].Value.ToString();
                        lbl_ProductValue.Text = dtg_Items.CurrentRow.Cells[8].Value.ToString();
                        func.Reload_Images(Item_Image, txt_Barcode.Text, item_image_location);
                        txt_SupID.Text = dtg_Items.CurrentRow.Cells[10].Value.ToString();
                        txt_Sup_Name.Text = dtg_Items.CurrentRow.Cells[11].Value.ToString();
                        // Unsubscribe the event temporarily
                        //txt_TransRef.TextChanged -= txt_TransRef_SelectedIndexChanged;

                        // Assign the selected value to the combo box
                        txt_TransRef.Text = dtg_Items.CurrentRow.Cells[15].Value.ToString();

                        // Resubscribe the event
                        //txt_TransRef.TextChanged += txt_TransRef_SelectedIndexChanged;


                        //func.Change_Font_DTG(sender, e, dtg_Items);
                        txt_Qty_ValueChanged(sender, e);
                        bool success;
                        success = config.CreateOrUpdateInboundWarranty(dtp_warranty.Value, txt_Barcode.Text, txt_TransRef.Text, "select");
                        if (success)
                        {
                            WarrantySelection();
                        }

                    }


                    if (txt_Qty.Value <= Convert.ToDecimal(quantity))
                    {
                        lbl_stock_low.Text = "Stock Low Detected!";
                    }
                    else
                    {
                        lbl_stock_low.Text = "";
                    }

                    SupplierChangeDisabler();
                    dtg_was_clicked = true;
                    chk_select_all.Checked = false;
                }
            }
            catch (Exception ex)
            {
                lbl_error_message.Text = ex.Message;
            } 
        }
        private void SupplierChangeDisabler()
        {
            sql = string.Empty;
            config = new SQLConfig();
            if (!string.IsNullOrWhiteSpace(txt_TransRef.Text))
            {
                sql = "Select `Transaction Reference` from Stocks WHERE `Transaction Reference` = '" + txt_TransRef.Text + "' ";
                config.singleResult(sql);
                if (config.dt.Rows.Count > 0)
                {
                    supplierListToolStripMenuItem.Enabled = false;
                    btn_searchSup.Enabled = false;
                    txt_SupID.Enabled = false;
                    txt_Sup_Name.Enabled = false;
                }
                else
                {
                    supplierListToolStripMenuItem.Enabled = true;
                    btn_searchSup.Enabled = true;
                    txt_SupID.Enabled = true;
                    txt_Sup_Name.Enabled = true;
                }
            }
            else
            {
                supplierListToolStripMenuItem.Enabled = true;
                btn_searchSup.Enabled = true;
                txt_SupID.Enabled = true;
                txt_Sup_Name.Enabled = true;

            }
        }

        private void btn_edit_Click(object sender, EventArgs e)
        {
            sql = "";
            config = new SQLConfig();

            chk_select_all.Checked = false;
            if (txt_Barcode.Text != "")
            {
                sql = "Select `Stock ID` from Stocks where `Stock ID` = '" + txt_Barcode.Text + "' ";
                config.singleResult(sql);
                if (config.dt.Rows.Count <= 0)
                {
                    btn_AddStock_Click(sender, e);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to update current item? \n\nContinue?", "Warning Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
                {
                    txt_Qty_ValueChanged(sender, e);
                    img_loc = item_image_location + "\\" + txt_Barcode.Text + ".PNG";

                    sql = "Update Stocks set " +
                        " `Item Name` = '" + txt_ItemName.Text + "' " +
                        ", `Brand` = '" + cbo_brand.Text + "' " +
                        ", `Description` = '" + cbo_desc.Text + "' " +
                        ", `Quantity` = '" + Convert.ToInt32(txt_Qty.Text) + "' " +
                        ", `Price` = '" + Convert.ToDecimal(txt_Price.Text) + "' " +
                        ", `Total` = '" + Convert.ToDecimal(lbl_ProductValue.Text) + "' " +
                        ", `Image Path` = '" + img_loc + "' " +
                        ", `Supplier ID` = '" + txt_SupID.Text + "' " +
                        ", `Supplier Name` = '" + txt_Sup_Name.Text + "' " +
                        ", `User ID` = '"+ Global_ID +"' " +
                        ", `Warehouse Staff Name` = '"+ Fullname +"' " +
                        ", `Job Role` = '"+ JobRole +"' " +
                        " where `Stock ID` = '" + txt_Barcode.Text + "' ";
                    config.Execute_CUD(sql, "Unsuccessful to update item information", "Item information successfully updated!");
                    bool success;
                    success = config.CreateOrUpdateInboundWarranty(dtp_warranty.Value, txt_Barcode.Text, txt_TransRef.Text, "update");
                    refreshToolStripMenuItem1_Click(sender, e);

                }
            }
            else
            {
                MessageBox.Show("Barcode is missing!", "Invalid Prompt", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private async Task LoadResources()
        {
            config = new SQLConfig();
            sql = string.Empty;
            func = new usableFunction();

            sql = "Select Name from `Product Name`";
            config.fiil_CBO(sql, txt_ItemName);

            sql = "Select Name from Brand";
            config.fiil_CBO(sql, cbo_brand);


            if (!isWarrantyBusy)
            {
                isWarrantyBusy = true;
                await Task.Run(() => dtp_warranty_worker.RunWorkerAsync());
            }

            if (!isWorkerBusy)
            {
                isWorkerBusy = true;
                await Task.Run(() => LoadImageWorker.RunWorkerAsync());
            }

            func.Reload_Images(Item_Image, txt_Barcode.Text, item_image_location);
            cbo_srch_type.DropDownStyle = ComboBoxStyle.DropDownList;
            enable_them = true;
            Calculator_Timer.Start();
        }


        private void EnablePaginatorControl()
        {
            //num_max_pages.Enabled = true;
            //current_page_val.Enabled = true;
            //cbo_num_records.Enabled = true;
            //btn_load.Enabled = true;
        }
        private void DisablePaginatorControl()
        {
            //num_max_pages.Enabled = false;
            //current_page_val.Enabled = false;
            //cbo_num_records.Enabled = false;
            //btn_load.Enabled = false;
        }

        private void btn_Clear_Text_Click(object sender, EventArgs e)
        {

            chk_select_all.Checked = false;
            txt_TransRef.Text = "";
            txt_ItemName.Text = "";
            txt_Barcode.Text = "";
            cbo_desc.Text = "";
            cbo_brand.Text = "";
            txt_Qty.Text = "0";
            txt_SupID.Text = "";
            txt_Sup_Name.Text = "";
            txt_Price.Text = "0.00";
            txt_Barcode.Focus();
            SupplierChangeDisabler();

        }

        private void txt_Barcode_TextChanged(object sender, EventArgs e)
        {
            if (txt_Barcode.Text != "")
            {
                BarcodeWriter writer = new BarcodeWriter()
                {
                    Format = BarcodeFormat.CODE_128
                };
                pic_BarCode.Image = writer.Write(txt_Barcode.Text);
            }


            /*
            //FOR DECODING
            BarcodeReader reader = new BarcodeReader();
            var result = reader.Decode((Bitmap)pic_BarCode.Image);
            if (result != null )
            {
                txt_Barcode.Text = result.Text;
            }
            */
        }

        private void Item_Image_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dtg_Items.SelectedRows.Count == 1)
                {
                    chk_select_all.Checked = false;
                    func.DoubleClick_Picture_Then_Replace_Existing(Item_Image, txt_Barcode.Text, item_image_location);
                    func.Reload_Images(Item_Image, txt_Barcode.Text, item_image_location);
                    btn_edit_Click(sender, e);
                    refreshToolStripMenuItem1_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Please click again the item from the table. Thank you.", "Data Inconsistent", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                lbl_error_message.Text = ex.Message;
            }
        }
        string search_for = string.Empty;
        private void txt_Search_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if ( dtg_Items.Columns.Count >= 1 )
                {
                    dtg_Items.Columns.Clear();
                }
                if (cbo_srch_type.Text == "DATE")
                {
                    search_for = "`Entry Date`";
                }
                else if (cbo_srch_type.Text == "ID")
                {
                    search_for = "`Stock ID`";
                }
                else if (cbo_srch_type.Text == "NAME")
                {
                    search_for = "`Item Name`";
                }
                else if (cbo_srch_type.Text == "BRAND")
                {
                    search_for = "`Brand`";
                }
                else if (cbo_srch_type.Text == "DESCRIPTION")
                {
                    search_for = "`Description`";
                }
                else if (cbo_srch_type.Text == "QUANTITY")
                {
                    search_for = "`QUANTITY`";
                }
                else if (cbo_srch_type.Text == "PRICE")
                {
                    search_for = "`Price`";
                }
                else if (cbo_srch_type.Text == "SUPPLIER")
                {
                    search_for = "`Supplier Name`";
                }
                else if (cbo_srch_type.Text == "JOB")
                {
                    search_for = "`Job Role`";
                }
                else if (cbo_srch_type.Text == "TRANS REF")
                {
                    search_for = "`Transaction Reference`";
                }
                else
                {
                    search_for = "`Transaction Reference`";
                }


                if (txt_Search.Text == "")
                {
                    refreshToolStripMenuItem1_Click(sender, e);
                    return;
                }
                config = new SQLConfig();
                sql = "";
                Console.WriteLine(search_for);
                sql = "Select * from Stocks where '"+search_for+"' like '%"+txt_Search.Text+"%' ORDER BY `Entry Date` DESC ";
                config.Load_DTG(sql, dtg_Items);
     
                Calculator_Timer.Start();
                DTG_Property();
            }
            catch (Exception ex)
            {
                lbl_error_message.Text = ex.Message;
                return;
            }

        }

        private void btn_searchSup_Click(object sender, EventArgs e)
        {
            supplierListToolStripMenuItem_Click(sender, e);
        }

        private void btn_upload_Click(object sender, EventArgs e)
        {

            Item_Image_DoubleClick(sender, e);

        }

        private void dtg_Items_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Preview();
        }

        private void btn_preview_Click(object sender, EventArgs e)
        {
            Preview();
        }

        private void Preview()
        {

            chk_select_all.Checked = false;
            if (dtg_Items.Rows.Count >= 1)
            {
                if (dtg_was_clicked)
                {
                    if (dtg_Items.SelectedRows.Count == 1)
                    {
                        if (!string.IsNullOrWhiteSpace(txt_Barcode.Text))
                        {
                            if (dtg_Items.CurrentRow != null)
                            {
                                Item_Preview frm = new Item_Preview(
                                dtg_Items.CurrentRow.Cells[1].Value.ToString(),
                                txt_Barcode.Text,
                                txt_TransRef.Text,
                                txt_ItemName.Text,
                                cbo_brand.Text,
                                cbo_desc.Text,
                                txt_Qty.Text,
                                txt_Price.Text,
                                lbl_ProductValue.Text,
                                dtg_Items.CurrentRow.Cells[13].Value.ToString());

                                frm.ShowDialog();
                            }

                        }
                    }
                    else
                    {
                        dtg_Items.CurrentRow.Selected = true;
                    }
                }
            }
        }

        private void cbo_srch_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_Search_TextChanged(sender, e);
        }
        private ToolTip toolTip;
        private void btn_searchSup_MouseHover(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            toolTip.SetToolTip(btn_searchSup, "Click to search for an existing supplier.");
        }

        private void txt_Barcode_MouseHover(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            toolTip.SetToolTip(txt_Barcode, "If left empty system will auto create an item id.");
        }

        private void txt_SupID_MouseHover(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            toolTip.SetToolTip(txt_SupID, "Type any known ID, System will populate the names if exists.");
        }

        private void Item_Image_MouseHover(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            toolTip.SetToolTip(Item_Image, "Double click me to change picture.");
        }

        private void newItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_TransRef.Text))
            {
                btn_Gen_Click(sender, e);
            }
            Barcode_Generator();
            cbo_brand.Text = "Replace me!";
            txt_ItemName.Text = "Replace me!";
            txt_Price.Text = "0.00";
            txt_Qty.Text = "0";
            lbl_ProductValue.Text = "0.00";
            cbo_desc.Text = "None";

            lbl_error_message.Text = "New barcode is generated! " + txt_Barcode.Text;
            lbl_error_message.ForeColor = Color.Green;
            timer_Error_message.Enabled = true;
            txt_ItemName.Focus();
        }
        private void Barcode_Generator()
        {
            try
            {
                string id = string.Empty;
                bool hasDuplicate = true;
                ID_Generator gen = new ID_Generator();
                while (hasDuplicate)
                {
                    chk_select_all.Checked = false;
                    gen.Item_ID();
                    sql = "Select `Item ID` from ID_Generated";
                    config.singleResult(sql);
                    if (config.dt.Rows.Count > 0)
                    {
                        id = Convert.ToString(config.dt.Rows[0]["Item ID"]);

                        sql = "SELECT COUNT(*) FROM `Stocks` WHERE `Stock ID` = '" + id + "'";
                        config.singleResult(sql);
                        if (Convert.ToInt32(config.dt.Rows[0][0]) > 0)
                        {
                            gen.Item_ID();
                        }
                        else
                        {
                            txt_Barcode.Text = id;
                            txt_Barcode.Focus();
                            hasDuplicate = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lbl_error_message.Text = "An error occurred: " + ex.Message;
            }
        }

        private void txt_Qty_ValueChanged(object sender, EventArgs e)
        {
            decimal total_product = 0;
            decimal value1 = 0;
            decimal value2 = 0;
            if (string.IsNullOrWhiteSpace(txt_Qty.Value.ToString()))
            {
                txt_Qty.Text = "0";
                txt_Qty.Value = 0;
            }
            decimal.TryParse(Convert.ToString(txt_Qty.Value), out value1);
            decimal.TryParse(txt_Price.Text, out value2);


            total_product = value1 * value2;
            lbl_ProductValue.Text = total_product.ToString();
        }

        private void txt_Price_TextChanged(object sender, EventArgs e)
        {
            txt_Qty_ValueChanged(sender, e);
        }

        private void txt_Qty_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_Qty.Value.ToString()))
            {
                txt_Qty.Text = "0";
                txt_Qty.Value = 0;
            }
            txt_Qty_ValueChanged(sender, e);
        }

        private void view_trans_in_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(txt_TransRef.Text) && txt_TransRef.Text != "Empty Field!")
            {
                System.Threading.Tasks.Task task = rdlc.Invoice("in", txt_TransRef.Text, "preview");
                chk_select_all.Checked = false;
            }
            else
            {
                txt_TransRef.Text = "Empty Field!";
                txt_TransRef.Focus();
            }
        }

        private void batch_trans_in_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt_TransRef.Text) && txt_TransRef.Text != "Empty Field!")
            {
                chk_select_all.Checked = false;
                System.Threading.Tasks.Task task = rdlc.Invoice("in", txt_TransRef.Text, "batch");
            }
            else
            {
                txt_TransRef.Text = "Empty Field!";
                txt_TransRef.Focus();
            }
        }

        private void print_trans_in_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt_TransRef.Text) && txt_TransRef.Text != "Empty Field!")
            {
                chk_select_all.Checked = false;
                System.Threading.Tasks.Task task = rdlc.Invoice("in", txt_TransRef.Text, "print");
            }
            else
            {
                txt_TransRef.Text = "Empty Field!";
                txt_TransRef.Focus();
            }
        }

        private void view_tbl_in_Click(object sender, EventArgs e)
        {

            if (dtg_Items.Rows.Count >= 1)
            {
                chk_select_all.Checked = false;
                Load_DTG_VBPrint frm = new Load_DTG_VBPrint();
                frm.Search_Result("INBOUND SUMMARY", "preview", dtg_Items, lbl_items_count.Text, lbl_TotalQty.Text, lbl_TotalAmt.Text, cbo_srch_type.Text, txt_Search.Text);
            }
            else
            {
                MessageBox.Show("Table is empty", "Missing Rows", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void batch_tbl_in_Click(object sender, EventArgs e)
        {
            if (dtg_Items.Rows.Count >= 1)
            {
                chk_select_all.Checked = false;
                Load_DTG_VBPrint frm = new Load_DTG_VBPrint();
                frm.Search_Result("INBOUND SUMMARY", "batch", dtg_Items, lbl_items_count.Text, lbl_TotalQty.Text, lbl_TotalAmt.Text, cbo_srch_type.Text, txt_Search.Text);
                MessageBox.Show("Sent to My Documents!");
            }
            else
            {
                MessageBox.Show("Table is empty", "Missing Rows", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void print_tbl_in_Click(object sender, EventArgs e)
        {
            if (dtg_Items.Rows.Count >= 1)
            {
                chk_select_all.Checked = false;
                Load_DTG_VBPrint frm = new Load_DTG_VBPrint();
                frm.Search_Result("INBOUND SUMMARY", "print", dtg_Items, lbl_items_count.Text, lbl_TotalQty.Text, lbl_TotalAmt.Text, cbo_srch_type.Text, txt_Search.Text);
            }
            else
            {
                MessageBox.Show("Table is empty", "Missing Rows", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lbl_ProductValue_TextChanged(object sender, EventArgs e)
        {
            func.Label_Two_Decimal_Places(sender, e, lbl_ProductValue);
        }

        private void lbl_Today_Amt_TextChanged(object sender, EventArgs e)
        {
            func.Label_Two_Decimal_Places(sender, e, lbl_Today_Amt);
        }

        private void lbl_TotalAmt_TextChanged(object sender, EventArgs e)
        {
            func.Label_Two_Decimal_Places(sender, e, lbl_TotalAmt);
        }
        bool enable_them = false;
        private void SpecialFilterDisabler()
        {
            if (enable_them == false)
            {
                printTransactionToolStripMenuItem.Enabled = false;
                batchToolStripMenuItem.Enabled = false;
                viewToolStripMenuItem.Enabled = false;
                btn_AddStock.Enabled = false;
                btn_edit.Enabled = false;
                btn_delete.Enabled = false;
                btn_upload.Enabled = false;
                btn_preview.Enabled = false;

                txt_Search.Enabled = false;
                btn_Clear_Text.Enabled = false;


                if (dtg_Items.Columns.Count > 0)
                {
                    dtg_Items.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dtg_Items.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dtg_Items.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }

                lbl_items_count.Text = dtg_Items.Rows.Count.ToString();
                lbl_Today_Amt.Text = "0";
                lbl_Today_Qty.Text = "0";
                lbl_TotalAmt.Text = "0";
                lbl_TotalQty.Text = "0";
                txt_TransRef.Text = "";
                txt_ItemName.Text = "";
                txt_Barcode.Text = "";
                cbo_desc.Text = "";
                cbo_brand.Text = "";
                txt_Qty.Text = "0";
                txt_SupID.Text = "";
                txt_Sup_Name.Text = "";
                txt_Price.Text = "0.00";
            }
            else
            {
                printTransactionToolStripMenuItem.Enabled = true;
                batchToolStripMenuItem.Enabled = true;
                viewToolStripMenuItem.Enabled = true;
                btn_AddStock.Enabled = true;
                btn_edit.Enabled = true;
                btn_delete.Enabled = true;
                btn_upload.Enabled = true;
                btn_preview.Enabled = true;

                txt_Search.Enabled = true;
                btn_Clear_Text.Enabled = true;
            }

        }
        private void TableRefresher()
        {
            sql = "";
            config = new SQLConfig();
            chk_select_all.Checked = false;
            //Refresh and clear the columns this is important to remain the sorting
            if (dtg_Items.Columns.Count > 0)
            {
                dtg_Items.Columns.Clear();
            }
        }
        private void mostProductPurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableRefresher();
            sql = "SELECT Brand, COUNT(*) AS `Total Occurences` FROM `Stocks` GROUP BY Brand ORDER BY `Total Occurences` DESC";
            config.Load_DTG(sql, dtg_Items);
            enable_them = false;
            SpecialFilterDisabler();
        }

        private void leastProductPurchasedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableRefresher();
            sql = "SELECT Brand, COUNT(*) AS `Total Occurences` FROM `Stocks` GROUP BY Brand ORDER BY `Total Occurences` ASC";
            config.Load_DTG(sql, dtg_Items);
            enable_them = false;
            SpecialFilterDisabler();
        }

        private void mostItemPurchasedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableRefresher();
            sql = "SELECT `Item Name`, `Brand`, COUNT(*) AS `Total Occurences` FROM `Stocks` GROUP BY `Item Name` ORDER BY `Total Occurences` DESC";
            config.Load_DTG(sql, dtg_Items);
            enable_them = false;
            SpecialFilterDisabler();
        }

        private void mostItemPurchasedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TableRefresher();
            sql = "SELECT `Item Name`, `Brand`, COUNT(*) AS `Total Occurences` FROM `Stocks` GROUP BY `Item Name` ORDER BY `Total Occurences` ASC";
            config.Load_DTG(sql, dtg_Items);
            enable_them = false;
            SpecialFilterDisabler();
        }

        private void mosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableRefresher();
            sql = "SELECT `Supplier Name`, COUNT(*) AS `Total Occurences` FROM `Stocks` GROUP BY `Supplier Name` ORDER BY `Total Occurences` DESC";
            config.Load_DTG(sql, dtg_Items);
            enable_them = false;
            SpecialFilterDisabler();
        }

        private void divisionWithTheLeastPurchasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TableRefresher();
            sql = "SELECT `Supplier Name`, COUNT(*) AS `Total Occurences` FROM `Stocks` GROUP BY `Supplier Name` ORDER BY `Total Occurences` ASC";
            config.Load_DTG(sql, dtg_Items);
            enable_them = false;
            SpecialFilterDisabler();
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtg_Items.Rows.Count >= 1)
                {
                    if (JobRole != "Programmer/Developer" && JobRole != "Office Manager")
                    {
                        MessageBox.Show("No permission to delete item!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        chk_select_all.Checked = false;
                        return;
                    }

                    if (dtg_Items.SelectedRows.Count >= 1)
                    {
                        if (MessageBox.Show("You are about to delete selected item(s) from the table. Please confirm deletion.", "Warning Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                        {
                            config = new SQLConfig();
                            sql = "";
                            if (chk_select_all.Checked)
                            {
                                string sql = "DELETE FROM `Stocks` WHERE count = '1' ";
                                config.Execute_CUD(sql, "Unable to delete all items. Please try again!", "Successfully deleted all items!");
                                config.DeleteAllRowsFromTable("Inbound_Warranty");


                                chk_select_all.Checked = false;
                                refreshToolStripMenuItem1_Click(sender, e);
                                return;
                            }
                            else
                            {
                                foreach (DataGridViewRow rw in dtg_Items.SelectedRows)
                                {
                                    string barcode_id = rw.Cells[2].Value.ToString();
                                    string trans_ref = rw.Cells[0].Value.ToString();

                                    // Call the method to delete the warranty based on barcode_id and trans_ref
                                    config.CreateOrUpdateInboundWarranty(DateTime.Now, barcode_id, trans_ref, "delete");

                                    // Delete the row from Stocks table
                                    sql = "DELETE FROM Stocks WHERE `Stock ID` = '" + barcode_id + "'";
                                    config.Execute_Query(sql);
                                }
                                MessageBox.Show("Deleted from inbound stocks!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                refreshToolStripMenuItem1_Click(sender, e);
                                chk_select_all.Checked = false;
                                return;

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No selection from the table", "Nothing to Delete", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        chk_select_all.Checked = false;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                lbl_error_message.Text = "An error occurred: " + ex.Message;
            }
        }

        private void chk_select_all_CheckedChanged(object sender, EventArgs e)
        {
            if (dtg_Items.Columns.Count >= 1)
            {
                if (dtg_Items.Rows.Count >= 1)
                {
                    foreach (DataGridViewRow rw in dtg_Items.Rows)
                    {
                        if (chk_select_all.Checked)
                        {
                            rw.Selected = true;
                        }
                        else
                        {
                            rw.Selected = false;
                        }

                    }
                }
            }
        }
        private void txt_TransRef_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_Search.Text = txt_TransRef.Text;
        }
        private void txt_TransRef_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SQLConfig config = new SQLConfig();
                sql = string.Empty;
                sql = "SELECT `Transaction Reference` FROM `Stocks` WHERE `Transaction Reference` = '%" + txt_TransRef.Text + "%' ";
                config.singleResult(sql);
                if (config.dt.Rows.Count >= 1)
                {

                    if (!string.IsNullOrWhiteSpace(txt_TransRef?.Text))
                    {
                        sql = "Select `Supplier ID` from Stocks where `Transaction Reference` = '" + txt_TransRef.Text + "' ";
                        config.singleResult(sql);
                        if (config.dt.Rows.Count >= 1)
                        {
                            txt_SupID.Text = config.dt.Rows[0]["Supplier ID"]?.ToString() ?? string.Empty;
                            cbo_srch_type.Text = "TRANS REF";

                        }
                        else
                        {
                            throw new Exception("No supplier ID found for transaction reference: " + txt_TransRef.Text);
                        }
                    }
                    else
                    {
                        btn_Clear_Text_Click(sender, e);
                        txt_Search.Text = "";
                        txt_Search_TextChanged(sender, e);
                    }
                }
            }
            catch (SqlException ex)
            {
                // Handle MySQL-specific errors
                MessageBox.Show("SQL error: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle all other exceptions
                lbl_error_message.Text = ex.Message;
                timer_Error_message.Enabled = true;
            }

        }

        private void timer_Error_message_Tick(object sender, EventArgs e)
        {
            lbl_error_message.Text = "";
            timer_Error_message.Enabled = false;
        }

        private void todayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                sql = "";
                config = new SQLConfig();
                TableRefresher();
                sql = "Select * from Stocks where DATE(`Entry Date`) = '" + DateTime.Now.ToString(Includes.AppSettings.DateFormatRetrieve) + "' order by `Entry Date` desc";
                //config.Load_DTG(sql, dtg_Items);
                Calculator_Timer.Start();
                DTG_Property();
                enable_them = true;
                SpecialFilterDisabler();
                //current_page_val.Text = "0";

            }
            catch (SqlException ex)
            {
                lbl_error_message.Text = ex.Message;
                timer_Error_message.Enabled = true;
            }
            catch (Exception ex)
            {
                lbl_error_message.Text = ex.Message;
                timer_Error_message.Enabled = true;
            }
        }

        private void LoadImageWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            if (dtg_Items.Rows.Count >= 1)
            {
                try
                {
                    if (dtg_Items.Columns.Contains("Entry Date"))
                    {
                        dtg_Items.Columns["Entry Date"].DefaultCellStyle.Format = Includes.AppSettings.DateFormatRetrieve;
                        dtg_Items.Sort(dtg_Items.Columns["Entry Date"], ListSortDirection.Descending);
                    }

                    if (!dtg_Items.Columns.Contains("Image"))
                    {
                        dtg_Items.Columns.Add(new DataGridViewImageColumn() { Name = "Image", DataPropertyName = "Image", ImageLayout = DataGridViewImageCellLayout.Zoom });
                    }

                    foreach (DataGridViewRow row in dtg_Items.Rows)
                    {
                        if (row.Cells["Image Path"].Value != null && File.Exists(row.Cells["Image Path"].Value.ToString()))
                        {
                            row.Cells["Image"].Value = File.ReadAllBytes(row.Cells["Image Path"].Value.ToString());
                        }
                        else
                        {
                            string imagePath = item_image_location + "DONOTDELETE_SUBIMAGE";
                            string[] extensions = { ".jpg", ".JPG", ".png", ".PNG" };
                            foreach (string ext in extensions)
                            {
                                if (File.Exists(imagePath + ext))
                                {
                                    row.Cells["Image"].Value = File.ReadAllBytes(imagePath + ext);
                                    break;
                                }
                            }

                            if (row.Cells["Image"].Value == null || ((byte[])row.Cells["Image"].Value).Length == 0)
                            {
                                row.Cells["Image"].Value = null;
                            }
                        }
                    }

                    if (dtg_Items.Columns.Contains("Image"))
                    {
                        dtg_Items.Columns["Image"].DisplayIndex = 0;
                    }

                    for (int i = 0; i < dtg_Items.Columns.Count; i++)
                    {
                        if (dtg_Items.Columns[i] is DataGridViewImageColumn)
                        {
                            ((DataGridViewImageColumn)dtg_Items.Columns[i]).ImageLayout = DataGridViewImageCellLayout.Zoom;
                            break;
                        }
                    }

                    for (int i = 0; i < dtg_Items.Rows.Count; i++)
                    {
                        totalrows = i;
                    }

                    totalrows += 1;
                    lbl_items_count.Text = totalrows.ToString();

                    Calculator_Timer.Start();
                }
                catch (Exception ex)
                {
                    e.Result = ex.Message;
                }
            }

        }

        private void LoadImageWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                lbl_error_message.Text = e.Error.Message;
                lbl_error_message.ForeColor = Color.Red;
                timer_Error_message.Enabled = true;
                isWorkerBusy = false;
            }
        }

        private void LoadImageWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int rowIndex = e.ProgressPercentage;
            byte[] image = (byte[])e.UserState;

            DataGridViewRow row = dtg_Items.Rows[rowIndex];
            if (row.Cells["Image"].Value == null)
            {
                row.Cells["Image"].Value = image;
                row.Cells["Image"].ValueType = typeof(DataGridViewImageCell);
            }
            else
            {
                ((DataGridViewImageCell)row.Cells["Image"]).Value = Image.FromStream(new MemoryStream(image));
            }

            // Refresh the row to reflect the changes
            // dtg_Items.Refresh();

        }

        private void loadImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dtg_Items.Rows.Count >= 1)
            {
                try
                {
                    //Format to date dtg cell
                    dtg_Items.Columns["Entry Date"].DefaultCellStyle.Format = Includes.AppSettings.DateFormatRetrieve;
                    //sorting
                    dtg_Items.Sort(dtg_Items.Columns["Entry Date"], ListSortDirection.Descending);

                    if (!dtg_Items.Columns.Contains("Image"))
                    {
                        dtg_Items.Columns.Add(new DataGridViewImageColumn() { Name = "Image", DataPropertyName = "Image", ImageLayout = DataGridViewImageCellLayout.Zoom });
                    }

                    //Load image
                    foreach (DataGridViewRow row in dtg_Items.Rows)
                    {
                        if (File.Exists(row.Cells["Image Path"].Value.ToString()))
                        {
                            row.Cells["Image"].Value = File.ReadAllBytes(row.Cells["Image Path"].Value.ToString());
                        }
                        else
                        {
                            string imagePath = item_image_location + "DONOTDELETE_SUBIMAGE";
                            string[] extensions = { ".jpg", ".JPG", ".png", ".PNG" };
                            foreach (string ext in extensions)
                            {
                                if (File.Exists(imagePath + ext))
                                {
                                    row.Cells["Image"].Value = File.ReadAllBytes(imagePath + ext);
                                    break;
                                }
                            }
                            // If none of the image files exist, set the image to null or an empty byte array
                            if (row.Cells["Image"].Value == null || ((byte[])row.Cells["Image"].Value).Length == 0)
                            {
                                row.Cells["Image"].Value = null;
                                // row.Cells["Image"].Value = new byte[0];
                            }
                        }
                    }
                    dtg_Items.Columns["Image"].DisplayIndex = 0;


                    for (int i = 0; i < dtg_Items.Columns.Count; i++)
                    {
                        if (dtg_Items.Columns[i] is DataGridViewImageColumn)
                        {
                            ((DataGridViewImageColumn)dtg_Items.Columns[i]).ImageLayout = DataGridViewImageCellLayout.Zoom;
                            break;
                        }

                    }

                    DisablePaginatorControl();
                }
                catch (Exception ex)
                {
                    lbl_error_message.Text = ex.Message;
                    lbl_error_message.ForeColor = Color.Red;
                    timer_Error_message.Enabled = true;
                }
            }
        }

        private void stockLowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                sql = "Select * from Stocks where Quantity <= '" + quantity + "' order by `Entry Date` desc";
                config.Load_DTG(sql, dtg_Items);
                Calculator_Timer.Start();
                lbl_stock_low.Text = "Stock Low Detected!";
                //current_page_val.Text = "0";
            }
            catch (Exception ex)
            {
                lbl_error_message.Text = "An error occurred: " + ex.Message;
            }
        }
        //private void Load_Items(string sql)
        //{
        //    try
        //    {
        //        if (dtg_Items.Rows.Count == 0)
        //        {
        //            // Handle the null case, such as initializing the DataGridView or displaying an error message
        //            return;
        //        }
        //        else
        //        {
        //            dtg_Items.Columns.Clear();
        //            // calculate the total number of records and pages
        //            int totalRecords = config.GetTotalRecords(sql);

        //            double num_records = 0;
        //            double.TryParse(cbo_num_records.Text, out num_records);

        //            double totalPages = (int)Math.Ceiling((double)totalRecords / num_records);

        //            // update the maximum number
        //            num_max_pages.Maximum = Convert.ToDecimal(totalPages);
        //            num_max_pages.Value = Convert.ToDecimal(totalPages);

        //            // get the current page number and records per page from the paginator control

        //            int currentpage = (int)current_page_val.Value;
        //            int recordsperpage = (int)num_records;

        //            // build the sql query based on the search criteria

        //            // load the data into the datagridview with pagination
        //            config = new SQLConfig();
        //            config.Load_DTG_Paginator(sql, dtg_Items, currentpage, recordsperpage);

        //            DTG_Property();
        //        }
            
        //    }
        //    catch (Exception ex)
        //    {
        //        lbl_error_message.Text = ex.Message;
        //    } 
        //}

        private void cbo_num_records_KeyPress(object sender, KeyPressEventArgs e)
        {
            func = new usableFunction();
            func.combobox_numbers_only(sender, e);
        }

     

        private void dtp_warranty_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            /// Use for loading the current date with for formatting for warranty
            dtp_warranty.Invoke(new Action(() =>
            {
                dtp_warranty.Text = DateTime.Now.ToString(Includes.AppSettings.DateFormatRetrieve);
            }));
        }

        private void dtp_warranty_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            /// Use for the warranty box
            isWarrantyBusy = false;
        }

        private void itemByWarrantyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtg_Items.Columns.Clear();
            SQLConfig sqlConfig = new SQLConfig();
            DataTable warrantyReport = sqlConfig.GetItemWarrantyReport("item_warranty_report");
            dtg_Items.DataSource = warrantyReport;
            // Adjust column widths
            dtg_Items.Columns["Item Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtg_Items.Columns["Sales Invoice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            enable_them = false;
            SpecialFilterDisabler();
        }

        private void eXPIREDWarrantyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtg_Items.Columns.Clear();
            SQLConfig sqlConfig = new SQLConfig();
            DataTable warrantyReport = sqlConfig.GetItemWarrantyReport("expired_warranty_report");
            dtg_Items.DataSource = warrantyReport;
            // Adjust column widths
            dtg_Items.Columns["Item Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtg_Items.Columns["Sales Invoice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            enable_them = false;
            SpecialFilterDisabler();
        }

        private void fUTUREWarrantyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtg_Items.Columns.Clear();
            SQLConfig sqlConfig = new SQLConfig();
            DataTable warrantyReport = sqlConfig.GetItemWarrantyReport("future_warranty_report");
            dtg_Items.DataSource = warrantyReport;
            // Adjust column widths
            dtg_Items.Columns["Item Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtg_Items.Columns["Sales Invoice"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            enable_them = false;
            SpecialFilterDisabler();
        }

        private void nOWarrantyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtg_Items.Columns.Clear();
            SQLConfig sqlConfig = new SQLConfig();
            DataTable warrantyReport = sqlConfig.GetItemWarrantyReport("item_without_warranty_report");
            dtg_Items.DataSource = warrantyReport;
            // Adjust column widths
            dtg_Items.Columns["Item Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtg_Items.Columns["Barcode ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dtg_Items.Columns["Transaction Reference"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            enable_them = false;
            SpecialFilterDisabler();
        }

        private void refreshToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dtg_Items.Columns.Count >= 1)
            {
                dtg_Items.Columns.Clear();
            }

            button1_Click(sender, e);
            SupplierChangeDisabler();
            enable_them = true;
            SpecialFilterDisabler();
            chk_select_all.Checked = false;
            EnablePaginatorControl();
            this.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            sql = "Select * from Stocks ORDER BY `Entry Date` DESC ";
            config.Load_DTG(sql, dtg_Items);
            DTG_Property();
            Calculator_Timer.Start();
        }

        private void txt_Price_Click(object sender, EventArgs e)
        {
            txt_Price.SelectionStart = 0;
            txt_Price.SelectionLength = txt_Price.Text.Length;
        }

        private void txt_Price_KeyPress(object sender, KeyPressEventArgs e)
        {
            func.Make_Numeric_Only(sender, e);
        }

        private void supplierListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustSupplier.CustSupp frm = new CustSupplier.CustSupp(Global_ID, Fullname, JobRole, "Sup");

            DialogResult result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                txt_SupID.Text = frm.supID;
                chk_select_all.Checked = false;
            }
        }

        private void txt_SupID_TextChanged(object sender, EventArgs e)
        {
            sql = "";
            config = new SQLConfig();
            sql = "Select `Company Name` from Supplier where `Company ID` = '" + txt_SupID.Text + "' ";
            config.singleResult(sql);
            if (config.dt.Rows.Count > 0)
            {
                txt_Sup_Name.Text = config.dt.Rows[0].Field<string>("Company Name");
                func.Reload_Images(Sup_Image, txt_SupID.Text, Includes.AppSettings.Supplier_DIR);
            }
            if (txt_SupID.Text == "")
            {
                txt_Sup_Name.Text = "";
            }
        }
        private void btn_AddStock_Click(object sender, EventArgs e)
        {

            chk_select_all.Checked = false;
            if (string.IsNullOrWhiteSpace(txt_Barcode.Text))
            {
                Barcode_Generator();

            }
            else
            {
                config = new SQLConfig();
                sql = "";
                sql = "Select `Stock ID` from Stocks where `Stock ID` = '" + txt_Barcode.Text + "' ";
                config.singleResult(sql);
                if (config.dt.Rows.Count > 0)
                {
                    if (MessageBox.Show("Barcode already exists! Would you like to generate new id and save the item?", "Duplicate Found", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Barcode_Generator();
                        txt_Barcode.Focus();
                    }
                    else
                    {
                        txt_Barcode.Text = "";
                        txt_Barcode.Focus();
                        return;
                    }

                }
                else
                {
                    Item_ID1 = txt_Barcode.Text;
                }
            }

            if (string.IsNullOrWhiteSpace(txt_Barcode.Text))
            {
                func.Error_Message1 = "Item ID is not Generated!";
                txt_Barcode.Focus();
                func.Error_Message();
                Item_ID1 = "ERROR_ID";
                return;

            }
            else if (string.IsNullOrWhiteSpace(txt_ItemName.Text))
            {
                func.Error_Message1 = "Item Name";
                txt_ItemName.Focus();
                func.Error_Message();
                return;

            }
            else if (string.IsNullOrWhiteSpace(cbo_brand.Text))
            {
                func.Error_Message1 = "Item Brand";
                cbo_brand.Focus();
                func.Error_Message();
                return;

            }
            else if (string.IsNullOrWhiteSpace(cbo_desc.Text))
            {
                cbo_desc.Text = "None";

            }
            //else if (txt_Qty.Value == 0 || txt_Qty.Text == null)
            else if (string.IsNullOrWhiteSpace(txt_Qty.Text))
            {
                func.Error_Message1 = "Item Quantity";
                txt_Qty.Focus();
                func.Error_Message();
                return;

            }
            else if (string.IsNullOrWhiteSpace(txt_SupID.Text))
            {
                func.Error_Message1 = "Supplier";
                txt_SupID.Focus();
                func.Error_Message();
                return;
            }
            else if (string.IsNullOrWhiteSpace(txt_TransRef.Text))
            {
                func.Error_Message1 = "Transaction Reference";
                txt_TransRef.Focus();
                func.Error_Message();
                btn_Gen_Click(sender, e);
                txt_TransRef.Focus();
                return;
            }
            else
            {
                string stat_info = "Inbound from supplier " + txt_Sup_Name.Text;
                img_loc = item_image_location + txt_Barcode.Text + ".PNG";
                sql = "";
                config = new SQLConfig();
                sql = "Insert into Stocks ( " +
                    " `Entry Date` " +
                    ",`Stock ID` " +
                    ",`Item Name` " +
                    ",`Brand` " +
                    ",`Description` " +
                    ",`Quantity` " +
                    ",`Price` " +
                    ",`Total` " +
                    ",`Image Path` " +
                    ",`Supplier ID` " +
                    ",`Supplier Name` " +
                    ",`User ID` " +
                    ",`Warehouse Staff Name` " +
                    ",`Job Role` " +
                    ",`Transaction Reference` " +
                    ",`Status` ) values (" +
                    " '" + DateTime.Now.ToString(Includes.AppSettings.DateFormatSave) + "' " +
                    ",'" + txt_Barcode.Text + "' " +
                    ",'" + txt_ItemName.Text + "' " +
                    ",'" + cbo_brand.Text + "' " +
                    ",'" + cbo_desc.Text + "' " +
                    ",'" + Convert.ToInt32(txt_Qty.Text) + "' " +
                    ",'" + Convert.ToDecimal(txt_Price.Text) + "' " +
                    ",'" + Convert.ToDecimal(lbl_ProductValue.Text) + "' " +
                    ",'" + img_loc + "' " +
                    ",'" + txt_SupID.Text + "' " +
                    ",'" + txt_Sup_Name.Text + "' " +
                    ",'" + Global_ID + "' " +
                    ",'" + Fullname + "' " +
                    ",'" + JobRole + "' " +
                    ",'" + txt_TransRef.Text + "'" +
                    ",'" + stat_info + "' )";
                config.Execute_CUD(sql, "Unable to Record Item!", "Item successfully added to record!");

                bool success;
                success = config.CreateOrUpdateInboundWarranty(dtp_warranty.Value, txt_Barcode.Text, txt_TransRef.Text, "insert");

                save_Ref = txt_TransRef.Text;
                newItemToolStripMenuItem_Click(sender, e);
                txt_ItemName.Focus();
                txt_Search_TextChanged(sender, e);
                DTG_Property();
                txt_TransRef.Text = save_Ref;
                if (dtg_Items.Rows.Count > 0)
                {
                    dtg_Items.Rows[0].Selected = true;
                }
                Calculator_Timer.Start();
                return;
            }
        }
        string save_Ref = string.Empty;
    }
}
