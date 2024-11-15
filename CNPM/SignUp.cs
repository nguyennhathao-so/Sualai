using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.VisualStyles;

namespace CNPM
{
    public partial class SignUp : Form
    {

        public SignUp()
        {
            InitializeComponent();
        }
        SqlConnection conn = new SqlConnection("Data Source=Hphuc\\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True");
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter adapter = new SqlDataAdapter();
        private void DangNhapTaiDay_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            {
                LogIn logIn = new LogIn();
                this.Hide();
                logIn.ShowDialog();
                this.Close();
            }


        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (txtten.Text == "" && txtmk.Text == "" && txtconfirm.Text == "" && txtsdt.Text == "")
                {
                MessageBox.Show("Tên đăng nhập và mật khẩu chưa điền đầy đủ", "Đăng kí thất bại");
            }
            else if (txtmk.Text == txtconfirm.Text)
            {
                conn.Open();
                string register = "INSERT INTO Login values('" + txtten.Text + "','" + txtmk.Text + "')";
                cmd = new SqlCommand(register, conn);
                cmd.ExecuteReader();
                conn.Close();
                MessageBox.Show("Đăng kí thành công!");
            }
            else
            {
                MessageBox.Show("Mật khẩu không đúng, vui lòng thử lại!");

            }

        }
    } 
}
