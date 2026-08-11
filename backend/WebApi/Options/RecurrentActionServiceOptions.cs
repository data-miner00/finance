namespace WebApi.Options;

public class RecurrentActionServiceOptions
{
    public const string SectionName = "HostedServices:RecurrentActionService";

    public TimeSpan ExecutionIntervalTimeSpan { get; set; }
}
