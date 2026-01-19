using HotChocolate;
using LibraryDataService.DTOs;
using LibraryDataService.Services;
using LibraryWebAPI.GraphQL.Types;

namespace LibraryWebAPI.GraphQL.Queries
{
    public class StatisticsQuery
    {
        public async Task<List<MostBorrowedBookType>> MostBorrowedBooks(
            [Service] IStatisticsService statisticsService,
            int topN,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var result = await statisticsService.MostLoandedBooks(topN, startDate, endDate);
            
            return result
                .Select(x => new MostBorrowedBookType
                {
                    Book = BookType.FromDTO(x.Item1),
                    BorrowCount = x.Item2
                })
                .ToList();
        }

        public async Task<int> BookLoanInDays(
            [Service] IStatisticsService statisticsService,
            string bookTitle,
            int totalPages,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var book = new BookDTO(bookTitle) { TotalPages = totalPages };
            return await statisticsService.BookLoanInDays(book, startDate, endDate);
        }

        public async Task<BookCopiesStatusType> BookCopiesStatus(
            [Service] IStatisticsService statisticsService,
            string bookTitle,
            int totalPages)
        {
            var book = new BookDTO(bookTitle) { TotalPages = totalPages };
            var (borrowed, available) = await statisticsService.BookCopiesStatus(book);
            
            return new BookCopiesStatusType
            {
                Borrowed = borrowed,
                Available = available
            };
        }

        public async Task<List<UserType>> MostUsersMostBooks(
            [Service] IStatisticsService statisticsService,
            int topN,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var result = await statisticsService.MostUsersMostBooks(topN, startDate, endDate);
            return result.Select(UserType.FromDTO).ToList();
        }

        public async Task<List<BookType>> WhichBooksLoaned(
            [Service] IStatisticsService statisticsService,
            string userName,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var user = new UserDTO { UserName = userName };
            var result = await statisticsService.WhichBooksLoaned(user, startDate, endDate);
            return result.Select(BookType.FromDTO).ToList();
        }

        public async Task<List<BookType>> OtherBooksBorrowedWith(
            [Service] IStatisticsService statisticsService,
            string bookTitle,
            int totalPages,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var book = new BookDTO(bookTitle) { TotalPages = totalPages };
            var result = await statisticsService.OtherBooksBorrowedWith(book, startDate, endDate);
            return result.Select(BookType.FromDTO).ToList();
        }

        public async Task<int> ReadRate(
            [Service] IStatisticsService statisticsService,
            string bookTitle,
            int totalPages)
        {
            var book = new BookDTO(bookTitle) { TotalPages = totalPages };
            return await statisticsService.ReadRate(book);
        }
    }
}
