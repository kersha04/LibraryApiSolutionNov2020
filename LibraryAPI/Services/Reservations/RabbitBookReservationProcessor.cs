using LibraryAPI.Models.BookReservations;
using RabbitMqUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Services.Reservations
{
    public class RabbitBookReservationProcessor : IProcessBookReservations
    {
        IRabbitManager _manager;

        public RabbitBookReservationProcessor(IRabbitManager manager)
        {
            _manager = manager;
        }

        public Task LogOrder(GetReservationResponse request)
        {
            _manager.Publish(request, "", "direct", "reservations");
            
            return Task.CompletedTask;
        }
    }
}
