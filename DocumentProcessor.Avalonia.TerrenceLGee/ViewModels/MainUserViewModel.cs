using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using DocumentProcessor.Avalonia.TerrenceLGee.Interfaces.ServiceInterfaces;
using DocumentProcessor.Avalonia.TerrenceLGee.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DocumentProcessor.Avalonia.TerrenceLGee.ViewModels;

public partial class MainUserViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private ObservableObject? _currentSubView;

    [ObservableProperty]
    private ObservableObject? _previousSubView;

    public MainUserViewModel(IServiceProvider serviceProvider, IMessenger messenger)
    {
        _serviceProvider = serviceProvider;
        _messenger = messenger;
        CurrentSubView = _serviceProvider.GetRequiredService<ContactsViewModel>();
        MessageRegistration();
    }

    private void MessageRegistration()
    {
        _messenger.Register<SendEmailMessage>(this, (r, m) =>
        {
            PreviousSubView = CurrentSubView;
            var emailService = _serviceProvider.GetRequiredService<IEmailService>();
            var emailVM = new EmailViewModel(emailService, _messenger, m.Contact);
            CurrentSubView = emailVM;
        });

        _messenger.Register<NavigateBackToPreviousPageMessage>(this, (r, m) =>
        {
            CurrentSubView = PreviousSubView;
        });
    }
}
