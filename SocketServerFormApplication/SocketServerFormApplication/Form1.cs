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
namespace SocketServerFormApplication
{
 
    public partial class Form1 : Form
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 23000;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public Form1()
        {
            InitializeComponent();
            StartServer();

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
           //
            
        }
        private void StartServer()
        {
                //serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    //allDone.Reset();
                    serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
                    serverSocket.Listen(10);
                    serverSocket.BeginAccept(AcceptCallBack,serverSocket);
                    //allDone.WaitOne();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }
        private  void AcceptCallBack(IAsyncResult AR)
        {

            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            clientSockets.Add(socket);
            
            string xx = txtMessage.Text;
            byte[] data1 = Encoding.UTF8.GetBytes(xx);
            //socket.Send(data1);
            byte[] data2 = Encoding.UTF8.GetBytes(chcLambda.Checked.ToString());
            //socket.Send(data2);
            byte[] data3 = Encoding.UTF8.GetBytes(btnRandom.Text);
            //socket.Send(data3);
            socket.BeginSend(data1, 0, data1.Length, SocketFlags.None, SendCallBack, socket);
            Thread.Sleep(500);
            socket.BeginSend(data2, 0, data2.Length, SocketFlags.None, SendCallBack, socket);
            Thread.Sleep(500);
            socket.BeginSend(data3, 0, data3.Length, SocketFlags.None, SendCallBack, socket);
            Thread.Sleep(500);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, socket);
            AppendToTextBox("Client connected, waiting for request...");
            AppendToTextBox(socket.ToString() + "IP Endpoint : " + socket.RemoteEndPoint.ToString());
            serverSocket.BeginAccept(AcceptCallBack, null);
            
        }

       
        private  void ReceiveCallBack(IAsyncResult Ar)
        {
            Socket clientSocket = (Socket)Ar.AsyncState;
            int data_size;
            

            try
            {
                data_size = clientSocket.EndReceive(Ar);
            }
            catch(Exception ex)
            {
                AppendToTextBox("Client Disconnected");
                clientSocket.Close();
                clientSockets.Remove(clientSocket);
                return;
            }
            byte [] received_Buffer = new byte[data_size];
            Array.Copy(buffer, received_Buffer, data_size);
            string data = Encoding.ASCII.GetString(received_Buffer);
            int number;
            bool lab;
            if (int.TryParse(data, out number))
            {
                Invoke((Action)delegate
                {
                    btnRandom.Text = number.ToString();
                   // txtMessage.Text=number.ToString();
                    //clientSocket.Send(Encoding.UTF8.GetBytes(data));
                });
                

            }
            else if (bool.TryParse(data, out lab))
            {
                Invoke((Action)delegate
                {
                    chcLambda.Checked = lab;
                    //txtMessage.Text = lab.ToString();
                });

            }
            else
            {

                Invoke((Action)delegate
                {
                    txtMessage.Text = data;
                });

            }

            clientSocket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallBack, clientSocket);


        }
        private void SendAllClients(byte[] data)
        {
            foreach (Socket client in clientSockets)
            {
                client.Send(data);
            }
        }
         
        private  void AppendToTextBox(string a)
        {
            Invoke((Action)delegate
            {
                txtStatus.Text+=a+"\r\n";
            });

        }

        private void btnRandom_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int xx = random.Next();
            txtMessage.Text = xx.ToString();
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            //Array.Clear(buffer, 0, buffer.Length);
            //buffer = Encoding.UTF8.GetBytes(txtMessage.Text);
            byte[] data = new byte[BUFFER_SIZE];
            data = Encoding.UTF8.GetBytes(txtMessage.Text);
            SendAllClients(data);
            ////clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, null);

        }

        private void chcLambda_CheckedChanged(object sender, EventArgs e)
        {
            byte[] data = new byte[BUFFER_SIZE];
            data = Encoding.UTF8.GetBytes(chcLambda.Checked.ToString());
            SendAllClients(data);
        }

        private void btnRandom_TextChanged(object sender, EventArgs e)
        {
            byte[] data = new byte[BUFFER_SIZE];
            data = Encoding.UTF8.GetBytes(btnRandom.Text.ToString());
            SendAllClients(data);
        }
        private void SendCallBack(IAsyncResult AR)
        {
            Socket clientSocket = (Socket)AR.AsyncState;
            try
            {
               clientSocket.EndSend(AR);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
