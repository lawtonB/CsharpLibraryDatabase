using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace LibraryNameSpace
{
  public class Book
  {
    private int _id;
    private string _title;
    private bool _checked_out;


    public Book(string Title, bool Checked_out, int Id = 0)
    {
      _id = Id;
      _title = Title;
      _checked_out = Checked_out;
    }
    public override bool Equals(System.Object otherBook)
    {
        if (!(otherBook is Book))
        {
          return false;
        }
        else {
          Book newBook = (Book) otherBook;
          bool idEquality = this.GetId() == newBook.GetId();
          bool titleEquality = this.GetTitle() == newBook.GetTitle();
          bool Checked_outEquality = this.GetCheckedOut() == newBook.GetCheckedOut();
          return (idEquality && titleEquality && Checked_outEquality);
        }
    }
    public int GetId()
    {
      return _id;
    }
    public string GetTitle()
    {
      return _title;
    }

    public bool GetCheckedOut()
    {
      return _checked_out;
    }

    public bool SetCheckedOut(bool checked_out)
    {
      return checked_out;
    }


    public static List<Book> GetAll()
    {
      List<Book> AllBooks = new List<Book>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookTitle = rdr.GetString(1);
        bool Checked_out = rdr.GetBoolean(2);
        Book newBook = new Book(bookTitle, Checked_out, bookId);
        AllBooks.Add(newBook);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return AllBooks;
    }

    public static List<Book> SearchBooks(string searchBooks)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      List<Book> books = new List<Book>{};

      SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE title = @search", conn);

      SqlParameter AuthorIdParameter = new SqlParameter();
      AuthorIdParameter.ParameterName = "@search";
      AuthorIdParameter.Value = searchBooks;

      cmd.Parameters.Add(AuthorIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> booksIds = new List<int> {};

      int searchedBookId = 0;
      string searchedTitle = null;
      bool searchedChecked_out = false;



      while(rdr.Read())
      {
        searchedBookId = rdr.GetInt32(0);
        searchedTitle = rdr.GetString(1);
        searchedChecked_out = rdr.GetBoolean(2);

        Book newBook = new Book(searchedTitle, searchedChecked_out, searchedBookId);
        books.Add(newBook);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return books;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO books (title, checked_out) OUTPUT INSERTED.id VALUES (@BookTitle, @Checked_out)", conn);

      SqlParameter titleParameter = new SqlParameter();
      titleParameter.ParameterName = "@BookTitle";
      titleParameter.Value = this.GetTitle();

      SqlParameter checkedOutParameter = new SqlParameter();
      checkedOutParameter.ParameterName = "@Checked_out";
      checkedOutParameter.Value = this.GetCheckedOut();

      cmd.Parameters.Add(titleParameter);
      cmd.Parameters.Add(checkedOutParameter);


      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Author> GetAuthors()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      List<Author> authors = new List<Author>{};

      SqlCommand cmd = new SqlCommand("SELECT authors.* from books join books_authors on (books.id = books_authors.book_id) join authors on (books_authors.author_id = authors.id) where books.id = @BookId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();

      cmd.Parameters.Add(bookIdParameter);

      rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        int authorId = rdr.GetInt32(0);
        string authorTitle = rdr.GetString(1);
        Author newAuthor = new Author(authorTitle, authorId);
        authors.Add(newAuthor);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return authors;
    }

    public void Update(string newBook_Title)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE books SET title = @Title OUTPUT INSERTED.title WHERE id = @BookId;", conn);

      SqlParameter TitleParameter = new SqlParameter();
      TitleParameter.ParameterName = "@Title";
      TitleParameter.Value = newBook_Title;

      SqlParameter BookIdParameter = new SqlParameter();
      BookIdParameter.ParameterName = "@BookId";
      BookIdParameter.Value = this.GetId();

      cmd.Parameters.Add(TitleParameter);
      cmd.Parameters.Add(BookIdParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._title = rdr.GetString(0);
      }

      if (rdr != null)
      {
        rdr.Close();
      }

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void CheckedOutUpdateTrue()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE books SET checked_out = 1 OUTPUT INSERTED.checked_out WHERE id = @BookId;", conn);

      SqlParameter newBookIdParameter = new SqlParameter();
      newBookIdParameter.ParameterName = "@BookId";
      newBookIdParameter.Value =  this.GetId();

      cmd.Parameters.Add(newBookIdParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._checked_out = rdr.GetBoolean(0);
      }

      if (rdr != null)
      {
        rdr.Close();
      }

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void CheckedOutUpdateFalse()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE books SET checked_out = 0 OUTPUT INSERTED.checked_out WHERE id = @BookId;", conn);

      SqlParameter newBookIdParameter = new SqlParameter();
      newBookIdParameter.ParameterName = "@BookId";
      newBookIdParameter.Value =  this.GetId();

      cmd.Parameters.Add(newBookIdParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._checked_out = rdr.GetBoolean(0);
      }

      if (rdr != null)
      {
        rdr.Close();
      }

      if (conn != null)
      {
        conn.Close();
      }
    }


    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM books;", conn);
      cmd.ExecuteNonQuery();
    }

    public static Book Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE id = @BookId", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = id.ToString();
      cmd.Parameters.Add(bookIdParameter);
      rdr = cmd.ExecuteReader();

      int foundBookId = 0;
      string foundBookTitle = null;
      bool foundBookCheckedOut = false;


      while(rdr.Read())
      {
        foundBookId = rdr.GetInt32(0);
        foundBookTitle = rdr.GetString(1);
        foundBookCheckedOut = rdr.GetBoolean(2);
      }
      Book foundBook = new Book(foundBookTitle, foundBookCheckedOut, foundBookId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundBook;
    }

    public static Book FindTitle(string title)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE title = @Booktitle", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@Booktitle";
      bookIdParameter.Value = title.ToString();

      cmd.Parameters.Add(bookIdParameter);
      rdr = cmd.ExecuteReader();

      int foundBookId = 0;
      string foundBookTitle = null;
      bool foundBookCheckedOut = false;

      while(rdr.Read())
      {
        foundBookId = rdr.GetInt32(0);
        foundBookTitle = rdr.GetString(1);
        foundBookCheckedOut = rdr.GetBoolean(2);

      }
      Book foundBook = new Book(foundBookTitle, foundBookCheckedOut, foundBookId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundBook;
    }

    public void AddAuthor(Author newAuthor)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO books_authors (author_id, book_id) VALUES (@Author_id, @Book_id);", conn);

      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@Author_id";
      authorIdParameter.Value = newAuthor.GetId();
      cmd.Parameters.Add(authorIdParameter);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@Book_id";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id = @Book_id; DELETE FROM books_authors WHERE book_id = @Book_id;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@Book_id";
      bookIdParameter.Value = this.GetId();

      cmd.Parameters.Add(bookIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
