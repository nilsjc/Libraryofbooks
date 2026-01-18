using LibraryDataService.Models;
using LibraryDataService.Results;
namespace LibraryDataService.Database
{
    public interface IDatabaseService
    {
        Task InsertBook(Book book); // returns bookId
        Task InsertUser(string userName); // returns userId
        Task<LoanOperationResult> StartLoan(int bookId, int userId); // returns true or false depending if sucessful
        Task<LoanOperationResult> StopLoan(int bookId); // returns true or false depending if sucessful
        //Task<LoanOperationResult> StartLoan(Book book, int userId); // returns true or false depending if sucessful
        //Task<LoanOperationResult> StopLoan(Book book); // returns true or false depending if sucessful
        Task<Book> GetBookInfoByTitle(string title);
        Task<Book> GetBookInfoById(int bookId);
        Task<int> GetUserId(string userName);
        Task<UserInfo> GetUserInfo(int userId);
        Task<Loan> GetLoanInfo(int loanId);
        Task<IEnumerable<int>> GetMostPopularBooksId(int topN);
        // Task<LoanSpanInfo> GetTimeSpanInfo(DateTime date, int days);
        // Task<LoanSpanInfo> GetTimeSpanInfo(DateTime startDate, DateTime stopDate);

    }
}