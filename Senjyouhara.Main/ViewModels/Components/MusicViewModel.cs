using System;
using System.Collections.ObjectModel;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using NAudio.Wave;
using PropertyChanged;
using Senjyouhara.Common.Base;
using Senjyouhara.Main.Core.Manager.Dialog;
using Senjyouhara.Main.Model;
using Senjyouhara.Main.Views.Components;
using Stylet;

namespace Senjyouhara.Main.ViewModels.Components;

[AddINotifyPropertyChangedInterface]
public class MusicViewModel : MyScreen
{

    public ObservableCollection<FileNameItem> FileNameItems { get; set; }
    
    public bool IsLoadingTrack { get; set; }
    public bool ShowPause { get; set; }

    private MediaFoundationReader mr;
    private WaveOut wo;
    private Storyboard storyboard;
    private Storyboard storyboard2;

    public DelegateCommand<FileNameItem> StartCommand => new (StartHandler);

    public MusicViewModel()
    {
        FileNameItems = new()
        {
            new FileNameItem()
            {
                Uid = Guid.NewGuid().ToString(),
                OriginFileName = "あたらよ-夏霞",
                FileName = "あたらよ-夏霞",
                FilePath = "E:\\KwDownload\\song\\YOASOBI-たぶん.flac",
                Suffix = "flac",
                Uri = new Uri("E:\\KwDownload\\song\\YOASOBI-たぶん.flac")
            },
            new FileNameItem()
            {
                Uid = Guid.NewGuid().ToString(),
                OriginFileName = "あたらよ-夏霞",
                FileName = "あたらよ-夏霞",
                FilePath = "E:\\KwDownload\\song\\YOASOBI-たぶん.flac",
                Suffix = "flac",
                Uri = new Uri("E:\\KwDownload\\song\\YOASOBI-たぶん.flac")
            },
            new FileNameItem()
            {
                Uid = Guid.NewGuid().ToString(),
                OriginFileName = "あたらよ-夏霞",
                FileName = "あたらよ-夏霞",
                FilePath = "E:\\KwDownload\\song\\YOASOBI-たぶん.flac",
                Suffix = "flac",
                Uri = new Uri("E:\\KwDownload\\song\\YOASOBI-たぶん.flac")
            },
            new FileNameItem()
            {
                Uid = Guid.NewGuid().ToString(),
                OriginFileName = "あたらよ-夏霞",
                FileName = "あたらよ-夏霞",
                FilePath = "E:\\KwDownload\\song\\YOASOBI-たぶん.flac",
                Suffix = "flac",
                Uri = new Uri("E:\\KwDownload\\song\\YOASOBI-たぶん.flac")
            },
            new FileNameItem()
            {
                Uid = Guid.NewGuid().ToString(),
                OriginFileName = "あたらよ-夏霞",
                FileName = "あたらよ-夏霞",
                FilePath = "E:\\KwDownload\\song\\YOASOBI-たぶん.flac",
                Suffix = "flac",
                Uri = new Uri("E:\\KwDownload\\song\\YOASOBI-たぶん.flac")
            },
        };
    }

    private void AudioDispose()
    {
        wo?.Stop();
        wo?.Dispose();
        mr?.Close();
        mr?.Dispose();
    }
    
    private void CheckProgress(MediaFoundationReader audioFile)
    {
        double progress = (double)audioFile.Position / audioFile.Length * 100;
        Console.WriteLine($"播放进度: {progress:F2}%");
        if (View is MusicView view)
        {
            Execute.OnUIThread(() =>
            {
                var barWidth = view.Progressbar.ActualWidth;
                var totalWidth = view.ProgressLine.ActualWidth - barWidth;
                view.ProgressbarTransform.X = totalWidth * (progress / 100); 
                view.ProgressReadLine.Width = totalWidth * (progress / 100);
            });

        }
    }
    
    public void PreviousCommand()
    {
        AudioDispose();
    }

    public void StartHandler(FileNameItem item)
    {
        AudioDispose();
        ShowPause = true;
        mr = new(item.FilePath);
        wo = new();
        wo.DeviceNumber = 0;
        wo.Init(mr);
        if (View is MusicView view)
        {
                double progress = (double)mr.Position / mr.Length * 100;
                var barWidth = view.Progressbar.ActualWidth;
                var totalWidth = view.ProgressLine.ActualWidth - barWidth;
                
                if(view.ProgressReadLine.FindResource("ProgressLineAnime") is
                   Storyboard {
                       Children:  { Count: > 0 } and var child,
                   } sb)
                {
                    storyboard = sb;
                    storyboard.Stop();
                    var sban = child[0] as DoubleAnimation;
                    sban.Duration = TimeSpan.FromSeconds(mr.TotalTime.TotalSeconds);
                    sban.From = 0;
                    sban.To = totalWidth;
                    sban.FillBehavior = FillBehavior.Stop;
                    storyboard.Begin();
                }
                
                if(view.Progressbar.FindResource("ProgressbarAnime") is
                   Storyboard {
                       Children:  { Count: > 0 } and var child2,
                   } sb2)
                {
                    storyboard2 = sb2;
                    storyboard2.Stop();
                    var sban = child2[0] as DoubleAnimation;
                    sban.Duration = TimeSpan.FromSeconds(mr.TotalTime.TotalSeconds);
                    sban.FillBehavior = FillBehavior.Stop;
                    sban.From = 0;
                    sban.To = totalWidth;
                    storyboard2.Begin();
                }
              
                
        }
        // // 创建一个定时器，每500毫秒检查一次播放进度
        // timer?.Stop();
        // timer = new (80);
        // timer.Elapsed += (s, e) => CheckProgress(mr);
        // timer.Start();
        wo.Play();
    }

    public void SeekTimeLineCommand()
    {
        
    }
    
    public void PlayCommand()
    {
        ShowPause = true;
        wo?.Resume();
        if (View is MusicView view)
        {
            storyboard?.Resume();
            storyboard2?.Resume();
        }
    }

    public void PauseCommand()
    {
        ShowPause = false;
        wo?.Pause();
        if (View is MusicView view)
        {
            storyboard?.Pause();
            storyboard2?.Pause();
        }
    }

    public void NextCommand()
    {
        AudioDispose();
    }

    public void VolumeCommand()
    {
        // wo.Volume = 1f;    // 设置音量 0~1
    }

    public void PlayListCommand()
    {
    }
}