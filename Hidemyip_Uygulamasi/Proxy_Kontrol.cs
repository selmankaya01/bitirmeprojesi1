using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics;
using System.IO;

namespace Hidemyip_Uygulamasi

{
    public partial class Proxy_Kontrol : Form
    {
        public Proxy_Kontrol()
        {
            InitializeComponent();
        }

        private static string FolderPath => string.Concat(Directory.GetCurrentDirectory(),
            "\\VPN");
        double salise = 0;
        

        [Obsolete]
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        string Modem()
        {
            try 
            { 
            var webClient = new WebClient();
            string dnsString = webClient.DownloadString("http://checkip.dyndns.org");
            dnsString = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(dnsString).Value;
            webClient.Dispose();
            return dnsString;
            }
            catch
            {
                MessageBox.Show("Ýnternet Yavaþ");
                return null ;
            }


        }

        public void Baglan()
        {
          
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            var sb = new StringBuilder();
            sb.AppendLine("[VPN]");
            sb.AppendLine("MEDIA=rastapi");
            sb.AppendLine("Port=VPN2-0");
            sb.AppendLine("Device=WAN Miniport (IKEv2)");
            sb.AppendLine("DEVICE=vpn");
            sb.AppendLine("PhoneNumber=" + textBox3.Text);

            File.WriteAllText(FolderPath + "\\VpnConnection.pbk", sb.ToString());

            sb = new StringBuilder();
            sb.AppendLine("rasdial \"VPN\" " + textBox4.Text + " " + textBox5.Text + " /phonebook:\"" + FolderPath +
                          "\\VpnConnection.pbk\"");

            File.WriteAllText(FolderPath + "\\VpnConnection.bat", sb.ToString());

            var newProcess = new Process
            {
                StartInfo =
                {
                    FileName = FolderPath + "\\VpnConnection.bat",
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };

            newProcess.Start();
            newProcess.WaitForExit();
            button1.Enabled = false;
            button2.Enabled = true;
            label7.Text = "BAÐLANDI.";
            label7.ForeColor = Color.Green;
        }

        public void BaglantýKes()
        {

            
            File.WriteAllText(FolderPath + "\\VpnDisconnect.bat", "rasdial /d");

            var newProcess = new Process
            {
                StartInfo =
                {
                    FileName = FolderPath + "\\VpnDisconnect.bat",
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };

            newProcess.Start();
            newProcess.WaitForExit();
            button1.Enabled = true;
            button2.Enabled = false;
            label7.Text = "BAÐLANTI KESÝLDÝ.";
            label7.ForeColor = Color.Red;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox3.Text = listBox1.SelectedItem.ToString();
            textBox4.Text = "vpnbook";
            textBox5.Text = "rxtasfh";
            Baglan();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            BaglantýKes();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string bilgisayarAdi = Dns.GetHostName();
            textBox1.Text = bilgisayarAdi;
            textBox2.Text = Modem();
        }       
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Random r = new Random();
                int index = r.Next(0, listBox1.Items.Count);
                textBox3.Text = listBox1.Items[index].ToString();
                textBox4.Text = "vpnbook";
                textBox5.Text = "rxtasfh";
                Baglan();
                this.salise = (double.Parse(textBox6.Text) * 600);
                timer1.Start(); 
            }
            else
            {
                timer1.Stop();

            }
        }
        
       
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            salise--;            

            if (salise == 0)
            {               
                BaglantýKes();
            }
            else if (salise == -100)
            {
                timer1.Stop();
                Random r = new Random();
                int index = r.Next(0, listBox1.Items.Count);
                textBox3.Text = listBox1.Items[index].ToString();
                textBox4.Text = "vpnbook";
                textBox5.Text = "rxtasfh";
                Baglan();
                this.salise = (double.Parse(textBox6.Text) * 600);
                timer1.Start();
            }
        }
    }
}