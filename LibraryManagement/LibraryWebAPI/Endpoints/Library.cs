using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryDataService.Services;
using LibraryDataService.DTOs;
namespace LibraryWebAPI.Endpoints
{
    public static class Library
    {
        public static void MapLibraryEndpoints(this WebApplication app)
        {
            app.MapGet("/", () => "Welcome to the Library Management API!");
            app.MapPost("/books", AddBookAsync)
                .WithName("AddBook")
                .WithOpenApi();

            app.MapGet("/books/{title}", GetBookAsync)
                .WithName("GetBook")
                .WithOpenApi();
        }
        static async Task<IResult> AddBookAsync(IBookService bookService, BookDTO book)
        {
            await bookService.InsertBookAsync(book);
            return Results.Created($"/books/{book.Title}", book);
        }

        static async Task<List<BookDTO>> GetBookAsync(IBookService bookService, string title)
        {
            return [.. await bookService.GetBookAsync(title)];
        }
    }
}