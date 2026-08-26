using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinForm.UserControls
{
    public partial class ucWorkArea : UserControl
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, UserControl> MultiUserControls { get; set; } = new Dictionary<string, UserControl>();
        public ucWorkArea()
        {
            InitializeComponent();
        }
    }
}
