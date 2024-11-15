using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
//code1
namespace CNPM
{
    public partial class TaoDon : UserControl
    {
        private string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";
        private DataTable searchProductsTable;
        private DataTable selectedProductsTable;


        public TaoDon()
        {
            InitializeComponent();
            LoadProductData();
            InitializeComboBoxes(); // Gọi InitializeComboBoxes để tải dữ liệu cho ComboBox

            ComboBoxDonViVanChuyen.Items.Add("OLEND");
            ComboBoxDonViVanChuyen.Items.Add("ShoppoExpress");
            ComboBoxDonViVanChuyen.Items.Add("TOKO");
            ComboBoxDonViVanChuyen.Items.Add("Sendo");

            // Default selection (optional)
            ComboBoxDonViVanChuyen.SelectedIndex = 0;
            // Gán sự kiện double-click và các sự kiện liên quan đến TextBox
            guna2DataGridView1.CellDoubleClick += guna2DataGridView1_CellDoubleClick;
            TextBoxPhi.TextChanged += TextBoxPhi_TextChanged;
            tien1.TextChanged += CalculateRemainingAmount;
            tien2.TextChanged += CalculateRemainingAmount;
            tien3.TextChanged += CalculateRemainingAmount;
            tien4.TextChanged += CalculateRemainingAmount;


            // Tạo bảng để lưu sản phẩm đã chọn (giỏ hàng) và hiển thị trong DataGridView
            selectedProductsTable = new DataTable();
            selectedProductsTable.Columns.Add("Mã Sản Phẩm");
            selectedProductsTable.Columns.Add("Tên sản phẩm");
            selectedProductsTable.Columns.Add("Giá sản phẩm", typeof(decimal));
            selectedProductsTable.Columns.Add("Số lượng", typeof(int));

            // Ban đầu không hiển thị sản phẩm nào trong giỏ hàng (DataGridView trống)
            InitializeDataGridView(); // Thiết lập các cột ban đầu
                                      // Load dữ liệu cho ComboBox Tỉnh khi form khởi động
            
        }
        private void InitializeComboBoxes()
        {
            ComboBoxTinh.DropDownWidth = 250;
            ComboBoxQuanHuyen.DropDownWidth = 250;
            ComboBoxXaPhuong.DropDownWidth = 250;

            // Load data for ComboBoxTinh (Provinces) on startup
            LoadProvinces();

            // Event handlers for selection changes
            ComboBoxTinh.SelectedIndexChanged += ComboBoxTinh_SelectedIndexChanged;
            ComboBoxQuanHuyen.SelectedIndexChanged += ComboBoxQuanHuyen_SelectedIndexChanged;
        }
        private void LoadProvinces()
        {
            string query = "SELECT province_id, name FROM province";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable provincesTable = new DataTable();
                adapter.Fill(provincesTable);

                ComboBoxTinh.DataSource = provincesTable;
                ComboBoxTinh.DisplayMember = "name";
                ComboBoxTinh.ValueMember = "province_id";
            }
        }

        private void ComboBoxTinh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxTinh.SelectedValue != null)
            {
                int selectedProvinceID = (int)ComboBoxTinh.SelectedValue;
                LoadDistricts(selectedProvinceID);
            }
        }

        private void LoadDistricts(int provinceID)
        {
            string query = "SELECT district_id, name FROM district WHERE province_id = @province_id";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@province_id", provinceID);

                DataTable districtsTable = new DataTable();
                adapter.Fill(districtsTable);

                ComboBoxQuanHuyen.DataSource = districtsTable;
                ComboBoxQuanHuyen.DisplayMember = "name";
                ComboBoxQuanHuyen.ValueMember = "district_id";
                ComboBoxQuanHuyen.SelectedIndex = -1; // Reset selection
            }
        }

        private void ComboBoxQuanHuyen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxQuanHuyen.SelectedValue != null && ComboBoxQuanHuyen.SelectedValue is int)
            {
                int selectedDistrictID = (int)ComboBoxQuanHuyen.SelectedValue;
                LoadWards(selectedDistrictID);
            }
        }

        private void LoadWards(int districtID)
        {
            string query = "SELECT wards_id, name FROM wards WHERE district_id = @district_id";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@district_id", districtID);

                DataTable wardsTable = new DataTable();
                adapter.Fill(wardsTable);

                ComboBoxXaPhuong.DataSource = wardsTable;
                ComboBoxXaPhuong.DisplayMember = "name";
                ComboBoxXaPhuong.ValueMember = "wards_id";
                ComboBoxXaPhuong.SelectedIndex = -1; // Reset selection
            }
        }
        // Thiết lập cột cho DataGridView nhưng không có dữ liệu ban đầu
        private void InitializeDataGridView()
        {
            guna2DataGridView1.AutoGenerateColumns = false;  // Tắt tự động tạo cột
            guna2DataGridView1.Columns.Clear();  // Xóa các cột hiện tại

            // Định nghĩa các cột hiển thị
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Mã Sản Phẩm", DataPropertyName = "Mã Sản Phẩm" });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Tên sản phẩm", DataPropertyName = "Tên sản phẩm" });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Số lượng", DataPropertyName = "Số lượng" });
            guna2DataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "Giá sản phẩm", DataPropertyName = "Giá sản phẩm" });

            guna2DataGridView1.DataSource = null;  // Ban đầu không có dữ liệu
        }

        private void TextBoxPhi_TextChanged(object sender, EventArgs e)
        {
            tien2.Text = TextBoxPhi.Text;
        }

        private void CalculateRemainingAmount(object sender, EventArgs e)
        {
            decimal totalAmount = 0;
            decimal shippingFee = 0;
            decimal discount = 0;
            decimal paidAmount = 0;

            decimal.TryParse(tien1.Text, out totalAmount);   // Tổng tiền sản phẩm
            decimal.TryParse(tien2.Text, out shippingFee);   // Phí vận chuyển
            decimal.TryParse(tien3.Text, out discount);      // Giảm giá
            decimal.TryParse(tien4.Text, out paidAmount);    // Đã thanh toán

            // Tính số tiền còn lại
            decimal remainingAmount = (totalAmount + shippingFee - discount) - paidAmount;
            tien5.Text = remainingAmount.ToString("#,##0");
        }

        // LoadProductData method to ensure columns are loaded with proper names
        private void LoadProductData()
        {
            try
            {
                // Query to select product details
                string query = @"
    SELECT 
        p.ProductID AS 'Mã Sản Phẩm', 
        p.ProductName AS 'Tên sản phẩm', 
        p.Price AS 'Giá sản phẩm',
        (p.Stock - COALESCE(SUM(od.Quantity), 0)) AS 'Số lượng'
    FROM 
        Products p
    LEFT JOIN 
        OrderDetails od ON p.ProductID = od.ProductID
    GROUP BY 
        p.ProductID, p.ProductName, p.Price, p.Stock";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            searchProductsTable = new DataTable();
                            adapter.Fill(searchProductsTable); // Fill the DataTable with product data
                        }
                    }
                }

                // Bind the product data to the DataGridView
                guna2DataGridView1.DataSource = searchProductsTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        
        


        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchText = guna2TextBox1.Text.Trim().ToLower();

                if (!string.IsNullOrEmpty(searchText))
                {
                    // Lọc dữ liệu sản phẩm theo từ khoá tìm kiếm
                    DataView dv = searchProductsTable.DefaultView;
                    dv.RowFilter = $"[Tên sản phẩm] LIKE '%{searchText.Replace("'", "''")}%'"; // Lọc dữ liệu
                    guna2DataGridView1.DataSource = dv.ToTable(); // Hiển thị kết quả lọc
                }
                else
                {
                    // Khi thanh tìm kiếm trống, chỉ hiển thị lại các sản phẩm đã chọn
                    guna2DataGridView1.DataSource = selectedProductsTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm sản phẩm: " + ex.Message);
            }
        }
        // Sự kiện double-click để thêm sản phẩm vào giỏ hàng

        private void guna2DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Ensure the exact column indices match your DataGridView columns
                string productId = guna2DataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(); // Column 0 for ProductID
                string productName = guna2DataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(); // Column 1 for ProductName
                decimal productPrice = Convert.ToDecimal(guna2DataGridView1.Rows[e.RowIndex].Cells[3].Value); // Column 2 for Price

                // Add or update product in the selectedProductsTable
                DataRow existingRow = selectedProductsTable.Select($"[Mã Sản Phẩm] = '{productId}'").FirstOrDefault();
                if (existingRow != null)
                {
                    int currentQuantity = (int)existingRow["Số lượng"];
                    existingRow["Số lượng"] = currentQuantity + 1; // Increase the quantity
                }
                else
                {
                    DataRow newRow = selectedProductsTable.NewRow();
                    newRow["Mã Sản Phẩm"] = productId;
                    newRow["Tên sản phẩm"] = productName;
                    newRow["Giá sản phẩm"] = productPrice; // Ensure price is being set correctly
                    newRow["Số lượng"] = 1; // Initialize quantity
                    selectedProductsTable.Rows.Add(newRow);
                }

                // Update DataGridView to reflect selected products
                guna2DataGridView1.DataSource = selectedProductsTable;

                // Update total price
                UpdateTotalPrice();
            }
        }

        private void UpdateTotalPrice()
        {
            decimal totalPrice = 0;
            foreach (DataRow row in selectedProductsTable.Rows)
            {
                decimal productPrice = Convert.ToDecimal(row["Giá sản phẩm"]);
                int quantity = Convert.ToInt32(row["Số lượng"]);
                totalPrice += productPrice * quantity;
            }

            // Update the total amount display (adjust this to reflect where the total price is shown)
            tien1.Text = totalPrice.ToString("#,##0");
        }

        // Thêm hoặc cập nhật khách hàng
        private int AddOrUpdateCustomer(SqlConnection conn, SqlTransaction transaction, string fullAddress)
        {
            string query = @"INSERT INTO Customers (Name, Phone, Email, DateOfBirth, Address) 
                     VALUES (@Name, @Phone, @Email, @DateOfBirth, @Address);
                     SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Name", TextBoxKhachhang.Text);
                cmd.Parameters.AddWithValue("@Phone", TextBoxSDT.Text);
                cmd.Parameters.AddWithValue("@Email", TextBoxEmail.Text);
                cmd.Parameters.AddWithValue("@DateOfBirth", NgaySinh.Value);
                cmd.Parameters.AddWithValue("@Address", fullAddress); // Sử dụng địa chỉ đầy đủ

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Thêm thông tin vận chuyển
        private Guid AddShippingInfo(SqlConnection conn, SqlTransaction transaction, string orderId)
        {
            Guid shippingId = Guid.NewGuid();
            string selectedShippingCo = ComboBoxDonViVanChuyen.SelectedItem?.ToString() ?? "Unknown";

            string query = @"INSERT INTO Shipping2 (ShippingID, OrderID, ShippingCo, ShippingFee, ShippingCode) 
                     VALUES (@ShippingID, @OrderID, @ShippingCo, @ShippingFee, @ShippingCode)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@ShippingID", SqlDbType.UniqueIdentifier).Value = shippingId;
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@ShippingCo", selectedShippingCo);  // Use selected shipping company
                cmd.Parameters.AddWithValue("@ShippingFee", decimal.Parse(TextBoxPhi.Text));
                cmd.Parameters.AddWithValue("@ShippingCode", TextBoxMaVanChuyen.Text);

                cmd.ExecuteNonQuery();
            }

            return shippingId;
        }


        // Thêm đơn hàng
        // Add this list of statuses at the beginning of the class or within AddOrder
        private readonly string[] orderStatuses = new string[]
        {

    "Đang chuẩn bị",

        };

        // Modify AddOrder method to randomize the OrderStatus
        private string AddOrder(SqlConnection conn, SqlTransaction transaction, int customerId, Guid shippingId, string fullAddress)
        {
            string orderId = Guid.NewGuid().ToString();
            string selectedShippingCo = ComboBoxDonViVanChuyen.SelectedItem?.ToString() ?? "Unknown";

            // Randomly select an order status
            Random random = new Random();
            string randomStatus = orderStatuses[random.Next(orderStatuses.Length)];

            string query = @"INSERT INTO Orders (OrderID, CustomerID, OrderDate, ShippingID, ShippingAddress, ShippingCo, TotalPrice, OrderStatus) 
                     VALUES (@OrderID, @CustomerID, @OrderDate, @ShippingID, @ShippingAddress, @ShippingCo, @TotalPrice, @OrderStatus)";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@OrderID", SqlDbType.UniqueIdentifier).Value = new Guid(orderId);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);
                cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                cmd.Parameters.Add("@ShippingID", SqlDbType.UniqueIdentifier).Value = shippingId;
                cmd.Parameters.AddWithValue("@ShippingAddress", fullAddress); // Sử dụng địa chỉ đầy đủ
                cmd.Parameters.AddWithValue("@ShippingCo", selectedShippingCo);
                cmd.Parameters.AddWithValue("@TotalPrice", decimal.Parse(tien1.Text));
                cmd.Parameters.AddWithValue("@OrderStatus", randomStatus);

                cmd.ExecuteNonQuery();
            }

            return orderId;
        }


        // Thêm chi tiết đơn hàng
        private void AddOrderDetails(SqlConnection conn, SqlTransaction transaction, string orderId)
        {
            foreach (DataRow row in selectedProductsTable.Rows)
            {
                string query = @"INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) 
                                 VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice)";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.Parameters.AddWithValue("@ProductID", row["Mã Sản Phẩm"]);
                    cmd.Parameters.AddWithValue("@Quantity", row["Số lượng"]);
                    cmd.Parameters.AddWithValue("@UnitPrice", row["Giá sản phẩm"]);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string fullAddress = $"{TextBoxDiaChi.Text}, {ComboBoxXaPhuong.Text}, {ComboBoxQuanHuyen.Text}, {ComboBoxTinh.Text}";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    int customerId = AddOrUpdateCustomer(conn, transaction, fullAddress);
                    string orderId = AddOrder(conn, transaction, customerId, Guid.NewGuid(), fullAddress); // Tạo Order trước
                    Guid shippingId = AddShippingInfo(conn, transaction, orderId); // Sử dụng orderId đã tạo cho Shipping2
                    AddOrderDetails(conn, transaction, orderId);

                    transaction.Commit();
                    MessageBox.Show("Đơn hàng đã được tạo thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                    transaction?.Rollback();
                }
            }
        }

        private void TaoDon_Load(object sender, EventArgs e)
        {

        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
