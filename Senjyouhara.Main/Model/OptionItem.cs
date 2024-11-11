using PropertyChanged;

namespace Senjyouhara.Main.Model;

[AddINotifyPropertyChangedInterface]
public class OptionItem
{
    public string Label { get; set; }

    public object Value { get; set; }
}