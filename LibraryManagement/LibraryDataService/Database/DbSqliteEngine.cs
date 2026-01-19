using Microsoft.Data.Sqlite;
using LibraryDataService.Models;
using LibraryDataService.Results;

namespace LibraryDataService.Database
{
    public class DbSqliteEngine : IDatabaseService
    {
        private readonly string DbName;
        public DbSqliteEngine(string name)
        {
            DbName = name;
            CreateTables(DbName);
        }
        private void CreateTables(string DbName)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS Books (
                    BookId INTEGER PRIMARY KEY,
                    Title TEXT NOT NULL,
                    TotalPages INTEGER NOT NULL,
                    Copy INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Users (
                    UserId INTEGER PRIMARY KEY,
                    UserName TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Loans (
                    LoanId INTEGER PRIMARY KEY,
                    BookId INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    StartDate DATETIME NOT NULL,
                    StopDate DATETIME,
                    FOREIGN KEY(BookId) REFERENCES Books(BookId),
                    FOREIGN KEY(UserId) REFERENCES Users(UserId)
                );

                CREATE TABLE IF NOT EXISTS HistoricLoans (
                    HistoricLoanId INTEGER PRIMARY KEY,
                    LoanId INTEGER NOT NULL,
                    UserId INTEGER NOT NULL,
                    BookId INTEGER NOT NULL,
                    StartDate DATETIME NOT NULL,
                    StopDate DATETIME NOT NULL,
                    TotalDays INTEGER NOT NULL,
                    FOREIGN KEY(LoanId) REFERENCES Loans(LoanId)
                );
                ";
            command.ExecuteNonQuery();
        }
        public async Task InsertBook(Book book)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO Books (Title, TotalPages, Copy)
                VALUES ($title, $totalPages, $copy);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$title", book.Title);
            command.Parameters.AddWithValue("$totalPages", book.TotalPages);
            command.Parameters.AddWithValue("$copy", book.Copy);
            command.ExecuteNonQuery();
        }
        public async Task InsertUser(string userName)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO Users (UserName)
                VALUES ($userName);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$userName", userName);
            command.ExecuteNonQuery();
        }
        public async Task<LoanOperationResult> StartLoan(int bookId, int userId)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            // kolla om användaren existerar
            command.CommandText =
            @"
                SELECT COUNT(*)
                FROM Users
                WHERE UserId = $userId;
            ";
            command.Parameters.AddWithValue("$userId", userId);
            using var userReader = command.ExecuteReader();
            userReader.Read();
            if (userReader.GetInt32(0) == 0)
            {
                // användaren finns inte
                return new LoanOperationResult
                {
                    Status = LoanOperationStatus.UserNotFound,
                    ErrorMessage = "User not found."
                };
            }

            // kolla om boken är tillgänglig
            command.CommandText =
            @"
                SELECT COUNT(*)
                FROM Loans
                WHERE BookId = $bookId AND StopDate IS NULL;
            ";
            command.Parameters.AddWithValue("$bookId", bookId);
            using var reader = command.ExecuteReader();
            reader.Read();
            if (reader.GetInt32(0) > 0)
            {
                // boken är redan lånad
                return new LoanOperationResult
                {
                    Status = LoanOperationStatus.BookNotAvailable,
                    ErrorMessage = "Book is already loaned out."
                };
            }

            // skapa lån
            command.CommandText =
            @"
                INSERT INTO Loans (BookId, UserId, StartDate)
                VALUES ($bookId, $userId, $startDate);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$bookId", bookId);
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$startDate", DateTime.Now);
            command.ExecuteNonQuery();
            return new LoanOperationResult
            {
                Status = LoanOperationStatus.Success
            };
        }
        public async Task<LoanOperationResult> StopLoan(int bookId)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            
            using var transaction = connection.BeginTransaction();
            try
            {
                // Uppdatera lånet med stoppdatum
                var updateCommand = connection.CreateCommand();
                updateCommand.Transaction = transaction;
                updateCommand.CommandText =
                @"
                    UPDATE Loans
                    SET StopDate = $stopDate
                    WHERE BookId = $bookId AND StopDate IS NULL;
                ";
                updateCommand.Parameters.AddWithValue("$bookId", bookId);
                updateCommand.Parameters.AddWithValue("$stopDate", DateTime.Now);
                updateCommand.ExecuteNonQuery();
                
                // Second: Skapa historiskt lån med total dagar
                var insertCommand = connection.CreateCommand();
                insertCommand.Transaction = transaction;
                insertCommand.CommandText =
                @"
                    INSERT INTO HistoricLoans (LoanId, TotalDays)
                    SELECT LoanId, CAST((julianday($stopDate) - julianday(StartDate)) AS INTEGER)
                    FROM Loans
                    WHERE BookId = $bookId AND StopDate = $stopDate;
                ";
                insertCommand.Parameters.AddWithValue("$bookId", bookId);
                insertCommand.Parameters.AddWithValue("$stopDate", DateTime.Now);
                insertCommand.ExecuteNonQuery();
                
                transaction.Commit();
                return new LoanOperationResult
                {
                    Status = LoanOperationStatus.Success
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<IEnumerable<Book>> GetBooksByTitle(string title)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT BookId, Title, TotalPages, Copy
                FROM Books
                WHERE Title = $title;
            ";
            command.Parameters.AddWithValue("$title", title);
            using var reader = command.ExecuteReader();
            var books = new List<Book>();
            while (reader.Read())
            {
                books.Add(new Book(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetInt32(3)
                ));
            }
            return books;
        }
        public async Task<Book> GetBookInfoById(int bookId)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT BookId, Title, TotalPages, Copy
                FROM Books
                WHERE BookId = $bookId;
            ";
            command.Parameters.AddWithValue("$bookId", bookId);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var book = new Book(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetInt32(3)
                );
                return book;
            }
            return null;
        }
        public async Task<int> GetUserId(string userName)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT UserId
                FROM Users
                WHERE UserName = $userName;
            ";
            command.Parameters.AddWithValue("$userName", userName);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetInt32(0);
            }
            return -1; // User not found
        }
        public async Task<UserInfo> GetUserInfo(int userId)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT UserId, UserName
                FROM Users
                WHERE UserId = $userId;
            ";
            command.Parameters.AddWithValue("$userId", userId);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var userInfo = new UserInfo(
                    reader.GetInt32(0),
                    reader.GetString(1)
                );
                return userInfo;
            }
            return null;
        }

        public async Task<Loan> GetLoanInfo(int loanId)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT LoanId, BookId, UserId, StartDate, StopDate
                FROM Loans
                WHERE LoanId = $loanId;
            ";
            command.Parameters.AddWithValue("$loanId", loanId);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var loan = new Loan
                {
                    LoanId = reader.GetInt32(0),
                    BookId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    StartDate = reader.GetDateTime(3),
                    StopDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                };
                return loan;
            }
            return null;
        }

        public async Task<IEnumerable<int>> GetMostPopularBooksId(int topN)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT BookId, COUNT(*) AS LoanCount
                FROM HistoricLoans
                GROUP BY BookId
                ORDER BY LoanCount DESC
                LIMIT $topN;
            ";
            command.Parameters.AddWithValue("$topN", topN);
            using var reader = command.ExecuteReader();
            var popularBookIds = new List<int>();
            while (reader.Read())
            {
                popularBookIds.Add(reader.GetInt32(0));
            }
            return popularBookIds;
        }

        public async Task<IEnumerable<Book>> LoanedBooksHistoryByUser(int userId, DateTime? start, DateTime? end)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT BookId, Title, Author, ISBN
                FROM Books
                WHERE BookId IN (
                    SELECT BookId
                    FROM HistoricLoans
                    WHERE UserId = $userId
                    AND ($start IS NULL OR StartDate >= $start)
                    AND ($end IS NULL OR StopDate <= $end)
                );
            ";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$start", start);
            command.Parameters.AddWithValue("$end", end);
            using var reader = command.ExecuteReader();
            var books = new List<Book>();
            while (reader.Read())
            {
                books.Add(new Book(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetInt32(3)
                ));
            }
            return books;
        }

        public async Task<int> GetLoanCountForBook(string title, DateTime? start, DateTime? end)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            
            command.CommandText =
            @"
                SELECT COUNT(*) AS LoanCount
                FROM HistoricLoans hl
                INNER JOIN Books b ON hl.BookId = b.BookId
                WHERE b.Title = $title
                AND ($start IS NULL OR hl.StartDate >= $start)
                AND ($end IS NULL OR hl.StopDate <= $end);
            ";
            command.Parameters.AddWithValue("$title", title);
            command.Parameters.AddWithValue("$start", start);
            command.Parameters.AddWithValue("$end", end);
            
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetInt32(0);
            }
            return 0;
        }

        public async Task<int> GetLoanDaysForBook(string title, DateTime? start, DateTime? end)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            
            command.CommandText =
            @"
                SELECT CAST(SUM(JULIANDAY(hl.StopDate) - JULIANDAY(hl.StartDate)) AS INTEGER) AS LoanDays
                FROM HistoricLoans hl
                INNER JOIN Books b ON hl.BookId = b.BookId
                WHERE b.Title = $title
                AND ($start IS NULL OR hl.StartDate >= $start)
                AND ($end IS NULL OR hl.StopDate <= $end);
            ";
            command.Parameters.AddWithValue("$title", title);
            command.Parameters.AddWithValue("$start", start);
            command.Parameters.AddWithValue("$end", end);
            
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
            }
            return 0;
        }

        public async Task<IEnumerable<Book>> GetBooksInfoByIds(IEnumerable<int> bookIds)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT BookId, Title, TotalPages, Copy
                FROM Books
                WHERE BookId IN ($bookIds);
            ";
            command.Parameters.AddWithValue("$bookIds", string.Join(",", bookIds));
            using var reader = command.ExecuteReader();
            var books = new List<Book>();
            while (reader.Read())
            {
                books.Add(new Book(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetInt32(3)
                ));
            }
            return books;
        }


        // How many copies of a particular book are currently borrowed?
        public async Task<IEnumerable<Book>> GetCurrentlyBorrowedCopiesCount(string title)
        {
            using var connection = new SqliteConnection($"Data source={DbName}");
            connection.Open();
            var command = connection.CreateCommand();
            
            command.CommandText =
            @"
                SELECT COUNT(*) AS BorrowedCount
                FROM Loans l
                INNER JOIN Books b ON l.BookId = b.BookId
                WHERE b.Title = $title AND l.StopDate IS NULL;
            ";
            command.Parameters.AddWithValue("$title", title);
            
            using var reader = command.ExecuteReader();
            var books = new List<Book>();
            while (reader.Read())
            {
                books.Add(new Book(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetInt32(3)
                ));
            }
            return books;
        }
    }
}