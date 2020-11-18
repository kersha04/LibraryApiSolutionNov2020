using LibraryAPI.Models.BookReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Services.Reservations
{
    public interface IProcessBookReservations
    {
        Task LogOrder(GetReservationResponse request);
    }
}
