namespace WavesBot.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class UserDataRepository : IUserDataRepository
    {
        private readonly Func<UserDataDbContext> dbFactory;
        private UserDataDbContext DbContext => dbFactory();

        public UserDataRepository(Func<UserDataDbContext> dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public async Task<UserData> ReadAsync(long identifier)
        {
            return await DbContext.TelegramUsers.SingleOrDefaultAsync(x => x.Identifier == identifier);
        }

        public async Task<UserData[]> ReadAsync(long[] identifiers)
        {
            return await DbContext.TelegramUsers.Where(x => identifiers.Contains(x.Identifier)).ToArrayAsync();
        }

        public async Task<UserData> ReadAsync(string nickName)
        {
            return await DbContext.TelegramUsers.SingleOrDefaultAsync(x => x.NickName == nickName);
        }

        public async Task CreateAsync(long identifier, UserData user)
        {
            using (var ctx = DbContext)
            {
                await ctx.TelegramUsers.AddAsync(user);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(UserData user)
        {
            using (var ctx = DbContext)
            {
                ctx.Entry(user).State = EntityState.Modified;
                await ctx.SaveChangesAsync();
            }
        }
    }
}