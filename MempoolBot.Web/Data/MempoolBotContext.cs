using MempoolBot.Web.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MempoolBot.Web.Data
{
    public class MempoolBotContext : DbContext
    {
        public DbSet<Settings> Settings { get; set; }
        public DbSet<TelegramSettings> TelegramSettings { get; set; }

        public string DbPath { get; }

        public MempoolBotContext(DbContextOptions<MempoolBotContext> options): base(options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "blogging.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql($"Data Source={DbPath}");
    }
}

