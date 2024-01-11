using IMDB.BLL.Services.MovieService.Models;
using Newtonsoft.Json;

namespace IMDB.BLL.Services.MovieService
{
    public class MovieService : IMovieService
    {
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(Constants.GetMoviesRequestPerSecond, Constants.GetMoviesRequestPerSecond);
        private static Queue<DateTime> requestDates = new Queue<DateTime>(Constants.GetMoviesRequestPerSecond);

        private static object _pageLock = new object();
        private static object _movieLock = new object();
        private static object _requestDateLock = new object();

        //TODO: make parallel requests optional; possible data dublication; kill unnesessary tasks when data ends; 
        public GetMoviesResult GetMovies(string title, int count)
        {
            var result = new GetMoviesResult();

            var limit = Math.Min(Constants.GetMoviesPageLimit, count);

            var pagesCount = (int)Math.Ceiling((decimal)count / 20);

            var httpClient = new IMDBHttpClient();

            var tasks = new Task[pagesCount];

            var movies = new List<(int page, List<Movie> movies)>();

            var page = 0;

            for (int i = 0; i < pagesCount; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    _semaphore.Wait();

                    try
                    {
                        await RequestRateDelay();

                        lock (_requestDateLock)
                        {
                            if (requestDates.Count == Constants.GetMoviesRequestPerSecond) requestDates.Dequeue();

                            requestDates.Enqueue(DateTime.Now);
                        }

                        var currentPage = 0;

                        lock (_pageLock)
                        {
                            currentPage = page;
                            page++;
                        }

                        var response = await httpClient.GetAsync($"https://online-movie-database.p.rapidapi.com/title/v2/find?title={title}&limit={limit}&paginationKey={currentPage}&sortArg=moviemeter%2Casc");

                        response.EnsureSuccessStatusCode();

                        var responseBody = await response.Content.ReadAsStringAsync();

                        var responseResult = JsonConvert.DeserializeObject<GetMoviesResponse>(responseBody)!;

                        lock (_movieLock)
                        {
                            movies.Add((responseResult.PaginationKey, responseResult.Movies));
                        }
                    }
                    catch { result.HasError = true; }
                    finally { _semaphore.Release(); }
                });
            }

            Task.WaitAll(tasks);

            if (!result.HasError)
            {
                result.Movies = movies.OrderBy(x => x.page).SelectMany(x => x.movies).ToList();
            }

            return result;
        }

        public async Task RequestRateDelay()
        {
            if (requestDates.Any())
            {
                var requestDiff = DateTime.Now - requestDates.First();

                if (requestDiff < TimeSpan.FromSeconds(1))
                {
                    var delay = (int)Math.Ceiling((decimal)(TimeSpan.FromSeconds(1) - requestDiff).TotalMilliseconds);

                    await Task.Delay(delay);
                }
            }
        }


        //// no pararel request, optimized to minimise requests quantity
        //public async Task<GetMoviesResult> GetMovies(string title, int count)
        //{
        //    var result = new GetMoviesResult();

        //    var movies = new List<Models.Movie>();

        //    var limit = Math.Min(Constants.GetMoviesServiceLimit, count);

        //    var page = 0;

        //    var httpClient = new IMDBHttpClient();

        //    while (movies.Count < count)
        //    {
        //        await Task.Run(async () =>
        //        {
        //            //var stopwatch = Stopwatch.StartNew();

        //            _semaphore.Wait();

        //            try
        //            {
        //                var response1 = await httpClient.GetAsync($"https://online-movie-database.p.rapidapi.com/title/v2/find?title={title}&limit={limit}&paginationKey={page}&sortArg=moviemeter%2Casc");

        //                string responseBody1 = await response1.Content.ReadAsStringAsync();

        //                var responseResult1 = JsonConvert.DeserializeObject<GetMoviesResponse>(responseBody1);

        //                movies.AddRange(responseResult1.Movies);
        //            }
        //            finally
        //            {
        //                //if (stopwatch.Elapsed > TimeSpan.FromSeconds(1))
        //                //{
        //                //    var delay = (int)Math.Ceiling((decimal)(TimeSpan.FromSeconds(30) - stopwatch.Elapsed).TotalMilliseconds);

        //                //    await Task.Delay(delay);
        //                //}
        //                _semaphore.Release();
        //            }
        //        });


        //        //var stopwatch = Stopwatch.StartNew();

        //        //_semaphore.Wait();

        //        //try
        //        //{
        //        //    //Interlocked.Add(ref padding, 100);

        //        //    var response1 = await httpClient.GetAsync($"https://online-movie-database.p.rapidapi.com/title/v2/find?title={title}&limit={limit}&paginationKey={page}&sortArg=moviemeter%2Casc");

        //        //}
        //        //finally
        //        //{
        //        //    if (stopwatch.Elapsed > TimeSpan.FromSeconds(1))
        //        //    {
        //        //        await Task.Delay((int)(TimeSpan.FromSeconds(10) - stopwatch.Elapsed).TotalMilliseconds);
        //        //    }
        //        //    _semaphore.Release();
        //        //}

        //        var response = await httpClient.GetAsync($"https://online-movie-database.p.rapidapi.com/title/v2/find?title={title}&limit={limit}&paginationKey={page}&sortArg=moviemeter%2Casc");

        //        if (response.StatusCode != System.Net.HttpStatusCode.OK) return null;

        //        string responseBody = await response.Content.ReadAsStringAsync();

        //        var responseResult = JsonConvert.DeserializeObject<GetMoviesResponse>(responseBody);

        //        movies.AddRange(responseResult.Movies);

        //        if (responseResult.Movies.Count < 20)
        //        {
        //            break;
        //        }

        //        movies = movies.Distinct().ToList();

        //        page++;
        //    }

        //    result.Movies = movies;

        //    return result;
        //}
    }
}
