using System;
using System.Windows.Forms;

namespace pizzaplayer
{
    public partial class Start : Form
    {
        public int time = 0,degree=0;
        PizzaPlayer obj = new PizzaPlayer();
        RotateImageClass rot = new RotateImageClass();

        public Start()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = Properties.Settings.Default.bg;
            timer1.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            //do the rotation of the pizza logo
            time++;
            degree += 2;
            if (time == 50)
            {
                timer1.Stop();
                this.Hide();
                obj.Show();
            }
            pictureBox1.Image = rot.RotateImage(Properties.Resources.mainpic, degree);
        }

        private void Label3_Click(object sender, EventArgs e)
        {            
            Environment.Exit(0);
        }
    }
}
