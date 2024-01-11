namespace IMDB.BLL.Services.MovieService.Models
{
    public class GetMoviesResult : BaseResult
    {
        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}
