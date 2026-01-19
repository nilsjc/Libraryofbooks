using LibraryDataService.DTOs;

namespace LibraryDataService.Services
{
    public class BookService : IBookService
    {
        public Task<BookDTO> GetBook(string title)
        {
            throw new NotImplementedException();
        }

        public Task InsertBook(BookDTO book)
        {
            throw new NotImplementedException();
        }
    }
}