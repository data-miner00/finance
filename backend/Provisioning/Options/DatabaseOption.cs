using System;
using System.Collections.Generic;
using System.Text;

namespace Provisioning.Options
{
    internal class DatabaseOption
    {
        public const string SectionName = "Database";

        public string MasterConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string ConnectionString { get; set; }
    }
}
