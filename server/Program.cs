using System.Net;
using System.Net.Sockets;

class Program
{
    private static string savePath = "tmp";
    private static bool _exitRequested = false;
    private static int runningThreads = 0;
    private static ManualResetEventSlim waitForProcessShutdownStart = new ManualResetEventSlim();
    private static ManualResetEventSlim waitForMainExit = new ManualResetEventSlim();

    static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: <port> <max_file_size> <save_path>");
            return;
        }

        int port = int.Parse(args[0]);
        int maxThreads = int.Parse(args[1]);
        savePath = args[2];

        AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
        {
            Console.WriteLine("\nExit requested. SIGTERM got. Shutting down server...");
            _exitRequested = true;
            waitForProcessShutdownStart.Set(); 
            waitForMainExit.Wait(); 
        };

        Console.CancelKeyPress += (sender, e) =>
        {
            _exitRequested = true;
            e.Cancel = true;
            Console.WriteLine("\nExit requested. Shutting down server...");
            waitForProcessShutdownStart.Set();
            waitForMainExit.Wait();
        };

        ThreadPool.SetMaxThreads(maxThreads, maxThreads);

        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Server started. Listening on port {port}...");

        try
        {
            while (!_exitRequested)
            {
                if (listener.Pending())
                {
                    if (runningThreads < maxThreads)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(HandleClient), listener.AcceptTcpClient());
                        Interlocked.Increment(ref runningThreads);
                        Console.WriteLine("Got new user connection!");
                    }
                    else
                    {
                        Console.WriteLine("Max threads reached. Ignoring new client connection.");
                    }
                }

                Thread.Sleep(100);
            }
        }
        finally
        {
            listener.Stop();
            WaitForThreadsToFinish();
            Console.WriteLine("Server shutdown complete.");
            waitForMainExit.Set(); 
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

        try
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead;
            using (MemoryStream ms = new MemoryStream())
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }

                string receivedData = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                string fileName = Path.Combine(savePath, $"received_file_{DateTime.Now:yyyyMMddHHmmss}.txt");
                File.WriteAllText(fileName, receivedData);

                Console.WriteLine($"File saved: {fileName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            client.Close();
            Interlocked.Decrement(ref runningThreads);
        }
    }

    static void WaitForThreadsToFinish()
    {
        while (runningThreads > 0)
        {
            Thread.Sleep(100);
        }
    }
}
