using Senjyouhara.Main.ViewModels;
using System.Collections.Generic;
using System;
using System.Windows;
using Senjyouhara.Common.Utils;
using System.Windows.Threading;
using Senjyouhara.Main.Views;
using Senjyouhara.Main.Core.Manager.Dialog;
using System.Linq;
using System.Diagnostics;
using Stylet;
using StyletIoC;
using System.ComponentModel;
using System.Threading;
using Senjyouhara.Common.Log;
using Senjyouhara.Main.Config;
using IContainer = StyletIoC.IContainer;

namespace Senjyouhara.Main
{
    public class Bootstrapper : Stylet.Bootstrapper<ShellViewModel>
    {
        private static Mutex mutex = new Mutex(true, AppConfig.Name);

        private IContainer container;

        public Bootstrapper()
        {
        }

        protected override void OnStart()
        {
            // This is called just after the application is started, but before the IoC container is set up.
            // Set up things like logging, etc
            // if (!mutex.WaitOne(TimeSpan.Zero, true))
            // {
            //     MessageBox.Show("应用程序已运行。");
            //     Application.Current.Shutdown();
            // }
            Log.Init();
            Log.Info("程序启动成功");
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Bind your own types. Concrete types are automatically self-bound.
            builder.Bind<IDialogManager>().To<DialogManager>();
            builder.Bind<IMyScreen>().To<MyScreen>();
            var ioc = builder.BuildContainer();
            container = ioc;
        }

        protected override void Configure()
        {
            // This is called after Stylet has created the IoC container, so this.Container exists, but before the
            // Root ViewModel is launched.
            // Configure your services, etc, in here
        }

        protected override void OnLaunch()
        {
            // This is called just after the root ViewModel has been launched
            // Something like a version check that displays a dialog might be launched from here
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Called on Application.Exit
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            // Called on Application.DispatcherUnhandledException
        }
    }
}