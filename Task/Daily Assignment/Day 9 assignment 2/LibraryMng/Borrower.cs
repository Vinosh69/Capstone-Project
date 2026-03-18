using System.Collections.Generic;

namespace LibraryManagement
{
    public class Borrower
    {
        public string Name { get; set; }
        public string LibraryCardNumber { get; set; }
        public List<Book> borrowedBookList { get; private set; }

        public Borrower(string name, string libraryCardNumber)
        {
            Name = name;
            LibraryCardNumber = libraryCardNumber;
            borrowedBookList = new List<Book>();
        }

        public void BorrowBook(Book selectedBook)
        {
            if (!selectedBook.IsBorrowed)
            {
                selectedBook.Borrow();
                borrowedBookList.Add(selectedBook);
            }
        }

        public void ReturnBook(Book selectedBook)
        {
            if (borrowedBookList.Contains(selectedBook))
            {
                selectedBook.Return();
                borrowedBookList.Remove(selectedBook);
            }
        }
    }
}