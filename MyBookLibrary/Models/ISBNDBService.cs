using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace MyBookLibrary.Models
{
    public class ISBNDBService
    {
        public static Book GetBookInformation(string queryISBN)
        {
            try
            {
                //Create the REST Services 'Find by Query' request
                string bookRequest = CreateRequest(queryISBN);
                XmlDocument locationsResponse = MakeRequest(bookRequest);
                Book book = ProcessResponse(locationsResponse);
                return book;
            }
            catch (Exception e)
            {
                return null;
            }
        }        

        //Create the request URL
        private static string CreateRequest(string queryISBN)
        {
            // TODO: URL should be extracted from .config file
            string UrlRequest = "http://isbndb.com/api/books.xml?access_key=4FTGN3SW&results=texts&index1=isbn&value1=" + queryISBN;
            return (UrlRequest);
        }

        //Submit the HTTP Request and return the XML response
        private static XmlDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return (xmlDoc);

            }
            catch (Exception e)
            {               
                return null;
            }
        }

        private static Book ProcessResponse(XmlDocument booksResponse)
        {            
            //Create namespace manager
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(booksResponse.NameTable);
            nsmgr.AddNamespace("rest", "http://schemas.microsoft.com/search/local/ws/rest/v1");

            //Get all books in the response and extract the firstorDefault
            XmlNodeList bookElements = booksResponse.SelectNodes("//ISBNdb/BookList/BookData", nsmgr);
            foreach (XmlNode book in bookElements)
            {
                Book findBook = new Book(){
                    Title = book["Title"].InnerText,
                    Author = book["AuthorsText"].InnerText,
                    Publisher = book["PublisherText"].InnerText,
                    Summary = book["Summary"].InnerText,
                    Notes = book["Notes"].InnerText
                };
                // Return first avilable book information
                return findBook;
            }
            return null;
        }
    }
}