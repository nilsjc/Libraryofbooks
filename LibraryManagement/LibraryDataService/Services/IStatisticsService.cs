using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryDataService.DTOs;

namespace LibraryDataService.Services
{
    public interface IStatisticsService
    {
        // What are the most borrowed books?
        Task<List<(BookDTO, int)>> MostLoandedBooks(int topN, DateTime? start, DateTime? end);
        Task<int> BookLoanInDays(BookDTO book, DateTime? start, DateTime? end);

        // How many copies of a particular book are currently borrowed/available?
        Task<(int borrowed, int available)> BookCopiesStatus(BookDTO book);
        
        // Which users borrowed the most books in a given time frame?
        Task<List<UserDTO>> MostUsersMostBooks(int topN, DateTime? start, DateTime? end);
        
        // What books has an individual borrowed in each time frame?
        Task<List<BookDTO>> WhichBooksLoaned(UserDTO user, DateTime? start, DateTime? end);
        
        // What other books were borrowed by individuals that borrowed a particular book?
        Task<List<BookDTO>> OtherBooksBorrowedWith(BookDTO book, DateTime? start, DateTime? end);
        
        // Roughly, what is the read rate (pages per day) for a particular book, assuming users start reading a book as soon as they borrow it and return it as soon as they are done reading it?
        Task<int> ReadRate(BookDTO book);

    }
}