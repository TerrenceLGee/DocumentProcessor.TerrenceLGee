using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.Data;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.RepositoryInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Repositories;
using DocumentProcessor.Avalonia.TerrenceLGee.Services;
using DocumentProcessor.Avalonia.TerrenceLGee.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection collection)
    {
        public void AddCommonServices()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            collection.AddDbContextFactory<ContactDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

            collection.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            collection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            collection.AddSingleton<MainWindowViewModel>();
            collection.AddSingleton<ContactsViewModel>();
            collection.AddTransient<IContactRepository, ContactRepository>();
            collection.AddTransient<IContactService, ContactService>();
        }
    }
}
