using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Petersilie.Utils.SnippetReader
{
    public partial class AboutWindow : Form
    {
        private void LinkLabel_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel me = (LinkLabel)sender;
            if (me == null) { 
                return; 
            }
            System.Diagnostics.Process.Start(me.Text);
        }

        public AboutWindow()
        {
            InitializeComponent();

            this.lbl_Name.Text = this.lbl_Name.Text.Replace("{0}", Properties.Resources.IconCreator);
            this.lbl_Link1.Text = Properties.Resources.IconCreatorLink1;
            this.lbl_Link2.Text = Properties.Resources.IconCreatorLink2;
            this.lbl_Link3.Text = Properties.Resources.IconCreatorLink3;            
            this.lbl_Date.Text = this.lbl_Date.Text.Replace("{0}", Properties.Resources.IconCreatorInfoDate);
            this.pb_Icon.Image = Properties.Resources.app_icon_64px;
        }
    }
}
