namespace tc_trn
{
    partial class Form1
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
            this.lb_hotkeys = new System.Windows.Forms.Label();
            this.bt_start = new System.Windows.Forms.Button();
            this.bt_about = new System.Windows.Forms.Button();
            this.tmr_scan = new System.Windows.Forms.Timer(this.components);
            this.li_tele = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nm_interv = new System.Windows.Forms.NumericUpDown();
            this.nm_flyspeed = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.bt_tele_add = new System.Windows.Forms.Button();
            this.bt_tele_del = new System.Windows.Forms.Button();
            this.lb_tele_coords = new System.Windows.Forms.Label();
            this.bt_tele_upd = new System.Windows.Forms.Button();
            this.tb_tele_name = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nm_interv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nm_flyspeed)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_hotkeys
            // 
            this.lb_hotkeys.AutoSize = true;
            this.lb_hotkeys.Location = new System.Drawing.Point(13, 11);
            this.lb_hotkeys.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_hotkeys.Name = "lb_hotkeys";
            this.lb_hotkeys.Size = new System.Drawing.Size(78, 18);
            this.lb_hotkeys.TabIndex = 0;
            this.lb_hotkeys.Text = "Hotkeys";
            // 
            // bt_start
            // 
            this.bt_start.Location = new System.Drawing.Point(12, 403);
            this.bt_start.Name = "bt_start";
            this.bt_start.Size = new System.Drawing.Size(128, 32);
            this.bt_start.TabIndex = 1;
            this.bt_start.Text = "Start Game";
            this.bt_start.UseVisualStyleBackColor = true;
            this.bt_start.Click += new System.EventHandler(this.bt_start_Click);
            // 
            // bt_about
            // 
            this.bt_about.Location = new System.Drawing.Point(490, 403);
            this.bt_about.Name = "bt_about";
            this.bt_about.Size = new System.Drawing.Size(128, 32);
            this.bt_about.TabIndex = 1;
            this.bt_about.Text = "About";
            this.bt_about.UseVisualStyleBackColor = true;
            this.bt_about.Click += new System.EventHandler(this.bt_about_Click);
            // 
            // tmr_scan
            // 
            this.tmr_scan.Enabled = true;
            this.tmr_scan.Interval = 5000;
            this.tmr_scan.Tick += new System.EventHandler(this.tmr_scan_Tick);
            // 
            // li_tele
            // 
            this.li_tele.FormattingEnabled = true;
            this.li_tele.ItemHeight = 18;
            this.li_tele.Location = new System.Drawing.Point(443, 96);
            this.li_tele.Name = "li_tele";
            this.li_tele.Size = new System.Drawing.Size(175, 148);
            this.li_tele.TabIndex = 2;
            this.li_tele.SelectedIndexChanged += new System.EventHandler(this.lb_tele_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(440, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "Teleports";
            // 
            // nm_interv
            // 
            this.nm_interv.Location = new System.Drawing.Point(544, 9);
            this.nm_interv.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nm_interv.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nm_interv.Name = "nm_interv";
            this.nm_interv.Size = new System.Drawing.Size(74, 26);
            this.nm_interv.TabIndex = 4;
            this.nm_interv.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nm_interv.ValueChanged += new System.EventHandler(this.nm_interv_ValueChanged);
            // 
            // nm_flyspeed
            // 
            this.nm_flyspeed.Location = new System.Drawing.Point(544, 41);
            this.nm_flyspeed.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nm_flyspeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nm_flyspeed.Name = "nm_flyspeed";
            this.nm_flyspeed.Size = new System.Drawing.Size(74, 26);
            this.nm_flyspeed.TabIndex = 4;
            this.nm_flyspeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nm_flyspeed.ValueChanged += new System.EventHandler(this.nm_flyspeed_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(370, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 18);
            this.label3.TabIndex = 5;
            this.label3.Text = "Trainer Interval";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(440, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 18);
            this.label4.TabIndex = 5;
            this.label4.Text = "Fly Speed";
            // 
            // bt_tele_add
            // 
            this.bt_tele_add.Location = new System.Drawing.Point(443, 250);
            this.bt_tele_add.Name = "bt_tele_add";
            this.bt_tele_add.Size = new System.Drawing.Size(50, 32);
            this.bt_tele_add.TabIndex = 6;
            this.bt_tele_add.Text = "Add";
            this.bt_tele_add.UseVisualStyleBackColor = true;
            this.bt_tele_add.Click += new System.EventHandler(this.bt_tele_add_Click);
            // 
            // bt_tele_del
            // 
            this.bt_tele_del.Location = new System.Drawing.Point(568, 250);
            this.bt_tele_del.Name = "bt_tele_del";
            this.bt_tele_del.Size = new System.Drawing.Size(50, 32);
            this.bt_tele_del.TabIndex = 6;
            this.bt_tele_del.Text = "Del";
            this.bt_tele_del.UseVisualStyleBackColor = true;
            this.bt_tele_del.Click += new System.EventHandler(this.bt_tele_del_Click);
            // 
            // lb_tele_coords
            // 
            this.lb_tele_coords.AutoSize = true;
            this.lb_tele_coords.Location = new System.Drawing.Point(440, 323);
            this.lb_tele_coords.Name = "lb_tele_coords";
            this.lb_tele_coords.Size = new System.Drawing.Size(0, 18);
            this.lb_tele_coords.TabIndex = 7;
            // 
            // bt_tele_upd
            // 
            this.bt_tele_upd.Location = new System.Drawing.Point(506, 250);
            this.bt_tele_upd.Name = "bt_tele_upd";
            this.bt_tele_upd.Size = new System.Drawing.Size(50, 32);
            this.bt_tele_upd.TabIndex = 6;
            this.bt_tele_upd.Text = "Upd";
            this.bt_tele_upd.UseVisualStyleBackColor = true;
            this.bt_tele_upd.Click += new System.EventHandler(this.bt_tele_upd_Click);
            // 
            // tb_tele_name
            // 
            this.tb_tele_name.Location = new System.Drawing.Point(443, 288);
            this.tb_tele_name.Name = "tb_tele_name";
            this.tb_tele_name.Size = new System.Drawing.Size(175, 26);
            this.tb_tele_name.TabIndex = 8;
            this.tb_tele_name.Text = "Untitled";
            this.tb_tele_name.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_tele_name_KeyPress);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(356, 403);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 32);
            this.button1.TabIndex = 1;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.bt_save_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 447);
            this.Controls.Add(this.tb_tele_name);
            this.Controls.Add(this.lb_tele_coords);
            this.Controls.Add(this.bt_tele_del);
            this.Controls.Add(this.bt_tele_upd);
            this.Controls.Add(this.bt_tele_add);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nm_flyspeed);
            this.Controls.Add(this.nm_interv);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.li_tele);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bt_about);
            this.Controls.Add(this.bt_start);
            this.Controls.Add(this.lb_hotkeys);
            this.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "True Crime: Streets of LA Trainer by Kurtis";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.nm_interv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nm_flyspeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_hotkeys;
        private System.Windows.Forms.Button bt_start;
        private System.Windows.Forms.Button bt_about;
        private System.Windows.Forms.Timer tmr_scan;
        private System.Windows.Forms.ListBox li_tele;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nm_interv;
        private System.Windows.Forms.NumericUpDown nm_flyspeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bt_tele_add;
        private System.Windows.Forms.Button bt_tele_del;
        private System.Windows.Forms.Label lb_tele_coords;
        private System.Windows.Forms.Button bt_tele_upd;
        private System.Windows.Forms.TextBox tb_tele_name;
        private System.Windows.Forms.Button button1;
    }
}

