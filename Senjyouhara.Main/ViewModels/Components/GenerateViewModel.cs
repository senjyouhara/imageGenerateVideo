using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;
using PropertyChanged;
using Senjyouhara.Main.Core.Manager.Dialog;
using Senjyouhara.UI.Controls;
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

    public List<Uri> ImgList { get; set; }
    
    public GenerateViewModel(IContainer container, IEventAggregator eventAggregator, IDialogManager dialogManager)
    {
        _eventAggregator = eventAggregator;
        _dialogManager = dialogManager;
        _container = container;
        ImgList = new List<Uri>()
        {
            new("D:/3c6d55fbb2fb4316cb2fde3620a4462309f7d34e.jpg"),
            new("D:/document/图片/2252022177c3df144.jpg")
            // new ("pack://application:,,,Senjyouhara.Main;component/Resources/78087664f88cf7571.jpg"),
            // new ("pack://application:,,,/Resources/78087664f88cf7571.jpg"),
            // new ("pack://application:,,,/Resources/loading.png"),
        };
    }

    public void SelectImageCommand()
    {
        
    }
    public void SelectMusicCommand()
    {
        
    }

    public void ImgCommand()
    {
        var img = new ImageBrowser(ImgList[0], ImgList);
        img.IsFullScreen = true;
        img.Show();
    }
    
}