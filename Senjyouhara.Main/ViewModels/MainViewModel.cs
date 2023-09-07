using Caliburn.Micro;
using HandyControl.Controls;
using HandyControl.Tools;
using HandyControl.Tools.Extension;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using PropertyChanged;
using Senjyouhara.Common.Exceptions;
using Senjyouhara.Common.Log;
using Senjyouhara.Common.Utils;
using Senjyouhara.Main.Comparer;
using Senjyouhara.Main.Config;
using Senjyouhara.Main.Core.Manager.Dialog;
using Senjyouhara.Main.models;
using Senjyouhara.Main.Views;
using Senjyouhara.UI.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace Senjyouhara.Main.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : Screen
    {
        public ObservableCollection<FileNameItem> FileNameItems { get; set; } = new ObservableCollection<FileNameItem>();


        private IEventAggregator _eventAggregator;
        private IWindowManager _windowManager;
        private readonly IDialogManager dialogManager;

        public MainViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, IDialogManager dialogManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            this.dialogManager = dialogManager;
            _eventAggregator.SubscribeOnUIThread(this);
        }

        public void AddUpdateModal()
        {
            Task.Run(async () =>
            {
                if (UpdateConfig.IsEnableUpdate)
                {
                    var _updateInfo = await UpdateConfig.GetUpdateData();
                    if (_updateInfo.Version != AppConfig.Version)
                    {
                        await Application.Current.Dispatcher.BeginInvoke(async () =>
                        {
                            var update = IoC.Get<UpdateViewModel>();
                            await _windowManager.ShowDialogAsync(update);
                        });
                    }
                }
            });
        }

        public void AddLog()
        {
            var t = DateTime.Now;
            //Debug.WriteLine(t.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            Log.Debug($"添加日志~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
        }

        public void SelectFileHandle()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\desktop";    //初始的文件夹
            openFileDialog.Filter = "所有文件(*.*)|*.*";//在对话框中显示的文件类型
            openFileDialog.Title = "请选择文件";
            openFileDialog.Multiselect = true;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                var FileList = new List<string>(openFileDialog.FileNames);
            }
        }

        public void ClearListHandle()
        {
            FileNameItems.Clear();
        }
    }

    internal static class Utils
    {
        //根据子元素查找父元素
        public static T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                    return obj as T;

                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }
    }

}
