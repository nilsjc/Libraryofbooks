namespace LibraryDataService.Models
{
    public record Book(
        int BookId,
        string Title,
        int TotalPages,
        int Copy
    );
}