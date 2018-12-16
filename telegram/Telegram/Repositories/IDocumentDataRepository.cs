namespace WavesBot.Repositories
{
    using System.Threading.Tasks;
    using Data;

    public interface IDocumentDataRepository
    {
        Task<DocumentData> ReadAsync(long identifier);

        Task<DocumentData[]> ReadAsync(long[] identifiers);

        Task CreateAsync(DocumentData user);

        Task UpdateAsync(DocumentData user);
    }
}