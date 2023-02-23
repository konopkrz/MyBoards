using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;
using Newtonsoft.Json;

namespace MyBoards
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //P28
            // builder.Services.AddControllers();

            //// usuwanie zapêtleñ: sposób pierwszy
            //builder.Services.AddControllers().AddJsonOptions(option =>
            //    option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            //// usuwanie zapêtleñ: sposób drugi
            builder.Services.AddControllers().AddNewtonsoftJson(
                option => option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            //K28

            //P10 rejestracja kontekstu bazy
            builder.Services.AddDbContext<MyBoardsContext>( 
                option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnectionString"))
                );
            //K10

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();

            var pendingMigration = dbContext.Database.GetPendingMigrations();
            //czy nie jest null'em i czy ma jak¹kolwiek wartoœæ
            if (pendingMigration != null && pendingMigration.Any())
            {
                dbContext.Database.Migrate();
            }

            var users = dbContext.Users.ToList();

            if(!users.Any())
            {
                var user1 = new User() {
                    FullName = "John Smith",
                    Email = "jsmith@gmail.com",
                    Address = new Address()
                    {
                        Street = "Long st. 19",
                        City = "Chelsea",
                        PostalCode = "02150"
                    }
                };
                var user2 = new User()
                {
                    FullName = "Paul Newman",
                    Email = "paulnewman@gmail.com",
                    Address = new Address()
                    {
                        Street = "Victory st. 4",
                        City = "Derry",
                        PostalCode = "04550"
                    }
                };

                dbContext.Users.AddRange(user1, user2);

                dbContext.SaveChanges();
            }

            var tags = dbContext.Tags.ToList();

            if(!tags.Any())
            {
                var tagList = new List<Tag> {
                    new Tag { Value = "Web"},
                    new Tag { Value = "UI"},
                    new Tag { Value = "Desktop"},
                    new Tag { Value = "API"},
                    new Tag { Value = "Service"}
                };

                dbContext.Tags.AddRange(tagList);

                dbContext.SaveChanges();
            };

            app.MapGet("data", async (MyBoardsContext db) =>
            {

                //var newComments = await db
                //    .Comments
                //    .Where(c => c.CreatedDate > new DateTime(2022, 07, 23))
                //    .ToListAsync();

                //return newComments;

                //var top5Comments = await db
                //    .Comments
                //    .OrderByDescending(c => c.CreatedDate)
                //    .Take(5)
                //    .ToListAsync();

                //return top5Comments;

                //var epicList = await db
                //    .Epics
                //    .Where(e => e.State.Value == "On hold")
                //    .OrderBy(e => e.Priority)
                //    .ToListAsync();

                var authors = await db
                    .Comments
                    .GroupBy(c => c.AuthorId)
                    .Select(g => new { authorId = g.Key, count = g.Count() })
                    .ToListAsync();

                var topAuthor = authors
                    .FirstOrDefault(a => a.count == authors.Max(ac => ac.count));

                var selectedAuthor = await db
                    .Users
                    .Where(u => u.Id == topAuthor.authorId)
                    .FirstOrDefaultAsync();

                return new { selectedAuthor, count = topAuthor.count  };

            });

            app.Run();
        }
    }
}