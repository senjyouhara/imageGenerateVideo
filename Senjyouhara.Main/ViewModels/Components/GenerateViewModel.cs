using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PropertyChanged;
using Senjyouhara.Common.Utils;
using Senjyouhara.Main.Core.Manager.Dialog;
using Senjyouhara.Main.Model;
using Senjyouhara.Main.Views.Components;
using Senjyouhara.UI.Controls;
using Stylet;
using StyletIoC;
using TagLib.Riff;
using File = System.IO.File;

namespace Senjyouhara.Main.ViewModels.Components;


[AddINotifyPropertyChangedInterface]
public class GenerateViewModel: MyScreen
{
    
    public new string DisplayName { get; set; } = "配置";
    private readonly IEventAggregator _eventAggregator;
    private readonly IDialogManager _dialogManager;
    private readonly IContainer _container;

    private FileNameItem ActivateImg;
    public MusicViewModel MusicViewModel { get; set; }
    public string ImageInput { get; set; }
    public string MusicInput { get; set; }

    public List<FileNameItem> ImageFileList { get; set; } = new();
    public List<AudioFileItem> MusicFileList { get; set; } = new();

    public List<OptionItem> OutputResolutionList { get; private set; } = new()
    {
        new() { Label = "720P", Value = 720 },
        new() { Label = "1080P", Value = 1080 },
        new() { Label = "4096P", Value = 4096 }
    };

    public int OutputResolution { get; set; }



    public bool HasImageList => ImageFileList.Count > 0;
    public bool HasMusicList => MusicFileList.Count > 0;

    public List<Uri> ImageUriList => ImageFileList.Select(item => item.Uri).ToList();

    public GenerateViewModel(IContainer container, IEventAggregator eventAggregator, IDialogManager dialogManager)
    {
        _eventAggregator = eventAggregator;
        _dialogManager = dialogManager;
        _container = container;
        OutputResolution = (int)OutputResolutionList[1].Value;
    }

    protected override void OnInitialActivate()
    {
        base.OnInitialActivate();
        MusicViewModel = _container.Get<MusicViewModel>();
    }

    private List<FileNameItem> GetFileNameItems(List<string> files)
    {
        return files.Where(file =>
        {
            if (File.Exists(file)) return true;

            return false;
        }).Select(item =>
        {
            var v = new FileInfo(item);

            var suffixName = v.Name.LastIndexOf(".", StringComparison.Ordinal) >= 0
                ? v.Name.Substring(v.Name.LastIndexOf(".", StringComparison.Ordinal) + 1)
                : string.Empty;
            var fileName = v.Name.LastIndexOf(".", StringComparison.Ordinal) >= 0
                ? v.Name.Substring(0, v.Name.LastIndexOf(".", StringComparison.Ordinal))
                : string.Empty;

            return new FileNameItem()
            {
                Uid = Guid.NewGuid().ToString(),
                OriginFileName = v.Name,
                FileName = fileName,
                FilePath = v.FullName,
                Suffix = suffixName,
                Uri = new Uri(v.FullName)
            };
        }).ToList();
    }
    
    public void SelectImageCommand()
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = "c:\\desktop"; //初始的文件夹
        openFileDialog.Filter = "所有文件(*.jpg;*.jpeg;*.bmp;*.png)|*.jpg;*.jpeg;*.bmp;*.png"; //在对话框中显示的文件类型
        openFileDialog.Title = "请选择图片文件";

        openFileDialog.Multiselect = true;
        openFileDialog.RestoreDirectory = true;
        if (openFileDialog.ShowDialog() == true)
        {
            var fileList = new List<string>(openFileDialog.FileNames)
                .Where(item => ImageFileList.FirstOrDefault(v => v.FilePath.Equals(item)) == null).ToList();
            var list = GetFileNameItems(fileList);
            ImageFileList.AddRange(list);
            ImageFileList = new List<FileNameItem>(ImageFileList);

            if (ActivateImg == null)
                ActivateImg = ImageFileList[0];
        }
    }
    public void SelectMusicCommand()
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = "c:\\desktop"; //初始的文件夹
        openFileDialog.Filter = "所有文件(*.mp3;*.m4a;*.aac;*.flac,*.wav)|*.mp3;*.m4a;*.aac;*.flac,*.wav"; //在对话框中显示的文件类型
        openFileDialog.Title = "请选择音频文件";
        openFileDialog.Multiselect = true;
        openFileDialog.RestoreDirectory = true;
        if (openFileDialog.ShowDialog() == true)
        {
            var fileList = new List<string>(openFileDialog.FileNames)
                .Where(item => MusicFileList.FirstOrDefault(v => v.FilePath.Equals(item)) == null).ToList();
            var list = GetFileNameItems(fileList);
            var newList = list.Select(iteem =>
            {
                var tmp = new AudioFileItem();
                ObjectCopy.Copy(iteem, tmp);
                
                using (TagLib.File file = TagLib.File.Create(tmp.FilePath))
                {
                    // 获取专辑信息
                    tmp.Album = file.Tag.Album;
                    tmp.Title = file.Tag.Title;
                    if (string.IsNullOrWhiteSpace(tmp.Title))
                        tmp.Title = tmp.FileName;
                    else
                        tmp.Title += " - " + string.Join("、", file.Tag.AlbumArtists);
                    tmp.Time = file.Properties.Duration;
                }
                
                return tmp;

            });
            MusicFileList.AddRange(newList);
            MusicFileList = new List<AudioFileItem>(MusicFileList);
            _eventAggregator.PublishOnUIThread(new ObservableCollection<FileNameItem>(MusicFileList), "music");
        }
    }

    public void PrevImgCommand()
    {
        if (View is GenerateView view)
        {
            // 获取要定位之前 ScrollViewer 目前的滚动位置
            var item = ImageFileList.FindIndex(item => item == ActivateImg);
            if (item >= 0)
            {
                var width = view.ImgListBox.ActualWidth;
                var split = int.Parse(Math.Floor(width / 200).ToString());
                if (item - split > 0)
                {
                    ActivateImg = ImageFileList[item - split];
                    view.ImgListBox.ScrollIntoView(ActivateImg);
                }
                else
                {
                    ActivateImg = ImageFileList.First();
                    view.ImgListBox.ScrollIntoView(ActivateImg);
                }
            }
        }
    }

    public void NextImgCommand()
    {
        if (View is GenerateView view)
        {
            // 获取要定位之前 ScrollViewer 目前的滚动位置
            var item = ImageFileList.FindIndex(item => item == ActivateImg);

            if (item >= 0)
            {
                var width = view.ImgListBox.ActualWidth;
                var split = int.Parse(Math.Floor(width / 200).ToString());
                if (ImageFileList.Count > item + split + split - 1)
                {
                    ActivateImg = ImageFileList[item + split];
                    view.ImgListBox.ScrollIntoView(ImageFileList[item + split + split - 1]);
                }
                else
                {
                    if (ImageFileList.Count - split >= 0)
                    {
                        ActivateImg = ImageFileList[ImageFileList.Count - split];
                        view.ImgListBox.ScrollIntoView(ImageFileList.Last());
                    }
                }
            }
        }
    }

    public void ProcessCommand()
    {
    }

    public void ImgListItemPreviewCommand(FileNameItem item)
    {
        var img = new ImageBrowser(item.Uri, ImageUriList)
        {
            IsFullScreen = true
        };
        img.Show();
    }
    
}