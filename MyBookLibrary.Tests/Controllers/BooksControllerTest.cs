using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyBookLibrary.Controllers;
using System.Web.Mvc;
using MyBookLibrary.Models;

namespace MyBookLibrary.Tests.Controllers
{
    [TestClass]
    public class BooksControllerTest
    {
        [TestMethod]
        public void TestIndexData()
        {
            var controller = new BooksController();
            var result = controller.Index("0061031321", "", "") as ViewResult;
            var book = (Book)result.ViewData.Model;
            Assert.AreEqual("Terry Pratchett", book.Author);
        }

        [TestMethod]
        public void TestDetailsViewData()
        {
            var controller = new BooksController();
            var result = controller.Details(1) as ViewResult;
            var book = (Book)result.ViewData.Model;
            Assert.AreEqual("0061031321", book.ISBN);
        }
    }
}
