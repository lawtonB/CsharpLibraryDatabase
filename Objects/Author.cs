using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace LibraryNameSpace
{
  public class Author
  {
    private int _id;
    private string _author_name;

    public Author(string Author_name, int Id = 0)
    {
      _id = Id;
      _author_name = Author_name;
    }

    public override bool Equals(System.Object otherAuthor)
    {
        if (!(otherAuthor is Author))
        {
          return false;
        }
        else
        {
          Author newAuthor = (Author) otherAuthor;
          bool idEquality = this.GetId() == newAuthor.GetId();
          bool author_nameEquality = this.GetAuthor_name() == newAuthor.GetAuthor_name();
          return (idEquality && author_nameEquality);
        }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetAuthor_name()
    {
      return _author_name;
    }
    public void SetAuthor_name(string newAuthor_name)
    {
      _author_name = newAuthor_name;
    }

    public void AddBook(Book newBook)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO books_authors (author_id, book_id) VALUES (@AuthorId, @BookId)", conn);

      SqlParameter AuthorIdParameter = new SqlParameter();
      AuthorIdParameter.ParameterName = "@AuthorId";
      AuthorIdParameter.Value = this.GetId();
      cmd.Parameters.Add(AuthorIdParameter);

      SqlParameter booksIdParameter = new SqlParameter();
      booksIdParameter.ParameterName = "@BookId";
      booksIdParameter.Value = newBook.GetId();
      cmd.Parameters.Add(booksIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public static List<Author> GetAll()
    {
      List<Author> allAuthors = new List<Author>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM authors;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int AuthorId = rdr.GetInt32(0);
        string AuthorAuthor_name= rdr.GetString(1);
        Author newAuthor = new Author(AuthorAuthor_name, AuthorId);
        allAuthors.Add(newAuthor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allAuthors;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO authors (author_name) OUTPUT INSERTED.id VALUES (@AuthorAuthor_name);", conn);

      SqlParameter author_nameParameter = new SqlParameter();
      author_nameParameter.ParameterName = "@AuthorAuthor_name";
      author_nameParameter.Value = this.GetAuthor_name();
      cmd.Parameters.Add(author_nameParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM authors;", conn);
      cmd.ExecuteNonQuery();
    }

    public static Author Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM authors WHERE id = @AuthorId;", conn);

      SqlParameter AuthorIdParameter = new SqlParameter();
      AuthorIdParameter.ParameterName = "@AuthorId";
      AuthorIdParameter.Value = id.ToString();
      cmd.Parameters.Add(AuthorIdParameter);
      rdr = cmd.ExecuteReader();

      int foundAuthorId = 0;
      string foundAuthorAuthor_name = null;

      while(rdr.Read())
      {
        foundAuthorId = rdr.GetInt32(0);
        foundAuthorAuthor_name = rdr.GetString(1);
      }
      Author foundAuthor = new Author(foundAuthorAuthor_name, foundAuthorId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundAuthor;
    }

    public List<Book> GetBooks()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      List<Book> books = new List<Book>{};

      SqlCommand cmd = new SqlCommand("SELECT books.* FROM authors JOIN books_authors on (authors.id = books_authors.author_id) JOIN books on (books.id = books_authors.book_id) where authors.id = @AuthorId", conn);

      //select from the books table
      //get your targets from the authors table
      //use the join table to bring these together
      //specifically, on the join table where the real author id is equal to the join table author_id column Value
      //then, come from the other side, the books table.  look in the join table where the real book id = the join table book_id column
      //and do all of this based on teh authorID

      //select books for the author.  first look on teh join table where the author id matches and grab all those books
      // then look on the books table where it matches the author id


      SqlParameter AuthorIdParameter = new SqlParameter();
      AuthorIdParameter.ParameterName = "@AuthorId";
      AuthorIdParameter.Value = this.GetId();

      cmd.Parameters.Add(AuthorIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> booksIds = new List<int> {};
      while(rdr.Read())
      {
        int booksId = rdr.GetInt32(0);
        string booksName = rdr.GetString(1);
        Book newBook = new Book(booksName, booksId);
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

    // public static Book SearchBooks(string searchBooks)
    // {
    //   SqlConnection conn = DB.Connection();
    //   SqlDataReader rdr = null;
    //   conn.Open();
    //
    //   // List<Book> books = new List<Book>{};
    //
    //   SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE title = @search", conn);
    //
    //   SqlParameter AuthorIdParameter = new SqlParameter();
    //   AuthorIdParameter.ParameterName = "@search";
    //   AuthorIdParameter.Value = searchBooks;
    //
    //   cmd.Parameters.Add(AuthorIdParameter);
    //
    //   rdr = cmd.ExecuteReader();
    //
    //   // List<int> booksIds = new List<int> {};
    //   int foundBookId = 0;
    //   string foundBookTitle = null;
    //
    //
    //   while(rdr.Read())
    //   {
    //     int foundBookId = rdr.GetInt32(0);
    //     string foundBookTitle = rdr.GetString(1);
    //
    //     Book newBook = new Book(booksName, booksId);
    //
    //     // books.Add(newBook);
    //   }
    //   if (rdr != null)
    //   {
    //     rdr.Close();
    //   }
    //   if (conn != null)
    //   {
    //     conn.Close();
    //   }
    //
    //   return newBook;
    // }

    public void Update(string newAuthor_name)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE authors SET author_name = @NewAuthor_name OUTPUT INSERTED.author_name WHERE id = @AuthorId;", conn);

      SqlParameter newAuthor_nameParameter = new SqlParameter();
      newAuthor_nameParameter.ParameterName = "@NewAuthor_name";
      newAuthor_nameParameter.Value = newAuthor_name;
      cmd.Parameters.Add(newAuthor_nameParameter);


      SqlParameter AuthorIdParameter = new SqlParameter();
      AuthorIdParameter.ParameterName = "@AuthorId";
      AuthorIdParameter.Value = this.GetId();
      cmd.Parameters.Add(AuthorIdParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._author_name = rdr.GetString(0);
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
    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM authors WHERE id = @AuthorId; DELETE FROM books_authors WHERE Author_id = @AuthorId;", conn);
      SqlParameter AuthorIdParameter = new SqlParameter();
      AuthorIdParameter.ParameterName = "@AuthorId";
      AuthorIdParameter.Value = this.GetId();

      cmd.Parameters.Add(AuthorIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
