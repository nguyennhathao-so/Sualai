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

namespace CNPM
{
    public partial class LogIn : Form
    {

        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
        SqlConnection connection;
        public LogIn()
        {
            InitializeComponent();
        }

        private void LogIn_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SignUp signUp = new SignUp();
            this.Hide();
            signUp.ShowDialog();
            this.Close();   
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString)) { 
                    con.Open();
                    string tk = TenDN.Text;
                string mk = MK.Text;
                string sql = "SELECT * FROM Login WHERE taikhoan  = @tk and matkhau = @mk";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@tk", tk);
                cmd.Parameters.AddWithValue("@mk", mk);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read() == true)
                {
                    TrangChu dashboard = new TrangChu();
                    this.Hide();
                    dashboard.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Đăng nhập thất bại, vui lòng thử lại!");
                }
                
               }

            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}\nCode: {ex.Number}");
            }
        }

        private void quenMK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Hệ thống chưa hỗ trợ!");
        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hệ thống chưa hỗ trợ!");
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hệ thống chưa hỗ trợ!");
        }
    }
}
