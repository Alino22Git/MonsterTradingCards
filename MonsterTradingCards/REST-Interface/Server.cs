using System.Data.Common;
using System.Net;
using System.Text;
using MonsterTradingCards.BasicClasses;
using MonsterTradingCards.Repository;
using Newtonsoft.Json;

namespace MonsterTradingCards.REST_Interface;

public class Server
{
    public string ConnectionString { get; }
    public Server(string Con)
    {
        ConnectionString = Con;
    }

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
                    UserRepo userRepo = new UserRepo(ConnectionString);

                    var responseString = "";
                    if (request.HttpMethod == "GET")
                        responseString = GetMethods(request, userRepo);
                    else if (request.HttpMethod == "POST")
                        responseString = PostMethods(request, userRepo);
                    else if (request.HttpMethod == "PUT")
                        responseString = PutMethods(request, userRepo);
                    else if (request.HttpMethod == "DELETE") responseString = DeleteMethods(request, userRepo);

                    if (responseString == "User not found.")
                    {
                        //Code 404
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                    }else if (responseString == "User successfully created")
                    {
                        //Code 201
                        response.StatusCode = (int)HttpStatusCode.Created;
                    }else if(responseString == "User with the username already registred")
                    {
                        //Code 409
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                    }else if (responseString == "User unauthorized")
                    {
                        //Code 401
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                    else if (responseString == "Data successfully retrieved")
                    {
                        //Code 200
                        response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else if (responseString == "Provided user is not admin")
                    {
                        //403
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                    }
                    else if (responseString == "The request was fine, but the user doesn't have any cards")
                    {
                        //204
                        response.StatusCode = (int)HttpStatusCode.NoContent;
                    }

                    var buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    var output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
    }

    public string GetMethods(HttpListenerRequest request, UserRepo userRepo)
    {
        var response = "";
        if (request.Url.AbsolutePath.StartsWith("/users/"))
        {
            //Get /{username}
            var username = request.Url.AbsolutePath.Substring("/users/".Length);
            List<User> users = (List<User>)userRepo.GetAll();

            //Using LINQ to save the user in foundUser
            var foundUser = users.FirstOrDefault(user => user.Username == username);

            if (foundUser != null)
            {
                /* //Send created user
                 var jsonResponse = JsonConvert.SerializeObject(foundUser);
                 response = jsonResponse;
                */
                response = "Data successfully retrieved";
            }
            else
            {
                response = "User not found.";
            }
        }
        else if (request.Url.AbsolutePath == "/deck")
        {
            response = "Get /deck";
        }
        else if (request.Url.AbsolutePath == "/cards")
        {
            response = "Get /cards";
        }
        else if (request.Url.AbsolutePath == "/stats")
        {
            response = "Get /stats";
        }
        else if (request.Url.AbsolutePath == "/scoreboard")
        {
            response = "Get /scoreboard";
        }
        else if (request.Url.AbsolutePath == "/tradings")
        {
            response = "Get /tradings";
        }

        return response;
    }

    public string PostMethods(HttpListenerRequest request, UserRepo userRepo)
    {
        var response = "";
        if (request.Url.AbsolutePath == "/TEST")
        {
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                string requestBody = reader.ReadToEnd();
                User user = JsonConvert.DeserializeObject<User>(requestBody);

                response = $"POST-Anfrage für /createUser empfangen: Username = {user.Username}, Password = {user.PasswordHash}";
            }
        
    }
        else if (request.Url.AbsolutePath == "/users")
        {
            using (var reader = new StreamReader(request.InputStream))
            {
                var requestBody = reader.ReadToEnd();
                var postUser = JsonConvert.DeserializeObject<User>(requestBody);

                if (postUser == null)
                {
                    return "No body";
                }
                List<User> users = (List<User>)userRepo.GetAll();

                //Using LINQ to save the user in foundUser
                var foundUser = users.FirstOrDefault(user => user.Username == postUser.Username);

                if (foundUser == null)
                {
                    userRepo.Add(postUser);

                    response = "Data successfully created";
                }
                else
                {
                    response = "User with same username already registered";
                }
            }
        }
        else if (request.Url.AbsolutePath == "/sessions")
            response = "Post /sessions";
        else if (request.Url.AbsolutePath == "/transactions/packages")
            response = "Post /transactions/packages";
        else if (request.Url.AbsolutePath == "/tradings")
            response = "Post /tradings";
        else if (request.Url.AbsolutePath == "/packages")
            response = "Post /packages";
        else if (request.Url.AbsolutePath == "/battles")
            response = "Post /battles";
        else if (request.Url.AbsolutePath == "/tradings")
            response = "Post /tradings";
        else if (request.Url.AbsolutePath.StartsWith("/tradings/")) response = "Post /tradings/{tradingdealid}";

        return response;
    }

    public string PutMethods(HttpListenerRequest request, UserRepo userRepo)
    {
        var response = "";
        if (request.Url.AbsolutePath == "/users")
            response = "Put /users";
        else if (request.Url.AbsolutePath.StartsWith("/users/"))
            response = "Put /users/{username}";
        else if (request.Url.AbsolutePath == "/deck") response = "Put /deck";

        return response;
    }

    public string DeleteMethods(HttpListenerRequest request, UserRepo userRepo)
    {
        var response = "";

        if (request.Url.AbsolutePath.StartsWith("/tradings/")) response = "Delete /tradings/{tradingdealid}";
        return response;
    }
}