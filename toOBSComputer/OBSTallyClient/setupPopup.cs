using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace OBSTallyClient
{
    public partial class setupPopup : Form
    {
        // Public variables
        public bool configComplete = false;
        public string wsPort = "4444";
        public string wsAddress = "127.0.0.1";
        public string source1;
        public string source2;
        public string source3;
        public string source4;
        public string wsPassword;

        public setupPopup()
        {
            InitializeComponent();
            textBox6.Text = wsAddress;
            textBox7.Text = wsPort;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode Root = xmlDoc.AppendChild(xmlDoc.CreateElement("root"));

            XmlNode Child1 = Root.AppendChild(xmlDoc.CreateElement("Websocket"));
            XmlAttribute ChildAtt1 = Child1.Attributes.Append(xmlDoc.CreateAttribute("password"));
            ChildAtt1.InnerText = textBox5.Text;

            XmlNode Child2 = Root.AppendChild(xmlDoc.CreateElement("Source1"));
            XmlAttribute ChildAtt2 = Child2.Attributes.Append(xmlDoc.CreateAttribute("name"));
            ChildAtt2.InnerText = textBox1.Text;

            XmlNode Child3 = Root.AppendChild(xmlDoc.CreateElement("Source2"));
            XmlAttribute ChildAtt3 = Child3.Attributes.Append(xmlDoc.CreateAttribute("name"));
            ChildAtt3.InnerText = textBox2.Text;

            XmlNode Child4 = Root.AppendChild(xmlDoc.CreateElement("Source3"));
            XmlAttribute ChildAtt4 = Child4.Attributes.Append(xmlDoc.CreateAttribute("name"));
            ChildAtt4.InnerText = textBox3.Text;

            XmlNode Child5 = Root.AppendChild(xmlDoc.CreateElement("Source4"));
            XmlAttribute ChildAtt5 = Child5.Attributes.Append(xmlDoc.CreateAttribute("name"));
            ChildAtt5.InnerText = textBox4.Text;

            XmlNode Child6 = Root.AppendChild(xmlDoc.CreateElement("WebsocketAddress"));
            XmlAttribute ChildAtt6 = Child6.Attributes.Append(xmlDoc.CreateAttribute("address"));

            XmlNode Child7 = Root.AppendChild(xmlDoc.CreateElement("WebsocketPort"));
            XmlAttribute ChildAtt7 = Child7.Attributes.Append(xmlDoc.CreateAttribute("port"));

            if (textBox6.Text != wsAddress)  { ChildAtt6.InnerText = textBox6.Text; }
            else { ChildAtt6.InnerText = wsAddress; }
            if (textBox7.Text != wsPort) { ChildAtt7.InnerText = textBox7.Text; }
            else { ChildAtt7.InnerText = wsPort; }

            try
            {
                xmlDoc.Save(Application.StartupPath + "\\config.xml");
                this.configComplete = true;
            }
            catch
            {
                MessageBox.Show("Unable to save config file.", "Failed to write file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Close();
        }

        private void setupPopup_Load(object sender, EventArgs e)
        {
            try
            {
                loadConfigXML(); // Load the XML file, catch if it doesn't exist
                textBox1.Text = source1;
                textBox2.Text = source2;
                textBox3.Text = source3;
                textBox4.Text = source4;
                textBox5.Text = wsPassword;
                textBox6.Text = wsAddress;
                textBox7.Text = wsPort;
            }
            catch (FileNotFoundException ex1) //if Config doesn't exist, show setupPopup
            {

            }
            catch
            {

            }
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
