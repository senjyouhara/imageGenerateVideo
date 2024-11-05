using PropertyChanged;
using Senjyouhara.Main.Core.Manager.Dialog;
using Stylet;
using StyletIoC;

namespace Senjyouhara.Main.ViewModels.Components;


[AddINotifyPropertyChangedInterface]
public class TaskViewModel: MyScreen
{
    
    public new string DisplayName { get; set; } = "队列";
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogManager _dialogManager;
    private readonly IContainer _container;
    
    public TaskViewModel(IContainer container, IEventAggregator eventAggregator, IDialogManager dialogManager)
    {
        _eventAggregator = eventAggregator;
        _dialogManager = dialogManager;
        _container = container;
    }
    
    
    
}