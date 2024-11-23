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
    public partial class ChiTietKhoHang : Form
    {
        public ChiTietKhoHang(
    string masp, string productName, string category, string stock,
    string trademark, string origin, string warranty, string weight,
    string size, string description,string daban,string price)
        {
            InitializeComponent();

            // Assign these values to respective textboxes or labels
            MaSP.Text = masp; // ma sp
            textboxtensanpham.Text = productName;// ten sp
            textboxnganhhang.Text = category; // nganh hang
            textboxkho.Text = stock; // ton kho
            thuonghieu.Text = trademark; // thuonghieu
            textboxxuatxu.Text = origin;// xuat xu
            textboxbaohanh.Text = warranty; // bao hanh
            textboxcannang.Text = weight; // can nang
            textboxkichthuoc.Text = size; // kich thuoc
            textboxmota.Text = description; // mo ta
            textboxdaban.Text = daban; // da ban 
            textboxgia.Text = price; // gia
        }


        private void ChiTietKhoHang_Load(object sender, EventArgs e)
        {
            nenChiTiet.FillColor= Color.FromArgb(153,255,255,255);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Sua_Click(object sender, EventArgs e)
        {

        }

        private void textboxtensanpham_TextChanged(object sender, EventArgs e)
        {

        }

        private void nenChiTiet_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
