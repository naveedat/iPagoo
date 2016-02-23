using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MyBookLibrary.Models
{
    public class Borrower
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int BookID { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }

    public class BorrowerDBContext : DbContext
    {
        public DbSet<Borrower> Borrowers { get; set; }
    }
}