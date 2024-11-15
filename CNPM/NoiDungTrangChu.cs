using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CNPM
{
    public partial class NoiDungTrangChu : UserControl
    {
        private Chart myNewChart;
        private Timer refreshTimer;

        private void InitializeTimer()
        {
            refreshTimer = new Timer();
            refreshTimer.Interval = 5000; // Set interval to 5 seconds or any preferred interval
            refreshTimer.Tick += (s, e) => LoadData();
            refreshTimer.Start();
        }

        public NoiDungTrangChu()
        {
            InitializeComponent();
            LoadData();         // Load the data into the DataGridView
            CreateNewChart();   // Create the new chart
            LoadChartData();    // Load data into the new chart
            LoadMetricsData();  // Load and display Doanh Số, Đơn Hàng, Tồn Kho in the panels
            InitializeTimer();   // Start the timer for real-time updates
        }

        // Method to retrieve data from the SQL database and display it in the DataGridView
        private void LoadData()
        {
            try
            {
                bangTrangChu.AutoGenerateColumns = false;

                string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";
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
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable data1 = new DataTable();
                            adapter.Fill(data1);

                            if (data1 != null && data1.Rows.Count > 0)
                            {
                                foreach (DataRow row in data1.Rows)
                                {
                                    row["DoanhThu"] = Convert.ToDecimal(row["DoanhThu"]) / 1_000_000;  // Convert to millions
                                }

                                bangTrangChu.DataSource = data1;
                                this.Column1.DataPropertyName = "SanPham";  // For 'Sản phẩm' column
                                this.Column2.DataPropertyName = "SoLuong";  // For 'Số lượng' column
                                this.Column3.DataPropertyName = "DoanhThu"; // For 'Doanh Thu' column
                            }
                            else
                            {
                                MessageBox.Show("No data found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        // Method to create a new chart and add it to the form
        private void CreateNewChart()
        {
            myNewChart = new Chart();
            myNewChart.Size = new System.Drawing.Size(500, 400);
            myNewChart.Location = new System.Drawing.Point(-10, 161);  // Set the location for the new chart

            ChartArea chartArea = new ChartArea();
            chartArea.AxisX.Title = "Tháng";
            chartArea.AxisY.Title = "Doanh thu (VND)";
            myNewChart.ChartAreas.Add(chartArea);

            Series series = new Series();
            series.ChartType = SeriesChartType.Column;  // Set the type of chart (Column chart)
            myNewChart.Series.Add(series);

            this.Controls.Add(myNewChart);
        }

        // Method to load data into the new chart
        private void LoadChartData()
        {
            try
            {
                string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

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

                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable data = new DataTable();
                            adapter.Fill(data);

                            myNewChart.Series[0].Points.Clear();

                            foreach (DataRow row in data.Rows)
                            {
                                int month = Convert.ToInt32(row["Thang"]);
                                decimal revenue = Convert.ToDecimal(row["DoanhThu"]) / 1_000_000;  // Convert to millions

                                myNewChart.Series[0].Points.AddXY(month, revenue);
                            }

                            myNewChart.ChartAreas[0].AxisX.Title = "Tháng";
                            myNewChart.ChartAreas[0].AxisY.Title = "Doanh thu (Triệu VND)";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chart data: " + ex.Message);
            }
        }

        // Method to fetch data for metrics like Doanh Số, Đơn Hàng, and Tồn Kho
        private void LoadMetricsData()
        {
            try
            {
                string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Fetch Doanh Số (Revenue)
                    string revenueQuery = @"
                        SELECT SUM(od.Quantity * od.UnitPrice) AS DoanhSo 
                        FROM OrderDetails od
                        JOIN Orders o ON od.OrderID = o.OrderID
                        WHERE YEAR(o.OrderDate) = 2023";

                    // Fetch Đơn Hàng (Orders count)
                    string ordersQuery = @"
                        SELECT COUNT(o.OrderID) AS DonHang 
                        FROM Orders o 
                        WHERE YEAR(o.OrderDate) = 2023";

                    // Fetch Tồn Kho (Remaining Stock)
                    string stockQuery = @"
                        SELECT SUM(p.Stock - ISNULL(od.Quantity, 0)) AS TonKho
                        FROM Products p
                        LEFT JOIN OrderDetails od ON p.ProductID = od.ProductID";

                    using (SqlCommand revenueCommand = new SqlCommand(revenueQuery, con))
                    using (SqlCommand ordersCommand = new SqlCommand(ordersQuery, con))
                    using (SqlCommand stockCommand = new SqlCommand(stockQuery, con))
                    {
                        decimal currentRevenue = Convert.ToDecimal(revenueCommand.ExecuteScalar());
                        textbox1.Text = (currentRevenue / 1_000_000).ToString("N0");  // Display in millions
                        textbox1.TextAlign = HorizontalAlignment.Center;

                        int currentOrders = Convert.ToInt32(ordersCommand.ExecuteScalar());
                        textbox3.Text = currentOrders.ToString();  // Display orders count
                        textbox3.TextAlign = HorizontalAlignment.Center;


                        int currentStock = Convert.ToInt32(stockCommand.ExecuteScalar());
                        textbox5.Text = currentStock.ToString();  // Display remaining stock
                        textbox5.TextAlign = HorizontalAlignment.Center;

                    }
                }

                // Calculate and display percentage increases only for Doanh Số and Đơn Hàng
                CalculateAndDisplayIncrease();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading metrics data: " + ex.Message);
            }
        }

        // Helper methods to fetch data for two periods
        private decimal GetRevenueForPeriod(string startDate, string endDate)
        {
            string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";
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
            string connectionString = @"Data Source=Hphuc\MSSQLSERVERF;Initial Catalog=CNPM_database;Integrated Security=True";
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

        // Calculate percentage increase and display them in textboxes
        private void CalculateAndDisplayIncrease()
        {
            decimal firstHalfRevenue = GetRevenueForPeriod("2023-01-01", "2023-06-30");
            decimal secondHalfRevenue = GetRevenueForPeriod("2023-07-01", "2023-12-31");
            double revenueIncrease = CalculatePercentageIncrease((double)firstHalfRevenue, (double)secondHalfRevenue);  // Convert to double
            textbox2.Text = "↑ " + revenueIncrease.ToString("F2") + "%";  // Display the increase in textbox2
            textbox2.TextAlign = HorizontalAlignment.Center;

            int firstHalfOrders = GetOrdersForPeriod("2023-01-01", "2023-06-30");
            int secondHalfOrders = GetOrdersForPeriod("2023-07-01", "2023-12-31");
            double ordersIncrease = CalculatePercentageIncrease(firstHalfOrders, secondHalfOrders);
            textbox4.Text = "↑ " + ordersIncrease.ToString("F2") + "%";
            textbox4.TextAlign = HorizontalAlignment.Center;
        }

        private double CalculatePercentageIncrease(double firstHalf, double secondHalf)
        {
            if (firstHalf == 0) return 0;  // Avoid division by zero
            return ((secondHalf - firstHalf) / firstHalf) * 100;
        }

        private void NoiDungTrangChu_Load(object sender, EventArgs e)
        {

        }

        private void bangTrangChu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
