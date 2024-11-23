using System;
using System.Configuration; // Để sử dụng ConfigurationManager
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CNPM
{
    public partial class ChiTietDonHang : Form
    {
        private readonly string orderId; // Lưu trữ mã đơn hàng
        private readonly string connectionString; // Lấy kết nối từ App.config
        public event Action OrderStatusUpdated; // Sự kiện thông báo trạng thái đơn hàng được cập nhật

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

            // Gán connection string từ App.config
            connectionString = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

            // Gán giá trị orderId vào biến toàn cục
            this.orderId = orderId;

            // Gán giá trị vào các TextBox
            TextBoxMaDon.Text = orderId;
            TextBoxTenKhachHang.Text = receiverName;
            TextBoxSDT.Text = phone;
            TextBoxMavanchuyen.Text = shippingCode;
            TextBoxDonVivanchuyen.Text = shippingCo;
            TextBoxNgayDat.Text = orderDate.ToString("dd/MM/yyyy");
            TextBoxTrangThai.Text = orderStatus;
            TextBoxDiaChi.Text = deliveryAddress;

            // Gán sự kiện định dạng ô cho DataGridView
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
            // Định dạng cột số lượng
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
            // Đặt màu nền
            nenChiTiet.FillColor = Color.FromArgb(153, 217, 217, 217);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateOrderStatus(string orderId, string newStatus)
        {
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
                            // Gọi sự kiện thông báo trạng thái đã cập nhật
                            OrderStatusUpdated?.Invoke();
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

        private void ButtonCapNhatTrangThai_Click(object sender, EventArgs e)
        {
            // Thay đổi trạng thái đơn hàng
            string currentStatus = TextBoxTrangThai.Text.Trim();
            string newStatus = GetNextOrderStatus(currentStatus);

            if (string.IsNullOrEmpty(newStatus))
            {
                MessageBox.Show("Trạng thái hiện tại không thể cập nhật thêm.");
                return;
            }

            UpdateOrderStatus(orderId, newStatus);
            TextBoxTrangThai.Text = newStatus; // Cập nhật giao diện
        }

        private string GetNextOrderStatus(string currentStatus)
        {
            // Các trạng thái đơn hàng
            var statuses = new[] { "Cần xử lí", "Đã xác nhận", "Đang chuẩn bị", "Chờ gửi hàng", "Đã gửi", "Đã nhận", "Đã hủy" };

            int currentIndex = Array.IndexOf(statuses, currentStatus);
            if (currentIndex >= 0 && currentIndex < statuses.Length - 2) // Không cập nhật khi là "Đã nhận" hoặc "Đã hủy"
            {
                return statuses[currentIndex + 1];
            }

            return null; // Không thể cập nhật trạng thái
        }
    }
}
