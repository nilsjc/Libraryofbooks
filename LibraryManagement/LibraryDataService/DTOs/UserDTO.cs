namespace LibraryDataService.DTOs
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public List<HistoricLoanDTO> HistoricalLoans { get; set; }
        public List<LoanDTO> ActualLoans { get; set; }
    }
}