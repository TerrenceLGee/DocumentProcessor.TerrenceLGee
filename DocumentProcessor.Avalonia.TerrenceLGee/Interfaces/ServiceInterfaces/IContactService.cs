using DocumentProcessor.Avalonia.TerrenceLGee.Common.Pagination;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Parameters;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Results;
using DocumentProcessor.Avalonia.TerrenceLGee.DTOs;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;

public interface IContactService
{
    Task<Result<bool>> AddContactAsync(CreateContactDto contact);
    Task<Result<bool>> UpdateContactAsync(UpdateContactDto contact);
    Task<Result<bool>> DeleteContactAsync(int contactId);
    Task<Result<RetrievedContactDto?>> GetContactAsync(int contactId);
    Task<Result<PagedList<RetrievedContactDto>>> GetContactsAsync(PaginationParams paginationParams);
}
