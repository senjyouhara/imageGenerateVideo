using System.Windows;
using System.Windows.Controls;
using Senjyouhara.UI.Enums;
using Senjyouhara.UI.Utils;

namespace Senjyouhara.UI.Controls;



[StyleTypedProperty(Property = "LabelStyle", StyleTargetType = typeof(TextBlock))]
public class FormItem:UserControl
{
    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label),
        typeof(object),
        typeof(FormItem),
        new PropertyMetadata(null, LabelPropertyChangedCallback)
    );
    
    public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(
        nameof(Layout),
        typeof(LayoutEnum),
        typeof(FormItem),
        new PropertyMetadata(LayoutEnum.Horizontal)
    );
    
    public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register(
        nameof(LabelStyle),
        typeof(Style),
        typeof(FormItem),
        new PropertyMetadata(null)
    );
    
    
    public static readonly DependencyProperty ColonProperty = DependencyProperty.Register(
        nameof(Colon),
        typeof(bool),
        typeof(FormItem),
        new PropertyMetadata(true)
    );
    
    public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register(
        nameof(LabelWidth),
        typeof(double),
        typeof(FormItem),
        new PropertyMetadata(double.NaN, LabelWidthPropertyChangedCallback)
    );
    

    public static readonly DependencyProperty RequiredMarkProperty = DependencyProperty.Register(
        nameof(RequiredMark),
        typeof(bool),
        typeof(FormItem),
        new PropertyMetadata(true)
    );
    
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
        new PropertyMetadata(LabelWarpEnum.Default)
    );
    
    public object Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
    public Style LabelStyle
    {
        get => (Style)GetValue(LabelStyleProperty);
        set => SetValue(LabelStyleProperty, value);
    }
    
    public LayoutEnum Layout
    {
        get => (LayoutEnum) GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }
    
    public bool Colon
    {
        get => (bool)GetValue(ColonProperty);
        set => SetValue(ColonProperty, value);
    }
    
    public double LabelWidth
    {
        get => (double)GetValue(LabelWidthProperty);
        set => SetValue(LabelWidthProperty, value);
    }
    
    public bool RequiredMark
    {
        get => (bool)GetValue(RequiredMarkProperty);
        set => SetValue(RequiredMarkProperty, value);
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

    public FormItem()
    {
        Loaded += (sender, args) =>
        {
    
        };
    }

    private static void LabelPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var uiElement = d as FrameworkElement;
        var formitem = uiElement as FormItem;
        if (!formitem.IsLoaded)
        {
            RoutedEventHandler re = null;
            re = (sender, args) =>
            {
                if (formitem.Label is string str)
                {
                    var textBlock = new TextBlock();
        
                    if (formitem.Colon)
                    {
                        str += ":";
                    }
                    if (formitem.LabelStyle != null)
                    {
                        textBlock.Style = formitem.LabelStyle;
                    }
                    textBlock.HorizontalAlignment = formitem.LabelAlign;
                    if (formitem.LabelWarp == LabelWarpEnum.Omit)
                    {
                        textBlock.TextTrimming = TextTrimming.WordEllipsis;
                        textBlock.TextWrapping = TextWrapping.NoWrap;
                        textBlock.ToolTip = str;
                    }
                    else if(formitem.LabelWarp == LabelWarpEnum.Warp)
                    {
                        textBlock.TextWrapping = TextWrapping.Wrap;
                    }
                    textBlock.Text = str;
                    d.SetValue(LabelProperty, textBlock );
                }
                uiElement.Loaded -= re;
            };
            uiElement.Loaded += re;
        }
        else
        {
            if (formitem.Label is string str)
            {
                var textBlock = new TextBlock();
        
                if (formitem.Colon)
                {
                    str += ":";
                }
                if (formitem.LabelStyle != null)
                {
                    textBlock.Style = formitem.LabelStyle;
                }
                textBlock.HorizontalAlignment = formitem.LabelAlign;
                if (formitem.LabelWarp == LabelWarpEnum.Omit)
                {
                    textBlock.TextTrimming = TextTrimming.WordEllipsis;
                    textBlock.TextWrapping = TextWrapping.NoWrap;
                    textBlock.ToolTip = str;
                }
                else if(formitem.LabelWarp == LabelWarpEnum.Warp)
                {
                    textBlock.TextWrapping = TextWrapping.Wrap;
                }
                textBlock.Text = str;
                d.SetValue(LabelProperty, textBlock );
            }
        }
    }
    
    private static void LabelWidthPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

        var uiElement = d as FrameworkElement;
        var formitem = uiElement as FormItem;
        var form = ResourceUtil.FindParent<Form>(uiElement);

        if (!uiElement.IsLoaded)
        {
            RoutedEventHandler re = null;
            re = (sender, args) =>
            {
                if (form != null)
                {
                    if (form.LabelWidth != null && formitem?.LabelWidth == null)
                    {
                        d.SetValue(LabelWidthProperty, form.LabelWidth);
                    }
                }
                uiElement.Loaded -= re;
            };

            uiElement.Loaded += re;
        }
        else
        {
            if (form != null)
            {
                if (form.LabelWidth != null && formitem?.LabelWidth == null)
                {
                    d.SetValue(LabelWidthProperty, form.LabelWidth);
                }
            }
        }

    }
    
}