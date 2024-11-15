using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CNPM
{
    public partial class ChiTietDonHang : Form
    {
        private string orderId; // Biến lưu trữ mã đơn hàng
        public event Action OrderStatusUpdated; // Tạo sự kiện để thông báo cập nhật

        public ChiTietDonHang(string orderId,
                              string receiverName,
                              string phone,
                              string shippingCode,
                              string shippingCo,
                              DateTime orderDate,
                              string orderStatus,
                              string deliveryAddress,
                              DataTable productDetails)
        {
            InitializeComponent();

            // Gán giá trị orderId vào biến toàn cục để dùng trong hàm cập nhật
            this.orderId = orderId;

            // Gán các giá trị vào các TextBox
            TextBoxMaDon.Text = orderId;
            TextBoxTenKhachHang.Text = receiverName;
            TextBoxSDT.Text = phone;
            TextBoxMavanchuyen.Text = shippingCode;
            TextBoxDonVivanchuyen.Text = shippingCo;
            TextBoxNgayDat.Text = orderDate.ToString("dd/MM/yyyy");
            TextBoxTrangThai.Text = orderStatus;
            TextBoxDiaChi.Text = deliveryAddress;

            // Gán sự kiện CellFormatting cho DataGridView
            DataGridViewBangChiTiet.CellFormatting += DataGridViewBangChiTiet_CellFormatting;

            // Kiểm tra và tải dữ liệu chi tiết sản phẩm vào DataGridView
            if (productDetails != null)
            {
                LoadProductDetailsIntoDataGridView(productDetails);
            }
            else
            {
                MessageBox.Show("Không có dữ liệu chi tiết sản phẩm để hiển thị.");
            }
        }

        private void DataGridViewBangChiTiet_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Kiểm tra xem cột thứ 4 (index 3) có phải là cột Số lượng và có giá trị hay không
            if (e.ColumnIndex == 3 && e.Value != null)
            {
                if (int.TryParse(e.Value.ToString(), out int quantity))
                {
                    e.Value = $"x{quantity}";
                    e.FormattingApplied = true;
                }
            }
        }

        private void LoadProductDetailsIntoDataGridView(DataTable productDetails)
        {
            DataGridViewBangChiTiet.Rows.Clear();

            foreach (DataRow row in productDetails.Rows)
            {
                DataGridViewBangChiTiet.Rows.Add(
                    row["Mã sản phẩm"],
                    row["Tên sản phẩm"],
                    string.Format("{0:#,##0}", row["Đơn giá"]),
                    row["Số lượng"]
                );
            }
        }

        private void ChiTiet_Load(object sender, EventArgs e)
        {
            nenChiTiet.FillColor = Color.FromArgb(153, 217, 217, 217);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        

        private void UpdateOrderStatus(string orderId, string newStatus)
        {
            string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = "UPDATE Orders SET OrderStatus = @OrderStatus WHERE OrderID = @OrderID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderStatus", newStatus);
                        cmd.Parameters.AddWithValue("@OrderID", orderId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cập nhật trạng thái đơn hàng thành công!");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy đơn hàng để cập nhật.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật đơn hàng: " + ex.Message);
            }
        }

        

    }
}
