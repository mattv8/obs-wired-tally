﻿
using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.Windows.Forms;
using OBSWebsocketDotNet;
using System.Drawing;
using System.Xml;
//using System.Threading;

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
        public bool messageShown = false;

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
            catch
            {
                
            }

            try
            {
                mainWebsocket.Connect("ws://127.0.0.1:4444", wsPassword);
                if (mainWebsocket.IsConnected)
                {
                    lastLiveScene = mainWebsocket.GetCurrentScene().Name; //Initialize lastLiveScene at the current live scene
                    RefreshLabels(PreviewSceneSources, Color.Green); //Set label colors for preview sources
                    RefreshLabels(LiveSceneSources, Color.Red); //Set label colors for live sources
                }
            }
            catch (OBSWebsocketDotNet.AuthFailureException)
            {
                MessageBox.Show("Failed to connect with OBS: Websocket authentication has failed." +
                    " Please run the setup again and verify the password is correct.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                
            }
        }

        // Main loop //
        private void MainLoop(object sender, EventArgs e) // Loops continuously 100ms
        {
            
            try //try even if serial port is closed
            {

                // Prevent running of websockets if config file doesn't exist.
                // Otherwise these values initialize as nulls, and everything crashes.
                if (exitFlag == true && mainWebsocket.IsConnected && serialPort1 != null)
                {

                    ////////////// Update Websocket Variables //////////////
                    string currentLiveScene = mainWebsocket.GetCurrentScene().Name; // Get current live scene name
                    LiveSceneSources = mainWebsocket.GetCurrentScene().Items; // List all sources in current live scene
                    string currentPreviewScene = mainWebsocket.GetPreviewScene().Name; // Get current preview scene name
                    PreviewSceneSources = mainWebsocket.GetPreviewScene().Items; // List all sources in current preview scene

                    ////////////// LIVE //////////////
                    if (currentLiveScene != lastLiveScene) //If live scene state changes
                    {
                        //Console.WriteLine("Live state has changed"); //Debugging
                        // Update live label colors for UI app
                        ColorAllLabels(Color.Gray); //Gray out all labels
                        RefreshLabels(LiveSceneSources, Color.Red); //Refresh all live labels

                        // Send serial bits for live to Arduino
                        foreach (var live in LiveSceneSources)
                        {
                            if (live.SourceName == source1) { serialPort1.Write("0\r\n"); }
                            else if (live.SourceName == source2) { serialPort1.Write("1\r\n"); }
                            else if (live.SourceName == source3) { serialPort1.Write("2\r\n"); }
                            else if (live.SourceName == source4) { serialPort1.Write("3\r\n"); }
                            else { serialPort1.Write("4\r\n"); }
                        }
                        lastLiveScene = currentLiveScene; // Finally, update scene state
                    }

                    ////////////// PREVIEWS //////////////
                    if (button2.Text == "Previews ON") // If previews are turned ON
                    {
                        // Update previews after prieview on/off is toggled
                        if (lastbutton2State != button2_ClickCount)
                        {
                            //Console.WriteLine("Previews on."); //Debugging
                            RefreshLabels(PreviewSceneSources, Color.Green); //Refresh preview labels
                            lastbutton2State = button2_ClickCount; //Update lastbutton2State
                        }

                        // Update preview label colors for UI app
                        if (currentPreviewScene != lastPreviewScene) //If preview scene state changes
                        {
                            //Console.WriteLine("Preview state has changed"); //Debugging
                            ColorAllLabels(Color.Gray); //Gray out all labels
                            RefreshLabels(PreviewSceneSources, Color.Green); //Set label colors for preview sources
                            RefreshLabels(LiveSceneSources, Color.Red); //Set label colors for live sources
                            lastPreviewScene = currentPreviewScene; //Update preview scene state

                            // Serial writes (PREVIEW)
                            foreach (var preview in PreviewSceneSources)
                            {
                                //Console.WriteLine(preview.SourceName);
                                if (preview.SourceName == source1) { serialPort1.Write("5\r\n"); }
                                else if (preview.SourceName == source2) { serialPort1.Write("6\r\n"); }
                                else if (preview.SourceName == source3) { serialPort1.Write("7\r\n"); }
                                else if (preview.SourceName == source4) { serialPort1.Write("8\r\n"); }
                                else { serialPort1.Write("9\r\n"); }
                            }
                        }

                    }//end if previews ON check
                    else // Previews are toggled off
                    {
                        if (lastbutton2State != button2_ClickCount) // If preview on/off toggle changes
                        {
                            //Console.WriteLine("Previews off."); //Debugging
                            ColorAllLabels(Color.Gray); //Gray out all labels
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

        // Poll websockets to check if connected. This loop is for error handling.
        private void WebsocketHeartbeat(object sender, EventArgs e) // Loops continuously 100ms
        {
            if (mainWebsocket.IsConnected)
            {
                exitFlag = true;
                messageShown = false; // Set messageShown flag to false
            }
            else
            {
                ColorAllLabels(Color.Gray);
                exitFlag = false;

                if (!messageShown) // If the message hasn't already been shown
                {
                    messageShown = true; // Set messageShown flag to true
                    DialogResult result = MessageBox.Show("Please verify the following:\n" +
                        "1. OBS is open and running.\n" +
                        "2. OBS Websockets is installed and enabled.\n\n" +
                        "Would you like to attempt to reconnect?", "OBS TALLY: Lost connection to OBS",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (result == DialogResult.Yes)
                    {
                        Form1_Load(sender, e); // Re-run setup loop
                        messageShown = false; // Set messageShown flag to false
                    }
                    else if (result == DialogResult.No)
                    {
                        this.Close();
                    }
                }
            }
        }

        private void RefreshLabels(List<OBSWebsocketDotNet.Types.SceneItem> SceneSources, Color color)
        {
            //Update labels
            foreach (var source in SceneSources)
            {
                if (source.SourceName == source1) { label1.BackColor = color; }
                else if (source.SourceName == source2) { label2.BackColor = color; }
                else if (source.SourceName == source3) { label3.BackColor = color; }
                else if (source.SourceName == source4) { label4.BackColor = color; }
                else { label5.BackColor = color; }
            }
        }

            private void ColorAllLabels(Color color)
        {
            label1.BackColor = color;   label2.BackColor = color;
            label3.BackColor = color;   label4.BackColor = color;
            label5.BackColor = color;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.Close();
            serialPort1.PortName = comboBox1.Text;
            if (serialPort1 != null)
            {
                try
                {
                    serialPort1.Open(); //serialPort1.Close(); //Try opening and closing the port to test connectivity
                    label5.Text = serialPort1.PortName;
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //Initialize data recieved event handler
                }
                catch
                {
                    MessageBox.Show("Could not establish serial connection on " + comboBox1.Text, "Communication Error");
                }
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // See example here: https://stackoverflow.com/questions/16215741/c-sharp-read-only-serial-port-when-data-comes
            // https://forum.arduino.cc/index.php?topic=40336.0
            //if (serialPort1.ReadLine() != string.Empty)
            //{
                string line = serialPort1.ReadLine();
                BeginInvoke(new LineReceivedEvent(LineReceived), line);
            //}
        }

        private delegate void LineReceivedEvent(string line);
        private void LineReceived(string line)
        {
            Console.Write("From Arduino: " + line);
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
