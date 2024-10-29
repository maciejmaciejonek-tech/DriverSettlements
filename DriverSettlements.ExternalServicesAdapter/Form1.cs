using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DriverSettlements.ExternalServicesAdapter
{
    public partial class Form1 : Form
    {
        private Func<WebView2?, bool> _performAction;
        public Form1()
        {
            InitializeComponent();
        }

        public void SetPerformAction(Func<WebView2? , bool> performAction)
        {
            _performAction = performAction;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_performAction != null)
                _performAction(this.webView2);
        }
    }
}
