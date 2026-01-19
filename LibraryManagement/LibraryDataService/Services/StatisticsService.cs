using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryDataService.Database;
using LibraryDataService.DTOs;
using LibraryDataService.Models;

namespace LibraryDataService.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IBookService _bookService;
        private readonly ILoanService _loanService;

        public StatisticsService(
            IDatabaseService databaseService,
            IBookService bookService,
            ILoanService loanService)
        {
            _databaseService = databaseService;
            _bookService = bookService;
            _loanService = loanService;
        }

        /// <summary>
        /// Get the most borrowed books in a given time frame
        /// </summary>
        public async Task<List<(BookDTO, int)>> MostLoandedBooks(int topN, DateTime? start, DateTime? end)
        {
            var mostPopularBookIds = await _databaseService.GetMostPopularBooksId(topN);
            var result = new List<(BookDTO, int)>();
            var booksInfo = await _databaseService.GetBooksInfoByIds(mostPopularBookIds);
            
            foreach (var bookInfo in booksInfo)
            {
                var loanCount = await _databaseService.GetLoanCountForBook(bookInfo.Title, start, end);
                var bookDto = new BookDTO(bookInfo.Title)
                {
                    TotalPages = bookInfo.TotalPages,
                    Copy = bookInfo.Copy
                };
                result.Add((bookDto, loanCount));
            }

            return result;
        }

        /// <summary>
        /// Get total days a book was borrowed (sum of all loans)
        /// </summary>
        public async Task<int> BookLoanInDays(BookDTO book, DateTime? start, DateTime? end)
        {
            return await _databaseService.GetLoanDaysForBook(book.Title, start, end);
        }

        /// <summary>
        /// Get how many copies are currently borrowed and available
        /// </summary>
        public async Task<(int borrowed, int available)> BookCopiesStatus(BookDTO book)
        {
            // var bookInfo = (await _databaseService.GetBooksByTitle(book.Title)).FirstOrDefault();
            // if (bookInfo == null)
            //     return (0, 0);

            // var borrowed = await _databaseService.GetCurrentlyBorrowedCopiesCount(book.Title);
            // var available = bookInfo.Copy - borrowed;

            // return (borrowed, available);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get users who borrowed the most books in a time frame
        /// </summary>
        public async Task<List<UserDTO>> MostUsersMostBooks(int topN, DateTime? start, DateTime? end)
        {
            // This would require querying all loans and aggregating by user
            // For now, returning empty list - needs full database query capability
            return new List<UserDTO>();
        }

        /// <summary>
        /// Get all books borrowed by a specific user in a time frame
        /// </summary>
        public async Task<List<BookDTO>> WhichBooksLoaned(UserDTO user, DateTime? start, DateTime? end)
        {
            var userId = await _databaseService.GetUserId(user.UserName);
            var books = await _databaseService.LoanedBooksHistoryByUser(userId, start, end);
            return [.. books.Select(b => new BookDTO(b.Title)
            {
                TotalPages = b.TotalPages,
                Copy = b.Copy
            })];
        }

        /// <summary>
        /// Get books that were borrowed by the same user who borrowed this book
        /// </summary>
        public async Task<List<BookDTO>> OtherBooksBorrowedWith(BookDTO book, DateTime? start, DateTime? end)
        {
            var bookInfo = await _databaseService.GetBooksByTitle(book.Title);
            // This requires complex join queries across loans and books
            // For now, returning empty list - needs full database query capability
            return new List<BookDTO>();
        }

        /// <summary>
        /// Calculate average read rate (pages per day) for a book
        /// </summary>
        public async Task<int> ReadRate(BookDTO book)
        {
            var totalDays = await _databaseService.GetLoanDaysForBook(book.Title, null, null);
            
            if (totalDays == 0)
                return 0;

            return book.TotalPages / totalDays;
        }

        public async Task<List<LoanDTO>> GetLoansByBook(int bookId, DateTime? start, DateTime? end)
        {
            throw new NotImplementedException();
        }
        public async Task<List<LoanDTO>> GetLoansByUser(int userId, DateTime? start, DateTime? end)
        {
            throw new NotImplementedException();
        }

        public async Task<List<(int UserId, int LoanCount)>> GetUserLoanCounts(DateTime? start, DateTime? end)
        {
            throw new NotImplementedException();
        }
    }
}
