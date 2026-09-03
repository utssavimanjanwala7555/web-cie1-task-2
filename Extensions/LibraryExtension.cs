using LibraryManagementMVC.Models;

namespace LibraryManagementMVC.Extensions;

public static class LibraryExtensions
{
    public static IEnumerable<Book> Search(
        this IEnumerable<Book> books,
        string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return books;

        search = search.Trim();

        return books.Where(book =>
            book.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            book.Author.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            book.ISBN.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            book.Category.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    public static decimal CalculateFine(this Book book)
    {
        if (book.IsAvailable || book.DueDate == null)
            return 0;

        if (DateTime.Today <= book.DueDate.Value.Date)
            return 0;

        int overdueDays =
            (DateTime.Today - book.DueDate.Value.Date).Days;

        // ₹5 fine per overdue day
        return overdueDays * 5;
    }

    public static string AvailabilityText(this Book book)
    {
        return book.IsAvailable ? "Available" : "Issued";
    }
}