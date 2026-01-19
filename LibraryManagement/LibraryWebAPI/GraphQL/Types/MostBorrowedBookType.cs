using LibraryDataService.DTOs;

namespace LibraryWebAPI.GraphQL.Types
{
    public class MostBorrowedBookType
    {
        public BookType Book { get; set; } = default!;
        public int BorrowCount { get; set; }
    }
}
