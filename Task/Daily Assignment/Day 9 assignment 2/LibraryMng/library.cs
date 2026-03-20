using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement
{
    public class Library
    {
        public List<Book> bookCollection { get; private set; }
        public List<Borrower> borrowerRegistry { get; private set; }

        public Library()
        {
            bookCollection = new List<Book>();
            borrowerRegistry = new List<Borrower>();
        }

        public void AddBook(Book newBook)
        {
            bookCollection.Add(newBook);
        }

        public void RegisterBorrower(Borrower newBorrower)
        {
            borrowerRegistry.Add(newBorrower);
        }

        public void BorrowBook(string isbn, string libraryCardNumber)
        {
            var selectedBook = bookCollection.FirstOrDefault(b => b.ISBN == isbn);
            var selectedBorrower = borrowerRegistry.FirstOrDefault(b => b.LibraryCardNumber == libraryCardNumber);

            if (selectedBook != null && selectedBorrower != null && !selectedBook.IsBorrowed)
            {
                selectedBorrower.BorrowBook(selectedBook);
            }
        }

        public void ReturnBook(string isbn, string libraryCardNumber)
        {
            var selectedBook = bookCollection.FirstOrDefault(b => b.ISBN == isbn);
            var selectedBorrower = borrowerRegistry.FirstOrDefault(b => b.LibraryCardNumber == libraryCardNumber);

            if (selectedBook != null && selectedBorrower != null)
            {
                selectedBorrower.ReturnBook(selectedBook);
            }
        }

        public List<Book> ViewBooks()
        {
            return bookCollection;
        }

        public List<Borrower> ViewBorrowers()
        {
            return borrowerRegistry;
        }
    }
}