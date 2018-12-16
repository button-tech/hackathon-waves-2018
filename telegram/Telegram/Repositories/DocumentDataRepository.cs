namespace WavesBot.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using IAL;
    using Microsoft.EntityFrameworkCore;

    public class DocumentDataRepository : IDocumentDataRepository
    {
        private readonly Func<UserDataDbContext> dbFactory;
        private UserDataDbContext DbContext => dbFactory();

        public DocumentDataRepository(Func<UserDataDbContext> dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public async Task<DocumentData> ReadAsync(long identifier)
        {
            return await DbContext.Documents.SingleOrDefaultAsync(x => x.Identifier == identifier);
        }

        public async Task<DocumentData[]> ReadAsync(long[] identifiers)
        {
            return await DbContext.Documents.Where(x => identifiers.Contains(x.Identifier)).ToArrayAsync();
        }

        public async Task CreateAsync(DocumentData user)
        {
            using (var ctx = DbContext)
            {
                await ctx.Documents.AddAsync(user);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(DocumentData user)
        {
            using (var ctx = DbContext)
            {
                ctx.Entry(user).State = EntityState.Modified;
                await ctx.SaveChangesAsync();
            }
        }
    }
}