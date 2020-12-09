using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CLIENT2
{
    public partial class Form1 : Form
    {
        static Socket clientSoc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static bool connected = false;
        delegate void StringArgReturningVoidDelegate(string text);
        public Form1()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void ip_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string serverIP = ip.Text;

            int portNum = int.Parse(Port.Text);

            try
            {
                clientSoc.Connect(serverIP, portNum);
                connected = true;
                AppendTextBox1("Connection established...");
                Thread userThread = new Thread(UserConnected);
                userThread.Start();
                                
                // infiniteUserInput();
            }
            catch
            {
                AppendTextBox1("Connection failed");

            }

        }
        private void Receive()
        {
            while (connected)
            {

                try
                {
                    Byte[] buffer5 = new Byte[64];
                    //clientSoc.Receive(buffer5);
                    int size;
                    size = clientSoc.Receive(buffer5);

                    if (size > 0)
                    {
                        string s = Encoding.Default.GetString(buffer5);
                        AppendTextBox1("Server: " + Encoding.Default.GetString(buffer5));
                    }
                }
                catch
                {
                    AppendTextBox1("There is a problem! Check the connection...");
                    connected = false;
                    clientSoc.Close();
                }

            }
        }
     

        private void AppendTextBox1(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.richTextBox1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(AppendTextBox1);

                try
                {
                    this.Invoke(d, new object[] { text });
                }
                catch { }
            }
            else
            {
                this.richTextBox1.AppendText(text);
            }
        }

       
        private void label5_Click(object sender, System.EventArgs e)
        {

        }



        private void UserConnected()
        {

            try
            {
                string username = "USERNAME"+Usertxt.Text;

                
                clientSoc.Send(Encoding.Default.GetBytes(username));
               // AppendTextBox1("Your message has been sent.");
                Thread recieveThread = new Thread(Receive);

                recieveThread.Start();
                

            }
            catch
            {

                AppendTextBox1("There is a problem! Check the connection...");
                connected = false;
                clientSoc.Close();

            }
        }

        private void send_button_Click(object sender, System.EventArgs e)
        {


            if (question.Text != "") //
            {
                string ques = "&" + question.Text + "!" + answer.Text;

                clientSoc.Send(Encoding.Default.GetBytes(ques));

                AppendTextBox1("Your question has been sent.");
            }

            else 
            {
                string ans = "!" + answer.Text;

            }

        }



        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            //if (e.CloseReason == CloseReason.WindowsShutDown)
            

                string username = Usertxt.Text;
                string message = username + " has left";
               // Byte[] buffer = Encoding.Default.GetBytes(message);

                try
                {
                    clientSoc.Send(Encoding.Default.GetBytes(message));
                }
                catch {
                    AppendTextBox1("Server closed");
                } 


                clientSoc.Close();
            



        }

        private void question_TextChanged(object sender, System.EventArgs e)
        {
           
        }

    }
}
    

