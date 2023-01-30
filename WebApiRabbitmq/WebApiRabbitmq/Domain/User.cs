using System.Text.Json.Serialization;

namespace WebApiRabbitmq.Domain
{
    public class User
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public string Job { get; set; }

        public string Queue { get; set; }
    }
}
