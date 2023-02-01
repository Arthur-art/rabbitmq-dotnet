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
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "DeadLetterRunningQueue1",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "DeadLetterRunningQueue2",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "DeadLetterRunningQueue3",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "DeadLetterRunningQueue4",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "DeadLetterRunningQueue5",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "RunningQueue1",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                    channel.QueueDeclare(queue: "RunningQueue2",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "RunningQueue3",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "RunningQueue4",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "RunningQueue5",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message1 = JsonSerializer.Serialize(user[0]);
                    var body1 = Encoding.UTF8.GetBytes(message1);
                    string message2 = JsonSerializer.Serialize(user[1]);
                    var body2 = Encoding.UTF8.GetBytes(message2);
                    string message3 = JsonSerializer.Serialize(user[2]);
                    var body3 = Encoding.UTF8.GetBytes(message3);
                    string message4 = JsonSerializer.Serialize(user[3]);
                    var body4 = Encoding.UTF8.GetBytes(message4);
                    string message5 = JsonSerializer.Serialize(user[4]);
                    var body5 = Encoding.UTF8.GetBytes(message5);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "DeadLetterRunningQueue1",
                                         basicProperties: null,
                                         body: body1);
                    channel.BasicPublish(exchange: "",
                                       routingKey: "DeadLetterRunningQueue2",
                                       basicProperties: null,
                                       body: body2);
                    channel.BasicPublish(exchange: "",
                                       routingKey: "DeadLetterRunningQueue3",
                                       basicProperties: null,
                                       body: body3);
                    channel.BasicPublish(exchange: "",
                                       routingKey: "DeadLetterRunningQueue4",
                                       basicProperties: null,
                                       body: body4);
                    channel.BasicPublish(exchange: "",
                                       routingKey: "DeadLetterRunningQueue5",
                                       basicProperties: null,
                                       body: body5);

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
