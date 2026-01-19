using LibraryDataService.Database;
using LibraryDataService.Services;
using LibraryWebAPI.GraphQL.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<StatisticsQuery>();

// Add LibraryDataService
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IDatabaseService>(s => new DbSqliteEngine("library01"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map GraphQL endpoint
app.MapGraphQL();

app.Run();
