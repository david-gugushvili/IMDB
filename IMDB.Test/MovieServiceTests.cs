using IMDB.BLL.Services.MovieService;
using IMDB.Test.Fixtures;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;


namespace IMDB.Test
{
    public class MovieServiceTests : TestBed<MovieServiceFixture>
    {
        private readonly AppSettings _options;

        public MovieServiceTests(ITestOutputHelper testOutputHelper, MovieServiceFixture fixture)
            : base(testOutputHelper, fixture) => _options = _fixture.GetService<IOptions<AppSettings>>(_testOutputHelper)!.Value;

        [Fact]
        public void IsServiceUp()
        {
            var movieService = _fixture.GetService<IMovieService>(_testOutputHelper)!;

            var result = movieService.GetMovies("game", 1);

            Assert.True(result.Movies.Any(), "Service down");
        }
    }
}