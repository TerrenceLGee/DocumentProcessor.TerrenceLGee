using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.Data;
using DocumentProcessor.Avalonia.TerrenceLGee.Extensions;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.ViewModels;
using DocumentProcessor.Avalonia.TerrenceLGee.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentProcessor.Avalonia.TerrenceLGee;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public async override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();

        var services = collection.BuildServiceProvider();

        using (var scope = services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ContactDbContext>();
            await context.Database.EnsureCreatedAsync();
            await DatabaseSeeder.SeedDatabaseAsync(context);
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var contactService = services.GetRequiredService<IContactService>();
            var messenger = services.GetRequiredService<IMessenger>();
            desktop.MainWindow = new ContactsView
            {
                DataContext = new ContactsViewModel(contactService, messenger),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}