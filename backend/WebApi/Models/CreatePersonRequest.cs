namespace WebApi.Models
{
    public class CreatePersonRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Alias { get; set; }
    }
}
