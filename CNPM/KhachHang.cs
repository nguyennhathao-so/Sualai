using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CNPM
{
    public partial class KhachHang : UserControl
    {
        private DataTable customerDataTable;

        public KhachHang()
        {
            InitializeComponent();
            ConfigureDataGridView();  // Set up columns once
            LoadCustomerData();       // Load data on initialization
            TimKiem.TextChanged += TimKiem_TextChanged; // Attach search event to TimKiem
        }

        private void ConfigureDataGridView()
        {
            DataGridViewKhachhang.AutoGenerateColumns = false;
            DataGridViewKhachhang.Columns.Clear();

            // Define the columns for DataGridView
            DataGridViewKhachhang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Tên khách hàng",
                DataPropertyName = "Tên khách hàng",
                HeaderText = "Tên khách hàng"
            });
            DataGridViewKhachhang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Ngày sinh",
                DataPropertyName = "Ngày sinh",
                HeaderText = "Ngày sinh"
            });
            DataGridViewKhachhang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Số điện thoại",
                DataPropertyName = "Số điện thoại",
                HeaderText = "Số điện thoại"
            });
            DataGridViewKhachhang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Ngành hàng ưu chuộng",
                DataPropertyName = "Ngành hàng ưu chuộng",
                HeaderText = "Ngành hàng ưu chuộng"
            });
            DataGridViewKhachhang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Số đơn mua",
                DataPropertyName = "Số đơn mua",
                HeaderText = "Số đơn mua"
            });
            DataGridViewKhachhang.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Đã nhận",
                DataPropertyName = "Đã nhận",
                HeaderText = "Đã nhận"
            });
        }

        private void LoadCustomerData()
        {
            string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

            try
            {
                string query = @"
                SELECT 
                    c.CustomerID,
                    c.Name AS 'Tên khách hàng',
                    c.Phone AS 'Số điện thoại',
                    c.DateOfBirth AS 'Ngày sinh',
                    cat.CategoryName AS 'Ngành hàng ưu chuộng',
                    COUNT(od.ProductID) AS 'Số đơn mua',
                    CASE 
                        WHEN EXISTS (SELECT 1 FROM Orders o WHERE o.CustomerID = c.CustomerID AND o.OrderStatus = N'Đã nhận')
                        THEN N'Đã nhận'
                        ELSE N'Chưa nhận'
                    END AS 'Đã nhận'
                FROM 
                    Customers c
                JOIN 
                    Orders o ON c.CustomerID = o.CustomerID
                JOIN 
                    OrderDetails od ON o.OrderID = od.OrderID
                JOIN 
                    Products p ON od.ProductID = p.ProductID
                JOIN 
                    Category cat ON p.CategoryID = cat.CategoryID
                GROUP BY 
                    c.CustomerID, c.Name, c.Phone, c.DateOfBirth, cat.CategoryName
                ORDER BY 
                    c.CustomerID;";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            customerDataTable = new DataTable();
                            adapter.Fill(customerDataTable);

                            if (customerDataTable.Rows.Count > 0)
                            {
                                DataGridViewKhachhang.DataSource = customerDataTable;
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
            string filterText = TimKiem.Text.Trim();

            if (customerDataTable != null)
            {
                if (!string.IsNullOrEmpty(filterText))
                {
                    // Apply filter for name and phone; exclude date filtering to avoid errors with LIKE operator
                    string filter = $"[Tên khách hàng] LIKE '%{filterText}%' OR [Số điện thoại] LIKE '%{filterText}%'";

                    // Add specific date filtering if the text matches a date pattern
                    DateTime date;
                    if (DateTime.TryParse(filterText, out date))
                    {
                        filter += $" OR CONVERT([Ngày sinh], 'System.String') LIKE '%{date.ToShortDateString()}%'";
                    }

                    customerDataTable.DefaultView.RowFilter = filter;
                }
                else
                {
                    customerDataTable.DefaultView.RowFilter = string.Empty;
                }

                // Update DataGridView
                DataGridViewKhachhang.DataSource = customerDataTable.DefaultView;
            }
        }

        private void KhachHang_Load(object sender, EventArgs e)
        {

        }
    }
}
