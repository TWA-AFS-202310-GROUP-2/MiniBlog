using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MiniBlog.Model
{
    public class User
    {
        public User(string name, string email = "anonymous@unknow.com")
        {
            this.Name = name;
            this.Email = email;
        }

        public static string CollectionName { get; set; } = "User";

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}
