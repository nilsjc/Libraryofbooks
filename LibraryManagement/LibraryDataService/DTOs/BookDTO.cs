namespace LibraryDataService.DTOs
{
    public class BookDTO
    {
        public BookDTO(string title)
        {
            Title = title;
        }
        public string Title { get; set; }
        public int TotalPages { get; set; }
        public bool Available { get; set; }
        public int Copy { get; set; }
        public LoanDTO? ActualLoan { get; set; }
        public HistoricLoanDTO? HistoricalLoan { get; set; }
    }
}