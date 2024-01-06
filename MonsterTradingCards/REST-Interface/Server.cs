using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.GameFunctions;
using MonsterTradingCards.Repository;
using Newtonsoft.Json;

namespace MonsterTradingCards.REST_Interface;

public class Server
{
    private static readonly object lockObject = new();
    private readonly string connectionString;
    private readonly TcpListener listener;
    private readonly Queue<string?> playerQueue = new();
    private readonly int port = 10001;
    private readonly List<string?> tokenlist = new();
    private string? token;
    private string? lastBattleLog;

    /// <summary>
    /// Initializes a new instance of the Server class with the specified connection string.
    /// </summary>
    /// <param name="con">The connection string for the server.</param>
    public Server(string con)
    {
        connectionString = con;
        listener = new TcpListener(IPAddress.Loopback, port);
        tokenlist.Add("admin-mtcgToken"); //Admins have special rights for some functionalities
    }

    /// <summary>
    /// Runs the server to handle incoming requests.
    /// </summary>
    public void RunServer()
    {
        listener.Start();
        Console.WriteLine("Server running. Waiting for Requests...");

        while (true)
            try
            {
                var client = listener.AcceptTcpClient(); //Starts the listening-state for the Server

                ThreadPool.QueueUserWorkItem(HandleRequest, client); //Every Request will be executed in a seperated thread
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in RunServer(): " + ex.Message);
            }
    }

    /// <summary>
    /// Handles a request from a client.
    /// </summary>
    /// <param name="oclient">The client object representing the incoming request.</param>
    private void HandleRequest(object? oclient)
    {
      
        try
        {
            var client = (TcpClient?)oclient;
            using (var stream = client!.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream) { AutoFlush = true })//Using prevents from errors -> closes Stream at the end
            {
                var requestLine = reader.ReadLine();
                if (!string.IsNullOrEmpty(requestLine))
                {
                    var parts = requestLine.Split(' ');
                    if (parts.Length == 3)
                    {
                        var httpMethod = parts[0];
                        var path = parts[1];

                        Console.WriteLine($"Received request: {httpMethod} {path}"); //Request log in Console
                        var dbRepo = new DbRepo(connectionString);

                        if (httpMethod == "GET") 
                            GetMethods(path, writer, reader, dbRepo);
                        else if (httpMethod == "POST")
                            PostMethods(path, reader, writer, dbRepo);
                        else if (httpMethod == "PUT")
                            PutMethods(path, reader, writer, dbRepo);
                        else if (httpMethod == "DELETE")
                            DeleteMethods(path,reader, writer, dbRepo);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRequest(): " + ex.Message);
        }
    }

    /// <summary>
    /// Retrieves methods based on the specified path and sends the response to the client.
    /// </summary>
    /// <param name="path">The path to determine the methods to retrieve.</param>
    /// <param name="writer">The StreamWriter for sending the response to the client.</param>
    /// <param name="reader">The StreamReader for reading client requests.</param>
    /// <param name="dbRepo">The database repository for method retrieval.</param>
    public void GetMethods(string path, StreamWriter writer, StreamReader reader, DbRepo dbRepo)
    {
        var objectResponse = "";
        var responseType = "Bad Request";
        var requestBody = ReadToEnd(ReadLength(reader), reader);
        var users = (List<User>)dbRepo.GetAllUsers();

        if (path.StartsWith("/users/"))
        {
            var username = path.Substring("/users/".Length); //Get /{username}
            var foundUser = users.FirstOrDefault(user => user.Username == username);

            if ((token != username + "-mtcgToken" && token != "admin-mtcgToken") || !tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else if (foundUser != null)
            {
                objectResponse = JsonConvert.SerializeObject(foundUser); //Serializes (object) User
                responseType = "Data successfully retrieved";
            }
            else
            {
                responseType = "User not found.";
            }
        }
        else if (path == "/cards")
        {
            User? foundUser = null;
            if (token != null)
            {
                var name = token.Split('-'); //TestUser-mtcgToken => TestUser
                foundUser = users.FirstOrDefault(user => user.Username == name[0]);
            }

            if (token == "admin-mtcgToken" || !tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else
            {
                var userCards = (List<Card>)dbRepo.UserGetCards(foundUser)!;
                if (userCards != null)
                {
                    responseType = "The user has cards, the response contains these";
                    objectResponse = JsonConvert.SerializeObject(userCards); //Serilializes (array of objects) Card
                }
                else
                {
                    responseType = "The request was fine, but the user doesn't have any cards";
                }
            }
        }
        else if (path == "/deck")
        {
            User? foundUser = null;
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
                var userDeck = (List<Card>)dbRepo.UserGetDeck(foundUser)!;
                if (userDeck != null)
                {
                    responseType = "The deck has cards, the response contains these";
                    objectResponse = JsonConvert.SerializeObject(userDeck); //Serializes (arry of objects) Card
                }
                else
                {
                    responseType = "The request was fine, but the user doesn't have any cards";
                }
            }
        }
        else if (path == "/stats")
        {
            User? foundUser = null;
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
                var amountofCards = (dbRepo.UserGetCards(foundUser) ?? throw new InvalidOperationException()).Count(); //?? controlls return of .UserGetCards (if null => throws exception)
                
                objectResponse = JsonConvert.SerializeObject("Stats from User: " + foundUser?.Username + "" +
                                                             " Battles played: " + foundUser!.Battles + "" +
                                                             " Wins: " + foundUser.Wins + "" +
                                                             " Losses: " + (foundUser.Battles - foundUser.Wins) + "" +
                                                             " Rounds played: " + foundUser.RoundsPlayed + "" +
                                                             " Rounds won: " + foundUser.RoundsWon + "" +
                                                             " Rounds lost " + foundUser.RoundsLost + "" +
                                                             " Current Elo: " + foundUser.Elo + "" +
                                                             " Current amount of Cards: " + amountofCards); //Serializes a custom made string 
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
                var scoreboard = new List<string>();
                scoreboard.Add("Scoreboard: " +
                               "");
                foreach (var u in users) //Scoreboard is created by for each loop
                {
                    scoreboard.Add(" Username: " + u.Username + "" +
                                   " Current Elo: " + u.Elo + "" +
                                   " Wins: " + u.Wins + "" +
                                   " Losses: " + (u.Battles - u.Wins) + "" +
                                   " Battles: " + u.Battles);
                }

                objectResponse = JsonConvert.SerializeObject(scoreboard);
                responseType = "The scoreboard could be retrieved successfully.";
            }
        }
        else if (path == "/tradings")
        {
            if (token == "admin-mtcgToken" || !tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else
            {
                var tradings = (List<Trade>)dbRepo.GetAllTrades()!;
                if (tradings != null)
                {
                    responseType = "There are trading deals available, the response contains these";
                    objectResponse = JsonConvert.SerializeObject(tradings); //Serializes (array of objects) Traiding
                }
                else
                {
                    responseType = "The request was fine, but there are no trading deals available";
                }
            }
        }
        
        CreateAndSendResponse(responseType, writer, objectResponse);
    }

    /// <summary>
    /// Processes and handles POST requests based on the specified path.
    /// </summary>
    /// <param name="path">The path to determine the action for the POST request.</param>
    /// <param name="reader">The StreamReader for reading client requests.</param>
    /// <param name="writer">The StreamWriter for sending the response to the client.</param>
    /// <param name="dbRepo">The database repository for handling POST requests.</param>
    public void PostMethods(string path, StreamReader reader, StreamWriter writer, DbRepo dbRepo)
    {
        var objectResponse = "";
        var responseType = "Bad Request";
        var requestBody = ReadToEnd(ReadLength(reader), reader);

        var trades = (List<Trade>)dbRepo.GetAllTrades()!;
        var users = (List<User>)dbRepo.GetAllUsers();
        var cards = (List<Card>)dbRepo.GetAllCards();

        if (path == "/users")
        {
            var postUser = JsonConvert.DeserializeObject<User>(requestBody); //Deserializes JSON into (object) User
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
            var postCards = JsonConvert.DeserializeObject<List<Card>>(requestBody); //Deserializes JSON into (List of object) Card
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
            User? foundUser = null;
            if (token != null)
            {
                var name = token.Split('-');
                foundUser = users.FirstOrDefault(user => user.Username == name[0]);
            }

            if (token == "admin-mtcgToken" || !tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else if (foundUser!.Money < 5)
            {
                responseType = "Not enough money for buying a card package";
            }
            else if (dbRepo.GetCardPackage() == null)
            {
                responseType = "No card package available for buying";
            }
            else
            {
                objectResponse = JsonConvert.SerializeObject((List<Card>)dbRepo.GetCardPackage()!); //Serializes (List of objects) Card into JSON
                dbRepo.UserAquireCards(foundUser);
                responseType = "A package has been successfully bought";
                foundUser.Money -= 5;
                dbRepo.UpdateUser(foundUser);
            }
        }
        else if (path == "/battles")
        {
            var battleWait = false;
            lock (lockObject)
            {
                if (!tokenlist.Contains(token))
                {
                    responseType = "Access token is missing or invalid";
                }
                else
                {
                    if (playerQueue.Contains(token))
                    {
                        responseType = "User can not fight against himself";
                    }
                    else
                    {
                        lastBattleLog = "no Log";
                        // Füge den aktuellen Spieler zur Warteschlange hinzu
                        playerQueue.Enqueue(token);
                        battleWait = true;
                        // Überprüfe, ob es bereits einen wartenden Spieler gibt
                        if (playerQueue.Count >= 2)
                        {
                            // Starte den Kampf zwischen den ersten beiden Spielern in der Warteschlange
                            var name = playerQueue.Dequeue()?.Split('-');
                            var player1 = users.FirstOrDefault(user => user.Username == name?[0]);
                            name = playerQueue.Dequeue()?.Split('-');
                            var player2 = users.FirstOrDefault(user => user.Username == name?[0]);
                            //GameLogic.StartBattle((List<Card>) dbRepo.UserGetDeck(player1), (List<Card>)dbRepo.UserGetDeck(player2),dbRepo);
                            
                            lastBattleLog = GameLogic.StartBattle(player1, player2, dbRepo);
                            battleWait = false;
                        }
                    }
                }
                Monitor.PulseAll(lockObject);
            }

            if (responseType != "User can not fight against himself")
            {
                lock (lockObject)
                {
                    if (battleWait)
                    {
                        Monitor.Wait(lockObject);
                    }
                    responseType = lastBattleLog;
                }
            }
        }
        else if (path == "/tradings")
        {
            var postTrade = JsonConvert.DeserializeObject<Trade>(requestBody);

            if (token == "admin-mtcgToken" || !tokenlist.Contains(token))
            {
                responseType = "Access token is missing or invalid";
            }
            else if (postTrade == null)
            {
                responseType = "Invalid JSON data";
            }
            else
            {
                var name = token?.Split('-');
                var user = users.FirstOrDefault(user => user.Username == name?[0]);
                var userCards = (List<Card>)dbRepo.UserGetCards(user)!;
                bool isOwned = false;
                bool isNotInDeck = true;

                if (userCards != null)
                    foreach (Card c in userCards)
                    {
                        if (c.Id == postTrade.CardToTrade) //Checks if the card is owned by player
                        {
                            isOwned = true;
                            if (c.Deck == 1) //Checks if the card is already in a deck
                            {
                                isNotInDeck = false;
                            }
                        }
                    }

                if (isOwned == false || isNotInDeck == false)
                {
                    responseType = "The deal contains a card that is not owned by the user or locked in the deck.";
                }
                else
                {
                  

                    if (trades != null)
                    {
                        var foundTrade = trades.FirstOrDefault(trade => trade.Id == postTrade.Id);
                        if (foundTrade != null)
                        {
                            responseType = "A deal with this deal ID already exists.";
                        }
                        else
                        {
                            postTrade.UserId = user!.UserId;
                            dbRepo.AddTrade(postTrade);
                            responseType = "Trading deal successfully created";
                        }
                    }
                    else
                    {
                            postTrade.UserId = user!.UserId;
                            dbRepo.AddTrade(postTrade);
                            responseType = "Trading deal successfully created";
                        
                    }
                }
            }
        }
        else if (path.StartsWith("/tradings/"))
        {
            var tradingId = path.Substring("/tradings/".Length);
            if (trades == null)
            {
                responseType = "The provided deal ID was not found.";
                CreateAndSendResponse(responseType, writer, objectResponse);
                return;
            }
            var trade = trades.FirstOrDefault(trade => trade.Id == tradingId); //Get Trade
            var requestingUser = users.FirstOrDefault(user => user.UserId == trade?.UserId); //Get requesting user out of trade-id
            var offeredCardId = JsonConvert.DeserializeObject<Card>(requestBody); //Get offered card-id by RequestBody
            var offeredCard = cards.FirstOrDefault(cards => cards.Id == offeredCardId?.Id); //Get offered card by card-id (RB)
            var name = token?.Split('-'); //Get name of the user of this request
            var offeringUser = users.FirstOrDefault(user => user.Username == name?[0]); //Get offering user
            var allofferingUserCards = dbRepo.UserGetCards(offeringUser); //Get all cards from offering user
            var offeringCardType = GameLogic.GetTypeFromCardName(offeredCard?.Name); //Get element of offering card

            if (trade == null)
            {
                responseType = "The provided deal ID was not found.";
            }
            else
            {
                if (!tokenlist.Contains(token))
                {
                    responseType = "Access token is missing or invalid";
                }
                else if (offeredCardId == null)
                {
                    responseType = "Invalid JSON data";
                }
                else
                {
                    bool isOwned = false;
                    bool isNotInDeck = true;
                    if (allofferingUserCards != null)
                        foreach (Card c in allofferingUserCards)
                        {
                            if (c.Id == offeredCardId.Id)
                            {
                                isOwned = true;
                                if (c.Deck == 1)
                                {
                                    isNotInDeck = false;
                                }
                            }
                        }

                    if (offeringUser != null && (isOwned == false || isNotInDeck == false || requestingUser?.UserId == offeringUser.UserId||offeredCard?.Damage <trade.MinimumDamage|| offeringCardType != trade.Type))
                    {
                        responseType = "The offered card is not owned by the user, or the requirements are not met (Type, MinimumDamage), or the offered card is locked in the deck, or the user tries to trade with self";
                    }
                    else
                    {
                        dbRepo.UpdateUserCardDependency(offeringUser!.UserId,trade.CardToTrade);
                        dbRepo.UpdateUserCardDependency(requestingUser!.UserId, offeredCardId.Id);
                        dbRepo.DeleteTrade(trade);
                        responseType = "Trading deal successfully executed.";
                    }
                }
            }
        }
       

        CreateAndSendResponse(responseType, writer, objectResponse);
    }

    /// <summary>
    /// Updates resources based on the specified path and client request in a PUT request.
    /// </summary>
    /// <param name="path">The path to determine the resource to update.</param>
    /// <param name="reader">The StreamReader for reading client requests.</param>
    /// <param name="writer">The StreamWriter for sending the response to the client.</param>
    /// <param name="dbRepo">The database repository for handling PUT requests.</param>
    public void PutMethods(string path, StreamReader reader, StreamWriter writer, DbRepo dbRepo)
    {
        var objectResponse = "";
        var responseType = "Bad Request";
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
                    foundUser.Bio = postUser.Bio; //Changes information of user
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

            User? foundUser = null;
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
                var userCards = (List<Card>)dbRepo.UserGetCards(foundUser)!;

                for (var i = 0; i < userCards.Count; i++)
                {
                    for (var x = 0; x < postCards.Count;x++)
                    {
                        var id = userCards[i].Id;
                        if (id != null && id.Equals(postCards[x].Id)) //Checks if the all cards are available for the user
                        {
                            found++;
                            break;
                        }
                    }

                    if (found == 4)  //If 4 Cards are found exit the loop
                        break;
                }

                if (found == 4)
                {
                    
                    foreach (var c in postCards)
                    {
                        var card = userCards.FirstOrDefault(card => card.Id == c.Id);
                        if (card != null)
                        {
                            card.Deck = 1; //Marks every selected card with 1 (Deck Columns)
                            dbRepo.UpdateCard(card);
                        }
                    }

                    responseType = "The deck has been successfully configured";
                }
                else
                {
                    responseType =
                        "At least one of the provided cards does not belong to the user or is not available.";
                }
            }
        }

        CreateAndSendResponse(responseType, writer, objectResponse);
    }

    /// <summary>
    /// Deletes resources based on the specified path in a DELETE request.
    /// </summary>
    /// <param name="path">The path to determine the resource to delete.</param>
    /// <param name="reader">The StreamReader for reading client requests.</param>
    /// <param name="writer">The StreamWriter for sending the response to the client.</param>
    /// <param name="dbRepo">The database repository for handling DELETE requests.</param>
    public void DeleteMethods(string path,StreamReader reader, StreamWriter writer, DbRepo dbRepo)
    {
        var objectResponse = "";
        var responseType = "Bad Request";
        var trades = (List<Trade>)dbRepo.GetAllTrades()!;
        var users = (List<User>)dbRepo.GetAllUsers();
        ReadToEnd(ReadLength(reader), reader);

        if (path.StartsWith("/tradings/"))
        {
            if (trades == null)
            {
                responseType = "The provided deal ID was not found.";
                CreateAndSendResponse(responseType, writer, objectResponse);
                return;
            }
            //Get /{tradingid}
            var tradingId = path.Substring("/tradings/".Length);
            var foundTrade = trades.FirstOrDefault(trade => trade.Id == tradingId);
            if (foundTrade == null)
            {
                responseType = "The provided deal ID was not found.";
            }
            else
            {
                var usersTrade = users.FirstOrDefault(user => user.UserId == foundTrade.UserId);

                if(!tokenlist.Contains(token))
                {
                    responseType = "Access token is missing or invalid";
                }else if (token != usersTrade?.Username + "-mtcgToken" && token != "admin-mtcgToken")
                {
                    responseType = "The deal contains a card that is not owned by the user.";
                }
                else 
                {
                    dbRepo.DeleteTrade(foundTrade);
                    responseType = "Trading deal successfully deleted";
                }
            }
        }

        CreateAndSendResponse(responseType, writer, objectResponse);
    }

    /// <summary>
    /// Creates and sends a response to the client using the provided response string and object response.
    /// </summary>
    /// <param name="response">The response string to send to the client.</param>
    /// <param name="writer">The StreamWriter for sending the response to the client.</param>
    /// <param name="objectResponse">The object response to include in the response.</param>
    private void CreateAndSendResponse(string? response, StreamWriter writer, string objectResponse)
    {
        try
        {
            if (response == "User not found." || response == "No card package available for buying" || response == "The provided deal ID was not found.")
            {
                //Code 404
                writer.WriteLine("HTTP/1.1 404 Not Found");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "User successfully created" || response == "Package and cards successfully created"|| response== "Trading deal successfully created")
            {
                //Code 201
                writer.WriteLine("HTTP/1.1 201 Created");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "User with same username already registered" ||
                     response == "At least one card in the packages already exists" ||
                     response == "User can not fight against himself"||
                     response == "A deal with this deal ID already exists.")
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
                     response == "The stats could be retrieved successfully." ||
                     response == "The deck has been successfully configured" ||
                     response == "Waiting for an opponent" ||
                     response == "Battle finished"|| 
                     response == "There are trading deals available, the response contains these"|| 
                     response == "Trading deal successfully deleted"||
                     response == "Trading deal successfully executed."||
                     response!.StartsWith("Gamelog:"))
            {
                //Code 200
                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "Provided user is not admin" ||
                     response == "Not enough money for buying a card package" ||
                     response == "At least one of the provided cards does not belong to the user or is not available."||
                     response == "The deal contains a card that is not owned by the user or locked in the deck."||
                     response == "The deal contains a card that is not owned by the user."||
                     response == "The offered card is not owned by the user, or the requirements are not met (Type, MinimumDamage), or the offered card is locked in the deck, or the user tries to trade with self"
                     )
            {
                //Code 403
                writer.WriteLine("HTTP/1.1 403 Forbidden");
                writer.WriteLine("Content-Type: text/plain");
            }
            else if (response == "The request was fine, but the user doesn't have any cards"|| response== "The request was fine, but there are no trading deals available")
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

            writer.WriteLine(); //Empty Line is necessary for HTTP-Format
            if (response != "The request was fine, but the user doesn't have any cards"&& response != "The request was fine, but there are no trading deals available") //If the HTTP-Code is 204 No Content -> No Body
            {
                writer.WriteLine(response); //Sends respond to client
                writer.WriteLine(objectResponse); //Sends object to client
            }

            token = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in CreateAndSendResponse(): " + ex.Message);
        }
    }

    /// <summary>
    /// Reads and returns the specified length of characters from the StreamReader.
    /// </summary>
    /// <param name="len">The number of characters to read.</param>
    /// <param name="reader">The StreamReader for reading characters.</param>
    /// <returns>The read string of the specified length.</returns>
    private string ReadToEnd(int len, StreamReader reader)
    {
        var data = new StringBuilder(200);
        if (len > 0)
        {
            var chars = new char[1024];
            var bytesReadTotal = 0;
            while (bytesReadTotal < len) //Reads until end of request
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

    /// <summary>
    /// Reads and returns the length of the incoming data from the StreamReader.
    /// </summary>
    /// <param name="reader">The StreamReader for reading data length.</param>
    /// <returns>The length of the incoming data.</returns>
    private int ReadLength(StreamReader reader)
    {
        string? line;
        var content_length = 0; //we need the content_length later, to be able to read the HTTP-content
        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("Authorization:"))
            {
                var authorizationHeader = line;
                var match = Regex.Match(authorizationHeader, "Bearer\\s+(\\S+)"); //Extract the Bearer Token from the Authorization header
                if (match.Success)
                    token = match.Groups[1].Value; //Saves token for later use (routes)
            }

            Console.WriteLine(line);
            if (line == "") break; // empty line indicates the end of the HTTP-headers

            var parts = line.Split(':'); // Parse the header
            if (parts.Length == 2 && parts[0] == "Content-Length") content_length = int.Parse(parts[1].Trim());
        }

        return content_length;
    }
}