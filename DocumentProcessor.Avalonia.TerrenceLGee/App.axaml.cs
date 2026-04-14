using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.Data;
using DocumentProcessor.Avalonia.TerrenceLGee.Extensions;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.ViewModels;
using DocumentProcessor.Avalonia.TerrenceLGee.Views;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using Serilog;
using System.IO;

namespace DocumentProcessor.Avalonia.TerrenceLGee;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public async override void OnFrameworkInitializationCompleted()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        BindingPlugins.DataValidators.RemoveAt(0);

        var collection = new ServiceCollection();

        LoggingSetup();

        collection.AddCommonServices();

        var services = collection.BuildServiceProvider();

        using (var scope = services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ContactDbContext>();
            var readerFactory = scope.ServiceProvider.GetRequiredService<IFileReaderFactory>();
            var reader = readerFactory.GetReader("xlsx");
            await context.Database.EnsureCreatedAsync();
            await DatabaseSeeder.SeedDatabaseAsync(context, reader);
        }

        var messenger = services.GetRequiredService<IMessenger>();
        var mainWindowViewModel = services.GetRequiredService<MainWindowViewModel>();
        var mainUserViewModel = new MainUserViewModel(services, messenger);
        mainWindowViewModel.CurrentView = mainUserViewModel;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    void LoggingSetup()
    {
        var loggingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        Directory.CreateDirectory(loggingDirectory);
        var filePath = Path.Combine(loggingDirectory, "log-.txt");
        var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
            path: filePath,
            rollingInterval: RollingInterval.Day,
            outputTemplate: outputTemplate)
            .CreateLogger();
    }
}