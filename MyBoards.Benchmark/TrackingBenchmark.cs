using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBoards.Benchmark
{
    [Config(typeof(AntiVirusFriendlyConfig))]
    [MemoryDiagnoser]
    public class TrackingBenchmark
    {
        private const string MyBoardsConnectionString =
            "Server=(localdb)\\mssqllocaldb;Database=MyBoardsDb;Trusted_Connection=True;";

        [Benchmark]
        public int WithTracking()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyBoardsContext>();

            optionsBuilder.UseSqlServer(MyBoardsConnectionString);

            var _dbContext = new MyBoardsContext(optionsBuilder.Options);

            var comments = _dbContext.Comments
                .ToList();

            return comments.Count;
        }

        [Benchmark]
        public int WithNoTracking()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyBoardsContext>();

            optionsBuilder.UseSqlServer(MyBoardsConnectionString);

            var _dbContext = new MyBoardsContext(optionsBuilder.Options);

            var comments = _dbContext.Comments
                .AsNoTracking()
                .ToList();

            return comments.Count;
        }
    }
}
