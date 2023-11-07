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
            Id = Guid.NewGuid();
            UserName = userName;
            Title = title;
            Content = content;
        }

        public static string ArticleCollectionName { get; set; } = "Artical";

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Article article &&
                   Id.Equals(article.Id) &&
                   UserName == article.UserName &&
                   Title == article.Title &&
                   Content == article.Content;
        }
    }
}