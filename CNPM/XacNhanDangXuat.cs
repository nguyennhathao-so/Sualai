using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class XacNhanDangXuat : Form
    {
        public XacNhanDangXuat()
        {
            InitializeComponent();
        }

        private void guna2OK_Click(object sender, EventArgs e)
        {
            LogIn logIn = new LogIn();
            this.Hide();
            logIn.ShowDialog();
            this.Close();
        }

        private void huy_Click(object sender, EventArgs e)
        {
            TrangChu nt = new TrangChu();
            this.Hide();
            nt.ShowDialog();
            this.Close();
            
        }
    }
}
