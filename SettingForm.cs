using System;
using System.Windows.Forms;

namespace pizzaplayer
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            //in setting form we change simple app settings by changing the properties setting values
            this.BackColor = Properties.Settings.Default.bg;
            checkBox1.Checked = Properties.Settings.Default.startup;
            checkBox2.Checked = Properties.Settings.Default.autoupdate;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            DialogResult = cd.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                Properties.Settings.Default.bg = cd.Color;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            DialogResult = fd.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                Properties.Settings.Default.font = fd.Font;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                Properties.Settings.Default.startup = true;
            else
                Properties.Settings.Default.startup = false;
            if (checkBox2.Checked == true)
                Properties.Settings.Default.autoupdate = true;
            else
                Properties.Settings.Default.autoupdate = false;
            Properties.Settings.Default.Save();
            PizzaPlayer obj = new PizzaPlayer();
            obj.WindowState = FormWindowState.Normal;
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            PizzaPlayer obj = new PizzaPlayer();
            obj.WindowState = FormWindowState.Normal;
            this.Close();
        }
    }
}
