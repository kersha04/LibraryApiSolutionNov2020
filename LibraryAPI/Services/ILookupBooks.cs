using LibraryAPI.Models.Books;
using System.Threading.Tasks;

namespace LibraryAPI
{
    public interface ILookupBooks
    {
        Task<GetBooksResponse> GetAllBooks();
        Task<GetBookDetailsResponse> GetBookById(int id);
    }
}