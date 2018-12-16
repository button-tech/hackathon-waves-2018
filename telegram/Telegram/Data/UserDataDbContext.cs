namespace WavesBot.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class UserDataDbContext : DbContext
    {
        private readonly string connectionString;

        public UserDataDbContext(IOptions<SqlServerConfiguration> config)
        {
            connectionString = config.Value.ConnectionString;
        }

        public DbSet<UserData> TelegramUsers { get; set; }

        public DbSet<DocumentData> Documents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.UseInMemoryDatabase(connectionString);
#elif RELEASE || DEV || STAGE || MIGRATIONRELEASE || MIGRATIONSTAGE
            optionsBuilder.UseNpgsql(connectionString);
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserData>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<UserData>()
                .Property(x => x.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<DocumentData>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<DocumentData>()
                .Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}