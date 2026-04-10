using CommunityToolkit.Mvvm.ComponentModel;

namespace DocumentProcessor.Avalonia.TerrenceLGee.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _currentView;
}