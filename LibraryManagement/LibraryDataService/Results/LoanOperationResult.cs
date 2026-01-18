using LibraryDataService.Models;
namespace LibraryDataService.Results
{
    public class LoanOperationResult
    {
        public LoanOperationStatus Status { get; set; }
        public Loan? BookLoan { get; set; }
        public string? ErrorMessage { get; set; }
    }
    public enum LoanOperationStatus
    {
        Success,
        BookNotAvailable,
        UserNotFound,
        InvalidInput,
        DatabaseError
    }
}