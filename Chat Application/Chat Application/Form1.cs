using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Chat_Application
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Socket Binding
            epLocal = new IPEndPoint(IPAddress.Parse(textLocalIp.Text), Convert.ToInt32(textLocalPort.Text));
            sck.Bind(epLocal);

            //Remote IP Connection
            epRemote = new IPEndPoint(IPAddress.Parse(textRemoteIp.Text), Convert.ToInt32(textRemotePort.Text));
            sck.Connect(epRemote);

            buffer = new byte[1500];
            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Setting up Sockets
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // User IP
            textLocalIp.Text = GetLocalIP();
            textRemoteIp.Text = GetLocalIP();
        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            // String to Byte Message
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = aEncoding.GetBytes(textMessage.Text);

            //Message Sending
            sck.Send(sendingMessage);

            listMessage.Items.Add("Person 1: " + textMessage.Text);
            textMessage.Text = "";
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                byte[] recievedData = new byte[1500];
                recievedData = (byte[])aResult.AsyncState;

                //Byte to String
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string recievedMessage = aEncoding.GetString(recievedData);

                //Showing Message in List Box
                listMessage.Items.Add("Person 2: " + recievedMessage);

                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
