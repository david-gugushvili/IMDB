using IMDB.BLL.Services.MovieService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace IMDB.Test.Fixtures
{
    public class MovieServiceFixture : TestBedFixture
    {
        protected override void AddServices(IServiceCollection services, IConfiguration? configuration) => services
            .AddTransient<IMovieService, MovieService>()
            .Configure<AppSettings>(config => configuration?.GetSection("AppSettings").Bind(config));

        protected override ValueTask DisposeAsyncCore() => new();

        protected override IEnumerable<Xunit.Microsoft.DependencyInjection.TestAppSettings> GetTestAppSettings()
        {
            yield return new() { Filename = "appsettings.json", IsOptional = true };
        }
    }
}
