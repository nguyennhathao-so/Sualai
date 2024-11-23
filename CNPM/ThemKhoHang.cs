using System;
using System.Configuration; // Để sử dụng ConfigurationManager
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CNPM
{
    public partial class ThemKhoHang : Form
    {
        private string connectionString;

        public ThemKhoHang()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
        }

        private void ThemKhoHang_Load(object sender, EventArgs e)
        {
            LoadCategories();  // Load categories into ComboBox
            SetNextProductID(); // Set the next product ID when the form loads

            // Populate Bảo Hành ComboBox with "Có" and "Không"
            baohanh.Items.Add("Có");
            baohanh.Items.Add("Không");
        }

        private void LoadCategories()
        {
            string query = "SELECT CategoryID, CategoryName FROM Category";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable categories = new DataTable();
                    adapter.Fill(categories);

                    // Bind the ComboBox to the Category data
                    guna2ComboBox1.DataSource = categories;
                    guna2ComboBox1.DisplayMember = "CategoryName"; // Display the category name in the ComboBox
                    guna2ComboBox1.ValueMember = "CategoryID"; // Use the CategoryID for inserting into the database
                }
            }
        }

        private bool ValidateForm()
        {
            // Validate numeric fields, including removing any non-numeric characters
            if (string.IsNullOrEmpty(Tensp.Text) ||
                string.IsNullOrEmpty(GiaSp.Text) ||
                !decimal.TryParse(GiaSp.Text, out _) ||  // Validate price
                !int.TryParse(Soluong.Text, out _) ||    // Validate stock
                !decimal.TryParse(cannang.Text, out _))  // Validate weight, remove "kg" unit
            {
                MessageBox.Show("Please enter valid values for product fields.");
                return false;
            }

            return true;
        }

        private void SetNextProductID()
        {
            string query = "SELECT ISNULL(MAX(ProductID), 0) + 1 FROM Products";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                int nextProductID = (int)cmd.ExecuteScalar(); // Get the next ProductID
                Masp.Text = nextProductID.ToString(); // Set the TextBox with the next ProductID
            }
        }

        private void ExitKhoHang_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddNewProduct()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Query to insert new product into Products table (no need for ProductID as it's auto-increment)
                    string query = @"INSERT INTO Products (ProductName, CategoryID, Price, Description, Stock, Weight, Size, Trademark, Origin, Warranty)
                             VALUES (@ProductName, @CategoryID, @Price, @Description, @Stock, @Weight, @Size, @Trademark, @Origin, @Warranty)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", Tensp.Text);  // Product Name TextBox
                        cmd.Parameters.AddWithValue("@CategoryID", (int)guna2ComboBox1.SelectedValue);  // Get selected CategoryID from ComboBox as int
                        cmd.Parameters.AddWithValue("@Price", decimal.Parse(GiaSp.Text, System.Globalization.CultureInfo.InvariantCulture));  // For price
                        cmd.Parameters.AddWithValue("@Description", MoTaKhoHang.Text);  // Description TextBox
                        cmd.Parameters.AddWithValue("@Stock", Convert.ToInt32(Soluong.Text));  // Stock TextBox
                        cmd.Parameters.AddWithValue("@Weight", Convert.ToDecimal(cannang.Text.Replace("kg", "").Trim()));  // Weight TextBox (remove "kg" unit)
                        cmd.Parameters.AddWithValue("@Size", kichthuoc.Text);  // Size TextBox
                        cmd.Parameters.AddWithValue("@Trademark", NhaSX.Text);  // Trademark TextBox
                        cmd.Parameters.AddWithValue("@Origin", XuatXu.Text);  // Origin TextBox
                        cmd.Parameters.AddWithValue("@Warranty", baohanh.SelectedItem?.ToString() ?? "Không");  // Warranty ComboBox selected item

                        cmd.ExecuteNonQuery();  // Execute query
                    }

                    MessageBox.Show("Product added successfully!");
                    ClearForm();  // Optionally, clear the form after adding the product
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            Tensp.Clear();
            GiaSp.Clear();
            MoTaKhoHang.Clear();
            Soluong.Clear();
            cannang.Clear();
            kichthuoc.Clear();
            guna2ComboBox1.SelectedIndex = -1; // Reset category selection
            baohanh.SelectedIndex = -1; // Reset warranty selection
        }

        private void LuuKhoHang_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                AddNewProduct(); // Proceed to add the product
            }
        }

        private void nenChiTiet_Paint(object sender, PaintEventArgs e)
        {
            // Custom painting code can go here if needed
        }

        private void baohanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle any specific logic for warranty selection if needed
        }
    }
}
