using System.Collections.Generic;

namespace Provisioning.Options
{
    internal class SettingsOption
    {
        public const string SectionName = "Settings";

        public Dictionary<string, string> DefaultValues { get; set; } = new();
    }
}
