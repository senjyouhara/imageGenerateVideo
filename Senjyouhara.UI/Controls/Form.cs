using System.Windows;
using System.Windows.Controls;
using Senjyouhara.UI.Enums;

namespace Senjyouhara.UI.Controls;

public class Form: UserControl
{
    public static readonly DependencyProperty LabelAlignProperty = DependencyProperty.Register(
        nameof(LabelAlign),
        typeof(HorizontalAlignment),
        typeof(FormItem),
        new PropertyMetadata(HorizontalAlignment.Left)
    );
    
    public static readonly DependencyProperty LabelWarpProperty = DependencyProperty.Register(
        nameof(LabelWarp),
        typeof(LabelWarpEnum),
        typeof(FormItem),
        new PropertyMetadata(null)
    );
    
    public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(
        nameof(Layout),
        typeof(LayoutEnum),
        typeof(FormItem),
        new PropertyMetadata(null)
    );
    
    public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register(
        nameof(LabelWidth),
        typeof(double),
        typeof(FormItem),
        new PropertyMetadata(null)
    );
    
    public LayoutEnum Layout
    {
        get => (LayoutEnum) GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }
    public double LabelWidth
    {
        get => (double)GetValue(LabelWidthProperty);
        set => SetValue(LabelWidthProperty, value);
    }
    
    public HorizontalAlignment LabelAlign
    {
        get => (HorizontalAlignment)GetValue(LabelAlignProperty);
        set => SetValue(LabelAlignProperty, value);
    }
    public LabelWarpEnum LabelWarp
    {
        get => (LabelWarpEnum)GetValue(LabelWarpProperty);
        set => SetValue(LabelWarpProperty, value);
    }
    
}