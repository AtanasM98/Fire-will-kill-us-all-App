using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StatisticsForm
{
    public partial class Statistics : Form
    {
        public delegate void DeadHandler();
        public delegate void EscapedHandler();

        public event DeadHandler someoneDies;
        public event EscapedHandler someoneEscapes;

        public Statistics()
        {
            InitializeComponent();
        }
    }
}
