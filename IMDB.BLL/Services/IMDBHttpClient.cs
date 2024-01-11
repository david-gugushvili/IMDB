namespace IMDB.BLL.Services
{
    internal class IMDBHttpClient : HttpClient
    {
        public IMDBHttpClient()
        {
            DefaultRequestHeaders.Add("X-RapidAPI-Key", "c6b5ac88f7msh192c3d2e63a0e92p17799djsn375994122d42");
        }
    }
}
