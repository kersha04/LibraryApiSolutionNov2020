using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryAPI.Data;
using LibraryAPI.Models.Books;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Services
{
    public class EfSqlBooks : ILookupBooks, IBookCommands
    {
        private readonly LibraryDataContext _context;
        private readonly MapperConfiguration _config;
        private readonly IMapper _mapper;

        public EfSqlBooks(LibraryDataContext context, MapperConfiguration config, IMapper mapper)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
        }

        public async Task<GetBookDetailsResponse> AddBook(PostBookRequest bookToAdd)
        {
            var book = _mapper.Map<Book>(bookToAdd);
            _context.Books.Add(book);

            await _context.SaveChangesAsync();

            var response = _mapper.Map<GetBookDetailsResponse>(book);

            return response;
        }

        public async Task<GetBooksResponse> GetAllBooks()
        {
            var books = await _context.GetBooksThatAreInInventory()
                .ProjectTo<GetBooksResponseItem>(_config)
                .ToListAsync();

            var response = new GetBooksResponse
            {
                Data = books
            };

            return response;
        }

        public async Task<GetBookDetailsResponse> GetBookById(int id)
        {
            var response = await _context.GetBooksThatAreInInventory()
                .Where(b => b.Id == id)
                .ProjectTo<GetBookDetailsResponse>(_config)
                .SingleOrDefaultAsync();

            return response;
        }

        public async Task Remove(int id)
        {
            var savedBook = await _context.GetBooksThatAreInInventory().SingleOrDefaultAsync(b => b.Id == id);
            if (savedBook != null)
            {
                //_context.Books.Remove(savedBook);
                savedBook.IsInInventory = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> TryUpdateGenre(int id, string newGenre)
        {
            var book = await _context.GetBooksThatAreInInventory().SingleOrDefaultAsync(b => b.Id == id);
            
            if (book != null)
            {
                book.Genre = newGenre;
                await _context.SaveChangesAsync();
                return true;
            }
            
            return false;
        }
    }
}
