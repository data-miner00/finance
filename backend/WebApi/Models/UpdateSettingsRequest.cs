using System.Collections.Generic;

namespace WebApi.Models
{
    public class UpdateSettingsRequest
    {
        public Dictionary<string, string> Values { get; set; } = new();
    }
}
