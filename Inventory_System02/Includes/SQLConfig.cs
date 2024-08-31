
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComboBox = System.Windows.Forms.ComboBox;
using DataSet = System.Data.DataSet;
using TextBox = System.Windows.Forms.TextBox;

namespace Inventory_System02.Includes
{
    class SQLConfig
    {
        //public string computerName = System.Environment.MachineName;
        //private MySqlConnection con = new MySqlConnection("data source = 192.168.254.110; port = 3306; user id = root ; database = bhms ; password = pass;");
        //private MySqlConnection con = new MySqlConnection("server = localhost; user id = root ; database = bhms ; password = ;");

        SQLiteConnection con;
        SQLiteCommand cmd;
        SQLiteDataAdapter da;
        public DataTable dt;
        int result;
        usableFunction funct = new usableFunction();

        public void ConnectionString()
        {
            try
            {
                //con = new SQLiteConnection(Includes.AppSettings.Database(), true);
                // Create a new SQLite connection using the connection string from the AppSettings
                string connectionString = Includes.AppSettings.Database();
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("The connection string is null or empty.");
                }
                con = new SQLiteConnection(connectionString, true);
                if (con == null)
                {
                    throw new InvalidOperationException("Failed to initialize the database connection.");
                }
            }
            catch (Exception ex)
            {
                // log or show the exception message
                Console.WriteLine(ex.Message);
            }
        }




        [SQLiteFunction(Name = "Sha1", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Sha1 : SQLiteFunction
        {

            public override object Invoke(object[] args)
            {
                var buffer = args[0] as byte[];

                if (buffer == null)
                {
                    var s = args[0] as string;

                    if (s != null)
                        buffer = Encoding.Unicode.GetBytes(s);
                }

                if (buffer == null)
                    return null;

                using (var sha1 = SHA1.Create())
                {
                    return sha1.ComputeHash(buffer);
                }
            }
        }
        [SQLiteFunction(Name = "Sha512", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Sha512 : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                var buffer = args[0] as byte[];

                if (buffer == null)
                {
                    var s = args[0] as string;

                    if (s != null)
                        buffer = Encoding.Unicode.GetBytes(s);
                }

                if (buffer == null)
                    return null;

                using (var sha512 = SHA512.Create())
                {
                    return sha512.ComputeHash(buffer);
                }
            }
        }

        public void Execute_CUD(string sql, string msg_false, string msg_true)
        {
            ConnectionString();
            //execute query ( command, iffalse, iftrue)
            try
            {
                con.Open();
                cmd = new SQLiteCommand();
                cmd.Connection = con;
                cmd.CommandText = sql;
                result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show(msg_true);
                }
                else
                {
                    MessageBox.Show(msg_false);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public void Execute_Query(string sql)
        {
            ConnectionString();
            //execute query ( command)
            try
            {
                con.Open();
                cmd = new SQLiteCommand();
                cmd.Connection = con;
                cmd.CommandText = sql;
                result = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        public string ExecuteQueryAndReturnStringWithParameters(string sql, Dictionary<string, object> parameters)
        {
            string resultString = string.Empty;

            try
            {
                using (con = new SQLiteConnection(Includes.AppSettings.Database()))
                {
                    con.Open();

                    using (cmd = new SQLiteCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = sql;

                        // Add parameters if provided
                        if (parameters != null)
                        {
                            foreach (var parameter in parameters)
                            {
                                cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }
                        }

                        // Execute the query and return the result as a string
                        resultString = cmd.ExecuteScalar()?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return resultString;
        }


        public int InsertSingleRow(string sqlBase, DataGridViewRow row, Dictionary<string, string> parameters)
        {
            ConnectionString();
            con.Open();
            int rowsAffected = 0;

            using (SQLiteTransaction transaction = con.BeginTransaction())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    if (!row.IsNewRow)
                    {
                        string stat_info = "Inbound from supplier " + (string.IsNullOrEmpty(parameters["txt_Sup_Name"]) ? string.Empty : parameters["txt_Sup_Name"]);
                        string img_loc = parameters["item_image_location"] + (string.IsNullOrEmpty(parameters["Stock_ID"]) ? string.Empty : parameters["Stock_ID"]) + ".PNG";
                        string sql = sqlBase;
                        cmd.CommandText = sql;

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue(null, DateTime.Now.ToString(Includes.AppSettings.DateFormatSave));
                        cmd.Parameters.AddWithValue(null, string.IsNullOrEmpty(parameters["Stock_ID"]) ? string.Empty : parameters["Stock_ID"]);
                        cmd.Parameters.AddWithValue(null, row.Cells["name"].Value is null ? string.Empty : row.Cells["name"].Value.ToString());
                        cmd.Parameters.AddWithValue(null, row.Cells["brand"].Value is null ? string.Empty : row.Cells["brand"].Value.ToString());
                        cmd.Parameters.AddWithValue(null, row.Cells["description"].Value is null ? string.Empty : row.Cells["description"].Value.ToString());
                        cmd.Parameters.AddWithValue(null, row.Cells["quantity"].Value is null ? "0" : row.Cells["quantity"].Value.ToString());
                        cmd.Parameters.AddWithValue(null, row.Cells["price"].Value is null ? "0" : row.Cells["price"].Value.ToString());
                        cmd.Parameters.AddWithValue(null, row.Cells["total"].Value is null ? "0" : row.Cells["total"].Value.ToString());
                        cmd.Parameters.AddWithValue(null, img_loc);
                        cmd.Parameters.AddWithValue(null, string.IsNullOrEmpty(parameters["txt_SupID"]) ? string.Empty : parameters["txt_SupID"]);
                        cmd.Parameters.AddWithValue(null, string.IsNullOrEmpty(parameters["txt_Sup_Name"]) ? string.Empty : parameters["txt_Sup_Name"]);
                        cmd.Parameters.AddWithValue(null, string.IsNullOrEmpty(parameters["Global_ID"]) ? string.Empty : parameters["Global_ID"]);
                        cmd.Parameters.AddWithValue(null, string.IsNullOrEmpty(parameters["Fullname"]) ? string.Empty : parameters["Fullname"]);
                        cmd.Parameters.AddWithValue(null, string.IsNullOrEmpty(parameters["JobRole"]) ? string.Empty : parameters["JobRole"]);
                        cmd.Parameters.AddWithValue(null, string.IsNullOrEmpty(parameters["txt_TransRef"]) ? string.Empty : parameters["txt_TransRef"]);
                        cmd.Parameters.AddWithValue(null, stat_info);

                        rowsAffected += cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT COUNT(*) FROM Inbound_Warranty WHERE barcode_id=@barcode_id AND trans_ref=@trans_ref";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@barcode_id", parameters["Stock_ID"]);
                        cmd.Parameters.AddWithValue("@trans_ref", parameters["txt_TransRef"]);

                        var recordExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                        cmd.Parameters.Clear();

                        if (recordExists)
                        {
                            cmd.CommandText = "UPDATE Inbound_Warranty SET warranty=@warranty WHERE barcode_id=@barcode_id AND trans_ref=@trans_ref";
                        }
                        else
                        {
                            cmd.CommandText = "INSERT INTO Inbound_Warranty (warranty, barcode_id, trans_ref) VALUES (@warranty, @barcode_id, @trans_ref)";
                        }

                        DateTime warrantyDate;
                        string warrantyValue = row.Cells["warranty"].Value?.ToString() ?? string.Empty;
                        if (string.IsNullOrEmpty(warrantyValue) || !DateTime.TryParse(warrantyValue, out warrantyDate))
                        {
                            warrantyDate = DateTime.Now;
                        }
                        string warranty = warrantyDate.ToString(Includes.AppSettings.DateFormatSave);

                        cmd.Parameters.AddWithValue("@warranty", warranty);
                        cmd.Parameters.AddWithValue("@barcode_id", parameters["Stock_ID"]);
                        cmd.Parameters.AddWithValue("@trans_ref", parameters["txt_TransRef"]);

                        rowsAffected += cmd.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }

            con.Close();

            return rowsAffected;
        }


        public void Load_DTG(string sql, DataGridView dtg)
        {
            ConnectionString();
            try
            {

                con.Open();
                cmd = new SQLiteCommand();
                da = new SQLiteDataAdapter();
                dt = new DataTable();


                cmd.Connection = con;
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                da.Fill(dt);
                dtg.DataSource = dt;


                funct.ResponsiveDtg(dtg);
                //dtg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //dtg.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                da.Dispose();
                con.Close();
            }
        }

        //ASYNC DTG LOAD
        public async Task Load_DTG_Async(string sql, DataGridView dtg)
        {
            DataTable data = new DataTable();

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Includes.AppSettings.Database(), true))
                using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                {
                    await con.OpenAsync();

                    using (SQLiteDataReader reader = (SQLiteDataReader)await cmd.ExecuteReaderAsync())
                    {
                        data.Load(reader); // This will load the reader data into the DataTable and preserve the column names and types
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            dtg.Invoke((MethodInvoker)delegate
            {
                dtg.DataSource = data; // Set the data source of DataGridView to the DataTable

                funct.ResponsiveDtg(dtg);
            });
        }


        public int GetTotalRecords(string sql)
        {
            ConnectionString();
            int totalRecords = 0;

            try
            {
                // remove the ORDER BY and LIMIT clauses from the SQL query
                string countSql = Regex.Replace(sql, @"ORDER BY .+?$", "", RegexOptions.IgnoreCase);
                countSql = Regex.Replace(countSql, @"LIMIT .+?$", "", RegexOptions.IgnoreCase);


                // add a SELECT COUNT(*) clause to the SQL query
                countSql = $"SELECT COUNT(*) FROM ({countSql})";

                cmd = new SQLiteCommand(countSql, con);
                con.Open();
                totalRecords = Convert.ToInt32(cmd.ExecuteScalar());

                Console.WriteLine("total records issue");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return totalRecords;
        }

        public void Load_DTG_Paginator(string sql, DataGridView dtg, int currentPage, int recordsPerPage)
        {
            ConnectionString();

            try
            {
                Console.WriteLine("paginator issue");
                con.Open();

                // calculate the offset based on the current page and records per page
                int offset = (currentPage - 1) * recordsPerPage;

                // add LIMIT and OFFSET clauses to the SQL query
                sql += $" LIMIT {recordsPerPage} OFFSET {offset}";

                cmd = new SQLiteCommand();
                da = new SQLiteDataAdapter();
                dt = new DataTable();

                cmd.Connection = con;
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                da.Fill(dt);
                dtg.DataSource = dt;

                funct.ResponsiveDtg(dtg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                da.Dispose();
                con.Close();
            }
            return;
        }

        public void Load_Datasource(string sql, DataSet dtg)
        {
            ConnectionString();
            //to datagrid
            try
            {
                con.Open();
                cmd = new SQLiteCommand();
                da = new SQLiteDataAdapter();


                cmd.Connection = con;
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                da.Fill(dtg);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                da.Dispose();
                con.Close();
            }
            return;
        }
        public void fiil_CBO(string sql, ComboBox cbo)
        {
            ConnectionString();
            try
            {
                con.Open();
                cmd = new SQLiteCommand();
                da = new SQLiteDataAdapter();
                dt = new DataTable();


                cmd.Connection = con;
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                da.Fill(dt);
                cbo.DataSource = dt;
                cbo.ValueMember = dt.Columns[0].ColumnName;
                cbo.DisplayMember = dt.Columns[0].ColumnName;



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                da.Dispose();
                con.Close();
            }
        }
        public void singleResult(string sql)
        {
            ConnectionString();
            try
            {
                con.Open();
                cmd = new SQLiteCommand();
                da = new SQLiteDataAdapter();
                dt = new DataTable();


                cmd.Connection = con;
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                da.Fill(dt);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                da.Dispose();
                con.Close();
            }
        }

        public void LoadReports(string sql, DataSet ds)

        {
            ConnectionString();
            try
            {
                con.Open();
                cmd = new SQLiteCommand();
                da = new SQLiteDataAdapter();
                //DataTable dt = new DataTable();

                using (con)
                {
                    using (da = new SQLiteDataAdapter())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = sql;
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                    }


                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                da.Dispose();
                con.Close();
            }
        }

        public void autocomplete(string sql, ComboBox txt)
        {
            ConnectionString();
            try
            {

                con.Open();
                cmd = new SQLiteCommand();
                da = new SQLiteDataAdapter();
                dt = new DataTable();

                cmd.Connection = con;
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                da.Fill(dt);
                txt.AutoCompleteCustomSource.Clear();
                txt.AutoCompleteMode = AutoCompleteMode.Suggest;
                txt.AutoCompleteSource = AutoCompleteSource.CustomSource;

                foreach (DataRow r in dt.Rows)
                {
                    txt.AutoCompleteCustomSource.Add(r.Field<string>(0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                da.Dispose();
                con.Close();
            }
        }


        public void autonumber(string sql, TextBox txt)
        {
            ConnectionString();
            try
            {
                con.Open();
                cmd = new SQLiteCommand();
                da = new SQLiteDataAdapter();
                dt = new DataTable();


                cmd.Connection = con;
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                da.Fill(dt);

                txt.Text = dt.Rows[0].Field<string>(0);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                da.Dispose();
                con.Close();
            }
        }
        public void update_Autonumber(string id)
        {
            ConnectionString();
            Execute_Query("UPDATE `tblautonumber` SET `END`=`END`+`INCREMENT` WHERE `DESCRIPTION`='" + id + "'");
        }


        public void New_Autocomplete(string sql, TextBox txt)
        {
            ConnectionString();
            try
            {
                con.Open();
                cmd = new SQLiteCommand(sql, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                AutoCompleteStringCollection MyCollection = new AutoCompleteStringCollection();

                while (reader.Read())
                {
                    MyCollection.Add(reader.GetString(0));
                }

                txt.AutoCompleteMode = AutoCompleteMode.Suggest;
                txt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txt.AutoCompleteCustomSource = MyCollection;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public bool CreateOrUpdateInboundWarranty(DateTime warranty, string barcode_id, string trans_ref, string todo)
        {
            ConnectionString();
            try
            {
                con.Open();
                cmd = new SQLiteCommand(con);

                // Verify if the table exists
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Inbound_Warranty'";
                var tableExists = cmd.ExecuteScalar() != null;

                if (!tableExists)
                {
                    // Create the table if it doesn't exist
                    cmd.CommandText = "CREATE TABLE Inbound_Warranty (id INTEGER PRIMARY KEY AUTOINCREMENT, warranty TEXT, barcode_id TEXT, trans_ref TEXT)";
                    cmd.ExecuteNonQuery();
                }

                if (todo == "delete")
                {
                    if (tableExists)
                    {
                        // Delete the warranty record based on barcode_id and trans_ref
                        cmd.CommandText = "DELETE FROM Inbound_Warranty WHERE barcode_id=@barcode_id AND trans_ref=@trans_ref";
                        cmd.Parameters.AddWithValue("@barcode_id", barcode_id);
                        cmd.Parameters.AddWithValue("@trans_ref", trans_ref);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Table 'Inbound_Warranty' does not exist.");
                        return false;
                    }
                }
                else if (todo == "update" || todo == "insert")
                {
                    // Verify if the record exists based on barcode_id and trans_ref
                    cmd.CommandText = "SELECT COUNT(*) FROM Inbound_Warranty WHERE barcode_id=@barcode_id AND trans_ref=@trans_ref";
                    cmd.Parameters.AddWithValue("@barcode_id", barcode_id);
                    cmd.Parameters.AddWithValue("@trans_ref", trans_ref);
                    var recordExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;

                    if (recordExists)
                    {
                        // Update the existing record
                        cmd.CommandText = "UPDATE Inbound_Warranty SET warranty=@warranty WHERE barcode_id=@barcode_id AND trans_ref=@trans_ref";
                    }
                    else
                    {
                        // Insert a new record
                        cmd.CommandText = "INSERT INTO Inbound_Warranty (warranty, barcode_id, trans_ref) VALUES (@warranty, @barcode_id, @trans_ref)";
                    }

                    // Set the parameter values
                    cmd.Parameters.AddWithValue("@warranty", warranty.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.ExecuteNonQuery();
                    return true;
                }
                else if (todo == "select")
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return false; // Return false if an exception occurred or the operation was not executed
        }


        public void DeleteAllRowsFromTable(string tableName)
        {
            ConnectionString();
            try
            {
                con.Open();
                cmd = new SQLiteCommand(con);

                // Verify if the table exists
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
                cmd.Parameters.AddWithValue("@tableName", tableName);
                var tableExists = cmd.ExecuteScalar() != null;

                if (tableExists)
                {
                    // Delete all rows from the table
                    cmd.CommandText = "DELETE FROM " + tableName;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // Create the table if it doesn't exist
                    cmd.CommandText = "CREATE TABLE " + tableName + " (id INTEGER PRIMARY KEY AUTOINCREMENT, warranty TEXT, barcode_id TEXT, trans_ref TEXT)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public bool CreateOrUpdateTermsTable(string supplierId, string terms, string todo)
        {
            ConnectionString();
            try
            {
                con.Open();
                cmd = new SQLiteCommand(con);

                // Verify if the table exists
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Terms'";
                var tableExists = cmd.ExecuteScalar() != null;

                if (!tableExists)
                {
                    // Create the table if it doesn't exist
                    cmd.CommandText = "CREATE TABLE Terms (entry_date TEXT, terms TEXT, supplier_id TEXT, last_modified TEXT)";
                    cmd.ExecuteNonQuery();
                }

                // Check if the terms already exist for the supplier
                cmd.CommandText = "SELECT COUNT(*) FROM Terms WHERE supplier_id=@supplier_id";
                cmd.Parameters.AddWithValue("@supplier_id", supplierId);
                var recordExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;

                if (todo == "insert")
                {
                    if (!recordExists)
                    {
                        // Insert a new record
                        cmd.CommandText = "INSERT INTO Terms (entry_date, terms, supplier_id, last_modified) VALUES (@entry_date, @terms, @supplier_id, @last_modified)";
                        cmd.Parameters.AddWithValue("@entry_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                else if (todo == "update")
                {
                    if (recordExists)
                    {
                        // Update the existing terms and last_modified
                        cmd.CommandText = "UPDATE Terms SET terms=@terms, last_modified=@last_modified WHERE supplier_id=@supplier_id";
                    }
                    else
                    {
                        // Insert a new record
                        cmd.CommandText = "INSERT INTO Terms (entry_date, terms, supplier_id, last_modified) VALUES (@entry_date, @terms, @supplier_id, @last_modified)";
                        cmd.Parameters.AddWithValue("@entry_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }

                // Set the parameter values
                cmd.Parameters.AddWithValue("@terms", terms);
                cmd.Parameters.AddWithValue("@last_modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.ExecuteNonQuery();
                // MessageBox.Show("Terms updated/inserted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                con.Close();
            }
        }
        //NEW WARRANTY REPORTS
        public DataTable GetItemWarrantyReport(string need)
        {
            ConnectionString();
            dt = new DataTable();
            string sql = string.Empty;


            if (need == "item_warranty_report")
            {
                sql = @"SELECT s.`Item Name`, iw.warranty  as Warranty, iw.trans_ref as `Sales Invoice`
                   FROM Inbound_Warranty iw 
                   JOIN stocks s 
                   ON iw.barcode_id = s.`Stock ID` AND iw.trans_ref = s.`Transaction Reference`
                   ORDER BY iw.warranty DESC";
            }
            else if (need == "expired_warranty_report")
            {
                sql = @"SELECT s.`Item Name`, iw.warranty as Warranty, iw.trans_ref as `Sales Invoice`
                    FROM Inbound_Warranty iw 
                    JOIN stocks s 
                    ON iw.barcode_id = s.`Stock ID` AND iw.trans_ref = s.`Transaction Reference`
                    WHERE datetime('now') > date(iw.warranty)
                    ORDER BY iw.warranty DESC";
            }
            else if (need == "future_warranty_report")
            {
                sql = @"SELECT s.`Item Name`, iw.warranty as Warranty, iw.trans_ref as `Sales Invoice`
                    FROM Inbound_Warranty iw 
                    JOIN stocks s 
                    ON iw.barcode_id = s.`Stock ID` AND iw.trans_ref = s.`Transaction Reference`
                    WHERE datetime('now') < date(iw.warranty)
                    ORDER BY iw.warranty DESC";
            }
            else if (need == "item_without_warranty_report")
            {
                sql = @"SELECT s.`Stock ID` as `Barcode ID`, s.`Item Name`, s.`Transaction Reference`
                    FROM stocks s 
                    LEFT JOIN Inbound_Warranty iw 
                    ON iw.barcode_id = s.`Stock ID` AND iw.trans_ref = s.`Transaction Reference`
                    WHERE iw.warranty IS NULL";
            }


            try
            {
                con.Open();
                cmd = new SQLiteCommand();
                cmd.Connection = con;
                cmd.CommandText = sql;
                da = new SQLiteDataAdapter(cmd);
                da.Fill(dt);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                da.Dispose();
                con.Close();
            }

            return dt;
        }




    }
}