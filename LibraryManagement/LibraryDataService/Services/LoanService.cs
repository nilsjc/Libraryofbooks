using LibraryDataService.DTOs;
using LibraryDataService.Results;

namespace LibraryDataService.Services
{
    public class LoanService : ILoanService
    {
        public Task<LoanOperationStatus> CreateLoan(LoanDTO loan)
        {
            throw new NotImplementedException();
        }
    }
}