using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using Newtonsoft.Json;

namespace MonsterTradingCards.REST_Interface;

public class Server
{
    private readonly string connectionString;
    private readonly TcpListener listener;
    private readonly int port = 10001;
    private string token;
    private readonly List<string> tokenlist = new();

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

                            var dbRepo = new DbRepo(connectionString);

                            if (httpMethod == "GET")
                                GetMethods(path, writer, reader, dbRepo);
                            else if (httpMethod == "POST")
                                PostMethods(path, reader, writer, dbRepo);
                            else if (httpMethod == "PUT")
                                PutMethods(path, reader, writer, dbRepo);
                            else if (httpMethod == "DELETE")
                                DeleteMethods(path, writer, dbRepo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in RunServer(): " + ex.Message);
            }
    }

    public void GetMethods(string path, StreamWriter writer, StreamReader reader, DbRepo dbRepo)
    {
        var objectResponse = "";
        var responseType = "";
        var requestBody = ReadToEnd(ReadLength(reader), reader);
        var users = (List<User>)dbRepo.GetAllUsers();

        if (path.StartsWith("/users/"))
        {
            //Get /{username}
            var username = path.Substring("/users/".Length);
            var foundUser = users.FirstOrDefault(user => user.Username == username);

            if ((token != username + "-mtcgToken" && token != "admin-mtcgToken") || !tokenlist.Contains(token))
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
        else if (path == "/cards")
        {
            User foundUser = null;
            if (token != null)
            {
                var name = token.Split('-');
                foundUser = users.FirstOrDefault(user => user.Username == name[0]);
            }

            if (token == "admin-mtcgToken" || !tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else
            {
                var userCards = (List<Card>)dbRepo.UserGetCards(foundUser);
                if (userCards != null)
                {
                    responseType = "The user has cards, the response contains these";
                    objectResponse = JsonConvert.SerializeObject(userCards);
                }
                else
                {
                    responseType = "The request was fine, but the user doesn't have any cards";
                }
            }
        }
        else if (path == "/deck")
        {
            User foundUser = null;
            if (token != null)
            {
                var name = token.Split('-');
                foundUser = users.FirstOrDefault(user => user.Username == name[0]);
            }

            if (token == "admin-mtcgToken" || !tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else
            {
                var userDeck = (List<Card>)dbRepo.UserGetDeck(foundUser);
                if (userDeck != null)
                {
                    responseType = "The deck has cards, the response contains these";
                    objectResponse = JsonConvert.SerializeObject(userDeck);
                }
                else
                {
                    responseType = "The request was fine, but the user doesn't have any cards";
                }
            }
        }
        else if (path == "/stats")
        {
            User foundUser = null;
            if (token != null)
            {
                var name = token.Split('-');
                foundUser = users.FirstOrDefault(user => user.Username == name[0]);
            }

            if (token == "admin-mtcgToken" || !tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else
            {
                var amountofCards = dbRepo.UserGetCards(foundUser).Count();
                int wins = foundUser.Battles - (foundUser.Battles - (foundUser.Elo / 3)) / 2;
                int losses = foundUser.Battles - wins;
                objectResponse = JsonConvert.SerializeObject("Stats from User:"+foundUser.Username +"\nBattles played:"+foundUser.Battles + "\nWins:" + wins + "\nLosses:" + losses +"\nCurrent Elo:" + foundUser.Elo + "\nCurrent amount of Cards:"+amountofCards);
                responseType = "The stats could be retrieved successfully.";
            }
        }
        else if (path == "/scoreboard")
        {
            if (!tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else
            {
                List<String> scoreboard = new List<String>();
                foreach (var u in users)
                {
                    int wins = u.Battles - (u.Battles - (u.Elo / 3)) / 2;
                    int losses = u.Battles - wins;
                    scoreboard.Add("Scoreboard\nUsername:"+u.Username+"\nCurrent Elo:"+u.Elo+"\nWins:"+wins+"\nLosses:"+losses+"Battles:"+u.Battles);
                }
                objectResponse = JsonConvert.SerializeObject(scoreboard);
                responseType = "The scoreboard could be retrieved successfully.";
            }
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

    public void PostMethods(string path, StreamReader reader, StreamWriter writer, DbRepo dbRepo)
    {
        var objectResponse = "";
        var responseType = "";
        var requestBody = ReadToEnd(ReadLength(reader), reader);


        var users = (List<User>)dbRepo.GetAllUsers();
        var cards = (List<Card>)dbRepo.GetAllCards();

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
                    dbRepo.AddUserCredentials(postUser);
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
                    if (!tokenlist.Contains(foundUser.Username + "-mtcgToken"))
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
            var package = new HashSet<Card>();
            var postCards = JsonConvert.DeserializeObject<List<Card>>(requestBody);
            if (postCards == null)
            {
                responseType = "Invalid JSON data";
            }
            else
            {
                if (token != "admin-mtcgToken")
                {
                    responseType = "Access token is missing or invalid";
                    if (tokenlist.Contains(token)) responseType = "Provided user is not admin";
                }
                else
                {
                    foreach (var card in postCards)
                    {
                        var foundCard = cards.FirstOrDefault(dbcard => dbcard.Id == card.Id);

                        if (!package.Add(card) || foundCard != null)
                        {
                            responseType = "At least one card in the packages already exists";
                            break;
                        }
                    }

                    if (responseType != "At least one card in the packages already exists")
                    {
                        foreach (var card in package)
                            dbRepo.AddCard(card);
                        responseType = "Package and cards successfully created";
                    }
                }
            }
        }
        else if (path == "/transactions/packages")
        {
            User foundUser = null;
            if (token != null)
            {
                var name = token.Split('-');
                foundUser = users.FirstOrDefault(user => user.Username == name[0]);
            }

            if (token == "admin-mtcgToken" || !tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else if (foundUser.Money < 5)
            {
                responseType = "Not enough money for buying a card package";
            }
            else if (dbRepo.GetCardPackage() == null)
            {
                responseType = "No card package available for buying";
            }
            else
            {
                objectResponse = JsonConvert.SerializeObject((List<Card>)dbRepo.GetCardPackage());
                dbRepo.UserAquireCards(foundUser);
                responseType = "A package has been successfully bought";
                foundUser.Money -= 5;
                dbRepo.UpdateUser(foundUser);
            }
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

    public void PutMethods(string path, StreamReader reader, StreamWriter writer, DbRepo dbRepo)
    {
        var objectResponse = "";
        var responseType = "";
        var requestBody = ReadToEnd(ReadLength(reader), reader);
        var users = (List<User>)dbRepo.GetAllUsers();
        if (path.StartsWith("/users/"))
        {
            var postUser = JsonConvert.DeserializeObject<User>(requestBody);

            if (postUser == null)
            {
                responseType = "Invalid JSON data";
            }
            else
            {
                var username = path.Substring("/users/".Length);
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
                    dbRepo.UpdateUser(foundUser);
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
            var postCards = JsonConvert.DeserializeObject<List<Card>>(requestBody);

            User foundUser = null;
            if (token != null)
            {
                var name = token.Split('-');
                foundUser = users.FirstOrDefault(user => user.Username == name[0]);
            }

            if (token == "admin-mtcgToken" || !tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else if (postCards == null)
            {
                responseType = "Invalid JSON data";
            }
            else if (postCards.Count != 4)
            {
                responseType = "The provided deck did not include the required amount of cards";
            }
            else
            {
                var found = 0;
                List<Card> userCards = (List<Card>) dbRepo.UserGetCards(foundUser);

                for (int i = 0; i < userCards.Count; i++)
                {
                    for (int x = found; x < postCards.Count; x++)
                    {
                        if (userCards[i].Id.Equals(postCards[x].Id))
                        {
                            found++;
                            break;
                        }
                        if (found == 4)
                            break;
                    }
                }

                if (found==4)
                {
                    foreach (Card c in postCards)
                    {
                        c.Deck = 1;
                       Console.Write(c);
                        dbRepo.UpdateCard(c);
                    }

                    responseType = "The deck has been successfully configured";
                }
                else
                {
                    responseType ="At least one of the provided cards does not belong to the user or is not available.";
                }
            }
        }
        else
        {
            responseType = "Bad Request";
        }

        CreateAndSendResponse(responseType, writer, objectResponse);
    }

    public void DeleteMethods(string path, StreamWriter writer, DbRepo dbRepo)
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
            if (response == "User not found." || response == "No card package available for buying")
            {
                //Code 404
                writer.WriteLine("HTTP/1.1 404 Not Found");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "User successfully created" || response == "Package and cards successfully created")
            {
                //Code 201
                writer.WriteLine("HTTP/1.1 201 Created");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "User with same username already registered" ||
                     response == "At least one card in the packages already exists")
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
                     response == "User login successful" || response == "A package has been successfully bought" ||
                     response == "The user has cards, the response contains these" || 
                     response == "The deck has cards, the response contains these" ||
                     response == "The scoreboard could be retrieved successfully." ||
                     response == "The stats could be retrieved successfully.")
            {
                //Code 200
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "Provided user is not admin" ||
                     response == "Not enough money for buying a card package" || 
                     response == "At least one of the provided cards does not belong to the user or is not available." || 
                     response == "The deck has been successfully configured")
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
                     response == "Bad Request" ||
                     response == "The provided deck did not include the required amount of cards")
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