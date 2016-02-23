using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyBookLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookLibrary.Tests.Controllers
{
     [TestClass]
    public class ISBNServiceTest
    {
         [TestMethod]
         public void GetBookInformationTest()
         {
             // calling external Restfull service (ISBNDB)
             Book book = ISBNDBService.GetBookInformation("0061031321");
             Assert.AreEqual("Thief of time", book.Title);
         }
    }
}
