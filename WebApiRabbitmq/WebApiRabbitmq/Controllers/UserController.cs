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

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost("create-publish-queue")]
        public ActionResult InsertUser(User user, string queue)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message = JsonSerializer.Serialize(user);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: queue,
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", message);

                }

                return Accepted(user);
            }catch(Exception ex)
            {
                _logger.LogError("Erro ao tentar criar um novo pedido", ex);
                return new StatusCodeResult(500);
            }
        }

        [HttpPost("consumer-publisher-queue")]
        public void ConsumerQueue(string queueNameConsumer, string queueNamePublisher )
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueNamePublisher, false, false, false, null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("[x] Received {0}", message);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: queueNamePublisher,
                            basicProperties: null,
                            body: body
                        );
                    };
                    channel.BasicConsume(queue: queueNameConsumer,
                                        autoAck: true,
                                        consumer: consumer);
                    Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }

        [HttpGet("consumer-queue")]
        public string[] ConsumerOrderQueue(string queueNameConsumer)
        {
            List<string> message = new List<string>() {""};
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                
                using (var channel = connection.CreateModel())
                {

                    var consumer = new EventingBasicConsumer(channel);
                    
                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body.ToArray();
                            var newMessage = Encoding.UTF8.GetString(body);
                            Console.WriteLine("[x] Received {0}", newMessage);
                            message.Add(newMessage);
                           
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError("Erro ao tentar criar um novo pedido", ex);
                        }
                        
                    };
                    channel.BasicConsume(queue: queueNameConsumer,
                                        autoAck: true,
                                        consumer: consumer);
                    Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();
                   
                }
            }
            Console.WriteLine("[x] message {0}", message.ToArray());
            return message.ToArray();
        }
    }
}
