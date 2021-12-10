using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lh5801_Emu
{
    public partial class Dis_Window : Form
    {
        public Dis_Window()
        {
            InitializeComponent();
        }

        public void SetDump(string dump)
        {
            tbDump.Text = dump;
        }

    }
}
