using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyBoards.Dto;
using MyBoards.Entities;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

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
            builder.Services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            //K28


            //P10 rejestracja kontekstu bazy
            builder.Services.AddDbContext<MyBoardsContext>(
                option => option
         //       .UseLazyLoadingProxies()
                .UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnectionString"))
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

            if (!users.Any())
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

            if (!tags.Any())
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

            app.MapGet("pagination", async (MyBoardsContext db) =>
            {
                //user input
                var filter = "mail";
                string sortBy = "FullName"; // "FullName", "Email" null
                bool sortByDescending = false;
                int pageNumber = 4;
                int pageSize = 5;
                //

                //1. filtrowanie
                //2. Sortowanie
                //3. Paginacja


                //1. filtrowanie
                var query = db.Users
                    .Where(u => filter == null || (u.FullName.ToLower().Contains(filter.ToLower())
                                                    || u.Email.ToLower().Contains(filter.ToLower())));
                var totalItems = query.Count();

                //2. Sortowanie    
                if (sortBy != null)
                {
                    
                    var columnsSelector = new Dictionary<string, Expression<Func<User, object>>>
                    {
                        { nameof(User.Email), user => user.Email },
                        { nameof(User.FullName), user => user.FullName} 
                    };

                    //Expression<Func<User, object>> sortByExpression = columnsSelector[sortBy];
                    var sortByExpression = columnsSelector[sortBy];
                    query = sortByDescending 
                        ? query.OrderByDescending(sortByExpression)
                        : query.OrderBy(sortByExpression);

                }

                //3. Paginacja
                var result = query.Skip((pageNumber-1)*pageSize)
                        .Take(pageSize)
                        .ToList();

                var pagedResult = new PagedResult<User>(result, totalItems, pageNumber, pageSize);

                return pagedResult;


            }); 

            app.MapGet("data", async (MyBoardsContext db) =>
            {
            //var minWorkItemsCount = 85;
            //var states = db.WorkItemsStates
            //    .FromSqlInterpolated($@"SELECT wis.Id, wis.Value
            //                FROM WorkItems AS wi, WorkItemsStates AS wis
            //                WHERE wi.StateId = wis.Id
            //                GROUP BY wis.Id, wis.Value
            //                HAVING Count(*) > {minWorkItemsCount}")
            //    .Select(u => u.Value)
            //    .ToList();

            //db.Database.ExecuteSqlRaw(@"UPDATE Comments
            //                SET UpdatedDate = GETDATE()
            //                 WHERE AuthorId = '0AD08268-24F5-47CA-CBE8-08DA10AB0E61'");

            //return states;

            var user = db.Users
                        .FirstOrDefault(u => u.Id == Guid.Parse("78CF834E-7724-4995-CBC4-08DA10AB0E61"));


                return user;


            });

            app.MapGet("dataV2", async (MyBoardsContext db) =>
            {
                var user = await db.Users
                            .Include(u => u.Comments)
                            .Include(u => u.Address)
                            .Include(u => u.WorkItems).ThenInclude(wi => wi.Comments)
                            .FirstOrDefaultAsync(u => u.Id == Guid.Parse("0AC6DA2A-CF70-48A0-CC21-08DA10AB0E61"));
                return  user;

            });

           app.MapPut("update", async (MyBoardsContext db) =>
            {
                Epic epic = await db.Epics.FirstAsync(e => e.Id == 1);

                var RejectedStateId = await db.WorkItemsStates.FirstAsync(wis => wis.Value == "To do");

                epic.State = RejectedStateId;

                await db.SaveChangesAsync();

                return epic;

            });


            app.MapPost("create", async (MyBoardsContext db) =>
            {

                var user = new User()
                {
                    FullName = "MyAdmin3",
                    Email = "admin@gmail.com",
                    Address = new Address()
                    {
                        Country = "USA",
                        City = "ChelseaMa",
                        Street = "23 Front st",
                        PostalCode = "02150"
                    }
                };

                await db.Users.AddAsync(user);
                //await db.SaveChangesAsync();

                return user;

            });

            app.MapDelete("delete", async (MyBoardsContext db, Guid id) =>
            {
                var user = await db.Users
                                    .Include(u => u.Comments)
                                    .FirstOrDefaultAsync(u => u.Id == id);

                db.Users.RemoveRange(user);

                await db.SaveChangesAsync();

            })
            .Accepts<Guid>("application/json");

            app.Run();
        }
    }
}