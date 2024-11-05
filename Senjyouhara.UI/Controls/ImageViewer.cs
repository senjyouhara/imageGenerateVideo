using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Senjyouhara.UI.Controls;

public class ImageViewer : HandyControl.Controls.ImageViewer
{
    public static readonly DependencyProperty ImageSourceListProperty = DependencyProperty.Register(
        nameof(ImageSourceList), typeof(ObservableCollection<BitmapFrame>), typeof(ImageViewer),
        new PropertyMetadata(default(ObservableCollection<BitmapFrame>)));

    public static readonly DependencyProperty IsShowPrevNextProperty = DependencyProperty.Register(
        nameof(IsShowPrevNext), typeof(bool), typeof(ImageViewer), new PropertyMetadata(true));

    public ObservableCollection<BitmapFrame> ImageSourceList
    {
        get => (ObservableCollection<BitmapFrame>)GetValue(ImageSourceListProperty);
        set => SetValue(ImageSourceListProperty, value);
    }

    public bool IsShowPrevNext
    {
        get => (bool)GetValue(IsShowPrevNextProperty);
        set => SetValue(IsShowPrevNextProperty, value);
    }
}