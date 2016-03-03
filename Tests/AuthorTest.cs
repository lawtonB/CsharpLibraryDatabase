using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace LibraryNameSpace
{
  public class AuthorTest : IDisposable
  {
    public AuthorTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_EmptyAtFirst()
    {
      //Arrange, Act
      int result = Author.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameDescription()
    {
      //Arrange, Act
      Author firstAuthor = new Author("Magic Johnson");
      Author secondAuthor = new Author("Magic Johnson");

      //Assert
      Assert.Equal(firstAuthor, secondAuthor);

    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Author testAuthor = new Author("Magic Johnson");
      testAuthor.Save();

      //Act
      List<Author> result = Author.GetAll();
      List<Author> testList = new List<Author>{testAuthor};

      //Assert
      Assert.Equal(testList, result);

    }



    [Fact]
    public void Test_SaveAssignsIdToObject()
    {
      //Arrange
      Author testAuthor = new Author("Magic Johnson");
      testAuthor.Save();

      //Act
      Author savedAuthor = Author.GetAll()[0];

      int result = savedAuthor.GetId();
      int testId = testAuthor.GetId();

      //Assert
      Assert.Equal(testId, result);

    }

    [Fact]
    public void Test_FindFindsAuthorInDatabase()
    {
      //Arrange
      Author testAuthor = new Author("Magic Johnson");
      testAuthor.Save();

      //Act
      Author result = Author.Find(testAuthor.GetId());

      //Assert
      Assert.Equal(testAuthor, result);

    }

    [Fact]
    public void Test_AddBook_AddsBookToAuthor()
    {
      //Arrange
      Author testAuthor =new Author("Magic Johnson");
      testAuthor.Save();

      Book testBook = new Book("Math", false);
      testBook.Save();

      testAuthor.AddBook(testBook);

      //Act
      List<Book> result = testAuthor.GetBooks();

      List<Book> testList = new List<Book>{testBook};

      //Assert
      Assert.Equal(testList, result);


    }

  [Fact]
  public void Test_GetBooks_ReturnsAllAuthorBooks()
  {
    //Arrange
    Author testAuthor = new Author("Magic Johnson");
    testAuthor.Save();

    Book testBook1 = new Book("Math", false);
    testBook1.Save();

    Book testBook2 = new Book("Gym", false);
    testBook2.Save();

    //Act
    testAuthor.AddBook(testBook1);
    List<Book> result = testAuthor.GetBooks();
    List<Book> testList = new List<Book> {testBook1};

    //Assert
    Assert.Equal(testList, result);


  }

  // [Fact]
  // public void Test_Search_ReturnsAllSearchedBooks()
  // {
  //   //Arrange
  //   Author testAuthor = new Author("Magic Johnson");
  //   testAuthor.Save();
  //
  //   Book testBook1 = new Book("Math", false);
  //   testBook1.Save();
  //
  //   Book testBook2 = new Book("Gym", false);
  //   testBook2.Save();
  //
  //   //Act
  //   testAuthor.AddBook(testBook1);
  //   List<Book> result = Author.SearchBooks("Math");
  //   List<Book> testList = new List<Book> {testBook1};
  //
  //   //Assert
  //   Assert.Equal(testList, result);
  //
  // }

  public void Test_Delete_DeletesAuthorAssociationsFromDatabase()
  {
    //Arrange
    Book testBook = new Book("Math", false);
    testBook.Save();

    Author testAuthor = new Author("Magic Johnson");
    testAuthor.Save();

    //Act
    testAuthor.AddBook(testBook);
    testAuthor.Delete();

    List<Author> resultBookAuthors = testBook.GetAuthors();
    List<Author> testBookAuthors = new List<Author> {};

    //Assert
    Assert.Equal(testBookAuthors, resultBookAuthors);


  }


    public void Dispose()
    {
      Author.DeleteAll();
      Book.DeleteAll();
    }
  }
}
