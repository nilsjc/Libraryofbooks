using LibraryDataService.DTOs;

namespace LibraryDataService.Services
{
    public interface IBookService
    {
        Task InsertBook(BookDTO book);
        Task<BookDTO> GetBook(string title);
        
    }
}