using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using Newtonsoft.Json;

namespace MonsterTradingCards.REST_Interface;

public class Server
{
    private readonly string connectionString;
    private readonly TcpListener listener;
    private readonly int port = 10001;
    private List<string> tokenlist = new List<string>();
    private string token;

    public Server(string con)
    {
        connectionString = con;
        listener = new TcpListener(IPAddress.Loopback, port);
        tokenlist.Add("admin-mtcgToken");
    }

    public void RunServer()
    {
        listener.Start();
        Console.WriteLine("Server running. Waiting for Requests...");

        while (true)
            try
            {
                using (var client = listener.AcceptTcpClient())
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                using (var writer = new StreamWriter(stream) { AutoFlush = true })
                {
                    var requestLine = reader.ReadLine();
                    if (!string.IsNullOrEmpty(requestLine))
                    {
                        var parts = requestLine.Split(' ');
                        if (parts.Length == 3)
                        {
                            var httpMethod = parts[0];
                            var path = parts[1];
                            Console.WriteLine($"Received request: {httpMethod} {path}");

                            var userRepo = new UserRepo(connectionString);

                            if (httpMethod == "GET")
                                GetMethods(path, writer, reader, userRepo);
                            else if (httpMethod == "POST")
                                PostMethods(path, reader, writer, userRepo);
                            else if (httpMethod == "PUT")
                                PutMethods(path, reader, writer, userRepo);
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

    public void GetMethods(string path, StreamWriter writer, StreamReader reader, UserRepo userRepo)
    {
        var objectResponse = "";
        var responseType = "";
        var requestBody = ReadToEnd(ReadLength(reader), reader);
        var users = (List<User>)userRepo.GetAll();

        if (path.StartsWith("/users/"))
        {
            //Get /{username}
            var username = path.Substring("/users/".Length);
            var foundUser = users.FirstOrDefault(user => user.Username == username);

            if ((token != username + "-mtcgToken" && token != "admin-mtcgToken")||!tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else if (foundUser != null)
            {
                objectResponse = JsonConvert.SerializeObject(foundUser);
                responseType = "Data successfully retrieved";
            }
            else
            {
                responseType = "User not found.";
            }
        }
        else if (path == "/deck")
        {
        }
        else if (path == "/cards")
        {
        }
        else if (path == "/stats")
        {
        }
        else if (path == "/scoreboard")
        {
        }
        else if (path == "/tradings")
        {
        }
        else
        {
            responseType = "Bad Request";
        }

        CreateAndSendResponse(responseType, writer, objectResponse);
    }

    public void PostMethods(string path, StreamReader reader, StreamWriter writer, UserRepo userRepo)
    {
        var objectResponse = "";
        var responseType = "";
        var requestBody = ReadToEnd(ReadLength(reader), reader);
        

        var users = (List<User>)userRepo.GetAll();

        if (path == "/users")
        {
            var postUser = JsonConvert.DeserializeObject<User>(requestBody);
            //using (var requestBodyReader = new StreamReader(reader.BaseStream)) {
            if (postUser == null)
            {
                responseType = "Invalid JSON data";
            }
            else
            {
                var foundUser = users.FirstOrDefault(user => user.Username == postUser.Username);

                if (foundUser == null)
                {
                    userRepo.AddUserCredentials(postUser);
                    responseType = "User successfully created";
                }
                else
                {
                    responseType = "User with same username already registered";
                }
                // }
            }
        }
        else if (path == "/sessions")
        {
            var postUser = JsonConvert.DeserializeObject<User>(requestBody);
            if (postUser == null)
            {
                responseType = "Invalid JSON data";
            }
            else
            {
                var foundUser = users.FirstOrDefault(user => user.Username == postUser.Username);

                if (foundUser != null && foundUser.Password == postUser.Password)
                {
                    tokenlist.Add(foundUser.Username + "-mtcgToken");
                    objectResponse = JsonConvert.SerializeObject(foundUser.Username + "-mtcgToken");
                    responseType = "User login successful";
                }
                else
                {
                    responseType = "Invalid username/password provided";
                }
            }
        }
        else if (path == "/packages")
        {
            HashSet<Card> package = new HashSet<Card>();
            List<Card> postCards = JsonConvert.DeserializeObject<List<Card>>(requestBody);
            if (postCards == null)
            {
                responseType = "Invalid JSON data";
            }
            else
            {

                if (token != "admin-mtcgToken")
                {
                    responseType = "Access token is missing or invalid";
                }
                else foreach (var card in postCards)
                    {
                        if (package.Add(card))
                        {
                            responseType = "Package and cards successfully created";
                        }
                        else
                        {
                            responseType = "At least one card in the packages already exists";
                            break;
                        }
                    }
            if(responseType== "Package and cards successfully created")
                {

                }
            }
            
        }
        else if (path == "/tradings")
        {
        }
        else if (path == "/transactions/packages")
        {

        }
        else if (path == "/battles")
        {
        }
        else if (path == "/tradings")
        {
        }
        else if (path.StartsWith("/tradings/"))
        {
        }
        else
        {
            responseType = "Bad Request";
        }

        CreateAndSendResponse(responseType, writer, objectResponse);
    }

    public void PutMethods(string path, StreamReader reader, StreamWriter writer, UserRepo userRepo)
    {
        var objectResponse = "";
        var responseType = "";
        if (path.StartsWith("/users/"))
        {
            var requestBody = ReadToEnd(ReadLength(reader), reader);
            var postUser = JsonConvert.DeserializeObject<User>(requestBody);

            if (postUser == null)
            {
                responseType = "Invalid JSON data";
            }
            else
            {
                var username = path.Substring("/users/".Length);
                var users = (List<User>)userRepo.GetAll();
                var foundUser = users.FirstOrDefault(user => user.Username == username);

                if ((token != username + "-mtcgToken" && token != "admin-mtcgToken") || !tokenlist.Contains(token))
                {
                    responseType = "Access token is missing or invalid";
                }
                else if (foundUser != null)
                {
                    foundUser.Bio = postUser.Bio;
                    foundUser.Name = postUser.Name;
                    foundUser.Image = postUser.Image;
                    userRepo.Update(foundUser);
                    responseType = "Data successfully retrieved";
                }
                else
                {
                    responseType = "User not found.";
                }
            }
        }
        else if (path == "/deck")
        {

        }
        else
        {
            responseType = "Bad Request";
        }

        CreateAndSendResponse(responseType, writer, objectResponse);
    }

    public void DeleteMethods(string path, StreamWriter writer, UserRepo userRepo)
    {
        var objectResponse = "";
        var responseType = "";
        if (path.StartsWith("/tradings/")) objectResponse = "Delete /tradings/{tradingdealid}";

        CreateAndSendResponse(responseType, writer, objectResponse);
    }

    private void CreateAndSendResponse(string response, StreamWriter writer, string objectResponse)
    {
        try
        {
            if (response == "User not found.")
            {
                //Code 404
                writer.WriteLine("HTTP/1.1 404 Not Found");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "User successfully created"||response== "Package and cards successfully created")
            {
                //Code 201
                writer.WriteLine("HTTP/1.1 201 Created");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "User with same username already registered"||response== "At least one card in the packages already exists")
            {
                //Code 409
                writer.WriteLine("HTTP/1.1 409 Conflict");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "Access token is missing or invalid" ||
                     response == "Invalid username/password provided")
            {
                //Code 401
                writer.WriteLine("HTTP/1.1 401 Unauthorized");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "Data successfully retrieved" ||
                     response == "User successfully updated" ||
                     response == "User login successful")
            {
                //Code 200
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "Provided user is not admin")
            {
                //Code 403
                writer.WriteLine("HTTP/1.1 403 Forbidden");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "The request was fine, but the user doesn't have any cards")
            {
                //Code 204
                writer.WriteLine("HTTP/1.1 204 No Content");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "Invalid JSON data" ||
                     response == "Bad Request" )
            {
                //Code 400
                writer.WriteLine("HTTP/1.1 400 Bad Request");
                writer.WriteLine("Content-Type: text/plain");
            }

            writer.WriteLine();
            writer.WriteLine(response);
            writer.WriteLine(objectResponse);
                token = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in CreateAndSendResponse(): " + ex.Message);
        }
    }

    private string ReadToEnd(int len, StreamReader reader)
    {
        var data = new StringBuilder(200);
        if (len > 0)
        {
            var chars = new char[1024];
            var bytesReadTotal = 0;
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
        string? line;
        var content_length = 0; // we need the content_length later, to be able to read the HTTP-content
        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("Authorization:"))
            {
                var authorizationHeader = line;
                // Extract the Bearer Token from the Authorization header
                var match = Regex.Match(authorizationHeader, "Bearer\\s+(\\S+)");
                if (match.Success)
                    token = match.Groups[1].Value;
            }
                Console.WriteLine(line);
                if (line == "") break; // empty line indicates the end of the HTTP-headers

                // Parse the header
                var parts = line.Split(':');
                if (parts.Length == 2 && parts[0] == "Content-Length") content_length = int.Parse(parts[1].Trim());
            }
        
        return content_length;
    }
}

