using MongoDB.Bson;
using MongoDB.Driver;
using MotoHub.Application.Interfaces.Repositories;

namespace MotoHub.Infrastructure.Persistence;

public class MongoDbImageRepository(IMongoDatabase database) : IImageRepository
{
    private readonly IMongoCollection<BsonDocument> _imagesCollection = database.GetCollection<BsonDocument>("Images");

    public async Task<string?> GetImageAsBase64Async(string fileIdentifier, CancellationToken cancellationToken = default)
    {
        FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", fileIdentifier);
        BsonDocument doc = await _imagesCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return doc?["Base64"]?.AsString;
    }

    public async Task<string> UploadImageAsBase64Async(string base64Image, CancellationToken cancellationToken = default)
    {
        string id = ObjectId.GenerateNewId().ToString();
        BsonDocument doc = new()
        {
            { "_id", id },
            { "Base64", base64Image }
        };
        await _imagesCollection.InsertOneAsync(doc, null, cancellationToken);
        return id;
    }

    public async Task RemoveAsync(string fileIdentifier, CancellationToken cancellationToken = default)
    {
        FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", fileIdentifier);
        await _imagesCollection.DeleteOneAsync(filter, cancellationToken);
    }
}
