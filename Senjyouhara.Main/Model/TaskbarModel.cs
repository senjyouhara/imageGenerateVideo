using System.Windows.Shell;
using PropertyChanged;

namespace Senjyouhara.Main.Model;

[AddINotifyPropertyChangedInterface]
public class TaskbarModel
{
    public TaskbarItemProgressState State { get; set; }

    public double Value { get; set; }
}