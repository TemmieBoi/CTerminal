using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace CTerminal
{
    class Init
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting CTerminal...");
            CTerminal cTerminal = new CTerminal();
            cTerminal.Start();
        }
    }

    class CTerminal
    {
        public Thread updateThread;
        public bool isOpen = true;
        public Dictionary<string, Action> commandList = new Dictionary<string, Action>();
        public string commandPara;
        public string currentPath = @"C:\Users\Heath\";
        public List<string> log = new List<string>();

        public void Start()
        {

            Write("Executing Start-up Sequence...");

            updateThread = new Thread(Update);
            updateThread.Start();

            commandList.Add("clear", clear);
            commandList.Add("quit", quit);
            commandList.Add("collect", collect);
            commandList.Add("memory", memory);
            commandList.Add("time", time);
            commandList.Add("ls", ls);
            commandList.Add("cd", pcd);
            commandList.Add("getfilesize", pgetfilesize);
            commandList.Add("cat", pcat);
            commandList.Add("catbytes", pcatbytes);
            commandList.Add("exec", pexec);
            commandList.Add("rmf", prmf);
            commandList.Add("rmd", prmd);
            commandList.Add("mkdir", pmkdir);
            commandList.Add("ping", pping);
            commandList.Add("susumuhirasawaisthebest", hirasawasecret);

            AppDomain.CurrentDomain.ProcessExit += onExit;

            UpdateTitle();

            Write("CTerminal Started!");

        }

        private void onExit(object sender, EventArgs e)
        {
            log.Add("sender: " + sender);
            File.WriteAllLines(@".\logs\" + getTime() + ".log", log);
        }

        public void UpdateTitle()
        {
            Console.Title = currentPath + " - CTerminal";
        }

        public void clear()
        {
            Console.Clear();
        }

        public void quit()
        {
            Environment.Exit(1);
        }

        public void time()
        {
            Write(getTime());
        }

        public string getTime()
        {
            return DateTime.Now.ToString().Replace(':', '-');
        }

        public void memory()
        {
            long memory = Process.GetCurrentProcess().PrivateMemorySize64;
            DisplayMemory(memory);
        }

        public void DisplayMemory(long memory)
        {
            Write(memory + " bytes");
            Write(memory / 1000 + " kilobytes");
            Write(memory / 1000 / 1000 + " megabytes");
            Write(memory / 1000 / 1000 / 1000 + " gigabytes");
        }

        public void collect()
        {
            Write("before");
            long memory = Process.GetCurrentProcess().PrivateMemorySize64;
            DisplayMemory(memory);
            GC.Collect();
            memory = Process.GetCurrentProcess().PrivateMemorySize64;
            Write("after");
            DisplayMemory(memory);
        }

        public void ls()
        {

            string[] subDirectories = Directory.GetDirectories(currentPath);
            string[] subFiles = Directory.GetFiles(currentPath);

            string subStuff = string.Empty;

            foreach (string directory in subDirectories)
            {
                subStuff += directory.Split(@"\")[directory.Split(@"\").Length - 1] + "   ";
            }

            foreach (string file in subFiles)
            {
                subStuff += file.Split(@"\")[file.Split(@"\").Length - 1] + "   ";
            }

            Write(subStuff);

        }

        public void pping()
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            options.DontFragment = true;

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data);
            int timeout = 120;

            PingReply reply = pingSender.Send(commandPara, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                Write("Address: " + reply.Address.ToString());
                Write("time (ms): " + reply.RoundtripTime);
                Write("Time to live: " + reply.Options.Ttl);
                Write("Don't fragment: " + reply.Options.DontFragment);
                Write("Buffer size: " + reply.Buffer.Length);
            }
        }

        public void pcd()
        {

            if (commandPara != null)
            {
                if (commandPara == "/")
                {
                    currentPath = @"C:\";
                }
                else if (commandPara[1] == ':' && commandPara[2] == @"\"[0] || commandPara[2] == '/')
                {
                    if (Directory.Exists(commandPara))
                    {
                        currentPath = commandPara.Replace('/', @"\"[0]) + @"\";
                    }
                }
                else
                {
                    if (Directory.Exists(currentPath + commandPara))
                    {
                        currentPath += commandPara.Replace('/', @"\"[0]) + @"\";
                    }
                    else if (Directory.Exists(currentPath + @"\" + commandPara))
                    {
                        currentPath += @"\" + commandPara.Replace('/', @"\"[0]) + @"\";
                    }
                    else
                    {
                        Write("Directory doesn't exist!");
                    }
                }
            }
            else
            {
                currentPath = @"C:\Users\Heath\";
            }

            UpdateTitle();

            commandPara = null;

        }

        public void Update()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input != null)
                {
                    log.Add(input);
                    executeCommand(input);
                }
                Thread.Sleep(16);
            }
        }

        public void hirasawasecret()
        {
            Process.Start(new ProcessStartInfo("https://www.youtube.com/watch?v=ZFtRmBQ0oy0") { UseShellExecute = true });
        }

        public void pgetfilesize()
        {
            
            if (File.Exists(commandPara))
            {
                try
                {
                    Write(File.ReadAllBytes(commandPara).Length + " bytes");
                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else if (File.Exists(currentPath + commandPara.Replace("~/", "")) && commandPara[0] == '~')
            {
                try
                {
                    Write(File.ReadAllBytes(currentPath + commandPara.Replace(@"~/", "")).Length + " bytes");
                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else
            {
                Write("File not Found!");
            }

            commandPara = null;
        }

        public void pcat()
        {

            if (File.Exists(commandPara))
            {
                try
                {

                    Stopwatch sw = new Stopwatch();

                    sw.Start();

                    string[] lines = File.ReadAllLines(commandPara);

                    foreach (string line in lines)
                    {
                        Write(line);
                    }

                    sw.Stop();

                    Write(sw.ElapsedMilliseconds + " ms");

                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else if (File.Exists(currentPath + commandPara.Replace("~/", "")) && commandPara[0] == '~')
            {
                try
                {

                    Stopwatch sw = new Stopwatch();

                    sw.Start();

                    string[] lines = File.ReadAllLines(currentPath + commandPara.Replace("~/", ""));

                    foreach (string line in lines)
                    {
                        Write(line);
                    }

                    sw.Stop();

                    Write(sw.ElapsedMilliseconds + " ms");

                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else
            {
                Write("File not Found!");
            }

            commandPara = null;
        }

        public void pexec()
        {

            if (File.Exists(commandPara))
            {
                try
                {

                    Process.Start(commandPara);

                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else if (File.Exists(currentPath + commandPara.Replace("~/", "")) && commandPara[0] == '~')
            {
                try
                {

                    Process.Start(currentPath + commandPara.Replace("~/", ""));

                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else
            {
                Write("File not Found!");
            }

            commandPara = null;
        }

        public void pcatbytes()
        {

            if (File.Exists(commandPara))
            {
                try
                {

                    Stopwatch sw = new Stopwatch();

                    sw.Start();

                    byte[] bytes = File.ReadAllBytes(commandPara);

                    string bytestring = string.Join(',', bytes);

                    Write(bytestring);

                    bytes = null;
                    bytestring = null;

                    sw.Stop();

                    Write(sw.ElapsedMilliseconds + " ms");

                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else if (File.Exists(currentPath + commandPara.Replace("~/", "")) && commandPara[0] == '~')
            {
                try
                {

                    Stopwatch sw = new Stopwatch();

                    sw.Start();

                    byte[] bytes = File.ReadAllBytes(currentPath + commandPara.Replace("~/", ""));

                    string bytestring = string.Join(',', bytes);

                    Write(bytestring);

                    bytes = null;
                    bytestring = null;

                    sw.Stop();

                    Write(sw.ElapsedMilliseconds + " ms");

                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else
            {
                Write("File not Found!");
            }

            commandPara = null;
        }

        public void prmf()
        {

            if (File.Exists(commandPara))
            {
                try
                {
                    File.Delete(commandPara);
                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else if (File.Exists(currentPath + commandPara.Replace("~/", "")) && commandPara[0] == '~')
            {
                try
                {
                    File.Delete(currentPath + commandPara.Replace("~/", ""));
                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else
            {
                Write("File not Found!");
            }

            commandPara = null;
        }

        public void prmd()
        {

            if (Directory.Exists(commandPara))
            {
                try
                {
                    Directory.Delete(commandPara);
                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else if (Directory.Exists(currentPath + commandPara.Replace("~/", "")) && commandPara[0] == '~')
            {
                try
                {
                    Directory.Delete(currentPath + commandPara.Replace("~/", ""));
                }
                catch (Exception e)
                {
                    Write(e.Message);
                }
            }
            else
            {
                Write("Directory not Found!");
            }

            commandPara = null;
        }

        public void pmkdir()
        {

            try
            {
                if (currentPath[currentPath.Length - 1] == @"\"[0])
                {
                    Directory.CreateDirectory(currentPath + commandPara);
                }
                else
                {
                    Directory.CreateDirectory(currentPath + @"\" + commandPara);
                }
            }
            catch (Exception e)
            {
                Write(e.Message);
            }

            commandPara = null;
        }

        public void executeCommand(string command)
        {

            if (command.Contains(' '))
            {

                string[] commandParts = command.Split(' ', 2);
                Action action;

                try
                {
                    if (commandList.TryGetValue(commandParts[0], out action))
                    {
                        commandPara = commandParts[1];
                        action.Invoke();
                    }
                    else
                    {
                        Write("Unknown Command '" + command + "'");
                    }
                }
                catch (Exception e)
                {
                    Write(e.Message);
                }

            }
            else if (commandList.ContainsKey(command))
            {
                Action action;
                commandList.TryGetValue(command, out action);
                action.Invoke();
            }
            else
            {
                Write("Unknown Command '" + command + "'");
            }

        }

        public void Write(string text)
        {
            Console.WriteLine(text);
            log.Add(text);
        }

    }

}
