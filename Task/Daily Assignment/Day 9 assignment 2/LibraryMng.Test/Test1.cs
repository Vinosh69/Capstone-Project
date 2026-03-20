using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibraryManagement;
using System.Linq;

namespace LibraryManagement.Tests
{
    [TestClass]
    public class LibraryTests
    {
        private Library _library;

        [TestInitialize]
        public void Setup()
        {
            _library = new Library();
        }

        [TestMethod]
        public void AddBook_BookIsAddedToLibrary()
        {
            var book = new Book("Modern CSharp Guide", "John", "ISBN001");

            _library.AddBook(book);

            Assert.AreEqual(1, _library.bookCollection.Count);
            Assert.AreEqual("Modern CSharp Guide", _library.bookCollection.First().Title);
        }

        [TestMethod]
        public void RegisterBorrower_BorrowerIsAddedToLibrary()
        {
            var borrower = new Borrower("Ravi", "CARD001");

            _library.RegisterBorrower(borrower);

            Assert.AreEqual(1, _library.borrowerRegistry.Count);
            Assert.AreEqual("Ravi", _library.borrowerRegistry.First().Name);
        }

        [TestMethod]
        public void BorrowBook_BookIsMarkedAsBorrowed()
        {
            var book = new Book("Modern CSharp Guide", "John", "ISBN001");
            var borrower = new Borrower("Ravi", "CARD001");

            _library.AddBook(book);
            _library.RegisterBorrower(borrower);

            _library.BorrowBook("ISBN001", "CARD001");

            Assert.IsTrue(book.IsBorrowed);
        }

        [TestMethod]
        public void BorrowBook_BookIsAddedToBorrowersList()
        {
            var book = new Book("Modern CSharp Guide", "John", "ISBN001");
            var borrower = new Borrower("Ravi", "CARD001");

            _library.AddBook(book);
            _library.RegisterBorrower(borrower);

            _library.BorrowBook("ISBN001", "CARD001");

            Assert.AreEqual(1, borrower.borrowedBookList.Count);
            Assert.AreEqual("ISBN001", borrower.borrowedBookList.First().ISBN);
        }

        [TestMethod]
        public void ReturnBook_BookIsMarkedAsAvailable()
        {
            var book = new Book("Modern CSharp Guide", "John", "ISBN001");
            var borrower = new Borrower("Ravi", "CARD001");

            _library.AddBook(book);
            _library.RegisterBorrower(borrower);
            _library.BorrowBook("ISBN001", "CARD001");

            _library.ReturnBook("ISBN001", "CARD001");

            Assert.IsFalse(book.IsBorrowed);
        }

        [TestMethod]
        public void ReturnBook_BookIsRemovedFromBorrowersList()
        {
            var book = new Book("Modern CSharp Guide", "John", "ISBN001");
            var borrower = new Borrower("Ravi", "CARD001");

            _library.AddBook(book);
            _library.RegisterBorrower(borrower);
            _library.BorrowBook("ISBN001", "CARD001");

            _library.ReturnBook("ISBN001", "CARD001");

            Assert.AreEqual(0, borrower.borrowedBookList.Count);
        }

        [TestMethod]
        public void ViewBooks_ReturnsAllBooks()
        {
            var book1 = new Book("Modern CSharp Guide", "John", "ISBN001");
            var book2 = new Book("ASP.NET Core Essentials", "Mike", "ISBN002");

            _library.AddBook(book1);
            _library.AddBook(book2);

            var books = _library.ViewBooks();

            Assert.AreEqual(2, books.Count);
        }

        [TestMethod]
        public void ViewBorrowers_ReturnsAllBorrowers()
        {
            var borrower1 = new Borrower("Ravi", "CARD001");
            var borrower2 = new Borrower("Suresh", "CARD002");

            _library.RegisterBorrower(borrower1);
            _library.RegisterBorrower(borrower2);

            var borrowers = _library.ViewBorrowers();

            Assert.AreEqual(2, borrowers.Count);
        }
    }
}