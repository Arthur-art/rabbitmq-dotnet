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
        public void InsertUser(User[] user)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "DeathQueue1",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "DeathQueue2",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "DeathQueue3",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "DeathQueue4",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueDeclare(queue: "DeathQueue5",
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
                                         routingKey: "DeathQueue1",
                                         basicProperties: null,
                                         body: body1);
                    channel.BasicPublish(exchange: "",
                                       routingKey: "DeathQueue2",
                                       basicProperties: null,
                                       body: body2);
                    channel.BasicPublish(exchange: "",
                                       routingKey: "DeathQueue3",
                                       basicProperties: null,
                                       body: body3);
                    channel.BasicPublish(exchange: "",
                                       routingKey: "DeathQueue4",
                                       basicProperties: null,
                                       body: body4);
                    channel.BasicPublish(exchange: "",
                                       routingKey: "DeathQueue5",
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
                    channel.QueueDeclare(queue: "RunningQueue1", false, false, false, null);
                    channel.QueueDeclare(queue: "RunningQueue2", false, false, false, null);
                    channel.QueueDeclare(queue: "RunningQueue3", false, false, false, null);
                    channel.QueueDeclare(queue: "RunningQueue4", false, false, false, null);
                    channel.QueueDeclare(queue: "RunningQueue5", false, false, false, null);

                    var consumer1 = new EventingBasicConsumer(channel);
                    consumer1.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("[x] Received {0}", message);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "RunningQueue1",
                            basicProperties: null,
                            body: body
                        );
                    };
                    channel.BasicConsume(queue: "DeathQueue1",
                                        autoAck: true,
                                        consumer: consumer1);
                    var consumer2 = new EventingBasicConsumer(channel);
                    consumer2.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("[x] Received {0}", message);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "RunningQueue2",
                            basicProperties: null,
                            body: body
                        );
                    };
                    channel.BasicConsume(queue: "DeathQueue2",
                                        autoAck: true,
                                        consumer: consumer2);
                    var consumer3 = new EventingBasicConsumer(channel);
                    consumer3.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("[x] Received {0}", message);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "RunningQueue3",
                            basicProperties: null,
                            body: body
                        );
                    };
                    channel.BasicConsume(queue: "DeathQueue3",
                                        autoAck: true,
                                        consumer: consumer3);
                    var consumer4 = new EventingBasicConsumer(channel);
                    consumer4.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("[x] Received {0}", message);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "RunningQueue4",
                            basicProperties: null,
                            body: body
                        );
                    };
                    channel.BasicConsume(queue: "DeathQueue4",
                                        autoAck: true,
                                        consumer: consumer4);
                    var consumer5 = new EventingBasicConsumer(channel);
                    consumer5.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("[x] Received {0}", message);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "RunningQueue5",
                            basicProperties: null,
                            body: body
                        );
                    };
                    channel.BasicConsume(queue: "DeathQueue5",
                                        autoAck: true,
                                        consumer: consumer5);
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
                    
                    consumer.Received += async (model, ea) =>
                    {
                        try
                        {
                            var body = ea.Body.ToArray();
                            var newMessage = Encoding.UTF8.GetString(body);
                            Console.WriteLine("[x] Received {0}", newMessage);
                            message.Add(newMessage);
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError("Erro ao tentar criar um novo pedido", ex);
                            channel.BasicNack(ea.DeliveryTag, false, true);
                        }
                        
                    };
                    channel.BasicConsume(queue: queueNameConsumer,
                                        autoAck: true,
                                        consumer: consumer);
                    Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();

                }
            }
            Console.WriteLine("[x] message {0}");
            return message.ToArray();
        }
    }
}
