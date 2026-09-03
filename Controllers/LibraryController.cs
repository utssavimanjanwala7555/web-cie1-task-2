using LibraryManagementMVC.Extensions;
using LibraryManagementMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementMVC.Controllers;

public class LibraryController : Controller
{
    private static readonly List<Book> Books = new()
    {
        new Book
        {
            Id = 1,
            Title = "C# Programming",
            Author = "John Sharp",
            ISBN = "978-1234567890",
            Category = "Programming",
            IsAvailable = true
        },

        new Book
        {
            Id = 2,
            Title = "ASP.NET Core MVC",
            Author = "Adam Freeman",
            ISBN = "978-0987654321",
            Category = "Web Development",
            IsAvailable = true
        },

        new Book
        {
            Id = 3,
            Title = "Database Management Systems",
            Author = "Raghu Ramakrishnan",
            ISBN = "978-1111111111",
            Category = "Database",
            IsAvailable = true
        },

        new Book
        {
            Id = 4,
            Title = "Computer Networks",
            Author = "Andrew S. Tanenbaum",
            ISBN = "978-2222222222",
            Category = "Networking",
            IsAvailable = true
        },

        new Book
        {
            Id = 5,
            Title = "Operating System Concepts",
            Author = "Abraham Silberschatz",
            ISBN = "978-3333333333",
            Category = "Operating System",
            IsAvailable = true
        }
    };

    private static readonly List<LibraryMember> Members = new()
    {
        new LibraryMember
        {
            Id = 1,
            Name = "Rahul Sharma",
            StudentId = "STU001",
            Course = "BCA",
            Email = "rahul@example.com"
        },

        new LibraryMember
        {
            Id = 2,
            Name = "Priya Patel",
            StudentId = "STU002",
            Course = "BSc Computer Science",
            Email = "priya@example.com"
        },

        new LibraryMember
        {
            Id = 3,
            Name = "Amit Kumar",
            StudentId = "STU003",
            Course = "B.Tech",
            Email = "amit@example.com"
        }
    };

    // GET: /Library
    public IActionResult Index(string? search)
    {
        var books = Books.Search(search);

        ViewBag.Search = search;

        return View(books);
    }

    // GET: /Library/BookDetails/1
    public IActionResult BookDetails(int id)
    {
        var book = Books.FirstOrDefault(b => b.Id == id);

        if (book == null)
            return NotFound();

        var member = book.IssuedToMemberId.HasValue
            ? Members.FirstOrDefault(m => m.Id == book.IssuedToMemberId.Value)
            : null;

        ViewBag.Member = member;
        ViewBag.Fine = book.CalculateFine();

        return View(book);
    }

    // GET: /Library/IssueBook/1
    public IActionResult IssueBook(int id)
    {
        var book = Books.FirstOrDefault(b => b.Id == id);

        if (book == null)
            return NotFound();

        if (!book.IsAvailable)
        {
            TempData["Error"] = "This book is already issued.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Members = Members;

        return View(book);
    }

    // POST: /Library/IssueBook
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult IssueBook(int bookId, int memberId)
    {
        var book = Books.FirstOrDefault(b => b.Id == bookId);
        var member = Members.FirstOrDefault(m => m.Id == memberId);

        if (book == null)
            return NotFound();

        if (member == null)
        {
            TempData["Error"] = "Please select a valid library member.";
            return RedirectToAction(nameof(IssueBook), new { id = bookId });
        }

        if (!book.IsAvailable)
        {
            TempData["Error"] = "This book is already issued.";
            return RedirectToAction(nameof(Index));
        }

        book.IsAvailable = false;
        book.IssuedToMemberId = member.Id;
        book.IssueDate = DateTime.Today;
        book.DueDate = DateTime.Today.AddDays(14);

        TempData["Success"] =
            $"'{book.Title}' has been issued to {member.Name} successfully.";

        return RedirectToAction(nameof(Index));
    }

    // GET: /Library/ReturnBook/1
    public IActionResult ReturnBook(int id)
    {
        var book = Books.FirstOrDefault(b => b.Id == id);

        if (book == null)
            return NotFound();

        if (book.IsAvailable)
        {
            TempData["Error"] = "This book is not currently issued.";
            return RedirectToAction(nameof(Index));
        }

        decimal fine = book.CalculateFine();

        book.IsAvailable = true;
        book.IssuedToMemberId = null;
        book.IssueDate = null;
        book.DueDate = null;

        if (fine > 0)
        {
            TempData["Success"] =
                $"Book returned successfully. Fine collected: ₹{fine:0.00}";
        }
        else
        {
            TempData["Success"] =
                "Book returned successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Library/About
    public IActionResult About()
    {
        return View();
    }
}