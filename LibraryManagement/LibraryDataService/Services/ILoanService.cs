using LibraryDataService.DTOs;
using LibraryDataService.Results;

namespace LibraryDataService.Services
{
    public interface ILoanService
    {
        Task<LoanOperationStatus> CreateLoan(LoanDTO loan);
    }
}