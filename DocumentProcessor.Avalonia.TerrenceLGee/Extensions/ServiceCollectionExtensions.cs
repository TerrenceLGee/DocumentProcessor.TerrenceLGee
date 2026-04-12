using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.Data;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.RepositoryInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;
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

            collection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            collection.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            collection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            collection.AddSingleton<MainWindowViewModel>();
            collection.AddTransient<ContactsViewModel>();
            collection.AddTransient<MainUserViewModel>();
            collection.AddTransient<EmailViewModel>();
            collection.AddTransient<IContactRepository, ContactRepository>();
            collection.AddTransient<IContactService, ContactService>();
            collection.AddTransient<ISmtpClientFactory, SmtpClientFactory>();
            collection.AddTransient<IEmailService, EmailService>();
            collection.AddTransient<IXLService, XLService>();
            collection.AddTransient<ITextService, TextService>();

            collection.AddOptions<EmailConfiguration>()
                .Bind(configuration.GetSection("EmailConfiguration"))
                .Validate(config =>
                !string.IsNullOrEmpty(config.SenderName) &&
                !string.IsNullOrEmpty(config.SenderEmail) &&
                !string.IsNullOrEmpty(config.Password) &&
                !string.IsNullOrEmpty(config.Host) &&
                config.Port > 0 && config.Port <= 65535, "Invalid Email Configuration");
        }
    }
}
