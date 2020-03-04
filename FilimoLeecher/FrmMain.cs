using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilimoLeecher
{
    public partial class FrmMain : Form
    {
        public String SavePath { get; set; }
        public List<String> Usernames = new List<string>();
        public FrmMain()
        {
            InitializeComponent();
        }
        private void AppendUsers(List<String> users)
        {
            foreach (var i in users)
            {
                if (!this.Usernames.Exists(c => c == i))
                {
                    Usernames.Add(i);
                    Invoke((MethodInvoker)delegate { listView1.Items.Add(new ListViewItem(new String[] { i })); });
                }
            }

            File.AppendAllLines(SavePath, users);
        }
        private void Btn_browse_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog() { Filter = "TXT Files (*.txt)|*.txt" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                txt_savePath.Text = sfd.FileName;
                this.SavePath = sfd.FileName;
            }
        }

        private void Btn_execute_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txt_url.Text))
            {
                MessageBox.Show("Warning", "Please enter url!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            lbl_status.ForeColor = Color.Black;
            lbl_status.Text = "Status : None";
            Usernames.Clear();

            btn_browse.Enabled = false;
            btn_execute.Enabled = false;

            txt_savePath.Enabled = false;
            txt_url.Enabled = false;

            lbl_status.Text = "Status : Working ...";
            lbl_status.ForeColor = Color.Orange;

            Leecher l = new Leecher(txt_url.Text, this.SavePath == null || this.SavePath == "" ? Application.StartupPath + "\\Result.txt" : this.SavePath);
            l.LeechedEvent += L_LeechedEvent;
            l.StartLeech();
        }

        private void L_LeechedEvent(List<string> data)
        {
            if (data.Count > 0)
            {
                AppendUsers(data);

                Invoke((MethodInvoker)delegate
                {
                    btn_browse.Enabled = true;
                    btn_execute.Enabled = true;

                    txt_savePath.Enabled = true;
                    txt_url.Enabled = true;

                    lbl_status.ForeColor = Color.Green;
                    lbl_status.Text = "Status : Leeched " + data.Count + " ; From " + txt_url.Text;
                });
            }
            else
            {
                Invoke((MethodInvoker)delegate
                {
                    btn_browse.Enabled = true;
                    btn_execute.Enabled = true;

                    txt_savePath.Enabled = true;
                    txt_url.Enabled = true;

                    lbl_status.ForeColor = Color.Red;
                    lbl_status.Text = "Status : Failed to leech, count is 0";
                });
            }
        }
    }
}
