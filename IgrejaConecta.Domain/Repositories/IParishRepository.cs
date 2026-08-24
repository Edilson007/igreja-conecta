using IgrejaConecta.Domain.Entities;
namespace IgrejaConecta.Domain.Repositories;
public interface IParishRepository
{
    Task<IReadOnlyCollection<Parish>> SearchAsync(string? city, string? sector, string? query, CancellationToken cancellationToken = default);
    Task<Parish?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
