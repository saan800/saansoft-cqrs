using System.Text.Json;

namespace SaanSoft.Cqrs.Transport;

// TODO: this?

// public sealed class SystemTextJsonSerializer : ITransportSerializer
// {
//     private readonly JsonSerializerOptions _options;

//     public SystemTextJsonSerializer(JsonSerializerOptions? options = null)
//     => _options = options ?? new JsonSerializerOptions
//     {
//         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//         WriteIndented = false
//     };

//     public string Serialize<T>(T value) => JsonSerializer.Serialize(value, _options);
//     public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options)!;
//     public object Deserialize(string json, Type type) => JsonSerializer.Deserialize(json, type, _options)!;
// }

