using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Service;

namespace ProtienMarkerGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string inputString = txtInputString.Text;
            int noOfPartitions = Convert.ToInt32(txtNoOfPartitions.Text);
            var processor = new Processor();
            processor.ProcessProtein(inputString, noOfPartitions);
        }
    }
}
