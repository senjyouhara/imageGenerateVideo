using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Threading;
using Stylet;
using StyletIoC;

namespace Senjyouhara.Main.ViewModels
{
    public class StartLoadingViewModel : Conductor<IScreen>
    {
        private IWindowManager _WindowManager;
        private IContainer container;
        public StartLoadingViewModel(IWindowManager manager, IContainer container)
        {
            _WindowManager = manager;
            this.container = container;
        }
        
        public void CloseHandle()
        {
            var model = container.Get<ShellViewModel>();
            _WindowManager.ShowWindow(model);
        }

      
    }
}
