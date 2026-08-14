namespace WebApi.Models
{
    public class MergeCategoriesRequest
    {
        public string SourceCategoryId { get; set; }

        public string TargetCategoryId { get; set; }
    }
}
