using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniBlog.Model
{
    public class Article
    {
        public Article()
        {
        }

        public Article(string userName, string title, string content)
        {
            Id = Guid.NewGuid().ToString();
            UserName = userName;
            Title = title;
            Content = content;
        }

        public static string CollectionName { get; set; } = "Articles";

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = null!;

        public string UserName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
