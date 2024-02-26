using System.Net.Sockets;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: <file_name> <server_address> <server_port>");
            return;
        }

        string fileName = args[0];
        string serverAddress = args[1];
        int serverPort = int.Parse(args[2]);

        // Проверяем существование файла
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"File '{fileName}' not found.");
            return;
        }

        try
        {
            // Читаем содержимое файла
            string fileContent = File.ReadAllText(fileName);

            // Устанавливаем соединение с сервером
            using (TcpClient client = new TcpClient(serverAddress, serverPort))
            {
                Console.WriteLine($"Connected to server {serverAddress}");

                // Получаем сетевые потоки для передачи данных
                NetworkStream stream = client.GetStream();

                // Преобразуем содержимое файла в байты
                byte[] data = System.Text.Encoding.UTF8.GetBytes(fileContent);

                // Отправляем данные на сервер
                stream.Write(data, 0, data.Length);

                Console.WriteLine("File sent successfully");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
