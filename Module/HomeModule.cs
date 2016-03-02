using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ViewEngines.Razor;

namespace LibraryNameSpace
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => {
        List<Author> allAuthors = Author.GetAll();
        return View["index.cshtml", allAuthors];
      };
      Post["/"] = _ => {
        Author newAuthor = new Author(Request.Form["authorName"]);
        newAuthor.Save();
        List<Author> allAuthors = Author.GetAll();
        return View["index.cshtml", allAuthors];
      };
      Get["/authorbooks/{id}"] = parameters => {
        Dictionary<string, object> newDictionary = new Dictionary<string, object>();
        Author newAuthor = Author.Find(parameters.id);
        List<Book> allBooks = newAuthor.GetBooks();
        newDictionary.Add("bookList", allBooks);
        newDictionary.Add("author", newAuthor);
        return View["authorbooks.cshtml", newDictionary];
      };
      Get["/addBook/{id}"] = parameters => {
        Author newAuthor = Author.Find(parameters.id);
        return View["addBook.cshtml", newAuthor];
      };
      Post["/authorbooks/{id}"] = parameters => {
        Author newAuthor = Author.Find(parameters.id);
        Book newBook = new Book(Request.Form["bookName"]);
        newBook.Save();
        newAuthor.AddBook(newBook);
        Dictionary<string, object> newDictionary = new Dictionary<string, object>();
        List<Book> allBooks = newAuthor.GetBooks();
        newDictionary.Add("bookList", allBooks);
        newDictionary.Add("author", newAuthor);
        return View["authorbooks.cshtml", newDictionary];
      };

      Get["/edit/{id}"] = parameters => {
        Book newBook = Book.Find(parameters.id);
        return View["editbook.cshtml", newBook];
      };

      Patch["/authorbooks/update/{id}"] = parameters => {
        Book newBook = Book.Find(parameters.id);
        newBook.Update(Request.Form["bookName"]);
        List<Author> allAuthors = Author.GetAll();
        return View["index.cshtml", allAuthors];
      };

      Delete["/authorbooks/delete/{id}"] = parameters => {
        Book newBook = Book.Find(parameters.id);
        newBook.Delete();
        List<Author> allAuthors = Author.GetAll();
        return View["index.cshtml", allAuthors];
      };

      Post["/bookList"] = _ => {
        List<Book> foundBooks = Book.SearchBooks(Request.Form["Search"]);
        // Console.WriteLine("request form search: " + Request.Form["Search"]);
        // Book foundBook = new Book("hi", 3);
        return View["searchedList.cshtml", foundBooks];
      };

    }
  }
}
