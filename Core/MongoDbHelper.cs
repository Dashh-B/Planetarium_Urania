using MongoDB.Bson;
using MongoDB.Driver;
using Planetarium_Urania.Forms.Observations;

namespace Planetarium_Urania.Core;

public static class MongoDbHelper
{
    private static IMongoDatabase? _database;
    private static IMongoCollection<ObservationMetadata>? _collection;

    private const string ConnectionString = "mongodb://localhost:27017";
    private const string DatabaseName = "planetarium_urania";
    private const string CollectionName = "observation_metadata";

    // ── Инициализация ─────────────────────────────────────────────────────

    public static bool Initialize()
    {
        try
        {
            var client = new MongoClient(ConnectionString);
            _database = client.GetDatabase(DatabaseName);
            _collection = _database.GetCollection<ObservationMetadata>(CollectionName);
            return true;
        }
        catch (Exception ex) { ShowError("подключении к MongoDB", ex); return false; }
    }

    public static bool TestConnection()
    {
        try
        {
            new MongoClient(ConnectionString)
                .GetDatabase(DatabaseName)
                .RunCommandAsync((Command<BsonDocument>)"{ping:1}")
                .Wait();
            return true;
        }
        catch { return false; }
    }

    private static bool EnsureCollection()
    {
        if (_collection is not null) return true;
        return Initialize();
    }

    // ── CRUD ──────────────────────────────────────────────────────────────

    public static bool AddMetadata(ObservationMetadata metadata)
    {
        try
        {
            if (!EnsureCollection()) return false;
            metadata.CreatedAt = DateTime.Now;
            metadata.UpdatedAt = DateTime.Now;
            _collection!.InsertOne(metadata);
            return true;
        }
        catch (Exception ex) { ShowError("добавлении метаданных", ex); return false; }
    }

    public static ObservationMetadata? GetMetadataByObservationId(int observationId)
    {
        try
        {
            if (!EnsureCollection()) return null;
            var filter = Builders<ObservationMetadata>.Filter.Eq(m => m.ObservationId, observationId);
            return _collection!.Find(filter).FirstOrDefault();
        }
        catch (Exception ex) { ShowError("получении метаданных", ex); return null; }
    }

    // Возвращает HashSet ID наблюдений, для которых существуют метаданные
    public static HashSet<int> GetAllObservationIdsWithMetadata()
    {
        try
        {
            if (!EnsureCollection()) return [];

            var projection = Builders<ObservationMetadata>.Projection
                .Include(m => m.ObservationId)
                .Exclude(m => m.Id);

            var ids = _collection!
                .Find(FilterDefinition<ObservationMetadata>.Empty)
                .Project<ObservationMetadata>(projection)
                .ToList()
                .Select(m => m.ObservationId);

            return [.. ids];
        }
        catch (Exception ex) { ShowError("получении ID метаданных", ex); return []; }
    }

    public static List<ObservationMetadata> GetAllMetadata()
    {
        try
        {
            if (!EnsureCollection()) return [];
            return _collection!.Find(FilterDefinition<ObservationMetadata>.Empty).ToList();
        }
        catch (Exception ex) { ShowError("получении всех метаданных", ex); return []; }
    }

    public static bool UpdateMetadata(ObservationMetadata metadata)
    {
        try
        {
            if (!EnsureCollection()) return false;
            metadata.UpdatedAt = DateTime.Now;
            var filter = Builders<ObservationMetadata>.Filter.Eq(m => m.Id, metadata.Id);
            return _collection!.ReplaceOne(filter, metadata).ModifiedCount > 0;
        }
        catch (Exception ex) { ShowError("обновлении метаданных", ex); return false; }
    }

    public static bool DeleteMetadata(string id)
    {
        try
        {
            if (!EnsureCollection()) return false;
            var filter = Builders<ObservationMetadata>.Filter.Eq(m => m.Id, id);
            return _collection!.DeleteOne(filter).DeletedCount > 0;
        }
        catch (Exception ex) { ShowError("удалении метаданных", ex); return false; }
    }

    public static List<ObservationMetadata> SearchMetadata(string searchTerm)
    {
        try
        {
            if (!EnsureCollection()) return [];
            var regex = new BsonRegularExpression(searchTerm, "i");
            var filter = Builders<ObservationMetadata>.Filter.Or(
                Builders<ObservationMetadata>.Filter.Regex(m => m.DetailedNotes, regex),
                Builders<ObservationMetadata>.Filter.Regex(m => m.Photography.CameraModel, regex));
            return _collection!.Find(filter).ToList();
        }
        catch (Exception ex) { ShowError("поиске метаданных", ex); return []; }
    }

    public static Dictionary<string, object> GetStatistics()
    {
        try
        {
            if (!EnsureCollection()) return [];
            long total = _collection!.CountDocuments(FilterDefinition<ObservationMetadata>.Empty);
            long withPhotos = _collection!.CountDocuments(
                Builders<ObservationMetadata>.Filter.SizeGt(m => m.Photos, 0));
            return new()
            {
                ["total_metadata"] = total,
                ["with_photos"] = withPhotos,
                ["without_photos"] = total - withPhotos
            };
        }
        catch (Exception ex) { ShowError("получении статистики MongoDB", ex); return []; }
    }

    private static void ShowError(string operation, Exception ex) =>
        MessageBox.Show($"Ошибка при {operation}:\n{ex.Message}",
            "Ошибка MongoDB", MessageBoxButtons.OK, MessageBoxIcon.Error);
}