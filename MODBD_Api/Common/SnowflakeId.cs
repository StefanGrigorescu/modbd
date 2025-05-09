using MODBD_Common.NumericTypes.Random;
using MODBD_Common.Time;

namespace MODBD_Api.Common;

public static class SnowflakeId
{
    public static long New(
        Tenant tenant,
        IRandom random,
        Utc.Snapshot utcSnapshot
    ) {
        string datePart = utcSnapshot.AsDateTime().ToString("yyyyMMdd");

        int regionId = tenant.Id;
        string regionPart = regionId.ToString("D2");

        string randomPart = string.Concat(
            Enumerable
                .Range(0, 8)
                .Select(_ => random.Next(0, 10))
        );

        return long.Parse($"{datePart}{regionPart}{randomPart}");
    }
}
