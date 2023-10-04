using System.Net;
using System.Text;
using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using Newtonsoft.Json;

namespace MonsterTradingCards.REST_Interface;

public class Server
{
    public Server(string Con)
    {
        ConnectionString = Con;
    }

    public string ConnectionString { get; }

    public void RunServer()
    {
        //Define a port/url for the server
        var url = "http://localhost:10001/";

        //Create HttpListener
        var listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();

        Console.WriteLine("Server running. Waiting for Requests...");

        //Server running in a loop
        while (true)
            try
            {
                //Wait for request
                var context = listener.GetContext();

                // Verarbeiten Sie die Anforderung in einem separaten Thread
                ThreadPool.QueueUserWorkItem(o =>
                {
                    var request = context.Request;
                    var response = context.Response;
                    var userRepo = new UserRepo(ConnectionString);

                    if (request.HttpMethod == "GET")
                        GetMethods(request, response, userRepo);
                    else if (request.HttpMethod == "POST")
                        PostMethods(request, response, userRepo);
                    else if (request.HttpMethod == "PUT")
                        PutMethods(request, response, userRepo);
                    else if (request.HttpMethod == "DELETE")
                        DeleteMethods(request, response, userRepo);
                }, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in RunServer(): " + ex.Message);
            }
    }

    public void GetMethods(HttpListenerRequest request, HttpListenerResponse response, UserRepo userRepo)
    {
        var objectResponse = "";
        if (request.Url.AbsolutePath.StartsWith("/users/"))
        {
            //Get /{username}
            var username = request.Url.AbsolutePath.Substring("/users/".Length);
            var users = (List<User>)userRepo.GetAll();

            //Using LINQ to save the user in foundUser
            var foundUser = users.FirstOrDefault(user => user.Username == username);

            //USER NEEDS TO AUTHORIZE !!! User der sich selbst sucht oder Admin

            if (foundUser != null)
            {
                //Send created user
                objectResponse = JsonConvert.SerializeObject(foundUser);
                response.StatusDescription = "Data successfully retrieved";
            }
            else
            {
                response.StatusDescription = "User not found.";
            }
        }
        else if (request.Url.AbsolutePath == "/deck")
        {
            response.StatusDescription = "Get /deck";
        }
        else if (request.Url.AbsolutePath == "/cards")
        {
            response.StatusDescription = "Get /cards";
        }
        else if (request.Url.AbsolutePath == "/stats")
        {
            response.StatusDescription = "Get /stats";
        }
        else if (request.Url.AbsolutePath == "/scoreboard")
        {
            response.StatusDescription = "Get /scoreboard";
        }
        else if (request.Url.AbsolutePath == "/tradings")
        {
            response.StatusDescription = "Get /tradings";
        }

        CreateAndSendResponse(response, objectResponse);
    }

    public void PostMethods(HttpListenerRequest request, HttpListenerResponse response, UserRepo userRepo)
    {
        var objectResponse = "";
        if (request.Url.AbsolutePath == "/TEST")
            using (var reader = new StreamReader(request.InputStream))
            {
                var requestBody = reader.ReadToEnd();
                var user = JsonConvert.DeserializeObject<User>(requestBody);

                response.StatusDescription =
                    $"POST-Anfrage für /createUser empfangen: Username = {user.Username}, Password = {user.PasswordHash}";
            }
        else if (request.Url.AbsolutePath == "/users")
            using (var reader = new StreamReader(request.InputStream))
            {
                var requestBody = reader.ReadToEnd();
                var postUser = JsonConvert.DeserializeObject<User>(requestBody);

                if (postUser == null) return;

                //Get all users
                var users = (List<User>)userRepo.GetAll();

                //Using LINQ to save the user in foundUser
                var foundUser = users.FirstOrDefault(user => user.Username == postUser.Username);

                if (foundUser == null)
                {
                    userRepo.Add(postUser);

                    response.StatusDescription = "User successfully created";
                }
                else
                {
                    response.StatusDescription = "User with same username already registered";
                }
            }
        else if (request.Url.AbsolutePath == "/sessions")
            response.StatusDescription = "Post /sessions";
        else if (request.Url.AbsolutePath == "/transactions/packages")
            response.StatusDescription = "Post /transactions/packages";
        else if (request.Url.AbsolutePath == "/tradings")
            response.StatusDescription = "Post /tradings";
        else if (request.Url.AbsolutePath == "/packages")
            response.StatusDescription = "Post /packages";
        else if (request.Url.AbsolutePath == "/battles")
            response.StatusDescription = "Post /battles";
        else if (request.Url.AbsolutePath == "/tradings")
            response.StatusDescription = "Post /tradings";
        else if (request.Url.AbsolutePath.StartsWith("/tradings/"))
            response.StatusDescription = "Post /tradings/{tradingdealid}";

        CreateAndSendResponse(response, objectResponse);
    }

    public void PutMethods(HttpListenerRequest request, HttpListenerResponse response, UserRepo userRepo)
    {
        var objectResponse = "";
        if (request.Url.AbsolutePath.StartsWith("/users/"))
        {
            var username = request.Url.AbsolutePath.Substring("/users/".Length);
            var users = (List<User>)userRepo.GetAll();

            var foundUser = users.FirstOrDefault(user => user.Username == username);

            //USER NEEDS TO AUTHORIZE !!! User der sich selbst sucht oder Admin

            if (foundUser != null)
            {
                objectResponse = JsonConvert.SerializeObject(foundUser);
                response.StatusDescription = "Data successfully retrieved";
            }
            else
            {
                response.StatusDescription = "User not found.";
            }

            response.StatusDescription = "Put /users/{username}";
        }
        else if (request.Url.AbsolutePath == "/deck") 
            response.StatusDescription = "Put /deck";

        CreateAndSendResponse(response, objectResponse);
    }

    public void DeleteMethods(HttpListenerRequest request, HttpListenerResponse response, UserRepo userRepo)
    {
        var objectResponse = "";

        if (request.Url.AbsolutePath.StartsWith("/tradings/"))
            objectResponse = "Delete /tradings/{tradingdealid}";

        CreateAndSendResponse(response, objectResponse);
    }

    private void CreateAndSendResponse(HttpListenerResponse response, string objectResponse)
    {
        response.StatusCode = GetStatusCode(response);
        //response.StatusDescription = description;
        response.ContentType = "application/json";

        var buffer = Encoding.UTF8.GetBytes(objectResponse);
        response.ContentLength64 = buffer.Length;
        var output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }

    private int GetStatusCode(HttpListenerResponse response)
    {
        //Console.WriteLine(response.StatusDescription);
        if (response.StatusDescription == "User not found.")
            //Code 404
            return (int)HttpStatusCode.NotFound;
        if (response.StatusDescription == "User successfully created")
            //Code 201
            return (int)HttpStatusCode.Created;
        if (response.StatusDescription == "User with same username already registered")
            //Code 409
            return (int)HttpStatusCode.Conflict;
        if (response.StatusDescription == "User unauthorized")
            //Code 401
            return (int)HttpStatusCode.Unauthorized;
        if (response.StatusDescription == "Data successfully retrieved")
            //Code 200
            return (int)HttpStatusCode.OK;
        if (response.StatusDescription == "Provided user is not admin")
            //403
            return (int)HttpStatusCode.Forbidden;
        if (response.StatusDescription == "The request was fine, but the user doesn't have any cards")
            //204
            return (int)HttpStatusCode.NoContent;

        return (int)HttpStatusCode.Unused;
    }
}