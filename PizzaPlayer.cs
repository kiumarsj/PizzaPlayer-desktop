using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace pizzaplayer
{
    public partial class PizzaPlayer : Form
    {
        //here we define our variables and creating objects
        string dir = AppDomain.CurrentDomain.BaseDirectory + @"data\";
        int volbeformute = 0, degree = 0;
        string RepeatMode = "off", fileToOpen;
        bool ShuffleMode = false;
        double position = 0.0;
        WMPLib.WindowsMediaPlayer wmplayer = new WMPLib.WindowsMediaPlayer();
        
        public PizzaPlayer()
        {
            InitializeComponent();            
        }

        private void PizzaPlayer_Load(object sender, EventArgs e)
        {
            //initial config
            this.BackColor = Properties.Settings.Default.bg;            
            dataGridView1.BackgroundColor = Properties.Settings.Default.bg;
            dataGridView1.GridColor = Properties.Settings.Default.bg;
            comboBox1.BackColor = Properties.Settings.Default.bg;
            textBox1.BackColor = Properties.Settings.Default.bg;
            textBox2.BackColor = Properties.Settings.Default.bg;
            textBox3.BackColor = Properties.Settings.Default.bg;
            textBox5.BackColor = Properties.Settings.Default.bg;
            label1.Font = Properties.Settings.Default.font;
            label2.Font = Properties.Settings.Default.font;
            label3.Font = Properties.Settings.Default.font;
            label4.Font = Properties.Settings.Default.font;
            label7.Font = Properties.Settings.Default.font;
            label8.Font = Properties.Settings.Default.font;
            textBox1.Font = Properties.Settings.Default.font;
            textBox2.Font = Properties.Settings.Default.font;
            textBox3.Font = Properties.Settings.Default.font;
            textBox5.Font = Properties.Settings.Default.font;            
            textBox1.Text = "Welcome " + Environment.UserName;
            comboBox1.Font = Properties.Settings.Default.font;            
            dataGridView1.Rows[0].Cells[0].Value = "";
            trackBar1.Value = Properties.Settings.Default.volume;
            wmplayer.settings.volume = 100;
            comboBox1.Text = "";
            //here we create a directory for playlists files
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            import_pls();
        }

        private void import_pls()
        {
            //here we import playlists to the combobox
            try
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                FileInfo[] playLists = di.GetFiles("pl-" + "*" + ".txt");
                comboBox1.Items.Clear();
                foreach (FileInfo a in playLists)
                {
                    string plname = a.Name;
                    plname = plname.Remove(0, 3);
                    plname = plname.Replace(".txt", "");
                    comboBox1.Items.Add(plname);
                }
            }
            catch(Exception e)
            {
                textBox1.Text = e.Message;
            }            
        }

        private void SavePlayList()
        {
            //this function is triggered by save playlist button
            string plpath = dir + "pl-" + comboBox1.Text + ".txt";
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Error", "Please Enter a Name For Your PlayList!");
            }
            else
            {
                try
                {
                    File.Delete(plpath);
                    if (dataGridView1.Rows.Count >= 2)
                    {
                        List<string> playlist = new List<string> { };
                        using (StreamWriter sr = File.CreateText(plpath))
                        {
                            foreach (DataGridViewRow k in dataGridView1.Rows)
                            {
                                if (dataGridView1.Rows[k.Index].Cells[0].Value.ToString() != "")
                                {
                                    sr.WriteLine("#S#" + dataGridView1.Rows[k.Index].Cells[0].Value.ToString() + "#E#" + Environment.NewLine);
                                }
                            }
                        }
                        textBox1.Text = "Your PlayList Have Been Saved Successfully!";
                        import_pls();
                        //int ComboNewName = 0;
                        //if (comboBox1.Items.Count == 0)
                        //{
                        //    MessageBox.Show("null combo");
                        //    comboBox1.Items.Add(comboBox1.Text);
                        //}
                        //else
                        //{
                        //    foreach (string co in comboBox1.Items)
                        //    {
                        //        if (co.ToString() == comboBox1.Text)
                        //        {
                        //            ComboNewName++;
                        //        }
                        //    }
                        //    if (ComboNewName == 0)
                        //    {
                        //        MessageBox.Show("Added first pl");
                        //        comboBox1.Items.Add(comboBox1.Text);
                        //    }
                        //}
                    }
                    else
                    {
                        textBox1.Text = "The queue is empty.";
                    }
                }
                catch(Exception e)
                {
                    textBox1.Text = e.Message;
                }
            }
        }

        private void play_resume(double position)
        {
            wmplayer.controls.play();
            wmplayer.controls.currentPosition = position;
            textBox1.Text = "Playing " + wmplayer.currentMedia.name;
            button2.BackgroundImage = Properties.Resources.pause;
            TagLib.File musicinfo = TagLib.File.Create(fileToOpen);            
            textBox2.Text = musicinfo.Tag.FirstAlbumArtist;
            textBox3.Text = musicinfo.Tag.Album;
            textBox5.Text = musicinfo.Tag.Title;            
            Int32 minute = Convert.ToInt32(wmplayer.currentMedia.duration) / 60;
            Int32 second = Convert.ToInt32(wmplayer.currentMedia.duration) % 60;
            label8.Text = minute.ToString() + ":" + second.ToString();
            button2.BackgroundImage = Properties.Resources.pause;
            try
            {
                var bin = (byte[])(musicinfo.Tag.Pictures[0].Data.Data);
                pictureBox1.Image = Image.FromStream(new MemoryStream(bin));
            }
            catch (Exception)
            {
                pictureBox1.Image = Properties.Resources.mainpic;
                timer1.Start();
            }
            timer2.Start();
            timer3.Start();
        }

        private void pause()
        {
            wmplayer.controls.pause();
            timer3.Stop();
            textBox1.Text = "Paused  ||";
            button2.BackgroundImage = Properties.Resources.play;
            timer1.Stop();
        }

        private void stop()
        {
            timer3.Stop();
            wmplayer.controls.stop();
            timer3.Stop();
            button2.BackgroundImage = Properties.Resources.play;
        }

        private string GetPreviousChoice()
        {
            foreach (DataGridViewRow k in dataGridView1.Rows)
            {
                if (dataGridView1.Rows[k.Index].Cells[0].Value.ToString() == fileToOpen)
                {
                    if (k.Index == 0)
                    {
                        return dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[0].Value.ToString();
                    }
                    if (k.Index > 0)
                    {
                        return dataGridView1.Rows[k.Index - 1].Cells[0].Value.ToString();
                    }
                    else
                    {
                        return dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Value.ToString();
                    }
                }
            }
            return dataGridView1.Rows[0].Cells[0].Value.ToString(); //this should never be reached
        }

        private string GetNextChoice()
        {
            foreach (DataGridViewRow k in dataGridView1.Rows)
            {
                if (dataGridView1.Rows[k.Index].Cells[0].Value.ToString() == fileToOpen)
                {
                    if (k.Index == dataGridView1.Rows.Count - 2)
                    {
                        return dataGridView1.Rows[0].Cells[0].Value.ToString();
                    }
                    else
                    {
                        return dataGridView1.Rows[k.Index + 1].Cells[0].Value.ToString();
                    }
                }
            }
            return dataGridView1.Rows[0].Cells[0].Value.ToString(); //this should never be reached
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count < 2)
            {                
                MessageBox.Show("The Number Of Songs Is Less Than Two!");
            }
            else
            {
                fileToOpen = GetPreviousChoice();
                wmplayer.URL = fileToOpen;
                play_resume(0);
                foreach (DataGridViewRow k in dataGridView1.Rows)
                {
                    if (dataGridView1.Rows[k.Index].Cells[0].Value.ToString() == fileToOpen)
                    {
                        dataGridView1.Rows[k.Index].Cells[0].Selected = true;
                    }
                    else
                        dataGridView1.Rows[k.Index].Cells[0].Selected = false;
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {            
            position = wmplayer.controls.currentPosition;
            if (wmplayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                pause();
            }
            else if (wmplayer.playState == WMPLib.WMPPlayState.wmppsPaused || wmplayer.playState == WMPLib.WMPPlayState.wmppsStopped || wmplayer.playState == WMPLib.WMPPlayState.wmppsReady)
            {
                play_resume(position);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count < 2)
            {
                MessageBox.Show("The Number Of Songs Is Less Than Two!");
            }
            else
            {
                if (ShuffleMode == true)
                {
                    int songindex = 0;
                    foreach (DataGridViewRow n in dataGridView1.Rows)
                    {
                        if (fileToOpen == dataGridView1.Rows[n.Index].Cells[0].Value.ToString())
                        {
                            songindex = n.Index;
                        }
                    }

                    Random i = new Random();
                    int choice = i.Next(0, dataGridView1.Rows.Count - 1);
                    while (choice == songindex)
                    {
                        choice = i.Next(0, dataGridView1.Rows.Count - 1);
                    }

                    fileToOpen = dataGridView1.Rows[choice].Cells[0].Value.ToString();
                    dataGridView1.Rows[choice].Cells[0].Selected = true;
                    wmplayer.URL = fileToOpen;
                    play_resume(0);
                    foreach (DataGridViewRow k in dataGridView1.Rows)
                    {
                        if (dataGridView1.Rows[k.Index].Cells[0].Value.ToString() == fileToOpen)
                        {
                            dataGridView1.Rows[k.Index].Cells[0].Selected = true;
                        }
                        else
                            dataGridView1.Rows[k.Index].Cells[0].Selected = false;
                    }
                }
                else
                {
                    fileToOpen = GetNextChoice();
                    wmplayer.URL = fileToOpen;
                    play_resume(0);
                    foreach (DataGridViewRow k in dataGridView1.Rows)
                    {
                        if (dataGridView1.Rows[k.Index].Cells[0].Value.ToString() == fileToOpen)
                        {
                            dataGridView1.Rows[k.Index].Cells[0].Selected = true;
                        }
                        else
                            dataGridView1.Rows[k.Index].Cells[0].Selected = false;
                    }
                }

            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            stop();
        }
        private void Button5_Click(object sender, EventArgs e)
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            FD.Filter = "Music (*.mp3;*.wav;*.m4a)|*.mp3;*.wav;*.m4a|All Files(.)|*.*";
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileToOpen = FD.FileName;
                if (fileToOpen != null)
                {
                    int cont = 0;
                    foreach (DataGridViewRow k in dataGridView1.Rows)
                    {
                        if (dataGridView1.Rows[k.Index].Cells[0].Value != null)
                        {
                            if (fileToOpen == dataGridView1.Rows[k.Index].Cells[0].Value.ToString())
                            {
                                DialogResult result = MessageBox.Show("This Song Has Been Exist In PlayList Do You Wanna Add It Any Way (This Will Cause Some Problems In Feature!) ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                                if (result == DialogResult.Yes)
                                    break;
                                cont++;
                            }
                        }
                    }
                    if (cont == 0)
                    {
                        wmplayer.URL = fileToOpen;
                        play_resume(0.0);

                        int rownum = dataGridView1.Rows.Count;
                        if (rownum == 1 && dataGridView1.Rows[0].Cells[0].Value ==null)
                        {
                            dataGridView1.Rows[0].Cells[0].Value = fileToOpen;
                        }
                        else
                        {
                            int index = dataGridView1.Rows.Add();
                            dataGridView1.Rows[index].Cells[0].Value = fileToOpen;
                        }                        
                        foreach (DataGridViewRow k in dataGridView1.Rows)
                        {
                            if (dataGridView1.Rows[k.Index].Cells[0].Value != null)
                            {
                                if (dataGridView1.Rows[k.Index].Cells[0].Value.ToString() == fileToOpen)
                                {
                                    dataGridView1.Rows[k.Index].Cells[0].Selected = true;
                                }
                                else
                                    dataGridView1.Rows[k.Index].Cells[0].Selected = false;
                            }
                        }
                    }                   
                }
                else
                {
                    textBox1.Text = "Your file cannot be played!";
                }
            }
        }
        private void Button6_Click(object sender, EventArgs e)
        {
            if (RepeatMode == "off")
            {
                RepeatMode = "on";
                wmplayer.settings.setMode("Loop", true);
                button6.BackColor = Color.WhiteSmoke;
                button6.BackgroundImage = Properties.Resources.repeat;                
            }
            else
            if (RepeatMode == "on")
            {
                if (dataGridView1.Rows.Count > 2)
                {
                    RepeatMode = "PlayList";
                    wmplayer.settings.setMode("Loop", false);
                    button6.BackColor = Color.WhiteSmoke;
                    button6.BackgroundImage = Properties.Resources.repeat_playlist;
                }
                else
                {
                    RepeatMode = "off";
                    wmplayer.settings.setMode("Loop", false);
                    button6.BackColor = Color.Transparent;
                    button6.BackgroundImage = Properties.Resources.repeat_playlist;
                }
            }
            else
            if (RepeatMode == "PlayList")
            {
                RepeatMode = "off";
                wmplayer.settings.setMode("Loop", false);
                button6.BackColor = Color.Transparent;
                button6.BackgroundImage = Properties.Resources.repeat_playlist;
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (ShuffleMode == false)
            {
                ShuffleMode = true;
                button7.BackColor = Color.WhiteSmoke;
            }
            else
            {
                ShuffleMode = false;
                button7.BackColor = Color.Transparent;
            }
        }

        private void PictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
            fileToOpen = Convert.ToString(file);
            play_resume(0);
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (wmplayer.settings.mute == true)
            {
                if (trackBar1.Value > 0)
                {
                    wmplayer.settings.mute = false;
                }
            }
            int tbvalue = trackBar1.Value;
            wmplayer.settings.volume = trackBar1.Value;
            if (tbvalue <= 100 && tbvalue > 60)
            {
                button8.BackgroundImage = Properties.Resources.volume_100;
            }
            else
            if (tbvalue <= 60 && tbvalue > 30)
            {
                button8.BackgroundImage = Properties.Resources.volume_60;
            }
            else
            if (tbvalue <= 30 && tbvalue > 0)
            {
                button8.BackgroundImage = Properties.Resources.volume_30;
            }
            else
            if (tbvalue == 0)
            {
                button8.BackgroundImage = Properties.Resources.mute;
            }
            Properties.Settings.Default.volume = trackBar1.Value;
            Properties.Settings.Default.Save();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            int tbvalue = trackBar1.Value;
            if (tbvalue > 0)
            {
                volbeformute = trackBar1.Value;
                button8.BackgroundImage = Properties.Resources.mute;
                wmplayer.settings.mute = true;
                trackBar1.Value = 0;
            }
            if (tbvalue == 0)
            {
                button8.BackgroundImage = Properties.Resources.volume_30;
                wmplayer.settings.mute = false;
                wmplayer.settings.volume = volbeformute;
                trackBar1.Value = volbeformute;
                volbeformute = 0;
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                fileToOpen = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                wmplayer.URL = fileToOpen;
                play_resume(0.0);
                foreach (DataGridViewRow k in dataGridView1.Rows)
                {
                    if (dataGridView1.Rows[k.Index].Cells[0].Value.ToString() == fileToOpen)
                    {
                        dataGridView1.Rows[k.Index].Cells[0].Selected = true;
                    }
                    else
                    {
                        dataGridView1.Rows[k.Index].Cells[0].Selected = false;
                    }                        
                }
            }
            catch (Exception) { }
        }

        private void Button9_Click(object sender, EventArgs e)
        {           
            if (comboBox1.Text == "")
            {
                MessageBox.Show("Please Enter a Name For Your PlayList!", "Error");
            }
            else
            {
                DialogResult res = MessageBox.Show("Are Sure To Save The " + comboBox1.Text + " ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    SavePlayList();
                }
            }
        }

        private void Button9_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Save PlayList", button9);
        }

        private void Button8_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Mute/Unmute", button8);
        }

        private void Button7_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Shuffle", button7);
        }

        private void Button3_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Next Track", button3);
        }

        private void Button4_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Stop", button4);
        }

        private void Button2_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Play/Pause", button2);
        }

        private void Button1_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Previous Track", button1);
        }

        private void Button6_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Repeat/Repeat PlayList", button6);
        }

        private void TrackBar1_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Volume", trackBar1);
        }

        private void Label6_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Minimize", label4);
        }

        private void TrackBar2_MouseHover(object sender, EventArgs e)
        {

            toolTip1.Show(trackBar2.Value.ToString(), trackBar2);
        }

        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            timer2.Stop();
            Int32 SongDuration = Convert.ToInt32(wmplayer.currentMedia.duration);
            Int32 val = (trackBar2.Value * SongDuration) / 1000;
            wmplayer.controls.currentPosition = val;
            timer2.Start();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {            
            string plpath = dir + "pl-" + comboBox1.Text + ".txt";
            int startin = 0, endind = 0,rowind=0;
            if (File.Exists(plpath))
            {
                do
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        try
                        {
                            dataGridView1.Rows.Remove(row);
                        }
                        catch (Exception) { }
                    }
                } while (dataGridView1.Rows.Count > 1);

                dataGridView1.DataSource = null;
                using (StreamReader sr = File.OpenText(plpath))
                {
                    string pltext = File.ReadAllText(plpath);
                    for (; ; )
                    {
                        if (pltext.Contains("#S#") && pltext.Contains("#E#"))
                        {
                            startin = pltext.IndexOf("#S#") + 3;
                            endind = pltext.IndexOf("#E#");
                            string songurl = pltext.Substring(startin, endind - startin);

                            rowind = dataGridView1.Rows.Add();
                            dataGridView1.Rows[rowind].Cells[0].Value = songurl;
                            pltext = pltext.Remove(startin-3, endind + 4);
                        }
                        else
                        {
                            break;
                        }
                    }
                    textBox1.Text = "Enjoy The " + comboBox1.Text + " PlayList";
                }
            }
            else
            {
                MessageBox.Show("The PlayList Is Lost!","Error");
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            degree += 2;
            RotateImageClass rot = new RotateImageClass();
            TagLib.File musicinfo = TagLib.File.Create(fileToOpen);
            try
            {
                var bin = (byte[])(musicinfo.Tag.Pictures[0].Data.Data);
                pictureBox1.Image = Image.FromStream(new MemoryStream(bin));
            }
            catch (Exception)
            {
                pictureBox1.Image = rot.RotateImage(Properties.Resources.mainpic, degree);
            }

            if (wmplayer.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                Button4_Click(sender, e);
                timer1.Stop();
            }
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            Int32 SongDuration = Convert.ToInt32(wmplayer.currentMedia.duration);
            Int32 percent = ((Convert.ToInt32(wmplayer.controls.currentPosition)) * 1000) / (SongDuration + 1);
            trackBar2.Value = percent;
            Int32 minute = Convert.ToInt32(wmplayer.controls.currentPosition) / 60;
            Int32 second = Convert.ToInt32(wmplayer.controls.currentPosition) % 60;
            label7.Text = minute.ToString() + ":" + second.ToString();
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            if (wmplayer.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                if (ShuffleMode == true)
                {
                    int songindex = 0;
                    foreach (DataGridViewRow n in dataGridView1.Rows)
                    {
                        if (fileToOpen == dataGridView1.Rows[n.Index].Cells[0].Value.ToString())
                        {
                            songindex = n.Index;
                        }
                    }

                    Random i = new Random();
                    int choice = i.Next(0, dataGridView1.Rows.Count - 1);
                    while (choice == songindex)
                    {
                        choice = i.Next(0, dataGridView1.Rows.Count - 1);
                    }

                    fileToOpen = dataGridView1.Rows[choice].Cells[0].Value.ToString();
                    dataGridView1.Rows[choice].Cells[0].Selected = true;
                    wmplayer.URL = fileToOpen;
                    play_resume(0);
                }
                else
                {
                    if (fileToOpen != dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[0].Value.ToString())
                    {
                        int songindex = 0;
                        foreach (DataGridViewRow n in dataGridView1.Rows)
                        {
                            if (fileToOpen == dataGridView1.Rows[n.Index].Cells[0].Value.ToString())
                            {
                                songindex = n.Index;
                            }
                        }
                        foreach (DataGridViewRow k in dataGridView1.Rows)
                        {
                            dataGridView1.Rows[k.Index].Cells[0].Selected = false;
                        }
                        fileToOpen = dataGridView1.Rows[songindex + 1].Cells[0].Value.ToString();
                        dataGridView1.Rows[songindex + 1].Cells[0].Selected = true;
                        wmplayer.URL = fileToOpen;
                        play_resume(0);
                    }
                    if (RepeatMode == "PlayList")
                    {
                        if (fileToOpen == dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[0].Value.ToString())
                        {
                            foreach (DataGridViewRow k in dataGridView1.Rows)
                            {
                                dataGridView1.Rows[k.Index].Cells[0].Selected = false;
                            }
                            dataGridView1.Rows[0].Cells[0].Selected = true;
                            fileToOpen = dataGridView1.Rows[0].Cells[0].Value.ToString();
                            wmplayer.URL = fileToOpen;
                            play_resume(0);
                        }
                    }
                }
            }
        }

        private void PictureBox1_DragLeave(object sender, EventArgs e)
        {            
            fileToOpen = Convert.ToString(DataFormats.FileDrop);
            play_resume(0);
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            SettingForm obj = new SettingForm();
            obj.Show();
            this.WindowState = FormWindowState.Minimized;
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "";
            do
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    try
                    {
                        dataGridView1.Rows.Remove(row);
                    }
                    catch (Exception) { }
                }
            }
            while (dataGridView1.Rows.Count > 1);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            wmplayer.controls.stop();
            Environment.Exit(0);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)

        {

            KeyEventArgs e = new KeyEventArgs(keyData);
            if (e.Control && e.KeyCode == Keys.MediaNextTrack)
            {

                Button3_Click(msg, e);

            }

            return base.ProcessCmdKey(ref msg, keyData);

        }
    }
}
