using Guna.UI2.WinForms;
using System;
using System.Configuration; // Để sử dụng ConfigurationManager
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CNPM
{
    public partial class KhoHang : UserControl
    {
        private DataTable searchProductsTable;
        private readonly string connectionString;

        public KhoHang()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

            LoadProductData(); // Tải dữ liệu sản phẩm khi khởi tạo
            TimKiem.TextChanged += TimKiem_TextChanged; // Xử lý tìm kiếm
            bangKhoHang.CellDoubleClick += BangKhoHang_CellDoubleClick; // Xử lý double-click
        }

        private void buttonThem_Click(object sender, EventArgs e)
        {
            ThemKhoHang themKhoHang = new ThemKhoHang();
            themKhoHang.ShowDialog();
            LoadProductData(); // Tải lại dữ liệu sau khi thêm sản phẩm
        }

        private void LoadProductData()
        {
            bangKhoHang.AutoGenerateColumns = false;
            bangKhoHang.Columns.Clear(); // Xóa các cột cũ để tránh bị trùng

            try
            {
                string query = @"
                SELECT 
                    p.ProductID AS 'Mã sản phẩm', 
                    p.ProductName AS 'Tên sản phẩm', 
                    c.CategoryName AS 'Ngành hàng', 
                    (p.Stock - ISNULL(SUM(od.Quantity), 0)) AS 'Tồn kho',
                    p.Trademark AS 'Thương hiệu', 
                    ISNULL(SUM(od.Quantity), 0) AS 'Đã bán',
                    p.Origin AS 'Xuất xứ',       
                    p.Warranty AS 'Bảo hành', 
                    p.Weight AS 'Cân nặng', 
                    p.Size AS 'Kích thước',
                    p.Description AS 'Mô tả'
                FROM 
                    Products p
                JOIN 
                    Category c ON p.CategoryID = c.CategoryID
                LEFT JOIN 
                    OrderDetails od ON od.ProductID = p.ProductID  
                GROUP BY 
                    p.ProductID, 
                    p.ProductName, 
                    c.CategoryName, 
                    p.Stock, 
                    p.Trademark, 
                    p.Origin, 
                    p.Warranty,
                    p.Weight,
                    p.Size,
                    p.Description;";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        searchProductsTable = new DataTable();
                        adapter.Fill(searchProductsTable);

                        if (searchProductsTable.Rows.Count > 0)
                        {
                            bangKhoHang.DataSource = searchProductsTable;

                            // Định nghĩa các cột chính
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Mã sản phẩm",
                                DataPropertyName = "Mã sản phẩm",
                                HeaderText = "Mã sản phẩm"
                            });
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Tên sản phẩm",
                                DataPropertyName = "Tên sản phẩm",
                                HeaderText = "Tên sản phẩm"
                            });
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Ngành hàng",
                                DataPropertyName = "Ngành hàng",
                                HeaderText = "Ngành hàng"
                            });
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Tồn kho",
                                DataPropertyName = "Tồn kho",
                                HeaderText = "Tồn kho"
                            });
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Thương hiệu",
                                DataPropertyName = "Thương hiệu",
                                HeaderText = "Thương hiệu"
                            });
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Đã bán",
                                DataPropertyName = "Đã bán",
                                HeaderText = "Đã bán"
                            });

                            // Các cột ẩn
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Xuất xứ",
                                DataPropertyName = "Xuất xứ",
                                Visible = false
                            });
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Bảo hành",
                                DataPropertyName = "Bảo hành",
                                Visible = false
                            });
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Cân nặng",
                                DataPropertyName = "Cân nặng",
                                Visible = false
                            });
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Kích thước",
                                DataPropertyName = "Kích thước",
                                Visible = false
                            });
                            bangKhoHang.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                Name = "Mô tả",
                                DataPropertyName = "Mô tả",
                                Visible = false
                            });
                        }
                        else
                        {
                            MessageBox.Show("Không có dữ liệu để hiển thị.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu kho hàng: " + ex.Message);
            }
        }

        private void TimKiem_TextChanged(object sender, EventArgs e)
        {
            string searchText = TimKiem.Text.Trim();

            if (searchProductsTable != null && !string.IsNullOrEmpty(searchText))
            {
                FilterProductsByName(searchText);
            }
            else
            {
                ResetFilter();
            }
        }

        private void FilterProductsByName(string searchText)
        {
            DataView view = searchProductsTable.DefaultView;
            view.RowFilter = $"[Tên sản phẩm] LIKE '%{searchText.Replace("'", "''")}%'"; // Tránh lỗi SQL Injection
            bangKhoHang.DataSource = view;
        }

        private void ResetFilter()
        {
            searchProductsTable.DefaultView.RowFilter = string.Empty;
            bangKhoHang.DataSource = searchProductsTable.DefaultView;
        }

        private void BangKhoHang_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = bangKhoHang.Rows[e.RowIndex];
                string productId = row.Cells["Mã sản phẩm"].Value.ToString();

                try
                {
                    string query = @"
                    SELECT 
                        p.ProductID, 
                        p.ProductName, 
                        c.CategoryName, 
                        p.Stock, 
                        p.Trademark, 
                        p.Origin, 
                        p.Warranty, 
                        p.Weight, 
                        p.Size, 
                        p.Description, 
                        ISNULL(SUM(od.Quantity), 0) AS 'Đã bán', 
                        p.Price
                    FROM 
                        Products p
                    INNER JOIN 
                        Category c ON p.CategoryID = c.CategoryID
                    LEFT JOIN 
                        OrderDetails od ON od.ProductID = p.ProductID
                    WHERE 
                        p.ProductID = @ProductID
                    GROUP BY 
                        p.ProductID, 
                        p.ProductName, 
                        c.CategoryName, 
                        p.Stock, 
                        p.Trademark, 
                        p.Origin, 
                        p.Warranty, 
                        p.Weight, 
                        p.Size, 
                        p.Description, 
                        p.Price";

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@ProductID", productId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Lấy thông tin sản phẩm
                                    string masp = reader["ProductID"].ToString();
                                    string productName = reader["ProductName"].ToString();
                                    string category = reader["CategoryName"].ToString();
                                    string stock = reader["Stock"].ToString();
                                    string trademark = reader["Trademark"].ToString();
                                    string origin = reader["Origin"].ToString();
                                    string warranty = reader["Warranty"].ToString();
                                    string weight = reader["Weight"].ToString();
                                    string size = reader["Size"].ToString();
                                    string description = reader["Description"].ToString();
                                    string daban = reader["Đã bán"].ToString();
                                    string price = reader["Price"].ToString();

                                    // Hiển thị thông tin sản phẩm chi tiết
                                    ChiTietKhoHang chiTietForm = new ChiTietKhoHang(
                                        masp, productName, category, stock, trademark,
                                        origin, warranty, weight, size, description, daban, price);

                                    chiTietForm.ShowDialog();
                                }
                                else
                                {
                                    MessageBox.Show("Không tìm thấy chi tiết sản phẩm.");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lấy thông tin sản phẩm: " + ex.Message);
                }
            }
        }

        private void KhoHang_Load(object sender, EventArgs e)
        {
            // Nơi khởi tạo dữ liệu nếu cần thêm
        }
    }
}
