using LibraryDataService.DTOs;

namespace LibraryDataService.Services
{
    public interface IBookService
    {
        Task InsertBookAsync(BookDTO book);
        Task<IEnumerable<BookDTO>> GetBookAsync(string title);
        
    }
}