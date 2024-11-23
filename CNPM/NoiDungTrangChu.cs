using System;
using System.Configuration; // Để sử dụng ConfigurationManager
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CNPM
{
    public partial class NoiDungTrangChu : UserControl
    {
        private string connectionString;
        private Chart myNewChart;
        private Timer refreshTimer;

        public NoiDungTrangChu()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

            LoadData();         // Load the data into the DataGridView
            CreateNewChart();   // Create the new chart
            LoadChartData();    // Load data into the new chart
            LoadMetricsData();  // Load and display Doanh Số, Đơn Hàng, Tồn Kho in the panels
            InitializeTimer();  // Start the timer for real-time updates
        }

        private void InitializeTimer()
        {
            refreshTimer = new Timer
            {
                Interval = 5000 // 5 seconds or any preferred interval
            };
            refreshTimer.Tick += (s, e) => LoadData();
            refreshTimer.Start();
        }

        private void LoadData()
        {
            try
            {
                bangTrangChu.AutoGenerateColumns = false;

                string query = @"
                    SELECT p.ProductName AS SanPham, 
                           SUM(od.Quantity) AS SoLuong, 
                           SUM(od.Quantity * od.UnitPrice) AS DoanhThu
                    FROM OrderDetails od
                    JOIN Products p ON od.ProductID = p.ProductID
                    GROUP BY p.ProductName";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable data = new DataTable();
                        adapter.Fill(data);

                        foreach (DataRow row in data.Rows)
                        {
                            row["DoanhThu"] = Convert.ToDecimal(row["DoanhThu"]) / 1_000_000; // Convert to millions
                        }

                        bangTrangChu.DataSource = data;
                        Column1.DataPropertyName = "SanPham";  // Bind 'Sản phẩm'
                        Column2.DataPropertyName = "SoLuong";  // Bind 'Số lượng'
                        Column3.DataPropertyName = "DoanhThu"; // Bind 'Doanh thu'
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void CreateNewChart()
        {
            myNewChart = new Chart
            {
                Size = new System.Drawing.Size(500, 400),
                Location = new System.Drawing.Point(-10, 161)
            };

            ChartArea chartArea = new ChartArea
            {
                AxisX = { Title = "Tháng" },
                AxisY = { Title = "Doanh thu (Triệu VND)" }
            };
            myNewChart.ChartAreas.Add(chartArea);

            Series series = new Series
            {
                ChartType = SeriesChartType.Column
            };
            myNewChart.Series.Add(series);

            this.Controls.Add(myNewChart);
        }

        private void LoadChartData()
        {
            try
            {
                string query = @"
                    SELECT MONTH(o.OrderDate) AS Thang, 
                           SUM(od.Quantity * od.UnitPrice) AS DoanhThu
                    FROM OrderDetails od
                    JOIN Orders o ON od.OrderID = o.OrderID
                    WHERE MONTH(o.OrderDate) BETWEEN 1 AND 12  
                    GROUP BY MONTH(o.OrderDate)
                    ORDER BY MONTH(o.OrderDate)";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable data = new DataTable();
                        adapter.Fill(data);

                        myNewChart.Series[0].Points.Clear();
                        foreach (DataRow row in data.Rows)
                        {
                            int month = Convert.ToInt32(row["Thang"]);
                            decimal revenue = Convert.ToDecimal(row["DoanhThu"]) / 1_000_000; // Convert to millions

                            myNewChart.Series[0].Points.AddXY(month, revenue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chart data: " + ex.Message);
            }
        }

        private void LoadMetricsData()
        {
            try
            {
                string revenueQuery = @"
            SELECT SUM(od.Quantity * od.UnitPrice) AS DoanhSo 
            FROM OrderDetails od
            JOIN Orders o ON od.OrderID = o.OrderID
            WHERE YEAR(o.OrderDate) = 2024";

                string ordersQuery = @"
            SELECT COUNT(o.OrderID) AS DonHang 
            FROM Orders o 
            WHERE YEAR(o.OrderDate) = 2024";

                string stockQuery = @"
            SELECT SUM(p.Stock - ISNULL(od.TotalQuantity, 0)) AS TonKho
            FROM Products p
            LEFT JOIN (
                SELECT ProductID, SUM(Quantity) AS TotalQuantity
                FROM OrderDetails
                GROUP BY ProductID
            ) od ON p.ProductID = od.ProductID";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand revenueCmd = new SqlCommand(revenueQuery, con))
                    using (SqlCommand ordersCmd = new SqlCommand(ordersQuery, con))
                    using (SqlCommand stockCmd = new SqlCommand(stockQuery, con))
                    {
                        decimal currentRevenue = Convert.ToDecimal(revenueCmd.ExecuteScalar());
                        textbox1.Text = (currentRevenue / 1_000_000).ToString("N0");
                        textbox1.TextAlign = HorizontalAlignment.Center;

                        int currentOrders = Convert.ToInt32(ordersCmd.ExecuteScalar());
                        textbox3.Text = currentOrders.ToString();
                        textbox3.TextAlign = HorizontalAlignment.Center;

                        int currentStock = Convert.ToInt32(stockCmd.ExecuteScalar());
                        textbox5.Text = currentStock.ToString();
                        textbox5.TextAlign = HorizontalAlignment.Center;
                    }
                }

                CalculateAndDisplayIncrease();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading metrics data: " + ex.Message);
            }
        }


        private decimal GetRevenueForPeriod(string startDate, string endDate)
        {
            string query = @"
                SELECT SUM(od.Quantity * od.UnitPrice) AS DoanhSo 
                FROM OrderDetails od
                JOIN Orders o ON od.OrderID = o.OrderID
                WHERE o.OrderDate BETWEEN @startDate AND @endDate";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                con.Open();

                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        private int GetOrdersForPeriod(string startDate, string endDate)
        {
            string query = @"
                SELECT COUNT(OrderID) AS DonHang 
                FROM Orders 
                WHERE OrderDate BETWEEN @startDate AND @endDate";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                con.Open();

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void CalculateAndDisplayIncrease()
        {
            decimal firstHalfRevenue = GetRevenueForPeriod("2024-01-01", "2024-06-30");
            decimal secondHalfRevenue = GetRevenueForPeriod("2024-07-01", "2024-12-31");
            double revenueIncrease = CalculatePercentageIncrease((double)firstHalfRevenue, (double)secondHalfRevenue);
            textbox2.Text = "↑ " + revenueIncrease.ToString("F2") + "%";
            textbox2.TextAlign = HorizontalAlignment.Center;

            int firstHalfOrders = GetOrdersForPeriod("2024-01-01", "2024-06-30");
            int secondHalfOrders = GetOrdersForPeriod("2024-07-01", "2024-12-31");
            double ordersIncrease = CalculatePercentageIncrease(firstHalfOrders, secondHalfOrders);
            textbox4.Text = "↑ " + ordersIncrease.ToString("F2") + "%";
            textbox4.TextAlign = HorizontalAlignment.Center;
        }

        private double CalculatePercentageIncrease(double firstHalf, double secondHalf)
        {
            return firstHalf == 0 ? 0 : ((secondHalf - firstHalf) / firstHalf) * 100;
        }

        private void NoiDungTrangChu_Load(object sender, EventArgs e)
        {
            ConfigEncryption.EncryptConnectionStrings();
        }
        public static class ConfigEncryption
        {
            public static void EncryptConnectionStrings()
            {
                try
                {
                    // Lấy đường dẫn thực sự của file cấu hình hiện tại
                    string exePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                    ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap
                    {
                        ExeConfigFilename = exePath
                    };

                    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                    ConfigurationSection section = config.GetSection("connectionStrings");

                    // Kiểm tra nếu phần cấu hình không được mã hóa, thì tiến hành mã hóa
                    if (section != null && !section.SectionInformation.IsProtected)
                    {
                        section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                        config.Save(ConfigurationSaveMode.Modified);

                        // Thay vì hiện MessageBox, bạn có thể ghi log lại
                        Console.WriteLine("Chuỗi kết nối đã được mã hóa thành công.");
                    }
                    else
                    {
                        // Không cần làm gì nếu chuỗi kết nối đã được mã hóa hoặc không tìm thấy cấu hình
                        Console.WriteLine("Chuỗi kết nối đã được mã hóa trước đó hoặc không tìm thấy phần cấu hình.");
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lại lỗi nếu có xảy ra, thay vì hiển thị MessageBox
                    Console.WriteLine("Có lỗi xảy ra khi mã hóa: " + ex.Message);
                }
            }
        }
    }
}
