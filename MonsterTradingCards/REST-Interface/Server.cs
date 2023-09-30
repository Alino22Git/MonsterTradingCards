using MonsterTradingCards.BasicClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.REST_Interface
{
    public class Server
    {
        public void RunServer()
        {
            // Definieren Sie die URL und den Port für Ihren Server
            string url = "http://localhost:10001/";

            // Erstellen und starten Sie den HTTP-Listener
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            Console.WriteLine("Server läuft. Warten auf Anfragen...");

            while (true)
            {
                try
                {
                    // Warten Sie auf eine eingehende Anforderung
                    HttpListenerContext context = listener.GetContext();

                    // Verarbeiten Sie die Anforderung in einem separaten Thread
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        string responseString = "";

                        if (request.HttpMethod == "GET")
                        {
                            if (request.Url.AbsolutePath == "/users")
                            {
                                // Behandeln Sie GET-Anfragen für den /users-Endpunkt hier
                                responseString = "GET-Anfrage für /users empfangen";
                            }
                            else if (request.Url.AbsolutePath == "/posts")
                            {
                                // Behandeln Sie GET-Anfragen für den /posts-Endpunkt hier
                                responseString = "GET-Anfrage für /posts empfangen";
                            }
                        }
                        else if (request.HttpMethod == "POST")
                        {
                            if (request.Url.AbsolutePath == "/createUser")
                            {
                                // Behandeln Sie POST-Anfragen für den /createUser-Endpunkt hier
                                using (StreamReader reader = new StreamReader(request.InputStream))
                                {
                                    string requestBody = reader.ReadToEnd();
                                    User user = JsonConvert.DeserializeObject<User>(requestBody);
                                    // Verarbeiten Sie das empfangene Benutzerobjekt
                                    responseString =
                                        $"POST-Anfrage für /createUser empfangen: Username = {user.Username}, Password = {user.PasswordHash}";
                                }
                            }
                            else if (request.Url.AbsolutePath == "/createPost")
                            {
                                // Behandeln Sie POST-Anfragen für den /createPost-Endpunkt hier
                                using (StreamReader reader = new StreamReader(request.InputStream))
                                {
                                    string requestBody = reader.ReadToEnd();
                                    // Verarbeiten Sie das empfangene Post-Objekt
                                    responseString = "POST-Anfrage für /createPost empfangen";
                                }
                            }
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
                    Console.WriteLine("Fehler: " + ex.Message);
                }
            }
        }
    }
}