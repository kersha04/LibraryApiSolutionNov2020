using LibraryAPI.Models.BookReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Data
{
    public class BookReservation
    {
        public int Id { get; set; }
        public string For { get; set; }
        public string BooksReserved { get; set; }
        public BookReservationStatus Status { get; set; }
    }
}
