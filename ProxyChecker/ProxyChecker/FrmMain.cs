using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Net;
using System.Text.RegularExpressions;

namespace ProxyChecker
{
    public partial class FrmMain : Form
    {
        double salise = 0;
        private int index = 0;
        int sayac = 0;
        private static volatile object obj = new object();

        public FrmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            CheckerEngine.Finish = threadFinishEvent;
        }

        private void threadFinishEvent()
        {
            lock (obj)
            {
                if (index < listView.Items.Count)
                {
                    var lvi = listView.Items[index++];
                    var th = new Thread(() =>
                    {
                        CheckerEngine.Start(lvi);
                    });
                    th.IsBackground = true;
                    th.Start();
                }
            }
        }
        private void AddLviItem(string proxy)
        {
            try
            {
                var sp = proxy.Split(':');
                var lvi = new ListViewItem();
                lvi.Text = sp[0];
                lvi.SubItems.Add(sp[1]);
                lvi.SubItems.Add("");
                listView.Items.Add(lvi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddLviItem(txtProxy.Text);
        }

        private void menuProxyListLoad_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();
            file.Multiselect = false;
            file.Filter = "Txt Dosyası |*.txt";
            file.Title = "Select txt";

            if (file.ShowDialog() == DialogResult.OK)
            {
                string[] rows = File.ReadAllLines(file.FileName, Encoding.Default);
                foreach (string row in rows)
                {
                    if (!string.IsNullOrWhiteSpace(row))
                    {
                        AddLviItem(row);
                    }
                }
            }
            file.Dispose();
        }

        private void mnuClear_Click(object sender, EventArgs e)
        {
            listView.Items.Clear();
            CheckerEngine.Finish();
        }

        private void clear()
        {
            foreach (ListViewItem lvi in listView.Items)
            {
                lvi.ForeColor = Color.Black;
                lvi.SubItems[2].Text = "";
            }
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            clear();
            int finishIndex = listView.Items.Count < 5 ? listView.Items.Count : 5;
            for (index = 0; index < finishIndex; index++)
            {
                var lvi = listView.Items[index];
                var th = new Thread(() =>
                {
                    CheckerEngine.Start(lvi);
                });
                th.IsBackground = true;
                th.Start();
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            var list = new List<string>();
            foreach (ListViewItem lvi in listView.Items)
            {
                if (lvi.ForeColor == Color.Green)
                {
                    list.Add(lvi.Text + ":" + lvi.SubItems[1].Text);
                }
            }
            File.WriteAllLines("result.txt", list, Encoding.Default);
            MessageBox.Show("Kaydedildi", "Ben");



            /*string[] rows = File.ReadAllLines(@"C:\Users\Selman\source\repos\ProxyChecker-master\ProxyChecker-master\ProxyChecker\bin\Debug\result.txt", Encoding.Default);
            foreach (string row in rows)
            {
                if (!string.IsNullOrWhiteSpace(row))
                {
                   listBox1.Items.Add(row);
                }
            }*/
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        /*private void mnuSend_Click(object sender, EventArgs e)
        {
            
        }*/

        private void mnuSend_Click_1(object sender, EventArgs e)
        {
           
           // var list = new List<string>();
            foreach (ListViewItem lvi in listView.Items)
            {
                if (lvi.ForeColor == Color.Green)
                {
                    listView.Items.Remove(lvi);
                    listView1.Items.Add(lvi);                   

                }
            }
            //File.WriteAllLines("result.txt", list, Encoding.Default);
            // MessageBox.Show("Kaydedildi", "Ben");
        }

        private void btnCek_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (txtSndeger.Text == "")
                {
                    listBox1.Items.Add(listView1.Items[i].SubItems[0].Text + ":" + listView1.Items[i].SubItems[1].Text);
                }
                else if (Double.Parse(listView1.Items[i].SubItems[2].Text) <= (Double.Parse(txtSndeger.Text)))
                {
                    listBox1.Items.Add(listView1.Items[i].SubItems[0].Text + ":" + listView1.Items[i].SubItems[1].Text);
                }                 
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            var list = new List<string>();
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                list.Add(listBox1.Items[i].ToString());
            }
            File.WriteAllLines("Ipler.txt", list, Encoding.Default);
            MessageBox.Show("Kaydedildi", "Ben");
        }

        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool settingsReturn, refreshReturn;

        private void btnBaglan_Click(object sender, EventArgs e)

        {
            txtIp.Text = listBox1.SelectedItem.ToString();
            Baglan();
        }

        public void Baglan()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyEnable", 1);
            registry.SetValue("ProxyServer", txtIp.Text);

            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to realy update
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BaglantıKes();
        }

        public void BaglantıKes()
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyEnable", 0);
            label4.Text = "BAĞLANTI KESİLDİ";
            label4.ForeColor=Color.Red;

        }

        private void btnIpGoster_Click(object sender, EventArgs e)
        {
            txtMyIp.Text = Modem();
        }

        string Modem()
        {
            try
            {
                var webClient = new WebClient();
                string dnsString = webClient.DownloadString("http://www.ipsorgu.com/");
                dnsString = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(dnsString).Value;
                webClient.Dispose();
                return dnsString;
            }
            catch
            {
                MessageBox.Show("İnternet Yavaş");
                return null;
            }


        }
        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (txtDakika.Text == "") { txtDakika.Text = "1"; }
                Random r = new Random();
                int index = r.Next(0, listBox1.Items.Count);
                txtIp.Text = listBox1.Items[index].ToString();
                Baglan();
                sayac++;
                label4.Text = sayac.ToString() + ". IP BAĞLANDI";
                label4.ForeColor= Color.Green;
                this.salise = (double.Parse(txtDakika.Text) * 600);
                timer1.Start();
            }
            else
            {
                timer1.Stop();

            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            salise--;

            if (salise == 0)
            {
                BaglantıKes();
            }
            else if (salise == -20)
            {
                timer1.Stop();
                Random r = new Random();
                int index = r.Next(0, listBox1.Items.Count);
                txtIp.Text = listBox1.Items[index].ToString();
                Baglan();
                sayac++;
                label4.Text = sayac.ToString() + ". IP BAĞLANDI";
                label4.ForeColor = Color.Green;
                this.salise = (double.Parse(txtDakika.Text) * 600);
                timer1.Start();
            }
        }     

    }
}
