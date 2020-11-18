using LibraryAPI.Data;
using LibraryAPI.Filters;
using LibraryAPI.Models.BookReservations;
using LibraryAPI.Services.Reservations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Controllers
{
    public class BookReservationsController : ControllerBase
    {
        private readonly LibraryDataContext _context;
        private readonly IProcessBookReservations _orderProcessor;

        public BookReservationsController(LibraryDataContext context, IProcessBookReservations orderProcessor)
        {
            _context = context;
            _orderProcessor = orderProcessor;
        }

        [HttpPost("/bookreservations/approved")]
        public async Task<ActionResult> ApproveReservation([FromBody] GetReservationResponse request)
        {
            var savedReservation = await _context.Reservations.SingleOrDefaultAsync(b => b.Id == request.Id);

            if (savedReservation == null)
            {
                return BadRequest("That reservation do not exist!!");
            }

            savedReservation.Status = BookReservationStatus.Approved;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("/bookreservations/denied")]
        public async Task<ActionResult> DenyReservation([FromBody] GetReservationResponse request)
        {
            var savedReservation = await _context.Reservations.SingleOrDefaultAsync(b => b.Id == request.Id);

            if (savedReservation == null)
            {
                return BadRequest("That reservation do not exist!!");
            }

            savedReservation.Status = BookReservationStatus.Denied;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("bookreservations")]
        [ValidateModel]
        public async Task<ActionResult> AddReservation([FromBody] PostReservationRequest request)
        {
            var book = new BookReservation
            {
                For = request.For,
                BooksReserved = string.Join(",", request.Books),
                Status = BookReservationStatus.Pending
            };

            _context.Reservations.Add(book);

            await _context.SaveChangesAsync();

            var response = new GetReservationResponse
            {
                Id = book.Id,
                For = book.For,
                BooksReserved = book.BooksReserved,
                Status = book.Status
            };
            await _orderProcessor.LogOrder(response);

            return CreatedAtRoute("bookreservations#get-byid", new { id = book.Id }, response);
        }

        [HttpGet("bookreservations/{id:int}", Name = "bookreservations#get-byid")]
        public async Task<ActionResult> GetReservationById(int id)
        {
            var book = await _context.Reservations.SingleOrDefaultAsync(r => r.Id == id);
            
            if (book == null)
            {
                return NotFound();
            }

            var response = new GetReservationResponse
            {
                Id = book.Id,
                For = book.For,
                BooksReserved = book.BooksReserved,
                Status = book.Status
            };

            return Ok(response);
        }
    }
}
