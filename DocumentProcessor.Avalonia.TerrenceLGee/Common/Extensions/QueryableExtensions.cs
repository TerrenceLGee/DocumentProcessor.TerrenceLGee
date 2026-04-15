using DocumentProcessor.Avalonia.TerrenceLGee.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Common.Extensions;

public static class QueryableExtensions
{
    extension<T>(IQueryable<T> source)
    {
        public async Task<PagedList<T>> ToPagedListAsync(int page, int pageSize)
        {
            var count = await source.CountAsync();

            if (count > 0)
            {
                var items = await source
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedList<T>(items, count, page, pageSize);
            }

            return new([], 0, 0, 0);
        }
    }
}
