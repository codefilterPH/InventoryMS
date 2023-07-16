namespace Inventory_System02.Inbound
{
    partial class Import
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Import));
            this.dtg_Items = new System.Windows.Forms.DataGridView();
            this.btn_import = new System.Windows.Forms.Button();
            this.btn_save = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Gen = new System.Windows.Forms.Button();
            this.txt_TransRef = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btn_Close = new System.Windows.Forms.Button();
            this.lbl_error_message = new System.Windows.Forms.Label();
            this.btn_searchSup = new System.Windows.Forms.Button();
            this.txt_SupID = new System.Windows.Forms.TextBox();
            this.txt_Sup_Name = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.Sup_Image = new System.Windows.Forms.PictureBox();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.brand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.warranty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.quantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.price = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.total = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label18 = new System.Windows.Forms.Label();
            this.lbl_items_count = new System.Windows.Forms.Label();
            this.Calculator_Timer = new System.Windows.Forms.Timer(this.components);
            this.btn_unload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_Items)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Sup_Image)).BeginInit();
            this.SuspendLayout();
            // 
            // dtg_Items
            // 
            this.dtg_Items.AllowUserToOrderColumns = true;
            dataGridViewCellStyle17.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle17.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.Color.DimGray;
            this.dtg_Items.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle17;
            this.dtg_Items.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtg_Items.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_Items.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            dataGridViewCellStyle18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_Items.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle18;
            this.dtg_Items.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_Items.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.name,
            this.brand,
            this.description,
            this.warranty,
            this.quantity,
            this.price,
            this.total});
            this.dtg_Items.EnableHeadersVisualStyles = false;
            this.dtg_Items.GridColor = System.Drawing.Color.WhiteSmoke;
            this.dtg_Items.Location = new System.Drawing.Point(12, 140);
            this.dtg_Items.Name = "dtg_Items";
            this.dtg_Items.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle19.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            dataGridViewCellStyle19.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_Items.RowHeadersDefaultCellStyle = dataGridViewCellStyle19;
            dataGridViewCellStyle20.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle20.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_Items.RowsDefaultCellStyle = dataGridViewCellStyle20;
            this.dtg_Items.RowTemplate.Height = 40;
            this.dtg_Items.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_Items.Size = new System.Drawing.Size(776, 317);
            this.dtg_Items.TabIndex = 21;
            this.dtg_Items.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_Items_CellEndEdit);
            // 
            // btn_import
            // 
            this.btn_import.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_import.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_import.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_import.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_import.ForeColor = System.Drawing.Color.White;
            this.btn_import.Location = new System.Drawing.Point(568, 112);
            this.btn_import.Name = "btn_import";
            this.btn_import.Size = new System.Drawing.Size(107, 22);
            this.btn_import.TabIndex = 22;
            this.btn_import.Text = "Import";
            this.btn_import.UseVisualStyleBackColor = false;
            this.btn_import.Click += new System.EventHandler(this.btn_import_Click);
            // 
            // btn_save
            // 
            this.btn_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save.BackColor = System.Drawing.Color.SteelBlue;
            this.btn_save.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_save.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save.ForeColor = System.Drawing.Color.White;
            this.btn_save.Location = new System.Drawing.Point(568, 463);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(107, 22);
            this.btn_save.TabIndex = 23;
            this.btn_save.Text = "Save to Records";
            this.btn_save.UseVisualStyleBackColor = false;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(529, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(271, 30);
            this.label1.TabIndex = 24;
            this.label1.Text = "INBOUND CSV IMPORTER";
            // 
            // btn_Gen
            // 
            this.btn_Gen.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btn_Gen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_Gen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Gen.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Gen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Gen.Location = new System.Drawing.Point(341, 23);
            this.btn_Gen.Name = "btn_Gen";
            this.btn_Gen.Size = new System.Drawing.Size(99, 25);
            this.btn_Gen.TabIndex = 72;
            this.btn_Gen.Text = "Generate";
            this.btn_Gen.UseVisualStyleBackColor = false;
            this.btn_Gen.Click += new System.EventHandler(this.btn_Gen_Click);
            // 
            // txt_TransRef
            // 
            this.txt_TransRef.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt_TransRef.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_TransRef.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txt_TransRef.Location = new System.Drawing.Point(160, 23);
            this.txt_TransRef.Name = "txt_TransRef";
            this.txt_TransRef.Size = new System.Drawing.Size(175, 25);
            this.txt_TransRef.TabIndex = 73;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(22, 27);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(131, 16);
            this.label10.TabIndex = 74;
            this.label10.Text = "SALES INVOICE #";
            // 
            // btn_Close
            // 
            this.btn_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Close.BackColor = System.Drawing.Color.LightSlateGray;
            this.btn_Close.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Close.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Close.ForeColor = System.Drawing.Color.White;
            this.btn_Close.Location = new System.Drawing.Point(681, 463);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(107, 22);
            this.btn_Close.TabIndex = 75;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = false;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // lbl_error_message
            // 
            this.lbl_error_message.AutoSize = true;
            this.lbl_error_message.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_error_message.ForeColor = System.Drawing.Color.Firebrick;
            this.lbl_error_message.Location = new System.Drawing.Point(14, 118);
            this.lbl_error_message.Name = "lbl_error_message";
            this.lbl_error_message.Size = new System.Drawing.Size(0, 14);
            this.lbl_error_message.TabIndex = 96;
            // 
            // btn_searchSup
            // 
            this.btn_searchSup.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_searchSup.BackgroundImage")));
            this.btn_searchSup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn_searchSup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_searchSup.Location = new System.Drawing.Point(340, 53);
            this.btn_searchSup.Name = "btn_searchSup";
            this.btn_searchSup.Size = new System.Drawing.Size(33, 25);
            this.btn_searchSup.TabIndex = 97;
            this.btn_searchSup.UseVisualStyleBackColor = true;
            this.btn_searchSup.Click += new System.EventHandler(this.btn_searchSup_Click);
            // 
            // txt_SupID
            // 
            this.txt_SupID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txt_SupID.Location = new System.Drawing.Point(160, 53);
            this.txt_SupID.Name = "txt_SupID";
            this.txt_SupID.Size = new System.Drawing.Size(175, 25);
            this.txt_SupID.TabIndex = 98;
            this.txt_SupID.TextChanged += new System.EventHandler(this.txt_SupID_TextChanged);
            // 
            // txt_Sup_Name
            // 
            this.txt_Sup_Name.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txt_Sup_Name.Location = new System.Drawing.Point(160, 84);
            this.txt_Sup_Name.Name = "txt_Sup_Name";
            this.txt_Sup_Name.Size = new System.Drawing.Size(213, 25);
            this.txt_Sup_Name.TabIndex = 99;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Location = new System.Drawing.Point(81, 57);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 17);
            this.label9.TabIndex = 101;
            this.label9.Text = "Supplier ID";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Location = new System.Drawing.Point(97, 87);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 17);
            this.label8.TabIndex = 100;
            this.label8.Text = "Supplier";
            // 
            // Sup_Image
            // 
            this.Sup_Image.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Sup_Image.Location = new System.Drawing.Point(379, 53);
            this.Sup_Image.Name = "Sup_Image";
            this.Sup_Image.Size = new System.Drawing.Size(61, 56);
            this.Sup_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Sup_Image.TabIndex = 102;
            this.Sup_Image.TabStop = false;
            // 
            // name
            // 
            this.name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.name.HeaderText = "Item Name";
            this.name.Name = "name";
            this.name.Width = 96;
            // 
            // brand
            // 
            this.brand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.brand.HeaderText = "Brand";
            this.brand.Name = "brand";
            this.brand.Width = 66;
            // 
            // description
            // 
            this.description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.description.HeaderText = "Description";
            this.description.Name = "description";
            // 
            // warranty
            // 
            this.warranty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.warranty.HeaderText = "Warranty";
            this.warranty.Name = "warranty";
            this.warranty.Width = 84;
            // 
            // quantity
            // 
            this.quantity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.quantity.HeaderText = "Quantity";
            this.quantity.Name = "quantity";
            this.quantity.Width = 80;
            // 
            // price
            // 
            this.price.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.price.HeaderText = "Price";
            this.price.Name = "price";
            this.price.Width = 60;
            // 
            // total
            // 
            this.total.HeaderText = "Total";
            this.total.Name = "total";
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label18.Location = new System.Drawing.Point(9, 463);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(77, 14);
            this.label18.TabIndex = 103;
            this.label18.Text = "Rows Count";
            // 
            // lbl_items_count
            // 
            this.lbl_items_count.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_items_count.AutoSize = true;
            this.lbl_items_count.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_items_count.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lbl_items_count.Location = new System.Drawing.Point(87, 463);
            this.lbl_items_count.Name = "lbl_items_count";
            this.lbl_items_count.Size = new System.Drawing.Size(14, 14);
            this.lbl_items_count.TabIndex = 104;
            this.lbl_items_count.Text = "0";
            // 
            // Calculator_Timer
            // 
            this.Calculator_Timer.Tick += new System.EventHandler(this.Calculator_Timer_Tick);
            // 
            // btn_unload
            // 
            this.btn_unload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_unload.BackColor = System.Drawing.Color.Peru;
            this.btn_unload.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_unload.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_unload.ForeColor = System.Drawing.Color.White;
            this.btn_unload.Location = new System.Drawing.Point(681, 112);
            this.btn_unload.Name = "btn_unload";
            this.btn_unload.Size = new System.Drawing.Size(107, 22);
            this.btn_unload.TabIndex = 105;
            this.btn_unload.Text = "Unload";
            this.btn_unload.UseVisualStyleBackColor = false;
            this.btn_unload.Click += new System.EventHandler(this.btn_unload_Click);
            // 
            // Import
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(800, 503);
            this.Controls.Add(this.btn_unload);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.lbl_items_count);
            this.Controls.Add(this.Sup_Image);
            this.Controls.Add(this.btn_searchSup);
            this.Controls.Add(this.txt_SupID);
            this.Controls.Add(this.txt_Sup_Name);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lbl_error_message);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Gen);
            this.Controls.Add(this.txt_TransRef);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.btn_import);
            this.Controls.Add(this.dtg_Items);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Import";
            this.Text = "Import CSV";
            this.Load += new System.EventHandler(this.Import_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_Items)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Sup_Image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dtg_Items;
        private System.Windows.Forms.Button btn_import;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Gen;
        private System.Windows.Forms.TextBox txt_TransRef;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Label lbl_error_message;
        private System.Windows.Forms.Button btn_searchSup;
        private System.Windows.Forms.TextBox txt_SupID;
        private System.Windows.Forms.TextBox txt_Sup_Name;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox Sup_Image;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn brand;
        private System.Windows.Forms.DataGridViewTextBoxColumn description;
        private System.Windows.Forms.DataGridViewTextBoxColumn warranty;
        private System.Windows.Forms.DataGridViewTextBoxColumn quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn price;
        private System.Windows.Forms.DataGridViewTextBoxColumn total;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lbl_items_count;
        private System.Windows.Forms.Timer Calculator_Timer;
        private System.Windows.Forms.Button btn_unload;
    }
}