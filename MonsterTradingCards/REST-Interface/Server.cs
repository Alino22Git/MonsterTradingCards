using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MonsterTradingCards.REST_Interface
{
    public class Server
    {
        private readonly string ConnectionString;
        private readonly int Port = 10001;
        private readonly TcpListener listener;

        public Server(string con)
        {
            ConnectionString = con;
            listener = new TcpListener(IPAddress.Loopback, Port);
        }

        public void RunServer()
        {
            listener.Start();
            Console.WriteLine($"Server running. Waiting for Requests...");

            while (true)
            {
                try
                {
                    using (TcpClient client = listener.AcceptTcpClient())
                    using (NetworkStream stream = client.GetStream())
                    using (StreamReader reader = new StreamReader(stream))
                    using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
                    {
                        string requestLine = reader.ReadLine();
                        if (!string.IsNullOrEmpty(requestLine))
                        {
                            string[] parts = requestLine.Split(' ');
                            if (parts.Length == 3)
                            {
                                string httpMethod = parts[0];
                                string path = parts[1];

                                Console.WriteLine($"Received request: {httpMethod} {path}");

                                var userRepo = new UserRepo(ConnectionString);

                                if (httpMethod == "GET")
                                    GetMethods(path, writer, userRepo);
                                else if (httpMethod == "POST")
                                    PostMethods(path, reader, writer, userRepo);
                                else if (httpMethod == "PUT")
                                    PutMethods(path, writer, userRepo);
                                else if (httpMethod == "DELETE")
                                    DeleteMethods(path, writer, userRepo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in RunServer(): " + ex.Message);
                }
            }
        }

        public void GetMethods(string path, StreamWriter writer, UserRepo userRepo)
        {
            var objectResponse = "";
            if (path.StartsWith("/users/"))
            {
                //Get /{username}
                var username = path.Substring("/users/".Length);
                var users = (List<User>)userRepo.GetAll();

                //Using LINQ to save the user in foundUser
                var foundUser = users.FirstOrDefault(user => user.Username == username);

                //USER NEEDS TO AUTHORIZE !!! User der sich selbst sucht oder Admin

                if (foundUser != null)
                {
                    //Send created user
                    objectResponse = JsonConvert.SerializeObject(foundUser);
                    writer.WriteLine("HTTP/1.1 200 OK");
                    writer.WriteLine("Content-Type: application/json");
                    writer.WriteLine();
                }
                else
                {
                    writer.WriteLine("HTTP/1.1 404 Not Found");
                    writer.WriteLine();
                }
            }
            else if (path == "/deck")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine();
                writer.WriteLine("Get /deck");
            }
            else if (path == "/cards")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine();
                writer.WriteLine("Get /cards");
            }
            else if (path == "/stats")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Get /stats");
            }
            else if (path == "/scoreboard")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Get /scoreboard");
            }
            else if (path == "/tradings")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Get /tradings");
            }

            CreateAndSendResponse(writer, objectResponse);
        }

        public void PostMethods(string path, StreamReader reader, StreamWriter writer, UserRepo userRepo)
        {
            var objectResponse = "";
            if (path == "/TEST")
            {
                using (var requestBodyReader = new StreamReader(reader.BaseStream))
                {

                    var requestBody = ReadToEnd(ReadLength(reader),reader);
                    var user = JsonConvert.DeserializeObject<User>(requestBody);

                    writer.WriteLine("HTTP/1.1 200 OK");
                    writer.WriteLine("Content-Type: text/plain");
                    writer.WriteLine();
                    writer.WriteLine($"POST-Anfrage für /createUser empfangen: Username = {user.Username}, Password = {user.Password}");
                }
            }
            else if (path.StartsWith("/users"))
            {
                
                using (var requestBodyReader = new StreamReader(reader.BaseStream))
                {
                    try
                    {
                        var requestBody = requestBodyReader.ReadToEnd();
                        Console.WriteLine("Received request body: " + requestBody);

                        var postUser = JsonConvert.DeserializeObject<User>(requestBody);

                        if (postUser == null)
                        {
                            Console.WriteLine("Could not convert JSON to User!");
                            // Hier können Sie eine geeignete Fehlerantwort an den Client senden
                            writer.WriteLine("HTTP/1.1 400 Bad Request");
                            writer.WriteLine("Content-Type: text/plain");
                            writer.WriteLine();
                            writer.WriteLine("Invalid JSON data");
                        }
                        else
                        {

                            // Hier können Sie eine erfolgreiche Antwort an den Client senden
                            writer.WriteLine("HTTP/1.1 201 Created");
                            writer.WriteLine("Content-Type: text/plain");
                            writer.WriteLine();
                            writer.WriteLine("User successfully created");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error reading or processing request body: " + ex.Message);
                        // Hier können Sie eine geeignete Fehlerantwort an den Client senden
                        writer.WriteLine("HTTP/1.1 500 Internal Server Error");
                        writer.WriteLine("Content-Type: text/plain");
                        writer.WriteLine();
                        writer.WriteLine("An error occurred while processing the request");
                    }
                    finally
                    {
                        requestBodyReader.Close(); // Hier den Stream schließen
                    }
                }
            }
            else if (path == "/sessions")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine();
                writer.WriteLine("Post /sessions");
            }
            else if (path == "/transactions/packages")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Post /transactions/packages");
            }
            else if (path == "/tradings")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Post /tradings");
            }
            else if (path == "/packages")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Post /packages");
            }
            else if (path == "/battles")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Post /battles");
            }
            else if (path == "/tradings")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Post /tradings");
            }
            else if (path.StartsWith("/tradings/"))
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Post /tradings/{tradingdealid}");
            }

            CreateAndSendResponse(writer, objectResponse);
        }

        public void PutMethods(string path, StreamWriter writer, UserRepo userRepo)
        {
            var objectResponse = "";
            if (path.StartsWith("/users/"))
            {
                var username = path.Substring("/users/".Length);
                var users = (List<User>)userRepo.GetAll();

                var foundUser = users.FirstOrDefault(user => user.Username == username);

                //USER NEEDS TO AUTHORIZE !!! User der sich selbst sucht oder Admin

                if (foundUser != null)
                {
                    objectResponse = JsonConvert.SerializeObject(foundUser);
                    writer.WriteLine("HTTP/1.1 200 OK");
                    writer.WriteLine("Content-Type: application/json");
                    writer.WriteLine("Data successfully retrieved");
                }
                else
                {
                    writer.WriteLine("HTTP/1.1 404 Not Found");
                }

                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Put /users/{username}");
            }
            else if (path == "/deck")
            {
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
                writer.WriteLine("Put /deck");
            }

            CreateAndSendResponse(writer, objectResponse);
        }

        public void DeleteMethods(string path, StreamWriter writer, UserRepo userRepo)
        {
            var objectResponse = "";

            if (path.StartsWith("/tradings/"))
            {
                objectResponse = "Delete /tradings/{tradingdealid}";
            }

            CreateAndSendResponse(writer, objectResponse);
        }

        private void CreateAndSendResponse(StreamWriter writer, string objectResponse)
        {
            writer.WriteLine();
            writer.WriteLine(objectResponse);
            Console.WriteLine("Pushed Response");
        }

        private String ReadToEnd(int len, StreamReader reader)
        {
            var data = new StringBuilder(200);
            if (len > 0)
            {
                char[] chars = new char[1024];
                int bytesReadTotal = 0;
                while (bytesReadTotal < len)
                {
                    var bytesRead = reader.Read(chars, 0, chars.Length);
                    bytesReadTotal += bytesRead;
                    if (bytesRead == 0)
                        break;
                    data.Append(chars, 0, bytesRead);
                }
                Console.WriteLine(data.ToString());
            }
            return data.ToString();
        }

        private int ReadLength(StreamReader reader)
        {
            String? line;
            int content_length = 0; // we need the content_length later, to be able to read the HTTP-content
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
                if (line == "")
                {
                    break;  // empty line indicates the end of the HTTP-headers
                }

                // Parse the header
                var parts = line.Split(':');
                if (parts.Length == 2 && parts[0] == "Content-Length")
                {
                    content_length = int.Parse(parts[1].Trim());
                }
            }
            return content_length;

        }
    }
}