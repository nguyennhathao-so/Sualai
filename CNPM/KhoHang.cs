using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CNPM
{
    public partial class KhoHang : UserControl
    {
        private DataTable searchProductsTable;


        public KhoHang()
        {
            InitializeComponent();
            LoadProductData(); // Load product data when the user control is initialized
            TimKiem.TextChanged += TimKiem_TextChanged; // Attach TextChanged event for search
            bangKhoHang.CellDoubleClick += BangKhoHang_CellDoubleClick; // Ensure double-click event is set
        }

        private void buttonThem_Click(object sender, EventArgs e)
        {
            ThemKhoHang themKhoHang = new ThemKhoHang();
            themKhoHang.ShowDialog();
            LoadProductData(); // Reload data after adding a product
        }

        // Load product data into the DataGridView
        private void LoadProductData()
        {
            bangKhoHang.AutoGenerateColumns = false;
            bangKhoHang.Columns.Clear();  // Clear existing columns to avoid duplicates

            string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

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
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            searchProductsTable = new DataTable();
                            adapter.Fill(searchProductsTable);

                            if (searchProductsTable.Rows.Count > 0)
                            {
                                bangKhoHang.DataSource = searchProductsTable;

                                // Define columns with consistent names
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

                                // Add hidden columns for additional details
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
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
            view.RowFilter = $"[Tên sản phẩm] LIKE '%{searchText.Replace("'", "''")}%'";
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

                string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";
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
                OrderDetails od ON od.ProductID = p.ProductID
            INNER JOIN 
                Category c ON p.CategoryID = c.CategoryID
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
                                // Retrieve each field's value from the reader
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

                                // Create and show ChiTietKhoHang form with product details
                                ChiTietKhoHang chiTietForm = new ChiTietKhoHang(
                                    masp, productName, category, stock, trademark,
                                    origin, warranty, weight, size, description, daban, price);

                                chiTietForm.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("No product details found.");
                            }
                        }
                    }
                }
            }
        }

        private void KhoHang_Load(object sender, EventArgs e)
        {
            // Optionally load data here again if needed
        }

        private void TimKiem_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
