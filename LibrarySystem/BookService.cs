using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem
{
    public class BookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public bool AddBook(string title, string author, string isbn)
        {
            var book = new Book
            {
                Title = title,
                Author = author,
                ISBN = isbn,
                IsAvailable = true
            };

            _context.Books.Add(book);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteBook(string isbn)
        {
            var book = _context.Books.FirstOrDefault(b => b.ISBN == isbn);
            if (book == null) return false;

            _context.Books.Remove(book);
            _context.SaveChanges();
            return true;
        }


        public bool UpdateBook(string isbn, string newTitle, string newAuthor)
        {
            var book = _context.Books.FirstOrDefault(b => b.ISBN == isbn);
            if (book == null) return false;

            if (!string.IsNullOrWhiteSpace(newTitle)) book.Title = newTitle;
            if (!string.IsNullOrWhiteSpace(newAuthor)) book.Author = newAuthor;

            _context.SaveChanges();
            return true;
        }

        public bool IsBookAvailable(string isbn)
        {
            var book = _context.Books.FirstOrDefault(b => b.ISBN == isbn);
            return book != null && book.IsAvailable;
        }

        public bool LoanBook(int userId, string isbn)
        {
            var book = _context.Books.FirstOrDefault(b => b.ISBN == isbn);
            if (book == null || !book.IsAvailable) return false;

            book.IsAvailable = false;
            var loan = new Loan { UserId = userId, BookId = book.Id };
            _context.Loans.Add(loan);
            _context.SaveChanges();
            return true;
        }

        public bool ReturnBook(int userId, string isbn)
        {
            var book = _context.Books.FirstOrDefault(b => b.ISBN == isbn);
            if (book == null || book.IsAvailable) return false;

            var loan = _context.Loans.FirstOrDefault(l => l.BookId == book.Id && l.UserId == userId);
            if (loan == null) return false;

            _context.Loans.Remove(loan);
            book.IsAvailable = true;
            _context.SaveChanges();
            return true;
        }

        public Book GetBookByISBN(string isbn)
        {
            return _context.Books.FirstOrDefault(b => b.ISBN == isbn);
        }

        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public List<Book> SearchBooks(string query)
        {
            return _context.Books
                .Where(b => b.Title.Contains(query) || b.Author.Contains(query))
                .ToList();
        }

        public List<Book> GetBooksSortedByTitle()
        {
            return _context.Books.OrderBy(b => b.Title).ToList();
        }
    }

}