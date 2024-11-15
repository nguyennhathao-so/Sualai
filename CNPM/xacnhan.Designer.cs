namespace CNPM
{
    partial class xacnhan
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
            this.label1 = new System.Windows.Forms.Label();
            this.labelXacNhan = new System.Windows.Forms.Label();
            this.huy = new Guna.UI2.WinForms.Guna2Button();
            this.guna2OK = new Guna.UI2.WinForms.Guna2Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(579, 232);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "mã vận chuyển";
            // 
            // labelXacNhan
            // 
            this.labelXacNhan.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXacNhan.Location = new System.Drawing.Point(24, 11);
            this.labelXacNhan.Name = "labelXacNhan";
            this.labelXacNhan.Size = new System.Drawing.Size(287, 85);
            this.labelXacNhan.TabIndex = 1;
            this.labelXacNhan.Text = "Xác nhận hoàn tất công đoạn: ";
            this.labelXacNhan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.huy.Location = new System.Drawing.Point(45, 100);
            this.huy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.huy.Name = "huy";
            this.huy.Size = new System.Drawing.Size(112, 38);
            this.huy.TabIndex = 2;
            this.huy.Text = "Hủy";
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
            this.guna2OK.Location = new System.Drawing.Point(179, 100);
            this.guna2OK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.guna2OK.Name = "guna2OK";
            this.guna2OK.Size = new System.Drawing.Size(112, 38);
            this.guna2OK.TabIndex = 3;
            this.guna2OK.Text = "Ok";
            // 
            // xacnhan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 159);
            this.Controls.Add(this.guna2OK);
            this.Controls.Add(this.huy);
            this.Controls.Add(this.labelXacNhan);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "xacnhan";
            this.Text = "xacnhan";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelXacNhan;
        private Guna.UI2.WinForms.Guna2Button huy;
        private Guna.UI2.WinForms.Guna2Button guna2OK;
    }
}