using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Muhit.Application.DTOs.Google.Response
{
    public class GoogleNearbySearchResponse
    {
        [JsonPropertyName("places")]
        public List<GooglePlace> Places { get; set; } = new();
    }

    public class GooglePlace
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("types")]
        public List<string> Types { get; set; } = new();

        [JsonPropertyName("formattedAddress")]
        public string FormattedAddress { get; set; }

        [JsonPropertyName("location")]
        public GooglePlaceLocation Location { get; set; }

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("userRatingCount")]
        public int? UserRatingCount { get; set; }

        [JsonPropertyName("displayName")]
        public GooglePlaceDisplayName DisplayName { get; set; }
    }

    public class GooglePlaceLocation
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }

    public class GooglePlaceDisplayName
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; }
    }
}
