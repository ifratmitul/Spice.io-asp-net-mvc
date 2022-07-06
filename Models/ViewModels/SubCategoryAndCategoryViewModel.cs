namespace Spice.Models.ViewModels
{
    public class SubCategoryAndCategoryViewModel
    {
        public IEnumerable<Category>? Categories { get; set; }
        public SubCategory? SubCategory { get; set; }
        public List<string>? SubCategoryList { get; set; }
        public string? StatusMessage { get; set; }
    }
}
