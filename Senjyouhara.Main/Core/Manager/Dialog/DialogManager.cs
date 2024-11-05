using HandyControl.Controls;
using Senjyouhara.Main.ViewModels;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using Window = System.Windows.Window;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    public class DialogManager : WindowManager, IDialogManager
    {
        private IViewManager viewManager;
        private IWindowManager windowManager;

        public DialogManager(IWindowManager windowManager, IViewManager viewManager,
            Func<IMessageBoxViewModel> messageBoxViewModelFactory, IWindowManagerConfig config) : base(viewManager,
            messageBoxViewModelFactory, config)
        {
            this.windowManager = windowManager;
            this.viewManager = viewManager;
        }


        public IDialogResult ShowModal(object rootModel)
        {

            return ShowModal(rootModel, null);
        }

        public IDialogResult ShowModal(object rootModel, IDialogParameters parameters)
        {
            if (parameters == null) parameters = new DialogParameters();
            var view = viewManager.CreateViewForModel(rootModel);
            var Dialog = view as UserControl;
            if (!(rootModel is IDialogAware dialogView))
            {
                throw new Exception("页面须实现IDialogAware 接口");
            }
            var window = CreateWindow(rootModel, true, null);
            IDialogResult dialogResult = null;
            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = delegate(IDialogResult o)
            {
                if (!dialogView.CanCloseDialog())
                {
                    return;
                }

                dialogResult = o;
                window.Close();
            };
            RoutedEventHandler loadedHandler = null;
            loadedHandler = delegate
            {
                window.Loaded -= loadedHandler;
                dialogView.RequestClose += requestCloseHandler;
            };
            window.Loaded += loadedHandler;


            RoutedEventHandler closedHandler = null;
            closedHandler = delegate
            {
                window.Unloaded -= closedHandler;
                dialogView.OnDialogClosed();
                dialogView.RequestClose -= requestCloseHandler;
            };
            window.Unloaded += closedHandler;
            
            var dialogModel = (IDialogAware)rootModel;
            RoutedEventHandler OpenDialogHandle = delegate { dialogModel.OnDialogOpened(parameters); };
            Dialog.Loaded += OpenDialogHandle;
            ((Grid)(Application.Current.MainWindow)?.FindName("dialog"))?.Children.Add(Dialog);
            Dialog.Loaded -= OpenDialogHandle;
            return dialogResult;
        }

        public void ShowModalAsync(object rootModel)
        {
            ShowModalAsync(rootModel, null, null);
        }

        public void ShowModalAsync(object rootModel, IDialogParameters parameters)
        {
            ShowModalAsync(rootModel, parameters, null);
        }

        public void ShowModalAsync(object rootModel, Action<IDialogResult> callback)
        {
            ShowModalAsync(rootModel, null, callback);
        }

        public void ShowModalAsync(object rootModel, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            if (parameters == null) parameters = new DialogParameters();
            var view = viewManager.CreateViewForModel(rootModel);
            var Dialog = view as UserControl;
            var result = GetResult(rootModel, view as FrameworkElement, callback);
            var dialogModel = (IDialogAware)rootModel;
            RoutedEventHandler OpenDialogHandle = delegate { dialogModel.OnDialogOpened(parameters); };
            Dialog.Loaded += OpenDialogHandle;
            ((Grid)(Application.Current.MainWindow)?.FindName("dialog"))?.Children.Add(Dialog);
            Dialog.Loaded -= OpenDialogHandle;
        }

        public IDialogResult ShowWindowDialog(object rootModel)
        {
            return ShowWindowDialog(rootModel, null);
        }

        public IDialogResult ShowWindowDialog(object rootModel, IDialogParameters parameters)
        {
            if (parameters == null) parameters = new DialogParameters();
            if (!(rootModel is IDialogAware dialogView))
            {
                throw new Exception("页面须实现IDialogAware 接口");
            }
            var window = CreateWindow(rootModel, true, null);
            IDialogResult dialogResult = null;
            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = delegate(IDialogResult o)
            {
                if (!dialogView.CanCloseDialog())
                {
                    return;
                }

                dialogResult = o;
                window.Close();
            };
            RoutedEventHandler loadedHandler = null;
            loadedHandler = delegate
            {
                window.Loaded -= loadedHandler;
                dialogView.RequestClose += requestCloseHandler;
            };
            window.Loaded += loadedHandler;


            RoutedEventHandler closedHandler = null;
            closedHandler = delegate
            {
                window.Unloaded -= closedHandler;
                dialogView.OnDialogClosed();
                dialogView.RequestClose -= requestCloseHandler;
            };
            window.Unloaded += closedHandler;
            
            var dialogModel = (IDialogAware)rootModel;
            RoutedEventHandler OpenDialogHandle = delegate { dialogModel.OnDialogOpened(parameters); };
            window.Loaded += OpenDialogHandle;
            window.ShowDialog();
            window.Loaded -= OpenDialogHandle;
            return dialogResult;
        }

        public void ShowWindowDialogAsync(object rootModel)
        {
            ShowWindowDialogAsync(rootModel, null, null);
        }

        public void ShowWindowDialogAsync(object rootModel, IDialogParameters parameters)
        {
            ShowWindowDialogAsync(rootModel, parameters, null);
        }

        public void ShowWindowDialogAsync(object rootModel, Action<IDialogResult> callback)
        {
            ShowWindowDialogAsync(rootModel, null, callback);
        }

        public void ShowWindowDialogAsync(object rootModel, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            if (parameters == null) parameters = new DialogParameters();
            var window = CreateWindow(rootModel, true, null);
            var result = GetResult(rootModel, window, callback);
            var dialogModel = (IDialogAware)rootModel;
            RoutedEventHandler OpenDialogHandle = delegate { dialogModel.OnDialogOpened(parameters); };
            window.Loaded += OpenDialogHandle;
            window.ShowDialog();
            window.Loaded -= OpenDialogHandle;
            return;

            // var view = viewManager.CreateViewForModel(rootModel);
            // viewManager.BindViewToModel(view, rootModel);
            // var Dialog = view as UserControl;
            // var WinDialog = view as Window;
            // // MyControl DialogWarp = new MyControl();
            // if (!(rootModel is IDialogAware dialogView))
            // {
            //     throw new Exception("页面必须实现IDialogAware 接口");
            // }
            //
            // IDialogResult dialogResult = null;
            // Action<IDialogResult> requestCloseHandler = null;
            // requestCloseHandler = delegate (IDialogResult o)
            // {
            //     if (!((IDialogAware)dialogView).CanCloseDialog())
            //     {
            //         return;
            //     }
            //
            //     dialogResult = o;
            //     if (view is Window w)
            //     {
            //         w.Close();
            //     }
            //     else
            //     {
            //         ((Grid)(Application.Current.MainWindow)?.FindName("dialog"))?.Children.Remove(view);
            //     }
            // };
            // RoutedEventHandler loadedHandler = null;
            // loadedHandler = delegate
            // {
            //     if (WinDialog != null)
            //     {
            //         WinDialog.Loaded -= loadedHandler;
            //     }
            //
            //     if (Dialog != null)
            //     {
            //         Dialog.Loaded -= loadedHandler;
            //     }
            //     dialogView.RequestClose += requestCloseHandler;
            // };
            // if (WinDialog != null)
            // {
            //     WinDialog.Loaded += loadedHandler;
            // }
            //
            // if (Dialog != null)
            // {
            //     Dialog.Loaded += loadedHandler;
            // }
            //
            //
            // RoutedEventHandler closedHandler = null;
            // closedHandler = delegate
            // {
            //     if (WinDialog != null)
            //     {
            //         WinDialog.Unloaded -= closedHandler;
            //     }
            //
            //     if (Dialog != null)
            //     {
            //         Dialog.Unloaded -= closedHandler;
            //     }
            //     // IDialogResult result = new DialogResult();
            //     // var dialogParams = dialogView.OnDialogClosing();
            //     // result.Parameters = dialogParams;
            //     dialogView.OnDialogClosed();
            //     dialogView.RequestClose -= requestCloseHandler;
            //     callback?.Invoke(dialogResult);
            // };
            // if (WinDialog != null)
            // {
            //     WinDialog.Unloaded += closedHandler;
            // }
            //
            // if (Dialog != null)
            // {
            //     Dialog.Unloaded += closedHandler;
            // }
            // var dialogContent = (FrameworkElement)view;
            // var dialogModel = (IDialogAware)rootModel;
            // RoutedEventHandler OpenDialogHandle = delegate
            // {
            //     var dialog = WinDialog ?? Application.Current.MainWindow; 
            //     dialog.Dispatcher.BeginInvoke(() =>
            //     {
            //         dialogModel.OnDialogOpened(parameters);
            //     });
            // }; 
            //
            // if (innerDialog)
            // {
            //     var dialogContain = ((Grid)(Application.Current.MainWindow)?.FindName("dialog"));
            //     if (dialogContain == null)
            //     {
            //         throw new Exception("MainWindow必须有Grid且Name为dialog用来承载dialog容器展示");
            //     }
            //     
            //     if (WinDialog != null)
            //     {
            //         WinDialog.Loaded += OpenDialogHandle;
            //         windowManager.ShowDialog(dialogModel);
            //         WinDialog.Loaded -= OpenDialogHandle;
            //     }
            //
            //     if (Dialog != null)
            //     {
            //         Dialog.Loaded += OpenDialogHandle;
            //         ((Grid)(Application.Current.MainWindow)?.FindName("dialog"))?.Children.Add(Dialog);
            //         Dialog.Loaded -= OpenDialogHandle;
            //     }
            // }
            // else
            // {
            //     if (dialogContent is Window)
            //     {
            //         if (WinDialog != null && Application.Current.MainWindow != null && Application.Current.MainWindow != WinDialog)
            //         {
            //             WinDialog.Owner = Application.Current.MainWindow;
            //             WinDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //             // WinDialog.Top = Application.Current.MainWindow.Top +  Convert.ToInt32((Application.Current.MainWindow.ActualHeight - WinDialog.Height) / 2);
            //             // WinDialog.Left = Application.Current.MainWindow.Left +  Convert.ToInt32((Application.Current.MainWindow.ActualWidth - WinDialog.Height) / 2);
            //         }
            //   
            //         WinDialog.Loaded += OpenDialogHandle;
            //         WinDialog.ShowDialog();
            //         // windowManager.ShowDialog(dialogModel);
            //         WinDialog.Loaded -= OpenDialogHandle;
            //         
            //     }
            //     else
            //     {
            //         throw new Exception("dialogContent 必须为 Window");
            //     }
            // }
        }

        private IDialogResult GetResult(object rootModel, FrameworkElement view,  Action<IDialogResult> callback)
        {
            if (!(rootModel is IDialogAware dialogView))
            {
                throw new Exception("页面须实现IDialogAware 接口");
            }
            var Dialog = view as UserControl;
            IDialogResult dialogResult = null;
            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = delegate(IDialogResult o)
            {
                if (!dialogView.CanCloseDialog())
                {
                    return;
                }

                dialogResult = o;
                ((Grid)(Application.Current.MainWindow)?.FindName("dialog"))?.Children.Remove(view);
            };
            RoutedEventHandler loadedHandler = null;
            loadedHandler = delegate
            {
                view.Loaded -= loadedHandler;
                dialogView.RequestClose += requestCloseHandler;
            };
            view.Loaded += loadedHandler;


            RoutedEventHandler closedHandler = null;
            closedHandler = delegate
            {
                view.Unloaded -= closedHandler;
                dialogView.OnDialogClosed();
                dialogView.RequestClose -= requestCloseHandler;
                callback?.Invoke(dialogResult);
            };
            view.Unloaded += closedHandler;
            var dialogContain = ((Grid)(Application.Current.MainWindow)?.FindName("dialog"));
            if (dialogContain == null)
            {
                throw new Exception("MainWindow必须有Grid且Name为dialog用来承载dialog容器展示");
            }
            
            return dialogResult;
        }
        private IDialogResult GetResult(object rootModel, Window window,  Action<IDialogResult> callback)
        {
            if (!(rootModel is IDialogAware dialogView))
            {
                throw new Exception("页面须实现IDialogAware 接口");
            }
            
            IDialogResult dialogResult = null;
            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = delegate(IDialogResult o)
            {
                if (!dialogView.CanCloseDialog())
                {
                    return;
                }

                dialogResult = o;
                window.Close();
            };
            RoutedEventHandler loadedHandler = null;
            loadedHandler = delegate
            {
                window.Loaded -= loadedHandler;
                dialogView.RequestClose += requestCloseHandler;
            };
            window.Loaded += loadedHandler;


            RoutedEventHandler closedHandler = null;
            closedHandler = delegate
            {
                window.Unloaded -= closedHandler;
                dialogView.OnDialogClosed();
                dialogView.RequestClose -= requestCloseHandler;
                callback?.Invoke(dialogResult);
            };
            window.Unloaded += closedHandler;

            return dialogResult;
        }
        
    }
}