using MonsterTradingCards.BasicClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.REST_Interface
{
    public class Server
    {
        public void RunServer()
        {
            //Define a port/url for the server
            string url = "http://localhost:10001/";

            //Create HttpListener
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            Console.WriteLine("Server running. Waiting for Requests...");

            //Server running in a loop
            while (true)
            {
                try
                {
                    //Wait for request
                    HttpListenerContext context = listener.GetContext();

                    // Verarbeiten Sie die Anforderung in einem separaten Thread
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;


                        string responseString = "";
                        if (request.HttpMethod == "GET")
                        {
                            responseString = GetMethods(request);
                        }
                        else if (request.HttpMethod == "POST")
                        {
                            responseString = PostMethods(request);
                        }
                        else if (request.HttpMethod == "PUT")
                        {
                            responseString = PutMethods(request);
                        }
                        else if (request.HttpMethod == "DELETE")
                        {
                            responseString = DeleteMethods(request);
                        }

                        if (responseString == "User not found")
                        {
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                        }

                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

        }




        public string PostMethods(HttpListenerRequest request)
        {
            string response = "";
            if (request.Url.AbsolutePath == "/TEST")
            {
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    string requestBody = reader.ReadToEnd();
                    User user = JsonConvert.DeserializeObject<User>(requestBody);

                    response =$"POST-Anfrage für /createUser empfangen: Username = {user.Username}, Password = {user.PasswordHash}";
                }
            }
            else if (request.Url.AbsolutePath == "/users")
            {
                response = "Post /users";
            }
            else if (request.Url.AbsolutePath == "/sessions")
            {
                response = "Post /sessions";
            }
            else if (request.Url.AbsolutePath == "/transactions/packages")
            {
                response = "Post /transactions/packages";
            }
            else if (request.Url.AbsolutePath == "/tradings")
            {
                response = "Post /tradings";
            }
            else if (request.Url.AbsolutePath == "/packages")
            {
                response = "Post /packages";
            }
            else if (request.Url.AbsolutePath == "/battles")
            {
                response = "Post /battles";
            }
            else if (request.Url.AbsolutePath == "/tradings")
            {
                response = "Post /tradings";
            }
            else if (request.Url.AbsolutePath.StartsWith("/tradings/"))
            {
                response = "Post /tradings/{tradingdealid}";
            }

            return response;
        }

        public string GetMethods(HttpListenerRequest request)
        {
            string response = "";
            if (request.Url.AbsolutePath == "/users")
            {
                // Behandeln Sie GET-Anfragen für den /users-Endpunkt hier
                response = "GET-Anfrage für /users empfangen";
            }
            else if (request.Url.AbsolutePath.StartsWith("/users/"))
            {
                
                    //Get /{username}
                    string username = request.Url.AbsolutePath.Substring("/users/".Length);

                    // Hier sollten Sie die Benutzerdaten aus Ihrer Datenquelle abrufen!!!!!!!!!!!!!!!!!
                    // und das Ergebnis in UserData konvertieren.!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    //!!!!UserData userData = GetUserData(username);
                    string userData = username;

                    if (userData != null)
                    {
                        //Send created user
                        string jsonResponse = JsonConvert.SerializeObject(userData);
                        response = jsonResponse;
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



        public string PutMethods(HttpListenerRequest request)
            {
            string response = "";
            if (request.Url.AbsolutePath == "/users")
            {
                response = "Put /users";
            }
            else if (request.Url.AbsolutePath.StartsWith("/users/"))
            {
                response = "Put /users/{username}";
            }
            else if(request.Url.AbsolutePath == "/deck")
            {
                response = "Put /deck";
            }

            return response;
            }

        public string DeleteMethods(HttpListenerRequest request)
        {
            string response = "";

            if (request.Url.AbsolutePath.StartsWith("/tradings/"))
            {
                response = "Delete /tradings/{tradingdealid}";
            }
            return response;
        }
    }

}

