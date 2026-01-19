using LibraryDataService.DTOs;

namespace LibraryWebAPI.GraphQL.Types
{
    public class BookType
    {
        public string Title { get; set; } = default!;
        public int TotalPages { get; set; }
        public bool Available { get; set; }
        public int Copy { get; set; }

        public static BookType FromDTO(BookDTO dto)
        {
            return new BookType
            {
                Title = dto.Title,
                TotalPages = dto.TotalPages,
                Available = dto.Available,
                Copy = dto.Copy
            };
        }
    }
}
