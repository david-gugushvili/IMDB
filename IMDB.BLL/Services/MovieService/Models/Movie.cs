using Newtonsoft.Json;

namespace IMDB.BLL.Services.MovieService.Models
{
    public class Movie
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("title")]
        public required string Title { get; set; }

        [JsonProperty("year")]
        public int? Year { get; set; }
    }
}
