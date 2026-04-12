using DocumentProcessor.Avalonia.TerrenceLGee.Common.Pagination;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Parameters;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.RepositoryInterfaces;

public interface IContactRepository
{
    Task<Contact?> AddContactAsync(Contact contact);
    Task<bool> UpdateContactAsync(Contact contact);
    Task<bool> DeleteContactAsync(int contactId);
    Task<Contact?> GetContactAsync(int contactId);
    Task<PagedList<Contact>> GetContactsAsync(PaginationParams paginationParams);
}
