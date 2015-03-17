namespace QuoteFlow.Areas.Admin.ViewModels.Indexing
{
    public class IndexingViewModel
    {
        public string IndexPath { get; set; }

        public IndexingViewModel(string indexPath)
        {
            IndexPath = indexPath;
        }
    }
}