using DocumentProcessor.Avalonia.TerrenceLGee.Common.Extensions;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Pagination;
using DocumentProcessor.Avalonia.TerrenceLGee.Common.Parameters;
using DocumentProcessor.Avalonia.TerrenceLGee.Data;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.RepositoryInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Mappings;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly ContactDbContext _context;
    private readonly ILogger<ContactRepository> _logger;

    public ContactRepository(ContactDbContext context, ILogger<ContactRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Contact?> AddContactAsync(Contact contact)
    {
        var errorMessage = string.Empty;
        try
        {
            await _context.Contacts.AddAsync(contact);
            await _context.SaveChangesAsync();
            return contact;
        }
        catch (Exception ex)
        {
            errorMessage = $"{GetMessageForLogging(nameof(AddContactAsync))}" +
                $"There was an unexpected error adding a new contact: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return null;
        }
    }

    public async Task<bool> UpdateContactAsync(Contact contact)
    {
        var errorMessage = string.Empty;
        try
        {
            var contactToUpdate = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == contact.Id);

            if (contactToUpdate is null) return false;

            contact.FromContactToContact(contactToUpdate);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"{GetMessageForLogging(nameof(UpdateContactAsync))}" +
                $"There was an unexpected error updating contact {contact.Id}: {ex.Message}";
            _logger.LogError(ex,"{msg}", errorMessage);
            return false;
        }
    }

    public async Task<bool> DeleteContactAsync(int contactId)
    {
        var errorMessage = string.Empty;
        try
        {
            var contactToDelete = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id == contactId);

            if (contactToDelete is null) return false;

            _context.Contacts.Remove(contactToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"{GetMessageForLogging(nameof(DeleteContactAsync))}" +
                $"There was an unexpected error deleting contact {contactId}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return false;
        }
    }

    public async Task<Contact?> GetContactAsync(int contactId)
    {
        var errorMessage = string.Empty;
        try
        {
            var contact = await _context.Contacts
                .AsNoTracking()
                .FirstAsync(c => c.Id == contactId);

            return contact;
        }
        catch (Exception ex)
        {
            errorMessage = $"{GetMessageForLogging(nameof(GetContactAsync))}" +
                $"There was an unexpected error retrieving contact {contactId}: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return null;
        }
    }

    public async Task<PagedList<Contact>> GetContactsAsync(PaginationParams paginationParams)
    {
        var errorMessage = string.Empty;
        try
        {
            var contacts = _context.Contacts
                .AsNoTracking();

            return await contacts.ToPagedListAsync(paginationParams.Page, paginationParams.PageSize);
        }
        catch (Exception ex)
        {
            errorMessage = $"{GetMessageForLogging(nameof(GetContactsAsync))}" +
                $"There was an unexpected error retrieving the contacts: {ex.Message}";
            _logger.LogError(ex, "{msg}", errorMessage);
            return [];
        }
    }

    private string GetMessageForLogging(string methodName)
    {
        return $"\nClass: {nameof(ContactRepository)}\n" +
            $"Method: {methodName}\n";
    }
}
