using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration; // Để sử dụng ConfigurationManager
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace CNPM
{
    public partial class buttonTatCa : UserControl
    {
        private DataTable orderDataTable;
        private readonly string connectionString;
        private readonly List<string> orderStatuses = new List<string>
        {
            "Cần xử lí", "Đã xác nhận", "Đang chuẩn bị", "Chờ gửi hàng", "Đã gửi", "Đã nhận", "Đã hủy"
        };

        public buttonTatCa()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            InitializeComponent();

            LoadOrderData();
            TimKiem.TextChanged += TimKiem_TextChanged;
            LoadOrderStatusCounts();
            DataGridViewDonhang.CellDoubleClick += DataGridViewDonhang_CellDoubleClick;
            ButtonReload.Click += ButtonReload_Click;
            buttonupdate.Click += ButtonUpdateStatus_Click;

            TatCa.Click += (sender, e) => { HandleButtonClick(TatCa, "Tất cả"); };
            CanXuLi.Click += (sender, e) => { HandleButtonClick(CanXuLi, "Cần xử lí"); };
            DaXacNhan.Click += (sender, e) => { HandleButtonClick(DaXacNhan, "Đã xác nhận"); };
            DangChuanBi.Click += (sender, e) => { HandleButtonClick(DangChuanBi, "Đang chuẩn bị"); };
            ChoGuiHang.Click += (sender, e) => { HandleButtonClick(ChoGuiHang, "Chờ gửi hàng"); };
            DaGui.Click += (sender, e) => { HandleButtonClick(DaGui, "Đã gửi"); };
            DaNhan.Click += (sender, e) => { HandleButtonClick(DaNhan, "Đã nhận"); };
            DaHuy.Click += (sender, e) => { HandleButtonClick(DaHuy, "Đã hủy"); };
        }

        private void ResetButtonColors()
        {
            var buttons = new[] { TatCa, CanXuLi, DaXacNhan, DangChuanBi, ChoGuiHang, DaGui, DaNhan, DaHuy };
            foreach (var button in buttons)
            {
                button.FillColor = Color.FromArgb(240, 240, 240);
            }
        }

        private void HandleButtonClick(Guna2Button button, string status)
        {
            ResetButtonColors();
            button.FillColor = Color.FromArgb(183, 205, 240);
            FilterOrdersByStatus(status);
        }

        private void FilterOrdersByStatus(string status)
        {
            if (orderDataTable != null)
            {
                if (status == "Tất cả")
                {
                    orderDataTable.DefaultView.RowFilter = string.Empty;
                }
                else
                {
                    orderDataTable.DefaultView.RowFilter = $"[Trạng thái đơn hàng] = '{status}'";
                }
                DataGridViewDonhang.DataSource = orderDataTable.DefaultView;
            }
        }

        private void ButtonReload_Click(object sender, EventArgs e)
        {
            LoadOrderData();
            LoadOrderStatusCounts();
        }

        private void ButtonUpdateStatus_Click(object sender, EventArgs e)
        {
            if (DataGridViewDonhang.SelectedRows.Count > 0)
            {
                string orderId = DataGridViewDonhang.SelectedRows[0].Cells["Mã đơn hàng"].Value.ToString();
                string currentStatus = GetOrderStatusFromDatabase(orderId);

                int currentIndex = orderStatuses.IndexOf(currentStatus);
                if (currentIndex >= 0 && currentIndex < orderStatuses.Count - 2)
                {
                    string newStatus = orderStatuses[currentIndex + 1];
                    UpdateOrderStatus(orderId, newStatus);
                    MessageBox.Show($"Trạng thái đơn hàng đã được cập nhật thành: {newStatus}");
                    LoadOrderData();
                    LoadOrderStatusCounts();
                }
                else
                {
                    MessageBox.Show("Trạng thái đơn hàng hiện tại không thể cập nhật thêm.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để cập nhật trạng thái.");
            }
        }

        private void UpdateOrderStatus(string orderId, string newStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Orders SET OrderStatus = @OrderStatus WHERE OrderID = @OrderID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderStatus", newStatus);
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái đơn hàng: " + ex.Message);
            }
        }

        private void LoadOrderData()
        {
            DataGridViewDonhang.AutoGenerateColumns = false;
            DataGridViewDonhang.Columns.Clear(); // Clear existing columns to avoid duplication

           

            try
            {
                string query = @"
        SELECT 
            o.OrderID AS 'Mã đơn hàng', 
            c.Name AS 'Tên người nhận', 
            c.Phone AS 'Số điện thoại', 
            s.ShippingCode AS 'Mã vận chuyển',
            s.ShippingCo AS 'Đơn vị vận chuyển', 
            o.OrderDate AS 'Thời gian đặt hàng',
            o.OrderStatus AS 'Trạng thái đơn hàng',
            o.ShippingAddress AS 'Địa chỉ nhận'
        FROM 
            Orders o
        JOIN 
            Customers c ON o.CustomerID = c.CustomerID
        LEFT JOIN 
            Shipping2 s ON s.OrderID = o.OrderID";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            orderDataTable = new DataTable();
                            adapter.Fill(orderDataTable);

                            if (orderDataTable.Rows.Count > 0)
                            {
                                DataGridViewDonhang.DataSource = orderDataTable;

                                // Define columns for the DataGridView
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Mã đơn hàng",
                                    DataPropertyName = "Mã đơn hàng",
                                    HeaderText = "Mã đơn hàng"
                                });
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Tên người nhận",
                                    DataPropertyName = "Tên người nhận",
                                    HeaderText = "Tên người nhận"
                                });
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Số điện thoại",
                                    DataPropertyName = "Số điện thoại",
                                    HeaderText = "Số điện thoại"
                                });
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Mã vận chuyển",
                                    DataPropertyName = "Mã vận chuyển",
                                    HeaderText = "Mã vận chuyển"
                                });
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Đơn vị vận chuyển",
                                    DataPropertyName = "Đơn vị vận chuyển",
                                    HeaderText = "Đơn vị vận chuyển"
                                });
                                DataGridViewDonhang.Columns.Add(new DataGridViewTextBoxColumn
                                {
                                    Name = "Thời gian đặt hàng",
                                    DataPropertyName = "Thời gian đặt hàng",
                                    HeaderText = "Thời gian đặt hàng"
                                });

                                // Hide the empty data picture if there is data
                                TrongPicture.Visible = false;
                            }
                            else
                            {
                                // Show the empty data picture if no data is returned
                                TrongPicture.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                // Show the empty data picture if an error occurs
                TrongPicture.Visible = true;
            }
        }

        private void LoadOrderStatusCounts()
        {
            var statusCounts = new Dictionary<string, int>
            {
                { "Tất cả", 0 },
                { "Đã hủy", 0 },
                { "Cần xử lí", 0 },
                { "Đã xác nhận", 0 },
                { "Đang chuẩn bị", 0 },
                { "Chờ gửi hàng", 0 },
                { "Đã gửi", 0 },
                { "Đã nhận", 0 }
            };

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string totalQuery = "SELECT COUNT(*) FROM Orders";
                    using (SqlCommand totalCmd = new SqlCommand(totalQuery, con))
                    {
                        statusCounts["Tất cả"] = (int)totalCmd.ExecuteScalar();
                    }

                    string query = @"SELECT OrderStatus, COUNT(*) AS StatusCount FROM Orders GROUP BY OrderStatus";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string status = reader["OrderStatus"].ToString();
                            int count = Convert.ToInt32(reader["StatusCount"]);

                            if (statusCounts.ContainsKey(status))
                            {
                                statusCounts[status] = count;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái đơn hàng: " + ex.Message);
            }
        }

        private string GetOrderStatusFromDatabase(string orderId)
        {
            string orderStatus = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT OrderStatus FROM Orders WHERE OrderID = @OrderID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        orderStatus = cmd.ExecuteScalar()?.ToString() ?? "Không xác định";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy trạng thái đơn hàng: " + ex.Message);
            }

            return orderStatus;
        }

        private void TimKiem_TextChanged(object sender, EventArgs e)
        {
            string filterText = TimKiem.Text.Trim();

            if (orderDataTable != null)
            {
                if (!string.IsNullOrEmpty(filterText))
                {
                    orderDataTable.DefaultView.RowFilter =
                        $"[Mã đơn hàng] LIKE '%{filterText}%' OR " +
                        $"[Tên người nhận] LIKE '%{filterText}%' OR " +
                        $"[Số điện thoại] LIKE '%{filterText}%' OR " +
                        $"[Mã vận chuyển] LIKE '%{filterText}%'";
                }
                else
                {
                    orderDataTable.DefaultView.RowFilter = string.Empty;
                }

                DataGridViewDonhang.DataSource = orderDataTable.DefaultView;
            }
        }

        private void DataGridViewDonhang_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string orderId = DataGridViewDonhang.Rows[e.RowIndex].Cells["Mã đơn hàng"].Value.ToString();
                string receiverName = DataGridViewDonhang.Rows[e.RowIndex].Cells["Tên người nhận"].Value.ToString();
                string phone = DataGridViewDonhang.Rows[e.RowIndex].Cells["Số điện thoại"].Value.ToString();
                string shippingCode = DataGridViewDonhang.Rows[e.RowIndex].Cells["Mã vận chuyển"].Value.ToString();
                string shippingCo = DataGridViewDonhang.Rows[e.RowIndex].Cells["Đơn vị vận chuyển"].Value.ToString();
                DateTime orderDate = Convert.ToDateTime(DataGridViewDonhang.Rows[e.RowIndex].Cells["Thời gian đặt hàng"].Value);
                string orderStatus = GetOrderStatusFromDatabase(orderId);
                string deliveryAddress = GetDeliveryAddressFromDatabase(orderId);
                DataTable productDetails = GetOrderProductDetails(orderId);

                ChiTietDonHang chiTietForm = new ChiTietDonHang(orderId, receiverName, phone, shippingCode, shippingCo, orderDate, orderStatus, deliveryAddress, productDetails);
                chiTietForm.ShowDialog();
            }
        }

        private string GetDeliveryAddressFromDatabase(string orderId)
        {
            string deliveryAddress = string.Empty;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT c.Address 
                        FROM Orders o
                        JOIN Customers c ON o.CustomerID = c.CustomerID
                        WHERE o.OrderID = @OrderID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        deliveryAddress = cmd.ExecuteScalar()?.ToString() ?? "Không có địa chỉ";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy địa chỉ nhận: " + ex.Message);
            }

            return deliveryAddress;
        }

        private DataTable GetOrderProductDetails(string orderId)
        {
            DataTable productDetails = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            p.ProductID AS 'Mã sản phẩm', 
                            p.ProductName AS 'Tên sản phẩm', 
                            p.Price AS 'Đơn giá', 
                            od.Quantity AS 'Số lượng'
                        FROM OrderDetails od
                        JOIN Products p ON od.ProductID = p.ProductID
                        WHERE od.OrderID = @OrderID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(productDetails);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy chi tiết sản phẩm: " + ex.Message);
            }

            return productDetails;
        }

        private void buttonTatCa_Load(object sender, EventArgs e)
        {
            // Nếu cần thêm logic khi load UserControl
        }

        
    }
}
