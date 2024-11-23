using System;
using System.Configuration; // Để sử dụng ConfigurationManager
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CNPM
{
    public partial class SignUp : Form
    {
        private string connectionString;

        public SignUp()
        {
            InitializeComponent();
            // Lấy connection string từ App.config
            connectionString = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
        }

        private void DangNhapTaiDay_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LogIn logIn = new LogIn();
            this.Hide();
            logIn.ShowDialog();
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem các ô có được điền đầy đủ không
            if (string.IsNullOrWhiteSpace(txtten.Text) ||
                string.IsNullOrWhiteSpace(txtmk.Text) ||
                string.IsNullOrWhiteSpace(txtconfirm.Text) ||
                string.IsNullOrWhiteSpace(txtsdt.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Đăng ký thất bại");
                return;
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (txtmk.Text != txtconfirm.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp, vui lòng thử lại!", "Đăng ký thất bại");
                return;
            }

            // Tiến hành đăng ký
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string registerQuery = "INSERT INTO Login (taikhoan, matkhau,sdt) VALUES (@username, @password,@phone)";
                    using (SqlCommand cmd = new SqlCommand(registerQuery, conn))
                    {
                        // Thêm tham số vào truy vấn
                        cmd.Parameters.AddWithValue("@username", txtten.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", txtmk.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone",txtsdt.Text.Trim());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Đăng ký thành công!", "Thành công");
                            // Chuyển sang màn hình đăng nhập
                            LogIn logIn = new LogIn();
                            this.Hide();
                            logIn.ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Đăng ký thất bại, vui lòng thử lại!", "Thất bại");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi hệ thống");
            }
        }

        private void SignUp_Load(object sender, EventArgs e)
        {

        }
    }
}
