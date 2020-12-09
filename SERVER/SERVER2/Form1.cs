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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// NEW SERVER2 !!!!

namespace SERVER2
{
    
    


    public partial class Form1 : Form
    {

        class Player
        {
            public Socket s;
            public string username;
            public int score;
            
           
            public Player(Socket thisClient, string username2)
            {
                s = thisClient;
                username = username2;
                score = 0;
            }

            public void Ans_correct()
            {
                score++;
            }

        }

        delegate void StringArgReturningVoidDelegate(string text);
        static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static List<Socket> clientSockets = new List<Socket>();
        static List<Player> PlayerList = new List<Player>();
        bool connected = true;
        static bool terminating = false;
        static bool accept = true;
       static bool game = false;
        
        

        public Form1()
        {
            InitializeComponent();
        }



        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            terminating = true;
            AppendTextBox("Server Closed");
            string message = "Server Closed";
            Byte[] buffer = Encoding.Default.GetBytes(message);
             foreach (Socket client in clientSockets)
                {
                    client.Send(buffer);
                }

             serverSocket.Close();
        }

        private void StartButton_Click(object sender, System.EventArgs e)
        {

            int portNum;

            
           

            try
            {
                portNum = int.Parse(textBox1.Text);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, portNum);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(3);
                AppendTextBox("Server is listening...");
                Thread acceptThread = new Thread(Accept);
                acceptThread.Start();
                
            }
            catch
            {
               AppendTextBox("There is a problem! Check the port number and try again!");
               
            }
        }
     

        private void AppendTextBox(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.richTextBox1.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(AppendTextBox);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.richTextBox1.AppendText(text);
            }
        }

        private void Accept()
        {

            while (!game && accept)
            {
                try
                {
                    
                    
                        Socket newClient = serverSocket.Accept();
                        clientSockets.Add(newClient);
                       // AppendTextBox("New client is connected ");
                        Thread receiveThread = new Thread(Receive);
                        receiveThread.Start();
                    
                }
                catch
                {
                    if (terminating)
                    {

                        AppendTextBox("Server stopped working...");
                        accept = false;
                    }
                    else
                    {
                        AppendTextBox("Problem occured in accept function...");

                    }
                }

            }
        }




        private void Game() {

            int turn2 = int.Parse(turn.Text);

            for (int j = 0; j <= PlayerList.Count * turn2;j++) //PlayerList.Count * turn2
            {



                if (j == 0)
                {
                    string s = "game started";
                    AppendTextBox(s);
                    Byte[] buffer = Encoding.Default.GetBytes(s);
                    //The game has started
                    //buffer2 = Encoding.Default.GetBytes(s);
                    for (int e = 0; e < PlayerList.Count;e++ )
                    {
                        PlayerList[e].s.Send(buffer);
                        //thread sleep
                    }
                    
                }
                else
                {
                    string yturn = "your turn";
                    AppendTextBox(PlayerList[j % PlayerList.Count].username + "'s turn");
                    Byte[] buffer3 = Encoding.Default.GetBytes(yturn);
                    PlayerList[j % PlayerList.Count].s.Send(buffer3);
                    string c_answer, question;

                    Byte[] buffer2 = new Byte[64];
                    int size ;

                    
                    
                        size = PlayerList[j % PlayerList.Count].s.Receive(buffer2);

                        if (size > 0)
                        {
                            string str = Encoding.Default.GetString(buffer2);
                            question = str.Substring(str.IndexOf("&") + 1, str.IndexOf("!") - str.IndexOf("&") - 1);
                            c_answer = str.Substring(str.IndexOf("!") + 1, str.Length - 1 - str.IndexOf("!"));

                            c_answer = c_answer.Substring(0, c_answer.IndexOf("\0"));


                        }

                   
                   
                }
                 


            
            
            
            
            
            
            
            
            
            }



        
        
        }






         private void Receive()
         {
              
            
            while (connected && !game)
            {

                
                int lenClientSoc = clientSockets.Count();
                Socket thisClient = clientSockets[lenClientSoc - 1];

                try
                {
                    
                        Byte[] buffer = new Byte[64];
                        thisClient.Receive(buffer);

                        string message = Encoding.Default.GetString(buffer);
                        message = message.Substring(0, message.IndexOf("\0"));


                        if (message.Substring(0, 8) == "USERNAME")
                        {
                            int num = message.Length;
                            message = message.Substring(8, num - 8);
                            Player p = new Player(thisClient, message);
                            PlayerList.Add(p);
                            message = message + " joined";
                           
                            AppendTextBox(message);
                           
                            
                        }
                        /*
                     int receivedBytes = thisClient.Receive(buffer);
                    if (receivedBytes <= 0)
                    {
                        throw new SocketException();
                    }*/

       


                    
                     

                   

                }
                catch
                {

                      
                      connected = false;
                    if (!terminating)
                    {
                        AppendTextBox("Client has disconnected...");

                    }
                    thisClient.Close();
                    clientSockets.Remove(thisClient);
                }


            }
        }







         protected override void OnFormClosing(FormClosingEventArgs e)
         {

             connected = false;
             
             string message = "Server Closed";
             Byte[] buffer = Encoding.Default.GetBytes(message);
             
             
                 foreach (Socket client in clientSockets)
                 {
                     try { client.Send(buffer); }
                     catch { }
                 }
             
             Environment.Exit(Environment.ExitCode);

             serverSocket.Close();

             base.OnFormClosing(e);

             }







         private void button1_Click(object sender, System.EventArgs e)
         {

             if (PlayerList.Count >= 2)
             {

                 game = true;

                // Game();
                 Thread Startgame = new Thread(Game);
                 Startgame.Start();
             }

             else {

                 AppendTextBox("Player number is not enough to start game");
             }
         }
         }




        }


    

