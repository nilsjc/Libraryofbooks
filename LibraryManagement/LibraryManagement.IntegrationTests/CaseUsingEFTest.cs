using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LibraryManagement.IntegrationTests
{
    /*
    - What are the most borrowed books?
    - How many copies of a particular book are currently borrowed/available?
    - Which users borrowed the most books in a given time frame?
    - What books has an individual borrowed in each time frame?
    - What other books were borrowed by individuals that borrowed a particular book?
    - Roughly, what is the read rate (pages per day) for a particular book, assuming users start reading a book as soon as they borrow it and return it as soon as they are done reading it?

    */
    public class CaseUsingEFTest
    {
        
        [Fact]
        public void MostBorrowedBooks()
        {
            using SqliteConnection? connection = null;
            var dbService = CreateTestDatabaseService(connection);
            Assert.True(true);

        }

        [Fact]
        public void CopiesBorrowedAvailable()
        {
            Assert.True(true);
        }

        [Fact]
        public void TopUsersByBorrowedBooks()
        {
            Assert.True(true);
        }

        [Fact]
        public void BooksBorrowedByIndividualInTimeFrame()
        {
            Assert.True(true);
        }

        [Fact]
        public void OtherBooksBorrowedByIndividuals()
        {
            Assert.True(true);
        }

        [Fact]
        public void ReadRateForBook()
        {
            Assert.True(true);
        }

        public static EFDatabaseService CreateTestDatabaseService(SqliteConnection? connection)
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var services = new ServiceCollection();
            services.AddDbContextFactory<LibraryDbContext>(opts =>
            {
                opts.UseSqlite(connection);
            });
            var provider = services.BuildServiceProvider();

            var factory = provider.GetRequiredService<IDbContextFactory<LibraryDbContext>>();
            using (var ctx = factory.CreateDbContext())
            {
                ctx.Database.EnsureCreated();
            }

            return new EFDatabaseService(factory);
        }
    }
}