using PropertyChanged;
using Senjyouhara.Main.Config;
using Senjyouhara.Main.Core.Manager.Dialog;
using Stylet;
using System.Threading.Tasks;
using Senjyouhara.Main.ViewModels.Components;
using StyletIoC;

namespace Senjyouhara.Main.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : Conductor<IMyScreen>.Collection.OneActive
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogManager _dialogManager;
        private readonly IContainer _container;
        
        public MainViewModel(IContainer container, IEventAggregator eventAggregator, IDialogManager dialogManager)
        {
            _eventAggregator = eventAggregator;
            _dialogManager = dialogManager;
            _container = container;
            var generateViewModel = container.Get<GenerateViewModel>();
            var taskViewModel = container.Get<TaskViewModel>();
            Items.Add(generateViewModel);
            Items.Add(taskViewModel);
            ActiveItem = generateViewModel;
        }
        

        protected override void OnInitialActivate()
        {
            AddUpdateModal();
        }

        public bool CloseCommand()
        {
            // var result = model.QueueList.FirstOrDefault(x =>
            //     x.Status == QueueTaskStatusEnum.Start || x.Status == QueueTaskStatusEnum.Restart);
            // if (result != null)
            // {
            //     var box = MessageBox.Show("当前有正在执行的任务是否关闭?", "", MessageBoxButton.YesNoCancel,
            //         MessageBoxImage.Warning);
            //     return box == MessageBoxResult.Yes;
            // }

            return true;
        }

        private void AddUpdateModal()
        {
            Task.Run(async () =>
            {
                if (UpdateConfig.IsEnableUpdate)
                {
                    var updateInfo = await UpdateConfig.GetUpdateData();
                    if (updateInfo.Version != AppConfig.Version)
                    {
                        await Execute.OnUIThreadAsync(() =>
                        {
                            var update = _container.Get<UpdateViewModel>();
                            if (update != null)
                            {
                                _dialogManager.ShowDialog(update);
                            }
                        });
                    }
                }
            });
        }
        
    }
}