using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.Data;
using DocumentProcessor.Avalonia.TerrenceLGee.Extensions;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;
using DocumentProcessor.Avalonia.TerrenceLGee.ViewModels;
using DocumentProcessor.Avalonia.TerrenceLGee.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DocumentProcessor.Avalonia.TerrenceLGee;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public async override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        var collection = new ServiceCollection();
        collection.AddCommonServices();

        var services = collection.BuildServiceProvider();

        using (var scope = services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ContactDbContext>();
            var xlService = scope.ServiceProvider.GetRequiredService<IXLService>();
            await context.Database.EnsureCreatedAsync();
            await DatabaseSeeder.SeedDatabaseAsync(context, xlService);
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
}