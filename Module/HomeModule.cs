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



    }
  }
}
