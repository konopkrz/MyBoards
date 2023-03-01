using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Northwind.Entities;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Northwind
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

            // Add DbContext and configure string connection to database Northwind
            builder.Services.AddDbContext<NorthwindContext>(
                option => option
                .UseSqlServer(builder.Configuration.GetConnectionString("NorthwindConnectionString"))
                );
            //

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("data", async (NorthwindContext db) =>
            {
                var sampleData = await db.Products
                    //.Include(p => p.Supplier)
                    .Where(p => ((p.SupplierId != null) && (p.SupplierId > 20)))
                    .Take(10)
                    .Select(p => new { p.ProductName, p.Category.CategoryName, p.Supplier.CompanyName, p.Supplier.ContactName})
                    .ToListAsync();

                return sampleData;
            });

            app.MapGet("getOrderDetails", async (NorthwindContext db) =>
            {
                Order order = await GetOrder(10248, db, o => o.OrderDetails);

                return order;
            });

            app.MapGet("getOrderWithShipper", async (NorthwindContext db) =>
            {
                Order order = await GetOrder(10248, db, o => o.OrderDetails, o => o.ShipViaNavigation);

                return order;
            });

            app.MapGet("getOrderWithCustomer", async (NorthwindContext db) =>
            {
                Order order = await GetOrder(10248, db, o => o.Customer);

                return order;
            });

            app.Run();
        }

        private static async Task<Order> GetOrder(int orderId, NorthwindContext db, params Expression<Func<Order, object>>[] includes)
        {
            var baseQuery = db.Orders.AsQueryable();

            if(includes.Any())
            {
                foreach(var include in includes)
                {
                    baseQuery= baseQuery.Include(include);
                }
            }

            var order = await baseQuery.FirstAsync(o => o.OrderId == orderId);

            return order;
        }


    }

}