using System.Threading.Tasks;

namespace LibraryAPI
{
    public interface ILookupOnCallDevelopers
    {
        Task<string> GetOnCallDeveloperAsync();
    }
}