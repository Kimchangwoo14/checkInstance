using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace supermango
{
    public partial class Form1 : Form
    {
        private static Form1 instance = null;
        public static Form1 Instance
        {
            get
            {
                if (instance == null)
                    instance = new Form1();

                return instance;
            }
        }

        public Form1()
        {
            InitializeComponent();

            instance = this;
        }

        ~Form1()
        {

        }

       
        private Button createbutton( int left, int top, int width, int height, string text, Control parent )
        {
            Button button = new Button();
            button.Location = new Point(left, top);
            button.Width = width;
            button.Height = height;
            button.Text = text;

            parent.Controls.Add(button);

            return button;
        }
        private Label createtext(int left, int top, int width, string text, Control parent )
        {
            Label labelcontrol = new Label();
            labelcontrol.Location = new System.Drawing.Point(left, top);
            labelcontrol.Text = text;
            labelcontrol.Width = width;

            parent.Controls.Add(labelcontrol);

            return labelcontrol;
        }

        ToolTip tooltip = new ToolTip();

        private void CreateTooltip( Control parent , string text )
        {
            tooltip.AutoPopDelay = 10000;
            tooltip.InitialDelay = 500;
            
            tooltip.SetToolTip(parent, text);
        }
        private PictureBox createpicturebox( int left, int top, int width, int height, string filename, string filepath, Control parent, bool border = false )
        {
            Bitmap bmp;
            PictureBox pictureboxcontrol = new PictureBox();
            pictureboxcontrol.Left = left;
            pictureboxcontrol.Top = top;
            pictureboxcontrol.Width = width;
            pictureboxcontrol.Height = height;
            pictureboxcontrol.SizeMode = PictureBoxSizeMode.StretchImage;

            if (border)
                pictureboxcontrol.BorderStyle = BorderStyle.FixedSingle;

            string fullfilename = filepath + "\\" + filename;
            fullfilename += ".png";

            parent.Controls.Add(pictureboxcontrol);

            bmp = new Bitmap(fullfilename);
            pictureboxcontrol.Image = (Image)bmp;

            return pictureboxcontrol;
            //this.Controls["panel1"].Controls.Add(pictureboxcontrol);
        }
      
        private static string thread_getHttpRequest(string url)
        {
            WebRequest request = WebRequest.Create(url); // 호출할 url
            request.Method = "GET";
            string responseFromServer = "";

            try
            {
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                 responseFromServer = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return responseFromServer;
        }

        private string getHttpRequest( string url )
        {
            WebRequest request = WebRequest.Create(url); // 호출할 url
            request.Method = "GET";
            request.Timeout = -1;
            string responseFromServer = "";

            try
            {
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                responseFromServer = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }

            return responseFromServer;
        }

        private BackgroundWorker worker;
        private void checkState_Click(object sender, EventArgs e)
        {
            tb_state.ScrollBars = ScrollBars.Vertical;
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            worker.DoWork += BackgroundWorkerOnDoWork;
            worker.ProgressChanged += BackgroundWorkerOnProgressChanged;
            worker.RunWorkerAsync();

          
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                //Do your stuff here
                string ServerState = CheckServerStateLog();
                if (this.tb_state.InvokeRequired == true)
                {
                    this.tb_state.Invoke((MethodInvoker)delegate
                    {
                        tb_state.Text = ServerState;
                    });
                }

                string sendServerState = ServerState.Replace("\r\n", "*");

                string request = sendLog(sendServerState);

                Thread.Sleep(60000 * 5);
                worker.ReportProgress(0, "AN OBJECT TO PASS TO THE UI-THREAD");
            }
        }

       

        private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object userObject = e.UserState;
            int percentage = e.ProgressPercentage;
        }

        public string CheckServerStateLog()
        {
            tb_state.Text += "CheckServerStateLog" + "\n\r";
            string FolderName = @"C:\serverstate";//Input Batch "getServerState" get Log Path
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(FolderName);
            List<string> folderList = new List<string>();
            foreach (var File in di.GetDirectories())
            {
                folderList.Add(File.Name);
            }
            string SendFullText = "";

            string JustShowFullText = "";
            foreach (string path in folderList)
            {
                string fullpath_disk = FolderName + "\\" + path + "\\disk.txt";
                string fullpath_memory = FolderName + "\\" + path + "\\memory.txt";
                string SendText = "[" + path + "]";
                string JustShowText = "[" + path + "]";
                string[] disktext = System.IO.File.ReadAllLines(fullpath_disk);
                bool DistLack = false;
                for (int k = 0; k < disktext.Length; ++k)
                {
                    string Line = disktext[k];
                    if (string.IsNullOrEmpty(Line)) continue;
                    if (path == "LOG" && Line.Contains("D:"))
                    {
                        float Remainstorage = float.Parse(Regex.Replace(Line, @"[^0-9.]", ""));
                        if (Remainstorage < 1.0f)
                        {
                            DistLack = true;
                            SendText += " FreeSpace : " + Remainstorage + "GB";
                        }
                        JustShowText += " FreeSpace : " + Remainstorage + "GB";
                        break;
                    }
                    else if (path != "LOG" && Line.Contains("C:"))
                    {
                        float Remainstorage = float.Parse(Regex.Replace(Line, @"[^0-9.]", ""));
                        if (Remainstorage < 1.0f)
                        {
                            DistLack = true;
                            SendText += " FreeSpace : " + Remainstorage + "GB";
                        }
                        JustShowText += " FreeSpace : " + Remainstorage + "GB";
                        break;
                    }
                }

                string[] memorytext = System.IO.File.ReadAllLines(fullpath_memory);
                float UsingCPU = 1.0f;
                float UsingMemory = 1.0f;
                bool MemoryLack = false;
                bool CPULack = false;
                for (int k = 0; k < memorytext.Length; ++k)
                {
                    string Line = memorytext[k];
                    if (k == 0)
                    {
                        if (!float.TryParse(Line, out UsingCPU))
                            continue;
                    }
                    if (k == 1)
                    {
                        if (!float.TryParse(Line, out UsingMemory))
                            continue;
                    }
                }
                if (UsingCPU >= 95.0f)
                {
                    SendText += " UsingCPU : " + UsingCPU.ToString("N1") + "%";
                    CPULack = true;
                }
                JustShowText += " UsingCPU : " + UsingCPU.ToString("N1") + "%";
                if (UsingMemory >= 95.0f)
                {
                    SendText += " UsingMemory : " + UsingMemory.ToString("N1") + "%";
                    MemoryLack = true;
                }
                JustShowText += " UsingMemory : " + UsingMemory.ToString("N1") + "%";
                if (DistLack || MemoryLack || CPULack)
                    SendFullText += SendText + "\n";
                JustShowFullText += JustShowText + "\r\n";
            }
            if (!string.IsNullOrEmpty(SendFullText))
            {
                TelegramBotManager.SendMessage(SendFullText);
            }
               
            return JustShowFullText;
        }
        public static string ByteToString(byte[] byteText)
        {
            if (byteText != null)
                return System.Text.Encoding.Unicode.GetString(byteText);

            return "";
        }

        public static byte[] ToByte(string strText)
        {
            if (strText.Length > 0)
                return System.Text.Encoding.Unicode.GetBytes(strText);

            return null;
        }

        private string sendLog(string data)
        {
            string requesturl = "Input Send Log Data Url and add param" + data;

            return getHttpRequest(requesturl);
        }
    }
}
