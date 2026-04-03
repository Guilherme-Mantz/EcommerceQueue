using System.Diagnostics;

namespace Observability;

public static class ActivitySourceProvider
{
    public static ActivitySource CreateActivitySource(string serviceName)
    {
        return new ActivitySource(serviceName, "1.0.0");
    }
}
