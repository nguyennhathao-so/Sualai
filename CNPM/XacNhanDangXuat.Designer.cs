namespace CNPM
{
    partial class XacNhanDangXuat
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
            this.guna2OK = new Guna.UI2.WinForms.Guna2Button();
            this.huy = new Guna.UI2.WinForms.Guna2Button();
            this.labelXacNhan = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // guna2OK
            // 
            this.guna2OK.BackColor = System.Drawing.Color.Transparent;
            this.guna2OK.BorderRadius = 10;
            this.guna2OK.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2OK.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2OK.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2OK.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2OK.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.guna2OK.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2OK.ForeColor = System.Drawing.Color.Black;
            this.guna2OK.Location = new System.Drawing.Point(159, 84);
            this.guna2OK.Name = "guna2OK";
            this.guna2OK.Size = new System.Drawing.Size(100, 30);
            this.guna2OK.TabIndex = 6;
            this.guna2OK.Text = "Ok";
            // 
            // huy
            // 
            this.huy.BackColor = System.Drawing.Color.Transparent;
            this.huy.BorderRadius = 10;
            this.huy.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.huy.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.huy.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.huy.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.huy.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.huy.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.huy.ForeColor = System.Drawing.Color.Black;
            this.huy.Location = new System.Drawing.Point(40, 84);
            this.huy.Name = "huy";
            this.huy.Size = new System.Drawing.Size(100, 30);
            this.huy.TabIndex = 5;
            this.huy.Text = "Hủy";
            // 
            // labelXacNhan
            // 
            this.labelXacNhan.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXacNhan.Location = new System.Drawing.Point(21, 13);
            this.labelXacNhan.Name = "labelXacNhan";
            this.labelXacNhan.Size = new System.Drawing.Size(255, 68);
            this.labelXacNhan.TabIndex = 4;
            this.labelXacNhan.Text = "Xác nhận đăng xuất";
            this.labelXacNhan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // XacNhanDangXuat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 127);
            this.Controls.Add(this.guna2OK);
            this.Controls.Add(this.huy);
            this.Controls.Add(this.labelXacNhan);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(296, 127);
            this.MinimumSize = new System.Drawing.Size(296, 127);
            this.Name = "XacNhanDangXuat";
            this.Text = "XacNhanDangXuat";
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button guna2OK;
        private Guna.UI2.WinForms.Guna2Button huy;
        private System.Windows.Forms.Label labelXacNhan;
    }
}