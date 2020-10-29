using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace OBSTallyClient
{
    public partial class setupPopup : Form
    {
        public setupPopup()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(Application.StartupPath + "\\config.xml");
            XmlNode source1 = xmlDoc.SelectSingleNode("root/Source1");
            source1.Attributes["name"].Value = textBox1.Text;

            XmlNode source2 = xmlDoc.SelectSingleNode("root/Source2");
            source2.Attributes["name"].Value = textBox2.Text;

            XmlNode source3 = xmlDoc.SelectSingleNode("root/Source3");
            source3.Attributes["name"].Value = textBox3.Text;

            XmlNode source4 = xmlDoc.SelectSingleNode("root/Source4");
            source4.Attributes["name"].Value = textBox4.Text;

            XmlNode comSetup = xmlDoc.SelectSingleNode("root/Setup");
            comSetup.Attributes["state"].Value = "completed";

            XmlNode websocket = xmlDoc.SelectSingleNode("root/Websocket");
            websocket.Attributes["password"].Value = textBox5.Text;

            try
            {
                xmlDoc.Save(Application.StartupPath + "\\config.xml");
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
