using System.Linq;

namespace AKCore.Extensions;

public static class PaginationExtensions
{
    public static int TotalPages(this int itemCount, int pageSize) =>
        pageSize <= 0 ? 0 : ((itemCount - 1) / pageSize) + 1;
}
