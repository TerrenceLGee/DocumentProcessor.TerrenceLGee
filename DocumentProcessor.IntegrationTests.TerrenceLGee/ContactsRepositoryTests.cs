using DocumentProcessor.Avalonia.TerrenceLGee.Common.Parameters;
using DocumentProcessor.Avalonia.TerrenceLGee.Data;
using DocumentProcessor.Avalonia.TerrenceLGee.Models;
using DocumentProcessor.Avalonia.TerrenceLGee.Repositories;
using DocumentProcessor.IntegrationTests.TerrenceLGee.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DocumentProcessor.IntegrationTests.TerrenceLGee;

public class ContactsRepositoryTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;

    public ContactsRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    public async Task InitializeAsync()
    {
        using (var context = _fixture.CreateContext())
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            await Setup.DatabaseSeeder.SeedDatabaseAsync(context);
        }
    }

    [Fact]
    public async Task AddContactAsync_ContactSavedToDatabaseOnSuccess()
    {
        using var context = _fixture.CreateContext();

        var repo = CreateContactRepository(context);

        var contact = new Contact
        {
            FirstName = "Walter",
            MiddleInitial = "S.",
            LastName = "Skinner",
            EmailAddress = "PrincipalSkinner@example.com",
            TelephoneNumber = "123-456-7201"
        };

        var newlyCreatedContact = await repo.AddContactAsync(contact);

        using var verificationContext = _fixture.CreateContext();
        var retrievedContact = await verificationContext.Contacts
            .FirstOrDefaultAsync(c => c.EmailAddress.Equals(contact.EmailAddress));

        Assert.NotNull(newlyCreatedContact);
        Assert.NotNull(retrievedContact);
        Assert.Equal(newlyCreatedContact.FirstName, retrievedContact.FirstName);
    }

    [Fact]
    public async Task AddContactAsync_ReturnsNull_WhenContactEmailNotUnique()
    {
        using var context = _fixture.CreateContext();

        var repo = CreateContactRepository(context);

        var contact = new Contact
        {
            FirstName = "Julius",
            MiddleInitial = "M.",
            LastName = "Hibbert",
            EmailAddress = "HSimpson@example.com",
            TelephoneNumber = "123-456-7202"
        };

        var newlyCreateContact = await repo.AddContactAsync(contact);

        Assert.Null(newlyCreateContact);
    }

    [Fact]
    public async Task UpdateContactAsync_Successful_WhenContactFound()
    {
        using var context = _fixture.CreateContext();

        var repo = CreateContactRepository(context);

        var contactToUpdate = new Contact
        {
            Id = 2,
            FirstName = "Marge",
            MiddleInitial = "B.",
            LastName = "Simpson",
            EmailAddress = "MSimpson@example.com",
            TelephoneNumber = "123-456-7891"
        };

        var result = await repo.UpdateContactAsync(contactToUpdate);

        using var verificationContext = _fixture.CreateContext();
        var retrievedContact = await verificationContext.Contacts
            .FirstOrDefaultAsync(c => c.EmailAddress.Equals(contactToUpdate.EmailAddress));

        Assert.True(result);
        Assert.NotNull(retrievedContact);
        Assert.Equal(contactToUpdate.FirstName, retrievedContact.FirstName);
        Assert.Equal(contactToUpdate.Id, retrievedContact.Id);
    }

    [Fact]
    public async Task UpdateContactAsync_ReturnsFalse_WhenContactNotFound()
    {
        using var context = _fixture.CreateContext();

        var repo = CreateContactRepository(context);

        var contactToUpdate = new Contact
        {
            Id = 9999,
            FirstName = "Julius",
            MiddleInitial = "M.",
            LastName = "Hibbert",
            EmailAddress = "DrHibbert@example.com",
            TelephoneNumber = "123-456-7202"
        };

        var result = await repo.UpdateContactAsync(contactToUpdate);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateContactAsync_ReturnsFalse_WhenUpdateEmailAddressIsNotUnique()
    {
        using var context = _fixture.CreateContext();

        var repo = CreateContactRepository(context);

        var contactToUpdate = new Contact
        {
            Id = 2,
            FirstName = "Marge",
            MiddleInitial = "B.",
            LastName = "Simpson",
            EmailAddress = "HSimpson@example.com",
            TelephoneNumber = "123-456-7891"
        };

        var result = await repo.UpdateContactAsync(contactToUpdate);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteContactAsync_ReturnsTrue_WhenContactFound()
    {
        using var context = _fixture.CreateContext();

        var repo = CreateContactRepository(context);

        var contactId = 6;

        var result = await repo.DeleteContactAsync(contactId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteContactAsync_ReturnsFalse_WhenContactNotFound()
    {
        using var context = _fixture.CreateContext();

        var repo = CreateContactRepository(context);

        var contactId = 9999;

        var result = await repo.DeleteContactAsync(contactId);

        Assert.False(result);
    }

    [Fact]
    public async Task GetContactAsync_ReturnsContact_WhenContactFound()
    {
        using var context = _fixture.CreateContext();

        var repo = CreateContactRepository(context);

        var contactId = 1;

        var retrievedContact = await repo.GetContactAsync(contactId);

        Assert.NotNull(retrievedContact);
        Assert.Equal(contactId, retrievedContact.Id);
        Assert.Equal("Homer", retrievedContact.FirstName);
    }

    [Fact]
    public async Task GetContactAsync_ReturnsNull_WhenContactNotFound()
    {
        using var context = _fixture.CreateContext();

        var repo = CreateContactRepository(context);

        var contactId = 9999;

        var retrievedContact = await repo.GetContactAsync(contactId);

        Assert.Null(retrievedContact);
    }

    [Fact]
    public async Task GetContactAsync_ReturnsPagedListOfContact_WhenSuccessful()
    {
        using var context = _fixture.CreateContext();

        var repo = CreateContactRepository(context);

        var paginationParams = new PaginationParams
        {
            Page = 2,
            PageSize = 5
        };

        var pagedContacts = await repo.GetContactsAsync(paginationParams);

        Assert.NotNull(pagedContacts);
        Assert.Equal(15, pagedContacts.TotalEntities);
        Assert.Equal(3, pagedContacts.TotalPages);
        Assert.Equal(5, pagedContacts.Count);
    }


    private ContactRepository CreateContactRepository(ContactDbContext context)
    {
        var logger = NullLogger<ContactRepository>.Instance;
        return new ContactRepository(context, logger);
    }
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
