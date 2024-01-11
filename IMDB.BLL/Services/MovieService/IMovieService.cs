using IMDB.BLL.Services.MovieService.Models;

namespace IMDB.BLL.Services.MovieService
{
    public interface IMovieService
    {
        GetMoviesResult GetMovies(string title, int count);
    }
}
