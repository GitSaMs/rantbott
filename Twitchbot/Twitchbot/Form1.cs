using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Twitchbot
{
    public partial class Form1 : Form
    {

        bool requests = false;
        Queue<string> songrequests = new Queue<string>();
        Queue<string> sendMessageQueue;
        TcpClient Client;
        NetworkStream netstream;
        StreamReader reader;
        DateTime lastMessage;
        int timer = 0;
        StreamWriter writer;
        List<Command> list = new List<Command>();
        string username, password,channelname,trimchat;
        DateTime start = new DateTime();
        
        

        public Form1()
        {
            sendMessageQueue = new Queue<string>();
            this.username = "rantbott";
            this.channelname = "ropzelcius";
            this.password = File.ReadAllText("password.txt");
            this.trimchat = $":{username}!{username}@{username}.tmi.twitch.tv PRIVMSG #{channelname} :";
            InitializeComponent();
            Login();
            lastMessage = DateTime.Now;
        }
        public void Login()
        {
            Client = new TcpClient("irc.twitch.tv", 6667);
            netstream = Client.GetStream();
            reader = new StreamReader(netstream, Encoding.GetEncoding("iso8859-1"));
            writer = new StreamWriter(netstream, Encoding.GetEncoding("iso8859-1"));
            writer.WriteLine("PASS " + password);
            writer.Flush();
            writer.WriteLine("USER " + username + " 8 * :" + username);
            writer.Flush();
            writer.WriteLine("NICK " + username);
            writer.Flush();
            writer.WriteLine("JOIN #"+channelname);
            writer.Flush();
            textBox1.Text += Environment.NewLine + "Connected";
            SendMessage("RantBot Connected, Chat is gonna be deleted now #RANTSI");
            start = DateTime.Now;

        }
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            
            if (!Client.Connected)
            {
                Login();
            }
            
                
            
            TryReceiveMessages();
            TrySendingMessages();
            

        }
        public void SendMessage(string message)
        {
            sendMessageQueue.Enqueue(message);
            
        }


        void ReceiveMessage(string nick, string message,string channel,string type,string trimchat)
        {
            if (type == "PRIVMSG" && channel.Contains("#"))
            {
                textBox1.Text += Environment.NewLine + channel + "<" + DateTime.Now.ToString("HH:mm:ss") + "> <" + nick + "> " + message;
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }
            if(message.StartsWith("!hi"))
            {
                SendMessage($"Hello, {nick}");
            }
            if (message.StartsWith("!uptime"))
            {
                DateTime whenasked = DateTime.Now;
                var uptime = start - whenasked;
                SendMessage($"Stream uptime: {uptime:hh\\:mm\\:ss}");
            }

            if (requests==true && message.StartsWith("!songrequest"))
            {

                string[] spliturl = message.Split(' ');

                string mainurl = spliturl[1];
                

                
                songrequests.Enqueue(mainurl);
                if (axShockwaveFlash1.Movie==null)
                {
                    NextSong();
                    Song(mainurl, timer);
                }
                else
                {
                    Song(mainurl, timer);

                }
                

            }

            if (message.StartsWith("!"))
            {
                Command comm = new Command();
                string[] split1 = message.Split(' ');
                string command = split1[0];
                StreamReader sr = new StreamReader("commands.txt");
                string line;
                string[] split2;
                while (true)
                {
                    line = sr.ReadLine();
                    if (line != null && line.StartsWith("!"))
                    {
                        split2 = line.Split(';');
                        comm.Com = split2[0];
                        comm.Answer = split2[1];
                    }

                    if (command == comm.Com)
                    {
                        SendMessage(comm.Answer);
                        break;
                    }
                    if (line==null)
                    {
                        break;
                    }
                }
                sr.Close();
            }
            
            if (message.StartsWith("!commands"))
            {
                Command comm = new Command();
                StreamReader sr = new StreamReader("commands.txt");
                string line;
                string[] split2;
                string commands = "";
                while (true)
                {
                    
                    line = sr.ReadLine();
                    if (line != null && line.StartsWith("!"))
                    {
                        split2 = line.Split(';');
                        comm.Com = split2[0];
                        commands = commands + comm.Com + " ";

                    }
                    if (line == null)
                    {
                        break;
                    }
                }
                SendMessage(commands);
                sr.Close();

            }

        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
        }

        void TryReceiveMessages()
        {
            if (Client.Available >0 )
            {
                string data = "";
                data = reader.ReadLine();
                string nick = "";
                string type = "";
                string channel = "";
                string message = "";
                string[] split1 = data.Split(':');
                string[] ex;
                ex = data.Split(new char[] { ' ' }, 5);
                if (ex[0] == "PING")
                {
                    SendMessage("PONG");
                }

                if (data.Contains("PRIVMSG"))
                {
                    if (split1.Length > 1)
                    {
                        string[] split2 = split1[1].Split(' ');
                        nick = split2[0];
                        nick = nick.Split('!')[0];
                        type = split2[1]; 
                        channel = split2[2];
                        if (split1.Length > 2)
                        {
                            for (int i = 2; i < split1.Length; i++)
                            {
                                message += split1[i] + " ";
                            }
                        }
                        trimchat= $":{nick}!{nick}@{nick}.tmi.twitch.tv PRIVMSG #{channelname} :";
                    }
                }

                ReceiveMessage(nick, message, channel, type,trimchat);
            }
        }

        private void axShockwaveFlash1_OnReadyStateChange(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_OnReadyStateChangeEvent e)
        {
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer++;

        }

        void TrySendingMessages()
        {
            if(DateTime.Now-lastMessage>TimeSpan.FromSeconds(2))
            {
                if (sendMessageQueue.Count > 0)
                {
                    var message = sendMessageQueue.Dequeue();
                    writer.WriteLine($"{trimchat}{message}");
                    writer.Flush();
                    lastMessage = DateTime.Now;
                }

            }
            

        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            if (requests==false)
            {
                requests = true;
                button4.BackColor = Color.Green;
            }
            else
            {
                requests = false;
                button4.BackColor = Color.Red;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button4.BackColor = Color.Red;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendMessage(textBox2.Text);
            textBox2.Clear();
        }
        void NextSong()
        {        
            if (songrequests.Count > 0)
            {
                string mainurl = songrequests.Dequeue();
                string url = mainurl.Replace("watch?", "");
                url = url.Replace("=", "/");
                url = "https://www.youtube.com/" + url + "&autoplay=1";

                axShockwaveFlash1.Movie = url;
                Song(mainurl, timer);

            }
        }
        void Song(string url,int timer)
        {
            timer2.Start();
            string fullurl = "https://www.youtube.com/" + url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullurl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            string data = sr.ReadToEnd();
            int index = data.IndexOf("length_seconds");
            string duration = data.Substring(index, 24);
            string[] split;
            split = duration.Split(':');
            string[] split2;
            split2 = split[1].Split('"');
            int durations = int.Parse(split2[1]);
            if (timer==durations)
            {
                timer2.Stop();
                timer = 0;
                durations = 0;
                NextSong();
            }
            

        }

        
    }
}
