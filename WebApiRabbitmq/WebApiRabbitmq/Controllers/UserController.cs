using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using WebApiRabbitmq.Domain;

namespace WebApiRabbitmq.Controllers
{
    [AllowAnonymous]
    [Route("api/user-queue-rabbitmq")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ILogger<UserController> _logger;
        string[] deadLetters = {
            "DeadLetterRunningQueue1",
            "DeadLetterRunningQueue2",
            "DeadLetterRunningQueue3",
            "DeadLetterRunningQueue4",
            "DeadLetterRunningQueue5"
        };

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost("create-publish-queue")]
        public void InsertUser(User[] user)
        {
            int num = 1;
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();
                {
                    foreach (string deadLetter in deadLetters)
                    {
                        channel.QueueDeclare(queue: deadLetter,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                        channel.QueueDeclare(queue: deadLetter.Replace("DeadLetter", ""),
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                    }

                    foreach (User data in user)
                    {
                        
                        string message = JsonSerializer.Serialize(data);
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                         routingKey: "DeadLetterRunningQueue" + $"{num}",
                                         basicProperties: null,
                                         body: body);
                        num++;
                    }

                }
            }catch(Exception ex)
            {
                _logger.LogError("Erro ao tentar criar um novo pedido", ex);
            }
        }

        [HttpPost("consumer-publisher-queue")]
        public void ConsumerQueue()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    foreach (string deadLetterQueue in deadLetters)
                    {

                        channel.QueueDeclare(queue: deadLetterQueue.Replace("DeadLetter", ""), false, false, false, null);

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine("[x] Received {0}", message);

                            channel.BasicPublish(
                                exchange: "",
                                routingKey: deadLetterQueue.Replace("DeadLetter", ""),
                                basicProperties: null,
                                body: body
                            );
                        };
                        channel.BasicConsume(queue: deadLetterQueue,
                                                        autoAck: true,
                                                        consumer: consumer);
                    }
                    Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
