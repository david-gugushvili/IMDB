using Newtonsoft.Json;

namespace IMDB.BLL.Services.MovieService.Models
{
    internal class GetMoviesResponse
    {
        [JsonProperty("paginationKey")]
        public int PaginationKey { get; set; }

        [JsonProperty("results")]
        public required List<Movie> Movies { get; set; }

        [JsonProperty("totalMatches")]
        public int TotalMatches { get; set; }
    }
}
