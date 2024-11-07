using System;
using System.Collections.ObjectModel;
using PropertyChanged;
using Senjyouhara.Main.Core.Manager.Dialog;
using Senjyouhara.Main.Model;

namespace Senjyouhara.Main.ViewModels.Components;

[AddINotifyPropertyChangedInterface]
public class MusicViewModel : MyScreen
{

    public ObservableCollection<FileNameItem> FileNameItems { get; set; } = new()
    {
        new FileNameItem()
        {
            Uid = Guid.NewGuid().ToString(),
            OriginFileName = "あたらよ-夏霞",
            FileName = "あたらよ-夏霞",
            FilePath = "E:\\KwDownload\\song\\あたらよ-夏霞.flac",
            Suffix = "flac",
            Uri = new Uri("E:\\KwDownload\\song\\あたらよ-夏霞.flac")
        }
    };
    
    public bool IsLoadingTrack { get; set; }
    public bool ShowPause { get; set; }


    public void PreviousCommand()
    {
    }

    public void PlayCommand()
    {
        ShowPause = true;
    }

    public void PauseCommand()
    {
        ShowPause = false;
    }

    public void NextCommand()
    {
    }

    public void VolumeCommand()
    {
    }

    public void PlayListCommand()
    {
    }
}