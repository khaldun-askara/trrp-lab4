using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;
using System.Linq;
using System.Text.Json;
using RabbitMQ.Client;

namespace crocodileAppBack
{
    // класс сообщения в игровом чате
    [Serializable]
    public class ChatMessage
    {
        public string nickname { get; set; }
        public string message { get; set; }

        public ChatMessage() { }

        public ChatMessage(string nickname, string message)
        {
            this.nickname = nickname;
            this.message = message;
        }

        public string ToJson()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string tmp = JsonSerializer.Serialize(this, options);
            Console.WriteLine("ChatMessage" + tmp);
            return tmp;
        }
    }
    // класс сообщения вебсокета type определяет, какого типа сообщение было прислано
    [Serializable]
    public class WSMessage
    {
        public string type { get; set; }
        public string message { get; set; }
        public WSMessage() { }

        public WSMessage(string type, string message)
        {
            this.type = type;
            this.message = message;
        }
        public string ToJson()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string tmp = JsonSerializer.Serialize(this, options);
            Console.WriteLine("WSMessage" + tmp + type + message);
            return tmp;
        }
    }
    public static class XLExtensions
    {
        public static IEnumerable<string> SplitInGroups(this string original, int size)
        {
            var p = 0;
            var l = original.Length;
            while (l - p > size)
            {
                yield return original.Substring(p, size);
                p += size;
            }
            yield return original.Substring(p);
        }
    }

    public class Root
    {
        public string name { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string maiden_name { get; set; }
        public string birth_data { get; set; }
        public string phone_h { get; set; }
        public string phone_w { get; set; }
        public string email_u { get; set; }
        public string email_d { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string domain { get; set; }
        public string useragent { get; set; }
        public string ipv4 { get; set; }
        public string macaddress { get; set; }
        public string plasticcard { get; set; }
        public string cardexpir { get; set; }
        public int bonus { get; set; }
        public string company { get; set; }
        public string color { get; set; }
        public string uuid { get; set; }
        public int height { get; set; }
        public double weight { get; set; }
        public string blood { get; set; }
        public string eye { get; set; }
        public string hair { get; set; }
        public string pict { get; set; }
        public string url { get; set; }
        public string sport { get; set; }
        public string ipv4_url { get; set; }
        public string email_url { get; set; }
        public string domain_url { get; set; }
    }

    // крококлиент, хранит дцпклиента и его ник, выдаёт ники
    class CrocodileClient
    {
        public TcpClient client;
        public String nickname;

        public CrocodileClient(TcpClient client)
        {
            this.client = client;
            nickname = GetStupidNickname();
        }

        override public string ToString()
        {
            return nickname;
        }

        public static string GetStupidNickname()
        {
            try
            {
                WebRequest request = WebRequest.Create("https://api.namefake.com/ukrainian-ukraine/");
                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        Root temp = JsonSerializer.Deserialize<Root>(reader.ReadToEnd());
                        return temp.name;
                    }
                }
            }
            catch (Exception e)
            {
                return "Банановый крокодильчик №" + new Random().Next(999999);
            }
        }

    }

    class Server
    {
        // эти штуи делают красиво и позволяет закрыть все потоки нахуй при необходимости
        private static CancellationTokenSource SocketLoopTokenSource;
        private static CancellationTokenSource ListenerLoopTokenSource;
        private static TcpListener Listener;
        // мьютексы для текущего слова
        private static Mutex mut = new Mutex();
        private static Mutex mut_chat = new Mutex();
        private static Mutex mut_closing = new Mutex();
        // текущее загаданное слово
        private static string Current_word;
        // идентификатор рисующего
        private static int drawingClienID;
        // тут живут крококлиенты! ConcurrentDictionary вроде должен делать красиво при работе с потоками
        private static ConcurrentDictionary<int, CrocodileClient> Clients = new ConcurrentDictionary<int, CrocodileClient>();

        // это счётчик клиентов, который можно трогать только безопасно!!!!!!!!!!!
        private static int ClientCounter = 0;

        // делает магию при отправке и оно работает!!!!!!
        public static void SendMessageToClient(TcpClient client, string msg)
        {
            NetworkStream stream = client.GetStream();
            Queue<string> que = new Queue<string>(msg.SplitInGroups(125));
            int len = que.Count;

            while (que.Count > 0)
            {
                var header = GetHeader(
                    que.Count > 1 ? false : true,
                    que.Count == len ? false : true
                );

                byte[] list = Encoding.UTF8.GetBytes(que.Dequeue());
                header = (header << 7) + list.Length;
                stream.Write(IntToByteArray((ushort)header), 0, 2);
                stream.Write(list, 0, list.Length);
            }
        }

        protected static byte[] IntToByteArray(ushort value)
        {
            var ary = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(ary);
            }

            return ary;
        }

        protected static int GetHeader(bool finalFrame, bool contFrame)
        {
            int header = finalFrame ? 1 : 0;//fin: 0 = more frames, 1 = final frame
            header = (header << 1) + 0;//rsv1
            header = (header << 1) + 0;//rsv2
            header = (header << 1) + 0;//rsv3
            header = (header << 4) + (contFrame ? 0 : 1);//opcode : 0 = continuation frame, 1 = text
            header = (header << 1) + 0;//mask: server -> client = no mask

            return header;
        }

        // тут надо сделать доставание с сервака
        public static string GetRandomWord()
        {
            return "загаданное слово " + new Random().Next(100);
        }

        // если клиент присоединился первым, он автоматически становится рисующим. Иначе при подключении человек становится угадывающим.
        public static void NewClientRole(TcpClient client, int clientID)
        {
            // если у нас клиент первый
            if (Clients.Count == 1)
            {
                mut.WaitOne();
                Current_word = GetRandomWord();
                drawingClienID = clientID;
                // выдаём рисователю роль рисователя и слово для рисования
                SendMessageToClient(client, (new WSMessage("drawing_role", "true").ToJson()));
                SendMessageToClient(client, (new WSMessage("word", Current_word).ToJson()));
                mut.ReleaseMutex();
            }
            // если не первый, то он угадыватель
            else SendMessageToClient(client, (new WSMessage("drawing_role", "false").ToJson()));
        }

        // запускает работу лисенера
        public static void StartServer()
        {
            SocketLoopTokenSource = new CancellationTokenSource();
            ListenerLoopTokenSource = new CancellationTokenSource();
            Listener = new TcpListener(IPAddress.Parse(config.Default.ip), config.Default.port);
            Listener.Start();
            Task.Run(() => ListenerProcessingLoopAsync().ConfigureAwait(false));
        }

        // при подсоединении нового клиента выдаёт ему роль, добавляет его в список клиентов и запускает ему отдельный поток, читающий данные
        private static async Task ListenerProcessingLoopAsync()
        {
            var cancellationToken = ListenerLoopTokenSource.Token;
            try
            {
                // это чтобы можно было прервать снаружи
                while (!cancellationToken.IsCancellationRequested)
                {
                    TcpClient client = Listener.AcceptTcpClient();
                    int clientId = Interlocked.Increment(ref ClientCounter);
                    // выдаём ник
                    CrocodileClient crococlient = new CrocodileClient(client);
                    // добавляем клиента в списочек клиентов
                    Clients.TryAdd(clientId, crococlient);
                    Console.WriteLine("A client " + crococlient.ToString() + " connected.");
                    Console.WriteLine($"Websocket {clientId}: New connection.");

                    // каждый клиент получает обработчик
                    _ = Task.Run(() => ClientHandle(crococlient, clientId).ConfigureAwait(false));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " from ListenerProcessingLoopAsync");
            }
        }

        // отменяет все процессы, обрабатывающие потоки от клиентов + отменяет функцию лисенера
        public static void StopAsync()
        {
            Console.WriteLine("\nServer is stopping.");
            SocketLoopTokenSource.Cancel();
            ListenerLoopTokenSource.Cancel();   // safe to stop now that sockets are closed
            Listener.Stop();
        }

        // получает статус дцп клиента
        public static TcpState GetState(TcpClient tcpClient)
        {
            // Console.WriteLine("GetState open");
            var foo = IPGlobalProperties.GetIPGlobalProperties()
            .GetActiveTcpConnections()
            .SingleOrDefault(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint)
                             && x.RemoteEndPoint.Equals(tcpClient.Client.RemoteEndPoint));
            // Console.WriteLine("GetState close");
            return foo != null ? foo.State : TcpState.Unknown;
        }

        private static async Task ClientHandle(CrocodileClient crococlient, int id)
        {
            var loopToken = SocketLoopTokenSource.Token;
            TcpClient client = crococlient.client;
            Console.WriteLine("booop");
            NetworkStream stream = client.GetStream();

            // enter to an infinite cycle to be able to handle every change in stream
            while (!loopToken.IsCancellationRequested)
            {
                while (!stream.DataAvailable) ;
                while (client.Available < 3) ; // match against "get"

                byte[] bytes = new byte[client.Available];
                stream.Read(bytes, 0, client.Available);
                string s = Encoding.UTF8.GetString(bytes);

                // клиент и сервер переглядываются
                if (Regex.IsMatch(s, "^GET", RegexOptions.IgnoreCase))
                {
                    Console.WriteLine("=====Handshaking from client=====\n{0}", s);

                    // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                    // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                    // 3. Compute SHA-1 and Base64 hash of the new value
                    // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                    string swk = Regex.Match(s, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                    string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                    byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                    string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

                    // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
                    byte[] response = Encoding.UTF8.GetBytes(
                        "HTTP/1.1 101 Switching Protocols\r\n" +
                        "Connection: Upgrade\r\n" +
                        "Upgrade: websocket\r\n" +
                        "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n");

                    stream.Write(response, 0, response.Length);

                    // выдаём роль
                    NewClientRole(client, id);
                    SendToAll((new WSMessage("chat", (new ChatMessage("", crococlient.ToString() + " вошёл в чат!")).ToJson()).ToJson()));
                }
                else
                {
                    // ой а если клиент помер?
                    if (GetState(client) == TcpState.CloseWait)
                    {

                        client.Close();
                        Console.WriteLine(id + " is closed");
                        Clients.TryRemove(id, out crococlient);
                        foreach (var cl in Clients)
                            Console.WriteLine(cl.Key);

                        SendToAll((new WSMessage("chat", (new ChatMessage("", crococlient.ToString() + " помер")).ToJson()).ToJson()));
                        // если помер рисовака, рисоваку надо выбрать случайно из оставшихся, загадать новое слово и раздать роли
                        if (Clients.Count != 0 && id == drawingClienID)
                        {
                            int randomindex = new Random().Next(Clients.Count);
                            mut_closing.WaitOne();
                            Current_word = GetRandomWord();
                            var temp = Clients.ElementAt(randomindex);
                            SendMessageToClient(temp.Value.client, (new WSMessage("drawing_role", "true").ToJson()));
                            drawingClienID = temp.Key;
                            SendMessageToClient(temp.Value.client, (new WSMessage("word", Current_word).ToJson()));
                            SendToOthers((new WSMessage("drawing_role", "false").ToJson()), drawingClienID);
                            mut_closing.ReleaseMutex();
                        }
                        return;
                    }
                    bool fin = (bytes[0] & 0b10000000) != 0,
                        mask = (bytes[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"

                    int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                        msglen = bytes[1] - 128, // & 0111 1111
                        offset = 2;

                    if (msglen == 126)
                    {
                        // was ToUInt16(bytes, offset) but the result is incorrect
                        msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                        offset = 4;
                    }
                    else if (msglen == 127)
                    {
                        Console.WriteLine("TODO: msglen == 127, needs qword to store msglen");
                        // i don't really know the byte order, please edit this
                        // msglen = BitConverter.ToUInt64(new byte[] { bytes[5], bytes[4], bytes[3], bytes[2], bytes[9], bytes[8], bytes[7], bytes[6] }, 0);
                        // offset = 10;    
                    }

                    if (msglen == 0)
                        Console.WriteLine("msglen == 0");
                    else if (mask)
                    {
                        byte[] decoded = new byte[msglen];
                        byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                        offset += 4;

                        for (int i = 0; i < msglen; ++i)
                            decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

                        string text = Encoding.UTF8.GetString(decoded);
                        // получаем сообщение вебсокета
                        WSMessage wsmessage = JsonSerializer.Deserialize<WSMessage>(text);

                        // и в соответствии с типом...
                        switch (wsmessage.type)
                        {
                            // если отправляется линия (то есть рисовулькин рисует), то её надо отправить всем остальным
                            case "lineart":
                                SendToOthers(text, id);
                                break;
                            // если кто-то что-то пишет в чат, это рассылается всем (в том числе отправляющему)
                            case "chat":
                                SendToAll(new WSMessage("chat", (new ChatMessage(crococlient.ToString(), wsmessage.message)).ToJson()).ToJson());
                                Console.WriteLine("сообщение получено");
                                mut_chat.WaitOne();
                                // если в сообщении есть правильный ответ, необходимо сообщить о победе всем, загадать новое слово, отправить новую роль угадавшему
                                // сохранить новую роль рисоваки, остальным раздать роли угаывающих
                                if (Regex.IsMatch(wsmessage.message, Current_word, RegexOptions.IgnoreCase))
                                {
                                    SendToAll((new WSMessage("chat", (new ChatMessage("", "Правильное слово: " + Current_word + ". Победитель: " + crococlient.ToString())).ToJson()).ToJson()));
                                    Current_word = GetRandomWord();
                                    SendMessageToClient(client, (new WSMessage("drawing_role", "true").ToJson()));
                                    drawingClienID = id;
                                    SendMessageToClient(client, (new WSMessage("word", Current_word).ToJson()));
                                    SendToOthers((new WSMessage("drawing_role", "false").ToJson()), id);
                                }
                                mut_chat.ReleaseMutex();
                                break;
                            case "suggestion":
                                SendToMQ(wsmessage.message);
                                break;

                        }

                        Console.WriteLine(id + " отправил {0}", text);
                        // string resp_str = id + " отправил " + text;
                        // foreach (var other_client in Clients)
                        // {
                        //     if (other_client.Key != id)
                        //         SendMessageToClient(other_client.Value.client, id + " отправил " + text);
                        // }
                    }
                    else
                        Console.WriteLine("mask bit not set");

                    Console.WriteLine();
                }
            }
        }

        public static void SendToOthers(string message, int cur_id)
        {
            foreach (var other_client in Clients)
            {
                if (other_client.Key != cur_id)
                    SendMessageToClient(other_client.Value.client, message);
            }
        }

        public static void SendToAll(string message)
        {
            foreach (var client in Clients)
            {
                SendMessageToClient(client.Value.client, message);
            }
        }

        public static string DeleteExtraSpaces(string str)
        {
            return Regex.Replace(DeleteBorderSpaces(str), "\\s+", " ");
        }

        public static string DeleteBorderSpaces(string str)
        {
            if (str is null)
                return "";
            return Regex.Replace(Regex.Replace(str, "^\\s+", ""), "\\s+$", "");
        }

        public static void SendToMQ(string message)
        {
            message = DeleteExtraSpaces(message.ToLower());
            var factory = new ConnectionFactory() { Uri = new Uri(config.Default.MQ) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "suggestions",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "suggestions",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }

        public static void Main()
        {
            try
            {
                StartServer();
                Console.WriteLine("Press any key to exit...\n");
                Console.ReadLine();
                StopAsync();
            }
            catch (OperationCanceledException)
            {
                // this is normal when tasks are canceled, ignore it
            }
        }
    }
}