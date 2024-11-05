using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using PropertyChanged;
using Senjyouhara.Main.Core.Manager.Dialog;
using Stylet;
using StyletIoC;

namespace Senjyouhara.Main.ViewModels.Components;


[AddINotifyPropertyChangedInterface]
public class GenerateViewModel: MyScreen
{
    
    public new string DisplayName { get; set; } = "配置";
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogManager _dialogManager;
    private readonly IContainer _container;

    public string ImageInput { get; set; }
    public string MusicInput { get; set; }

    public BitmapImage Img { get; set; }
    
    public GenerateViewModel(IContainer container, IEventAggregator eventAggregator, IDialogManager dialogManager)
    {
        _eventAggregator = eventAggregator;
        _dialogManager = dialogManager;
        _container = container;
        // "pack://application:,,,/Senjyouhara.Main;component/Resources/78087664f88cf7571.jpg"
    }

    public void SelectImageCommand()
    {
        
    }
    public void SelectMusicCommand()
    {
        
    }
    
}