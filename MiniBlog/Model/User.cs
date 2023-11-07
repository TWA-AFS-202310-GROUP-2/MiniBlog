using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace MiniBlog.Model
{
    public class User
    {
        public User()
        {
        }

        public User(string name, string email = "anonymous@unknow.com")
        {
            this.Name = name;
            this.Email = email;
        }

        public static string CollectionName { get; set; } = "User";

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;
    }
}
