
namespace Dashboard.Models
{
    public class PageData<T> where T : class
    {
        public int NrOfRecs { get; set; } = 0;
        public int NrOfPages { get; set; } = 1;
        public int PageNo { get; set; } = 1;
        public IList<T> pageData { get; set; }
    }
}
