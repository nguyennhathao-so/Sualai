using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class TrangChu : Form
    {
        public TrangChu()
        {
            InitializeComponent();
        }

        private void TrangChu_Load(object sender, EventArgs e)
        {
            noiDungTrangChu1.Visible = false;
            donHang1.Visible = false;
            taoDon1.Visible = false;
            khoHang1.Visible=false;
            btnTrangChu.PerformClick();
        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {
            LogIn form = new LogIn();
            this.Hide();
            form.ShowDialog();
            this.Close();
        }

        private void btnDonHang_Click(object sender, EventArgs e)
        {
            taoDon1.Visible = true;
            taoDon1.BringToFront();
        }

        private void btnTrangChu_Click(object sender, EventArgs e)
        {
            noiDungTrangChu1.Visible = true;
            noiDungTrangChu1.BringToFront();
        }

        private void btnTaoDon_Click(object sender, EventArgs e)
        {
            donHang1.Visible = true;
            donHang1.BringToFront();
        }

        private void btnKhoHang_Click(object sender, EventArgs e)
        {
            khoHang1.Visible=true;
            khoHang1.BringToFront();    
        }

        private void btnKhachHang_Click(object sender, EventArgs e)
        {
            khachHang1.Visible = true;
            khachHang1.BringToFront() ;
        }

        private void thongBao_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hệ thống chưa hỗ trợ!");
        }

        private void khachHang1_Load(object sender, EventArgs e)
        {

        }
    }
}
