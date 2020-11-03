
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using OBSWebsocketDotNet;
using System.Drawing;
using System.Xml;
using System.Linq;

namespace OBSTallyClient
{
    public partial class MainProgram : Form
    {
        OBSWebsocket mainWebsocket = new OBSWebsocket();

        // Public variables
        public string source1;
        public string source2;
        public string source3;
        public string source4;
        public string wsPassword;

        public int button2_ClickCount = 1;
        public int lastbutton2State = 1;
        public bool exitFlag = false;

        public string lastLiveScene;
        public string lastPreviewScene;
        public List<OBSWebsocketDotNet.Types.SceneItem> LiveSceneSources = new List<OBSWebsocketDotNet.Types.SceneItem>();
        public List<OBSWebsocketDotNet.Types.SceneItem> PreviewSceneSources = new List<OBSWebsocketDotNet.Types.SceneItem>();

        public MainProgram()
        {
            InitializeComponent();
        }

        // On Load
        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                loadConfigXML(); // Load the XML file, catch if it doesn't exist
                exitFlag = true;
            }
            catch (FileNotFoundException ex1) //if Config doesn't exist, show setupPopup
            {
                setupPopup setItUp = new setupPopup();
                var setupResult = setItUp.ShowDialog();
                if (setItUp.exitFlag == true)
                {
                    loadConfigXML();
                    exitFlag = true;
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show("An unspecified error occured.", "FAILED TO LOAD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            try
            {
                //TO DO: set a websocket timeout: https://stackoverflow.com/questions/13546424/how-to-wait-for-a-websockets-readystate-to-change
                mainWebsocket.Connect("ws://127.0.0.1:4444", wsPassword);
                lastLiveScene = mainWebsocket.GetCurrentScene().Name; //Initialize lastLiveScene at the current live scene
                GrayAllLabels(); //Gray out all labels
            }
            catch (OBSWebsocketDotNet.AuthFailureException)
            {
                MessageBox.Show("Failed to connect with OBS: Websocket authentication has failed. Please run the setup again and verify the password is correct.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                MessageBox.Show("Failed to connect with OBS. Is OBS open?");
            }
        }

        // Main loop //
        private void timer1_Tick(object sender, EventArgs e) // Loops continuously 100ms
        {
            
            try //try even if serial port is closed
            {
                // Prevent running of websockets if config file doesn't exist.
                // Otherwise these values initialize as nulls, and everything crashes.
                if (exitFlag == true)
                {

                    ////////////// LIVE //////////////
                    // Get current live scene name
                    string currentLiveScene = mainWebsocket.GetCurrentScene().Name;

                    // List all sources in current live scene
                    LiveSceneSources = mainWebsocket.GetCurrentScene().Items;
                    
                    // Label colors (LIVE)
                    if (currentLiveScene != lastLiveScene) //If live scene state changes
                    {
                        Console.WriteLine("Live state has changed"); //Debugging
                        GrayAllLabels(); //Gray out all labels
                        RefreshLabels(LiveSceneSources, Color.Red); //Refresh all live labels
                        lastLiveScene = currentLiveScene; //Update scene state
                    }

                    // Serial writes (LIVE)
                    foreach (var live in LiveSceneSources)
                    {
                        //Console.WriteLine(live.SourceName);
                        //// ACTIVE SCENES ////
                        if (live.SourceName == source1) { serialPort1.Write("0\r\n"); }
                        else if (live.SourceName == source2) { serialPort1.Write("1\r\n"); }
                        else if (live.SourceName == source3) { serialPort1.Write("2\r\n"); }
                        else if (live.SourceName == source4) {  serialPort1.Write("3\r\n"); }
                        else { serialPort1.Write("4\r\n"); }
                    }

                    ////////////// PREVIEW //////////////
                    // If previews are turned ON
                    if (button2.Text == "Previews ON")
                    {
                        // Update previews after prieview on/off is toggled
                        if (lastbutton2State != button2_ClickCount)
                        {
                            Console.WriteLine("Previews on."); //Debugging
                            RefreshLabels(PreviewSceneSources, Color.Green); //Refresh preview labels
                            lastbutton2State = button2_ClickCount; //Update lastbutton2State
                        }

                        // Get current preview scene name
                        string currentPreviewScene = mainWebsocket.GetPreviewScene().Name;

                        // List all sources in current preview scene
                        PreviewSceneSources = mainWebsocket.GetPreviewScene().Items;

                        // Label colors (PREVIEW)
                        if (currentPreviewScene != lastPreviewScene) //If preview scene state changes
                        {
                            Console.WriteLine("Preview state has changed"); //Debugging
                            GrayAllLabels(); //Gray out all labels
                            RefreshLabels(PreviewSceneSources, Color.Green); //Set label colors for preview sources
                            RefreshLabels(LiveSceneSources, Color.Red); //Set label colors for live sources
                            lastPreviewScene = currentPreviewScene; //Update preview scene state
                        }

                        // Serial writes (PREVIEW)
                        foreach (var preview in PreviewSceneSources)
                        {
                            //Console.WriteLine(preview.SourceName);
                            //// PREVIEW SCENES ////
                            if (preview.SourceName == source1) { serialPort1.Write("5\r\n"); }
                            else if (preview.SourceName == source2) { serialPort1.Write("6\r\n"); }
                            else if (preview.SourceName == source3) { serialPort1.Write("7\r\n"); }
                            else if (preview.SourceName == source4) { serialPort1.Write("8\r\n"); }
                            else { serialPort1.Write("9\r\n"); }
                        }

                    }//end if previews ON check
                    else
                    {
                        if (lastbutton2State != button2_ClickCount)
                        {
                            Console.WriteLine("Previews off."); //Debugging
                            GrayAllLabels(); //Gray out all labels
                            RefreshLabels(LiveSceneSources, Color.Red); //Refresh live labels
                            lastbutton2State = button2_ClickCount; //Update lastbutton2State
                        }
                    }//end else previews ON check

                }//end exitFlag check
                
            }
            catch (System.InvalidOperationException)
            {
                Console.WriteLine("No serial connection established.");
            }
            catch
            {

            }

        }

        private void RefreshLabels(List<OBSWebsocketDotNet.Types.SceneItem> SceneSources, Color color)
        {
            //Update labels
            foreach (var live in SceneSources)
            {
                if (live.SourceName == source1) { label1.BackColor = color; }
                else if (live.SourceName == source2) { label2.BackColor = color; }
                else if (live.SourceName == source3) { label3.BackColor = color; }
                else if (live.SourceName == source4) { label4.BackColor = color; }
                else { label5.BackColor = color; }
            }
        }

        private void GrayAllLabels()
        {
            //Gray out all
            label1.BackColor = Color.Gray;
            label2.BackColor = Color.Gray;
            label3.BackColor = Color.Gray;
            label4.BackColor = Color.Gray;
            label5.BackColor = Color.Gray;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.Close();
            serialPort1.PortName = comboBox1.Text;
            if (serialPort1 != null)
            {
                try
                {
                    serialPort1.Open();
                    label5.Text = serialPort1.PortName;
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2_ClickCount % 2 == 0) //If button2_ClickCount is even
            {
                button2.Text = "Previews ON";
            }
            else                             //If button2_ClickCount is odd
            {
                button2.Text = "Previews OFF";
            }
            button2_ClickCount = button2_ClickCount + 1;
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

        private void loadConfigXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Application.StartupPath + "\\config.xml");
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

    }
}
