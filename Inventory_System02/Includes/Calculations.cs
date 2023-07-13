using System;
using System.Data;
using System.Windows.Forms;

namespace Inventory_System02.Includes
{


    class Calculations
    {
        SQLConfig config = new SQLConfig();
        string sql = string.Empty;
        decimal total_amt;
        int total_qty;

        public void CalculateTodayTotals(string table, Label quantity, Label amount, Label lbl_error)
        {
            try
            {
                int todayQty = 0;
                decimal todayAmt = 0;

                sql = $"Select Quantity, Total, `Entry Date` from {table}";
                config.singleResult(sql);
                if (config.dt.Rows.Count > 0)
                {
                    foreach (DataRow row in config.dt.Rows)
                    {
                        DateTime entryDate;
                        if (DateTime.TryParse(row["Entry Date"].ToString(), out entryDate) && entryDate.Date == DateTime.Now.Date)
                        {
                            int qty = 0;
                            decimal total = 0;
                            int.TryParse(row["Quantity"].ToString(), out qty);
                            decimal.TryParse(row["Total"].ToString(), out total);
                            todayQty += qty;
                            todayAmt += total;
                        }
                    }
                }

                quantity.Text = todayQty.ToString();
                amount.Text = todayAmt.ToString();
            }
            catch (Exception ex)
            {
                lbl_error.Text = ex.Message;
            }
        }

        public void CalculateOverallTotals(string table, Label quantity, Label amount, Label lbl_error)
        {
            try
            {
                int totalQty = 0;
                decimal totalAmt = 0;

                // fetch data from the database
                string sql = $"Select Quantity, Total from {table}";
                config.singleResult(sql);

                // check if data exists
                if (config.dt.Rows.Count > 0)
                {
                    // iterate over each row in the data
                    foreach (DataRow row in config.dt.Rows)
                    {
                        try
                        {
                            int qty = 0;
                            decimal total = 0;
                            if (!int.TryParse(row["Quantity"].ToString(), out qty))
                            {
                                throw new Exception("Failed to parse Quantity for row " + row["Quantity"].ToString());
                            }
                            if (!decimal.TryParse(row["Total"].ToString(), out total))
                            {
                                throw new Exception("Failed to parse Total for row " + row["Total"].ToString());
                            }
                            totalQty += qty;
                            totalAmt += total;
                        }
                        catch (Exception ex)
                        {
                            // You can log this exception or display it, here we just display a message
                            MessageBox.Show($"An error occurred while processing a row: {ex.Message}");
                        }
                    }
                }

                quantity.Text = totalQty.ToString();
                amount.Text = totalAmt.ToString();
            }
            catch (Exception ex)
            {
                lbl_error.Text = $"An error occurred while fetching or processing the data: {ex.Message}";
            }
        }



        public void Calculate_Todays_Entry_StockIn(string Type_Of_Process)
        {

            sql = "Select * from " + Type_Of_Process + " where `Entry Date` = '" + DateTime.Now.ToString(Includes.AppSettings.DateFormatRetrieve) + "' ";
            config.singleResult(sql);

            if (config.dt.Rows.Count >= 1)
            {
                total_amt = 0;
                total_qty = 0;
                for (int i = 0; i < config.dt.Rows.Count; i++)
                {
                    int qty;
                    decimal price;
                    int.TryParse(Convert.ToString(config.dt.Rows[i]["Quantity"]), out qty);
                    decimal.TryParse(Convert.ToString(config.dt.Rows[i]["Price"]), out price);

                    decimal amount = qty * price;

                    total_qty += qty;
                    total_amt += amount;
                }

                sql = "Select * from Calculations";
                config.singleResult(sql);
                if (config.dt.Rows.Count > 0)
                {
                    sql = "Update Calculations set `Total Quantity` = '" + total_qty + "', `Total Value` = '" + total_amt + "' ";
                    config.singleResult(sql);
                }
                else
                {
                    sql = "Insert into Calculations ( `Total Quantity`,  `Total Value` ) values ( '" + total_qty + "', '" + total_amt + "') ";
                    config.singleResult(sql);
                }
            }
        }
        public void Over_All()
        {
            sql = "SELECT Quantity, Price FROM Stocks";
            config.singleResult(sql);
            if (config.dt.Rows.Count > 0)
            {

                total_amt = 0;
                total_qty = 0;
                for (int i = 0; i < config.dt.Rows.Count; i++)
                {
                    int qty = 0;
                    decimal price = 0;
                    int.TryParse(config.dt.Rows[i]["Quantity"].ToString(), out qty);
                    decimal.TryParse(config.dt.Rows[i]["Price"].ToString(), out price);
                    decimal amount = qty * price;
                    total_qty += qty;
                    total_amt += amount;
                }

                sql = "SELECT * FROM Calculations";
                config.singleResult(sql);

                if (config.dt.Rows.Count > 0)
                {
                    sql = "UPDATE Calculations SET Overall_Qty = '" + total_qty + "', Overall_Total = '" + total_amt + "'";
                    config.Execute_Query(sql);

                }
                else
                {
                    sql = "INSERT INTO Calculations (Overall_Qty, Overall_Total) VALUES ('" + total_qty + "', '" + total_amt + "')";
                    config.Execute_Query(sql);
                }
            }
        }

        public void ReturnReason(string trans_ref, string cust_id, string type, string remarks)
        {
            sql = "Select * from `Return Reasons` where `Transaction Ref` = '" + trans_ref + "' and `Customer ID` = '" + cust_id + "' ";
            config.singleResult(sql);
            if (config.dt.Rows.Count > 0)
            {

                sql = " Update `Return Reasons` set Reason = '" + type + "', Remarks = '" + remarks + "' where `Transaction Ref` = '" + trans_ref + "' and `Customer ID` = '" + cust_id + "' ";
                config.Execute_Query(sql);
            }
            else
            {

                sql = " Insert into `Return Reasons` ( `Transaction Ref`, `Customer ID`, `Reason`, Remarks ) values ( '" + trans_ref + "', '" + cust_id + "', '" + type + "', '" + remarks + "' )  ";
                config.Execute_Query(sql);
            }
        }
    }
}
