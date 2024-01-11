namespace IMDB.BLL.Services
{
    public class BaseResult
    {
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
