using System;
using System.Collections.ObjectModel;
using PropertyChanged;
using Senjyouhara.Main.Core.Manager.Dialog;
using Senjyouhara.Main.Model;
using Stylet;
using StyletIoC;

namespace Senjyouhara.Main.ViewModels.Components;


[AddINotifyPropertyChangedInterface]
public class TaskViewModel : MyScreen, IHandle<QueueModel>, IDisposable
{
    
    public new string DisplayName { get; set; } = "队列";
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogManager _dialogManager;
    private readonly IContainer _container;

    public ObservableCollection<QueueModel> QueueModels { get; set; } = new();
    
    public TaskViewModel(IContainer container, IEventAggregator eventAggregator, IDialogManager dialogManager)
    {
        _eventAggregator = eventAggregator;
        _dialogManager = dialogManager;
        _container = container;
    }

    protected override void OnInitialActivate()
    {
        base.OnInitialActivate();
        _eventAggregator.Subscribe(this, "queue");
    }

    public void Handle(QueueModel message)
    {
        QueueModels.Add(message);
    }

    public void Dispose()
    {
        _eventAggregator.Unsubscribe(this, "queue");
    }
}