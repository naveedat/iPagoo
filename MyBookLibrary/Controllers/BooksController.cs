using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyBookLibrary.Models;

namespace MyBookLibrary.Controllers
{
    public class BooksController : Controller
    {
        private BookDBContext db = new BookDBContext();
        private BorrowerDBContext dbBorrower = new BorrowerDBContext();

        // GET: Books
        public ActionResult Index(string searchISBN, string searchTitle, string searchAuthor)
        {
            var books = from m in db.Books
                         select m;

            if (!String.IsNullOrEmpty(searchISBN))
            {
                // Filter by ISBN
                books = books.Where(s => s.ISBN.Contains(searchISBN));
            }
            if (!String.IsNullOrEmpty(searchTitle))
            {                
                // Filter by Title
                books = books.Where(s => s.Title.Contains(searchTitle));
            }
            if (!String.IsNullOrEmpty(searchAuthor))
            {
                // Filter by Author
                books = books.Where(s => s.Author.Contains(searchAuthor));
            } 
            return View(books);
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                // TODO: ERROR/EXCEPTION HANDLING
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create(string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                // calling external Restfull service (ISBNDB)
                Book book = ISBNDBService.GetBookInformation(searchString);
                if (book == null)
                {
                    // TODO: ERROR/EXCEPTION HANDLING
                    return HttpNotFound();
                }
                    book.ISBN = searchString;
                    return View(book);                               
            }
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ISBN,Title,Author,Publisher,Summary,Notes")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.isAvailable = true; // now this book is available to lend
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Borrow(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                // TODO: ERROR/EXCEPTION HANDLING
                return HttpNotFound();
            }
            Borrower borrower = new Borrower();
            borrower.BookID = book.ID;
            borrower.BorrowDate = DateTime.Now;
            return View(borrower);
        }

        // POST: Books/Borrow/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Borrow([Bind(Include = "ID,BookID,Name,Comment,BorrowDate")] Borrower borrower)
        {
            if (ModelState.IsValid)
            {
                // insert borrower record
                borrower.ReturnDate = borrower.BorrowDate; // LITTLE FUDGE AS DATABASE WAS NOT ACCETING NULL DATETIME
                dbBorrower.Borrowers.Add(borrower);
                dbBorrower.SaveChanges();

                // update the book status to "out of self"
                Book book = db.Books.Find(borrower.BookID);
                book.isAvailable = false;
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(borrower);
        }

        // GET: Return book:
        public ActionResult Return(int? id)
        {
            if (id == null)
            {
                // TODO: ERROR/EXCEPTION HANDLING
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // check this is a valid book record
            Book book = db.Books.Find(id);
            if (book == null)
            {
                // TODO: ERROR/EXCEPTION HANDLING
                return HttpNotFound();
            }

            // Make book available 
            book.isAvailable = true;
            db.Entry(book).State = EntityState.Modified;
            db.SaveChanges();

            // check borrower record against the book
            Borrower borrower = dbBorrower.Borrowers.Find(book.ID);
            if (borrower == null)
            {
                // TODO: ERROR/EXCEPTION HANDLING
                return HttpNotFound();
            }

            // update the borrower record with return date
            borrower.ReturnDate = DateTime.Now;
            dbBorrower.Entry(borrower).State = EntityState.Modified;
            dbBorrower.SaveChanges();

            return RedirectToAction("Index");
        }
        
        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                // TODO: ERROR/EXCEPTION HANDLING
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                // TODO: ERROR/EXCEPTION HANDLING
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
