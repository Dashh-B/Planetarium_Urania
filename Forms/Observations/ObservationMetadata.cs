using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Planetarium_Urania.Forms.Observations;

public class ObservationMetadata
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("observation_id")]
    public int ObservationId { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("weather_details")]
    public WeatherDetails? Weather { get; set; }

    [BsonElement("photography_settings")]
    public PhotographySettings? Photography { get; set; }

    [BsonElement("photos")]
    public List<PhotoInfo> Photos { get; set; } = [];

    [BsonElement("measurements")]
    public Dictionary<string, object> Measurements { get; set; } = [];

    [BsonElement("detailed_notes")]
    public string? DetailedNotes { get; set; }

    [BsonElement("additional_participants")]
    public List<string> AdditionalParticipants { get; set; } = [];
}

public class WeatherDetails
{
    [BsonElement("temperature")]
    public double? Temperature { get; set; }

    [BsonElement("humidity")]
    public int? Humidity { get; set; }

    [BsonElement("pressure")]
    public double? Pressure { get; set; }

    [BsonElement("wind_speed")]
    public double? WindSpeed { get; set; }

    [BsonElement("cloud_coverage")]
    public int? CloudCoverage { get; set; }

    [BsonElement("seeing_quality")]
    public string? SeeingQuality { get; set; }

    [BsonElement("transparency")]
    public string? Transparency { get; set; }
}

public class PhotographySettings
{
    [BsonElement("camera_model")]
    public string? CameraModel { get; set; }

    [BsonElement("exposure_time")]
    public double? ExposureTime { get; set; }

    [BsonElement("iso")]
    public int? Iso { get; set; }

    [BsonElement("filter")]
    public string? Filter { get; set; }

    [BsonElement("frames_count")]
    public int? FramesCount { get; set; }

    [BsonElement("stacking_method")]
    public string? StackingMethod { get; set; }
}

public class PhotoInfo
{
    [BsonElement("filename")]
    public string? Filename { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }

    [BsonElement("file_size")]
    public long FileSize { get; set; }

    [BsonElement("resolution")]
    public string? Resolution { get; set; }
}