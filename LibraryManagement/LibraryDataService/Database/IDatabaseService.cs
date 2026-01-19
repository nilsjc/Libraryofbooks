using LibraryDataService.Models;
using LibraryDataService.Results;
namespace LibraryDataService.Database
{
    public interface IDatabaseService
    {
        Task InsertBook(Book book);
        Task InsertUser(string userName);
        Task<LoanOperationResult> StartLoan(int bookId, int userId);
        Task<LoanOperationResult> StopLoan(int bookId); 
        Task<IEnumerable<Book>> GetBooksByTitle(string title); // can include multiple copies
        Task<IEnumerable<Book>> GetBooksInfoByIds(IEnumerable<int> bookIds);
        Task<Book> GetBookInfoById(int bookId);
        Task<int> GetUserId(string userName);
        Task<UserInfo> GetUserInfo(int userId);
        Task<Loan> GetLoanInfo(int loanId);
        Task<IEnumerable<int>> GetMostPopularBooksId(int topN);
        Task<IEnumerable<Book>> LoanedBooksHistoryByUser(int userId, DateTime? start, DateTime? end);
        Task<int> GetLoanCountForBook(string title, DateTime? start, DateTime? end);
        Task<int> GetLoanDaysForBook(string title, DateTime? start, DateTime? end);
        Task<IEnumerable<Book>> GetCurrentlyBorrowedCopiesCount(string title);
    }
}