using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Question2A.Model;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;

namespace Question2A.Controllers
{
   
    [ApiController]
    [Route("[controller]")]

    public class taskController : ControllerBase
    {
        static string _address = "https://reqres.in/api/register";

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> Task([FromBody] taskMessage credential)
        {

            var result = await RegisterMessage(credential);
            Console.WriteLine("Result2");
            Console.WriteLine(result);
            // var token = -1;


            try
            {
                var token = JObject.Parse(result)["token"];
                
             
                    if (token.ToString().Contains("undefined"))
                    {
                        Console.WriteLine("someting wrong");
                        return result;

                    }
                    else
                    {
                        Console.WriteLine("connect rabbit");
                        var factory = new ConnectionFactory()
                        {
                            //HostName = "localhost" , 
                            //Port = 30724
                            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                            Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
                        };

                        Console.WriteLine(factory.HostName + ":" + factory.Port);
                        using (var connection = factory.CreateConnection())
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue: "TaskQueue",
                                                 durable: true,
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                            string message = credential.task;
                            var body = Encoding.UTF8.GetBytes(message);

                            channel.BasicPublish(exchange: "",
                                                 routingKey: "TaskQueue",
                                                 basicProperties: null,
                                                 body: body);
                        }
                        return "1";
                    }

                }
            catch
            {
                return StatusCode(401);                

            }

        }

        [HttpPost]
        private static async Task<string> RegisterMessage(taskMessage taskmessage)
        {
            HttpClient httpClient = new HttpClient();
            Console.WriteLine("register API called");
            var postResponse = await httpClient.PostAsJsonAsync(_address, taskmessage);
            try
            {
                postResponse.EnsureSuccessStatusCode();
                var result = await postResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Result");
                Console.WriteLine(result);
                return result;
                // Handle success
            }
            catch (HttpRequestException)
            {
                var result = await postResponse.Content.ReadAsStringAsync();
                return result;
                //  return StatusCode(401, "My error message");
                // Handle failure
            }
            // postResponse.EnsureSuccessStatusCode();
          

         


            


        }


    }


}
