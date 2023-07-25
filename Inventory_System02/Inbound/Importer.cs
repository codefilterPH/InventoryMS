using CsvHelper;
using Inventory_System02.Includes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;



namespace Inventory_System02.Inbound
{
    public partial class Import : Form
    {
        string Global_ID, Fullname, JobRole;
        public Import(string global_id, string fullname, string jobrole)
        {
            InitializeComponent();
            Global_ID = global_id;
            Fullname = fullname;
            JobRole = jobrole;
        }

        private string GetUniqueFileName(string originalFilename, string destinationPath)
        {
            var filename = originalFilename;
            var fullDestinationPath = Path.Combine(destinationPath, filename);

            while (File.Exists(fullDestinationPath))
            {
                var datetimeString = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                var datetimeHash = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(datetimeString))).Replace("-", "").ToLower();
                datetimeHash = datetimeHash.Substring(0, 10);
                filename = Path.GetFileNameWithoutExtension(originalFilename) + "_" + datetimeHash + Path.GetExtension(originalFilename);
                fullDestinationPath = Path.Combine(destinationPath, filename);
                Thread.Sleep(10);
            }

            return filename;
        }

        private async void btn_import_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                try
                {
                    Console.WriteLine(Includes.AppSettings.Imports_Dir);
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var filePath = openFileDialog.FileName;
                        var originalFilename = Path.GetFileName(filePath);
                        var datetimeString = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var datetimeHash = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(datetimeString))).Replace("-", "").ToLower();
                        var filename = Path.GetFileNameWithoutExtension(filePath) + "_" + datetimeHash + Path.GetExtension(filePath);
                        var uniqueFilename = GetUniqueFileName(filename, Includes.AppSettings.Imports_Dir);
                        var destinationPath = Path.Combine(Includes.AppSettings.Imports_Dir, uniqueFilename);

                        try
                        {
                            File.Copy(filePath, destinationPath, true);
                            await ReadCsvFileAsync(destinationPath);
                            lbl_error_message.Text = $"Successfully imported: {originalFilename}";
                            lbl_error_message.ForeColor = Color.Green;
                            Calculator_Timer.Start();
                        }
                        catch (Exception ex)
                        {
                            lbl_error_message.Text = $"Failed to import: {originalFilename}. Error: {ex.Message}";
                            lbl_error_message.ForeColor = Color.Red;
                        }
                    }

                }
                catch (Exception ex)
                {
                    lbl_error_message.Text = $"An error occurred: {ex.Message}";
                    lbl_error_message.ForeColor = Color.Red;
                }
            }
        }

        private async Task ReadCsvFileAsync(string path)
        {
            // Converting both CSV column names and DataGridView column HeaderText to lower case
            var dgvColumnNames = dtg_Items.Columns.Cast<DataGridViewColumn>().ToDictionary(column => column.HeaderText.ToLower(), column => column.Index);

            try
            {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<dynamic>();
                    foreach (var record in records)
                    {
                        var recordDict = record as IDictionary<string, object>;
                        var newRow = new DataGridViewRow();
                        newRow.CreateCells(dtg_Items);

                        foreach (var pair in recordDict)
                        {
                            // Converting CSV column names to lower case to match with DataGridView HeaderText
                            var csvHeader = pair.Key.ToLower();
                            if (dgvColumnNames.TryGetValue(csvHeader, out var index))
                            {
                                newRow.Cells[index].Value = pair.Value.ToString();
                            }
                        }
                        dtg_Items.Rows.Add(newRow);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        private string Barcode_Generator()
        {
            try
            {
                string id = string.Empty;
                bool hasDuplicate = true;
                string sql = string.Empty;
                SQLConfig config = new SQLConfig();
                ID_Generator gen = new ID_Generator();

                while (hasDuplicate)
                {
                    id = gen.Item_ID();
                    sql = "SELECT COUNT(*) FROM `Stocks` WHERE `Stock ID` = '" + id + "'";
                    config.singleResult(sql);
                    if (Convert.ToInt32(config.dt.Rows[0][0]) > 0)
                    {
                        continue;
                    }
                    else
                    {
                       
                        hasDuplicate = false;
                        
                    }
                }
                Console.WriteLine(id);
                return id;
            }
            catch (Exception ex)
            {
                lbl_error_message.Text = "An error occurred: " + ex.Message;
            }
            return null;
        }

        private void ErrorPrinter(string text)
        {
            MessageBox.Show($"{text} is missing!");
        }
        private void btn_save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_SupID.Text))
            {
                ErrorPrinter("Supplier ID");
                txt_SupID.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(txt_Sup_Name.Text))
            {
                ErrorPrinter("Supplier Name");
                txt_Sup_Name.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(txt_TransRef.Text))
            {
                ErrorPrinter("Transaction Reference");
                txt_TransRef.Focus();
                return;
            }
            else
            {

                SQLConfig config = new SQLConfig();
                string sqlBase = "Insert into Stocks (`Entry Date`, `Stock ID`, `Item Name`, `Brand`, `Description`, `Quantity`, `Price`, `Total`, `Image Path`, `Supplier ID`, `Supplier Name`, `User ID`, `Warehouse Staff Name`, `Job Role`, `Transaction Reference`, `Status`) values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                parameters.Add("item_image_location", Includes.AppSettings.Image_DIR + "DONOTDELETE_SUBIMAGE");
                parameters.Add("txt_Sup_Name", txt_Sup_Name.Text);
                parameters.Add("txt_SupID", txt_SupID.Text);
                parameters.Add("Global_ID", Global_ID);
                parameters.Add("Fullname", Fullname);
                parameters.Add("JobRole", JobRole);
                parameters.Add("txt_TransRef", txt_TransRef.Text);

                int totalRowsAffected = 0;
                for (int i = 0; i < dtg_Items.Rows.Count; i++)
                {
                    if (!dtg_Items.Rows[i].IsNewRow)
                    {
                        string barcode = Barcode_Generator();
                        parameters["Stock_ID"] = barcode;

                        totalRowsAffected += config.InsertSingleRow(sqlBase, dtg_Items.Rows[i], parameters);
                    }
                }
                StatusSavingReporter(totalRowsAffected);

                parameters.Clear();

            }
        }
        private void StatusSavingReporter(int operations)
        {
            if (operations <= 0)
            {
                MessageBox.Show("No operations were executed!");
            }
            else if (operations == 1)
            {
                MessageBox.Show("1 operation successfully executed!");
            }
            else
            {
                MessageBox.Show($"{operations / 2} operations successfully executed!");
            }
        }

        private void btn_searchSup_Click(object sender, EventArgs e)
        {
            CustSupplier.CustSupp frm = new CustSupplier.CustSupp(Global_ID, Fullname, JobRole, "Sup");

            DialogResult result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                txt_SupID.Text = frm.supID;
            }
        }

        private void txt_SupID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                SQLConfig config = new SQLConfig();
                usableFunction func = new usableFunction();

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
            catch ( Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
          
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {  
            this.Close();
        }
        public int CountRows(DataGridView dtg_items)
        {
            // Check if the DataGridView is null
            if (dtg_items == null)
            {
                // Log the error and return a value indicating an error
                Console.WriteLine("The DataGridView is null.");
                return -1;
            }

            // Try to count the non-empty rows in the DataGridView
            try
            {
                int nonEmptyRowCount = 0;

                foreach (DataGridViewRow row in dtg_items.Rows)
                {
                    if (row.Cells.Cast<DataGridViewCell>().Any(cell => cell.Value != null && !String.IsNullOrWhiteSpace(cell.Value.ToString())))
                    {
                        nonEmptyRowCount++;
                    }
                }

                return nonEmptyRowCount;
            }
            catch (Exception ex)
            {
                // If an error occurred while counting the rows, log the error
                Console.WriteLine("An error occurred while counting rows: " + ex.Message);
                // Return a value indicating an error
                return -1;
            }
        }

        private void Calculator_Timer_Tick(object sender, EventArgs e)
        {
            lbl_items_count.Text = CountRows(dtg_Items).ToString();
            foreach (DataGridViewRow row in dtg_Items.Rows)
            {
                // Check if the entire row is empty
                if (row.Cells.Cast<DataGridViewCell>().All(c => c.Value == null || string.IsNullOrWhiteSpace(c.Value.ToString())))
                {
                    // Skip this row if it's empty
                    continue;
                }

                var warrantyCell = row.Cells["warranty"];
                if (warrantyCell.Value == null || string.IsNullOrWhiteSpace(warrantyCell.Value.ToString()))
                {
                    warrantyCell.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            Calculator_Timer.Stop();


        }

        private void dtg_Items_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Calculator_Timer.Start();
        }

        private void btn_unload_Click(object sender, EventArgs e)
        {
            if (dtg_Items.Rows.Count >= 1 )
            {
                dtg_Items.Rows.Clear();
                Calculator_Timer.Start();
            }
        }

      

        private void btn_Gen_Click(object sender, EventArgs e)
        {
            string sql = string.Empty;
            SQLConfig config = new SQLConfig();
            string id = string.Empty;
            bool hasDuplicate = true;

            try
            {
                while (hasDuplicate)
                {
                    ID_Generator gen = new ID_Generator();
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                lbl_error_message.Text = "An error occurred: " + ex.Message;
            }

        }

        private void Import_Load(object sender, EventArgs e)
        {

            btn_Gen_Click(sender, e);
            btn_searchSup_Click(sender, e);
        }
    }
}
