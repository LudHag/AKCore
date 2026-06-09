using System.Linq;

namespace AKCore.Extensions;

public static class PaginationExtensions
{
    public static int TotalPages(int itemCount, int pageSize) =>
        pageSize <= 0 ? 0 : ((itemCount - 1) / pageSize) + 1;
}
