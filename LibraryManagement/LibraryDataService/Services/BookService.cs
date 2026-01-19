using LibraryDataService.Database;
using LibraryDataService.DTOs;

namespace LibraryDataService.Services
{
    public class BookService : IBookService
    {
        IDatabaseService _databaseService;
        public BookService(IDatabaseService databaseService)
        {
           _databaseService = databaseService; 
        }
        public async Task<IEnumerable<BookDTO>> GetBookAsync(string title)
        {
            var result = await _databaseService.GetBooksByTitle(title);
            return result.Select(r => new BookDTO(r.Title)
            {
                Title = r.Title,
                TotalPages = r.TotalPages,
                Copy = r.Copy
            });
        }

        public Task InsertBookAsync(BookDTO book)
        {
            throw new NotImplementedException();
        }
    }
}