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
using System.Text;
using System.Threading;
namespace SocketClientFormApplication
{
    public partial class Form1 : Form
    {
        //private byte[] buffer;
        Socket clientSocket;
        IPAddress address = null;
        public Form1()
        {
            InitializeComponent();
            btnSendData.Text = "Random Generate";
            btnSendData.Enabled = true;
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            directConnect();

           
        }
        private void directConnect()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string ip = "127.0.0.1";
            var endpoint = new IPEndPoint(IPAddress.Parse(ip), 23000);
            clientSocket.BeginConnect(endpoint, ConnectCallback, null);
           // ReceiveData();
        }

        private void btnSendData_Click(object sender, EventArgs e)
        {
          Random random = new Random();
            int sayi = random.Next();
            SendString(sayi.ToString());
            //txtMessage.Text=sayi.ToString();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            clientSocket.Close();
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            //byte []buffer = new byte[1024];

            //string data = txtMessage.Text;
            //buffer = Encoding.ASCII.GetBytes(data);
            //clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, null);
            SendString(txtMessage.Text);
            //clientSocket.Send(buffer);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //byte[] buffer = new byte[1024];
            //buffer = Encoding.UTF8.GetBytes(checkBox1.Checked.ToString());
            //clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, null);
            SendString(checkBox1.Checked.ToString());
        }
        private  void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked=true;
            }
        }
        private void ReceiveCallBack(IAsyncResult AR)
        {
            var buffer = new byte[1024];
            try
            {
                while (true) { 
                ReceiveData();
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ConnectCallback(IAsyncResult AR)
        {
            
            try
            {
                Thread.Sleep(100);
                clientSocket.EndConnect(AR);
                byte[] buffer = new byte[clientSocket.ReceiveBufferSize];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallBack, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        private void SendCallBack(IAsyncResult Ar)
        {
            try
            {
                clientSocket.EndSend(Ar);
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        private void ReceiveData()
        {
            var buffer = new byte[2048];
            int received = clientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);

            int number;
            bool lab;
            if (int.TryParse(text, out number))
            {
                Invoke((Action)delegate
                {
                    btnSendData.Text = number.ToString();
                });

            }
            else if (bool.TryParse(text, out lab))
            {
                Invoke((Action)delegate
                {
                    checkBox1.Checked = lab;
                });

            }
            else
            {

                Invoke((Action)delegate
                {
                    txtMessage.Text = text;
                });

            }
        }
        private void AppendToTextBox(string a)
        {

        }
    }
}
