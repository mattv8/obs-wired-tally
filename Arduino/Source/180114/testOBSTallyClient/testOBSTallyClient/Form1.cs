using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OBSWebsocketDotNet;

namespace testOBSTallyClient
{
    public partial class Form1 : Form
    {
        OBSWebsocket mainWebsocket = new OBSWebsocket();
        //OBSScene currentScene;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                mainWebsocket.Connect("ws://127.0.0.1:4444", "debug");
            }
            catch
            {
                MessageBox.Show("Failed to connect with OBS");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    //serialPort1.Open();
                
                    string currentScene = mainWebsocket.GetCurrentScene().Name;
                    //serialPort1.WriteLine(currentScene);

                    

                


                    if (currentScene == "gameWithCam")
                    {
                        label1.BackColor = Color.Red;
                        label2.BackColor = Color.Green;
                        label3.BackColor = Color.Green;
                        label4.BackColor = Color.Green;
                        serialPort1.Write("1");
                    }
                    else if (currentScene == "cam")
                    {
                        label1.BackColor = Color.Green;
                        label2.BackColor = Color.Red;
                        label3.BackColor = Color.Green;
                        label4.BackColor = Color.Green;
                        serialPort1.Write("2");
                    }
                    else if (currentScene == "game")
                    {
                        label1.BackColor = Color.Green;
                        label2.BackColor = Color.Green;
                        label3.BackColor = Color.Red;
                        label4.BackColor = Color.Green;
                        serialPort1.Write("3");
                    }
                    else if (currentScene == "intro")
                    {
                        label1.BackColor = Color.Green;
                        label2.BackColor = Color.Green;
                        label3.BackColor = Color.Green;
                        label4.BackColor = Color.Red;
                        serialPort1.Write("4");
                    }
                    else
                    {
                        label1.BackColor = Color.Green;
                        label2.BackColor = Color.Green;
                        label3.BackColor = Color.Green;
                        label4.BackColor = Color.Green;
                        serialPort1.Write("5");
                    }

                    //serialPort1.Close();

                }
                catch
                {
                    //MessageBox.Show("Port " + comboBox1.Text + " není připojen!", "Chyba!");
                }


            }
            catch
            {

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
                    MessageBox.Show("Port " + comboBox1.Text + " není připojen!", "Chyba!");
                }
            }

        }



        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

        }
    }
}
