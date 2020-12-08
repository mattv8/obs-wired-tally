
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
        public string wsPort;
        public string wsAddress;
        public string source1;
        public string source2;
        public string source3;
        public string source4;
        public string wsPassword;

        public int button2_ClickCount = 1;
        public int lastbutton2State = 1;
        public bool configComplete = false;
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
            }
            catch (FileNotFoundException ex1) //if Config doesn't exist, show setupPopup
            {
                messageShown = true;
                setupPopup setItUp = new setupPopup();
                var setupResult = setItUp.ShowDialog();
                if (setItUp.configComplete == true)
                {
                    loadConfigXML();
                    messageShown = false;
                }
            }
            catch
            {
                
            }

            try
            {

                mainWebsocket.WSTimeout = TimeSpan.FromSeconds(1);
                mainWebsocket.Connect("ws://"+wsAddress+":"+wsPort, wsPassword);
                if (mainWebsocket.IsConnected)
                {

                    lastLiveScene = mainWebsocket.GetCurrentScene().Name; //Initialize lastLiveScene at the current live scene

                    //Update label text
                    label1.Text = source1; label1.Font = scaleFont(label1);
                    label2.Text = source2; label2.Font = scaleFont(label2);
                    label3.Text = source3; label3.Font = scaleFont(label3);
                    label4.Text = source4; label4.Font = scaleFont(label4);

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
                if (mainWebsocket.IsConnected && serialPort1 != null)
                {

                    ////////////// Update Websocket Variables //////////////
                    string currentLiveScene = mainWebsocket.GetCurrentScene().Name; // Get current live scene name
                    LiveSceneSources = mainWebsocket.GetCurrentScene().Items; // List all sources in current live scene
                    string currentPreviewScene = mainWebsocket.GetPreviewScene().Name; // Get current preview scene name
                    PreviewSceneSources = mainWebsocket.GetPreviewScene().Items; // List all sources in current preview scene

                    ////////////// LIVE //////////////
                    if (currentLiveScene != lastLiveScene) //If live scene state changes
                    {
                        Console.WriteLine("Live state has changed"); //Debugging
                        // Update live label colors for UI app
                        ColorAllLabels(Color.Gray); //Gray out all labels
                        if (button2.Text == "Previews ON") { RefreshLabels(PreviewSceneSources, Color.Green); } //Refresh preview labels
                        RefreshLabels(LiveSceneSources, Color.Red); //Refresh all live labels

                        // Send write (LIVE)
                        serialPort1.Write("51,"); // Send live state change bit to Arduino
                        SendSerial(LiveSceneSources); // Send live scene bits to Arduino

                        lastLiveScene = currentLiveScene; // Finally, update scene state
                    }

                    ////////////// PREVIEWS //////////////
                    if (button2.Text == "Previews ON") // If previews are turned ON
                    {
                        // Update previews after prieview on/off is toggled
                        if (lastbutton2State != button2_ClickCount)
                        {
                            Console.WriteLine("Previews on."); //Debugging
                            RefreshLabels(PreviewSceneSources, Color.Green); //Refresh preview labels
                            RefreshLabels(LiveSceneSources, Color.Red); //Set label colors for live sources
                            lastbutton2State = button2_ClickCount; //Update lastbutton2State

                            // Notify Arduino of the change
                            serialPort1.Write("50,"); SendSerial(PreviewSceneSources); serialPort1.Write("*\r\n"); // Send preview bits to Arduino
                        }

                        // Update preview label colors for UI app
                        if (currentPreviewScene != lastPreviewScene) //If preview scene state changes
                        {
                            Console.WriteLine("Preview state has changed"); //Debugging
                            ColorAllLabels(Color.Gray); //Gray out all labels
                            RefreshLabels(PreviewSceneSources, Color.Green); //Set label colors for preview sources
                            RefreshLabels(LiveSceneSources, Color.Red); //Set label colors for live sources
                            lastPreviewScene = currentPreviewScene; //Update preview scene state

                            // Serial writes (PREVIEW)
                            serialPort1.Write("50,"); // Send preview state change
                            SendSerial(PreviewSceneSources); // Send preview scene serial bits to Arduino
 
                        }

                    }//end if previews ON check
                    else // Previews are toggled off
                    {
                        if (lastbutton2State != button2_ClickCount) // If preview on/off toggle changes
                        {
                            Console.WriteLine("Previews off."); //Debugging
                            ColorAllLabels(Color.Gray); //Gray out all labels
                            RefreshLabels(LiveSceneSources, Color.Red); //Refresh live labels
                            lastbutton2State = button2_ClickCount; //Update lastbutton2State
                            
                            // Notify Arduino of the change
                            serialPort1.Write("50,"); serialPort1.Write("*\r\n"); // Blank out previews
                        }
                    }//end else previews ON check

                    serialPort1.Write(".*\r\n"); // Send heartbeat bit to Arduino

                }//end configComplete check

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
                messageShown = false; // Set messageShown flag to false
            }
            else
            {
                ColorAllLabels(Color.Gray);

                if (!messageShown) // If the message hasn't already been shown
                {
                    messageShown = true; // Set messageShown flag to true
                    DialogResult result = MessageBox.Show("Please verify the following:\n" +
                        "1. OBS is open and running.\n" +
                        "2. OBS Websockets is installed and enabled.\n" +
                        "3. You have entered the correct IP address and port.\n\n" +
                        "Would you like to attempt to reconnect? If not, click \"No\", then run the setup again.", "OBS TALLY: Lost connection to OBS",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (result == DialogResult.Yes)
                    {
                        Form1_Load(sender, e); // Re-run setup loop
                        messageShown = false; // Set messageShown flag to false
                    }
                    else if (result == DialogResult.No)
                    {
                    }
                }
            }
        }

        private void RefreshLabels(List<OBSWebsocketDotNet.Types.SceneItem> SceneSources, Color color)
        {

            //Update label colors
            foreach (var source in SceneSources)
            {
                if (source.SourceName == source1) { label1.BackColor = color; }
                else if (source.SourceName == source2) { label2.BackColor = color; }
                else if (source.SourceName == source3) { label3.BackColor = color; }
                else if (source.SourceName == source4) { label4.BackColor = color; }
                else {
                    if (color == Color.Red) { label5.Text = "A non-tallied source is live.";  }
                    if (color == Color.Green) { label5.Text = "A non-tallied source is in preview.";  }
                }
            }
            
        }
        private Font scaleFont(Label lab)
        {
            SizeF extent = TextRenderer.MeasureText(lab.Text, lab.Font);

            float hRatio = lab.Height / extent.Height;
            float wRatio = lab.Width / extent.Width;
            float ratio = (hRatio < wRatio) ? hRatio : wRatio;

            float newSize = lab.Font.Size * ratio;

            lab.Font = new Font(lab.Font.FontFamily, newSize - 2, lab.Font.Style);
            return lab.Font;
        }

        private void SendSerial(List<OBSWebsocketDotNet.Types.SceneItem> SceneSources)
        {
            foreach (var source in SceneSources)
            {
                if (source.SourceName == source1) { serialPort1.Write("0,"); }
                else if (source.SourceName == source2) { serialPort1.Write("1,"); }
                else if (source.SourceName == source3) { serialPort1.Write("2,"); }
                else if (source.SourceName == source4) { serialPort1.Write("3,"); }
                //else { serialPort1.Write("4,"); } //Out of range bit
            }
            serialPort1.Write("*\r\n"); // Send stop bit
        }

        private void ColorAllLabels(Color color)
        {
            label1.BackColor = color;   label2.BackColor = color;
            label3.BackColor = color;   label4.BackColor = color;
            label5.Text ="";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.Close();
            serialPort1.PortName = comboBox1.Text;
            if (serialPort1 != null)
            {
                try
                {
                    serialPort1.Open(); //Try opening the port to test connectivity
                    label6.Text = serialPort1.PortName;
                    label6.BackColor = Color.Blue;
                    serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //Initialize data recieved event handler
                    
                    // Notify Arduino of the change
                    if (button2.Text == "Previews ON") // If previews are turned ON
                    {
                        serialPort1.Write("50,"); SendSerial(PreviewSceneSources); serialPort1.Write("*\r\n"); // Send preview scene serial bits to Arduino
                    }
                    serialPort1.Write("51,"); SendSerial(LiveSceneSources); serialPort1.Write("*\r\n"); // Send live scene serial bits to Arduino
                }
                catch
                {
                    label6.BackColor = Color.Transparent;
                    MessageBox.Show("Could not establish serial connection on " + comboBox1.Text, "Communication Error");
                }
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // See example here: https://stackoverflow.com/questions/16215741/c-sharp-read-only-serial-port-when-data-comes
            // https://forum.arduino.cc/index.php?topic=40336.0
            string line = serialPort1.ReadLine();
            BeginInvoke(new LineReceivedEvent(LineReceived), line);
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
            XmlNode wesPort = xmlDoc.SelectSingleNode("root/WebsocketPort");
            wsPort = wesPort.Attributes["port"].Value;
            XmlNode wesAddress = xmlDoc.SelectSingleNode("root/WebsocketAddress");
            wsAddress = wesAddress.Attributes["address"].Value;
        }

    }
}
