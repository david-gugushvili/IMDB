using IMDB.BLL.Services.MovieService;

PrintMovies();

void PrintMovies() {

    var movieService = new MovieService();

    var result = movieService.GetMovies("game", 100);

    if (result?.Movies != null)
    {
        Console.WriteLine("\nMovies");

        for (int i = 0; i < result?.Movies.Count; i++)
        {
            Console.WriteLine(string.Format("N:{0}, Title: {1}, Year: {2}", i + 1, result?.Movies[i].Title, result?.Movies[i].Year));
        }

        var moviesGroupedByYear = result?.Movies.Where(x => x.Year != null)
            .GroupBy(x => x.Year).Select(x => new { Year = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count).ToList()!;

        Console.WriteLine("\nCount By Year");

        moviesGroupedByYear.ForEach(x => Console.WriteLine(string.Format("Year: {0}, Count: {1}", x.Year, x.Count)));
    }

    Console.ReadKey();
}