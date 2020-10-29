using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace OBSTallyClient
{
    public partial class setupPopup : Form
    {
        public bool exitFlag = false;
        public setupPopup()
        {
            InitializeComponent();
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

            try
            {
                xmlDoc.Save(Application.StartupPath + "\\config.xml");
                this.exitFlag = true;
            }
            catch
            {
                MessageBox.Show("Unable to save config file.", "Failed to write file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Close();
        }

        private void setupPopup_Load(object sender, EventArgs e)
        {

        }
    }
}
