
using System;
using System.IO;
using System.Windows.Forms;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using System.Drawing;
using System.Xml;

namespace OBSTallyClient
{
    
    public partial class MainProgram : Form
    {
        OBSWebsocket mainWebsocket = new OBSWebsocket();

        public string source1;
        public string source2;
        public string source3;
        public string source4;
        public string wsPassword;

        public Label newLabel;
        public Label prevLabel;
        public Label oldLabel;
        public Label oldprevLabel;

        public MainProgram()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // Initialize labels
            newLabel = label5; //debug label
            prevLabel = label5; //debug label

            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(Application.StartupPath + "\\config.xml");

                XmlNode comSetup = xmlDoc.SelectSingleNode("root/Setup");
                if (comSetup.Attributes["state"].Value != "completed")
                {
                    setupPopup setItUp = new setupPopup();
                    setItUp.ShowDialog();
                }

                XmlNode first = xmlDoc.SelectSingleNode("root/Source1");
                source1 = first.Attributes["name"].Value;
                XmlNode second = xmlDoc.SelectSingleNode("root/Source2");
                source2 = second.Attributes["name"].Value;
                XmlNode third = xmlDoc.SelectSingleNode("root/Source3");
                source3 = third.Attributes["name"].Value;
                XmlNode fourth = xmlDoc.SelectSingleNode("root/Source4");
                source4 = fourth.Attributes["name"].Value;
                XmlNode wesPass = xmlDoc.SelectSingleNode("root/Websocket");
                wsPassword = wesPass.Attributes["password"].Value;

            }
            catch
            {
                MessageBox.Show("Unable to load configuration file. Please run the setup.", "FAILED TO LOAD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            try
            {
                mainWebsocket.Connect("ws://127.0.0.1:4444", wsPassword);
                label1.BackColor = Color.Gray;
                label2.BackColor = Color.Gray;
                label3.BackColor = Color.Gray;
                label4.BackColor = Color.Gray;
            }
            catch
            {
                MessageBox.Show("Failed to connect with OBS");
            }
        }
        
        private void timer1_Tick(object sender, EventArgs e) // Loops continuously 100ms
        {
            try //try even if serial port is closed
            {
                // Get current and preview scene names
                string currentScene = mainWebsocket.GetCurrentScene().Name;
                string previewScene = mainWebsocket.GetPreviewScene().Name;

                // List all sources in active scene
                var currentSceneSources = mainWebsocket.GetCurrentScene().Items;
                foreach (var item in currentSceneSources)
                {
                    //Console.WriteLine(item.SourceName);

                }

                // List all sources in preview scene
                var previewSceneSources = mainWebsocket.GetPreviewScene().Items;
                foreach (var item in previewSceneSources)
                {
                    //Console.WriteLine(item.SourceName);

                }
                
                //// ACTIVE SCENES ////
                if (currentScene == source1)
                {
                    oldLabel = newLabel;
                    newLabel = label1;
                    serialPort1.Write("0\r\n");
                    label5.Text = serialPort1.PortName;
                }
                else if (currentScene == source2)
                {
                    oldLabel = newLabel;
                    newLabel = label2;
                    serialPort1.Write("1\r\n");
                    label5.Text = serialPort1.PortName;
                }
                else if (currentScene == source3)
                {
                    oldLabel = newLabel;
                    newLabel = label3;
                    serialPort1.Write("2\r\n");
                    label5.Text = serialPort1.PortName;
                }
                else if (currentScene == source4)
                {
                    oldLabel = newLabel;
                    newLabel = label4;
                    serialPort1.Write("3\r\n");
                    label5.Text = serialPort1.PortName;
                }
                else
                {
                    oldLabel = newLabel;
                    newLabel = label5;
                    serialPort1.Write("4\r\n");
                    //label5.Text = "Bad scene name!";
                }

                //// PREVIEW SCENES ////
                if (previewScene == source1)
                {
                    oldprevLabel = prevLabel;
                    prevLabel = label1;
                    serialPort1.Write("5\r\n");
                    label5.Text = serialPort1.PortName;
                }
                else if (previewScene == source2)
                {
                    oldprevLabel = prevLabel;
                    prevLabel = label2;
                    serialPort1.Write("6\r\n");
                    label5.Text = serialPort1.PortName;
                }
                else if (previewScene == source3)
                {
                    oldprevLabel = prevLabel;
                    prevLabel = label3;
                    serialPort1.Write("7\r\n");
                    label5.Text = serialPort1.PortName;
                }
                else if (previewScene == source4)
                {
                    oldprevLabel = prevLabel;
                    prevLabel = label4;
                    serialPort1.Write("8\r\n");
                    label5.Text = serialPort1.PortName;
                }
                else
                {
                    oldprevLabel = prevLabel;
                    prevLabel = label5;
                    serialPort1.Write("9\r\n");
                    //label5.Text = "Bad preview name!";
                }

                oldprevLabel.BackColor = Color.Gray;
                oldLabel.BackColor = Color.Gray;
                prevLabel.BackColor = Color.Green;
                newLabel.BackColor = Color.Red;
            }
            catch
            {
                // Nothing here so it tries even if Arduino is disconnected
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.Close();
            serialPort1.PortName = comboBox1.Text;
            label5.Text = serialPort1.PortName;
            if (serialPort1 != null)
            {
                try
                {
                    serialPort1.Open();
                }
                catch
                {
                    MessageBox.Show("Could not establish serial connection on " + comboBox1.Text, "Communication Error");
                }
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            setupPopup setItUp = new setupPopup();
            setItUp.ShowDialog();
            System.Diagnostics.Process.Start(Application.StartupPath + "\\OBSTallyClient.exe");
            this.Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void MainProgram_Resize(object sender, EventArgs e)
        {

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

    }
}
