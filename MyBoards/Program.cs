using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;

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

            app.Run();
        }
    }
}