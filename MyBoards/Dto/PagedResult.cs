using Microsoft.AspNetCore.Mvc.RazorPages;
using MyBoards.Entities;

namespace MyBoards.Dto
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalPages { get; set; }

        public int ItemsFrom { get; set; }
        public int ItemsTo { get; set; }

        public int TotalItemsCount { get; set; }

        public PagedResult(List<T> items, int totalItems, int pageNumber, int pageSize)
        {
            Items = items;
            TotalItemsCount = totalItems;
            ItemsFrom = ((pageNumber - 1) * pageSize) + 1;
            ItemsTo = ItemsFrom + (pageSize - 1);
            TotalPages = (int)Math.Ceiling(TotalItemsCount / (double) pageSize);
        }
    }
}
