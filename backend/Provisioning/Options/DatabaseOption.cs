namespace Provisioning.Options;

internal class DatabaseOption : ICloneable
{
    public const string SectionName = "Database";

    public string MasterConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public string ConnectionString { get; set; }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
