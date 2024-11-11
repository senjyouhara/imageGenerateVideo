using System;
using System.Collections;
using System.Collections.Generic;
using PropertyChanged;
using Senjyouhara.Common.Base;
using Senjyouhara.Common.Log;
using Senjyouhara.Main.Config;
using Stylet;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using Senjyouhara.Common.Utils;
using Senjyouhara.Main.Core.Manager.Dialog;
using Senjyouhara.Main.Model;
using Senjyouhara.Main.Views;
using StyletIoC;

namespace Senjyouhara.Main.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ShellViewModel : Conductor<Conductor<IMyScreen>.Collection.OneActive>, IHandle<TaskbarModel>
    {
        private IEventAggregator eventAggregator;
        private MainViewModel MainViewModel;

        public ShellViewModel(StyletIoC.IContainer _container, IEventAggregator _eventAggregator)
        {
            eventAggregator = _eventAggregator;
            container = _container;
        }

        public string Title { get; set; }

        public StyletIoC.IContainer container { get; set; }

        public void Handle(TaskbarModel message)
        {
            var taskBarInfo = ((ShellView)View)?.TaskBarInfo;
            Execute.OnUIThread(() =>
            {
                if (taskBarInfo != null)
                {
                    taskBarInfo.ProgressState = message.State;
                    taskBarInfo.ProgressValue = message.Value;
                    if (message.Value >= 1.0)
                    {
                        taskBarInfo.ProgressValue = 0;
                        FlashWindow.Flash((ShellView)View, 3);
                    }
                }
            });
        }

        public void CloseCommand()
        {
            MainViewModel = container.Get<MainViewModel>();
            var closeCommand = MainViewModel.CloseCommand();
            if (closeCommand)
            {
                CloseItem(MainViewModel);
                System.Windows.Application.Current.Shutdown();
            }
        }

        protected override void OnInitialActivate()
        {
            eventAggregator.Subscribe(this, "ProcessBar");
            Title = AppConfig.Title + " - v" + AppConfig.Version;
            MainViewModel = container.Get<MainViewModel>();
            var model = MainViewModel;
            Task.Run(() => { ActivateItem(model); });
        }
    }
}