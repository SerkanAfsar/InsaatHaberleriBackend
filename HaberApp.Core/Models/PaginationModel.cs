namespace HaberApp.Core.Models
{
    public class PaginationModel
    {
        private int _page = 1;
        private int _count = 10;
        private const int maxPageCount = 100;

        public int Page
        {
            get => _page;
            set
            {
                _page = value < 1 ? 1 : value;
            }
        }
        public int Count
        {
            get => _count;
            set
            {
                _count = _count < 0 ? maxPageCount : value;
            }
        }
    }
}
