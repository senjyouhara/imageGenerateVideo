using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using NAudio.Wave;
using PropertyChanged;
using Senjyouhara.Common.Base;
using Senjyouhara.Common.Log;
using Senjyouhara.Main.Core.Manager.Dialog;
using Senjyouhara.Main.Model;
using Senjyouhara.Main.Views.Components;
using Stylet;
using StyletIoC;

namespace Senjyouhara.Main.ViewModels.Components;

[AddINotifyPropertyChangedInterface]
public class MusicViewModel : MyScreen, IHandle<ObservableCollection<FileNameItem>>, IDisposable
{
    private readonly IContainer _container;
    private readonly IEventAggregator _eventAggregator;

    public ObservableCollection<FileNameItem> FileNameItems { get; set; }
    
    public bool IsLoadingTrack { get; set; }
    public bool ShowPause { get; set; }

    public int Volume { get; set; }
    public FileNameItem SelectedItem { get; set; }

    private MediaFoundationReader mr;
    private WaveOut wo;
    private Storyboard storyboard;
    private Storyboard storyboard2;


    public DelegateCommand<FileNameItem> StartCommand => new (StartHandler);

    public MusicViewModel(IContainer container, IEventAggregator eventAggregator)
    {
        _container = container;
        _eventAggregator = eventAggregator;
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        _eventAggregator.Subscribe(this, "music");
        // var shellViewModel = _container.Get<ShellViewModel>();
        // shellViewModel.View.MouseUp += BarMouseUp;
        // shellViewModel.View.MouseMove += BarMouseOver;
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
        var index = -1;
        for (var i = 0; i < FileNameItems.Count; i++)
            if (FileNameItems[i].Equals(SelectedItem))
            {
                index = i;
                break;
            }

        if (index >= 0)
            if (index - 1 >= 0)
            {
                SelectedItem = FileNameItems[index - 1];
                StartHandler(SelectedItem);
            }
    }

    public void StartHandler(FileNameItem item)
    {
        AudioDispose();
        ShowPause = true;
        mr = new(item.FilePath);
        wo = new();
        wo.DeviceNumber = 0;
        wo.Init(mr);
        SelectedItem = item;
        wo.PlaybackStopped += (sender, args) =>
        {
            AudioDispose();
        };
        if (View is MusicView view)
        {
                double progress = (double)mr.Position / mr.Length * 100;
                var barWidth = view.Progressbar.ActualWidth;
                var totalWidth = view.ProgressLine.ActualWidth - barWidth;
                
                if(view.ProgressReadLine.FindResource("ProgressLineAnime") is
                   Storyboard {
                       Children:  { Count: > 0 } and  [ DoubleAnimation child  ],
                   } sb)
                {
                    storyboard = sb;
                    storyboard.Stop();
                    child.Duration = TimeSpan.FromSeconds(mr.TotalTime.TotalSeconds);
                    child.From = 0;
                    child.To = totalWidth;
                    // child.FillBehavior = FillBehavior.Stop;
                    storyboard.Begin();
                }
                
                if(view.Progressbar.FindResource("ProgressbarAnime") is
                   Storyboard {
                       Children:  { Count: > 0 } and  [ DoubleAnimation child2  ],
                   } sb2)
                {
                    storyboard2 = sb2;
                    storyboard2.Stop();
                    child2.Duration = TimeSpan.FromSeconds(mr.TotalTime.TotalSeconds);
                    // child2.FillBehavior = FillBehavior.Stop;
                    child2.From = 0;
                    child2.To = totalWidth;
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

    public void SeekTimeLineCommand(object sender,  MouseButtonEventArgs  e)
    {
        if (View is MusicView view)
        {
            if (sender is Border border)
            {
                wo?.Pause();
                storyboard?.Pause();
                storyboard2?.Pause();
                var barWidth = view.Progressbar.ActualWidth;
                var width = border.ActualWidth;
                var position = e.GetPosition(border);
                var positionX = position.X;
                if (positionX < 0)
                {
                    positionX = 0;
                }
                if (positionX > width)
                {
                    positionX = width;
                }
                var percentageX = Math.Round(positionX / width, 8);
                var second = TimeSpan.FromSeconds(mr.TotalTime.TotalSeconds * percentageX);
                storyboard?.Seek(second);
                storyboard2?.Seek(second);
                mr?.Seek( Convert.ToInt64(mr.Length * percentageX), SeekOrigin.Begin);
                wo?.Init(mr);
                wo?.Play();
            }
        }
    }

    private double startX;
    public void BarMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (View is MusicView view && sender is Border border)
        {
            var position = e.GetPosition(border);
            startX = position.X;
        }
    }
    public void BarMouseOver(object sender, MouseEventArgs e)
    {
        if (View is MusicView view)
        {
            if (startX != 0.0)
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    BarMouseUp_Handler(e);
                }
                else
                {
                    storyboard?.Pause();
                    storyboard2?.Pause();
                    var position = e.GetPosition(view.ProgressLine);
                    var width = view.ProgressLine.ActualWidth;

                    var positionX = position.X;    
                    if (positionX < 0)
                    {
                        positionX = 0;
                    }
                    if (positionX > width)
                    {
                        positionX = width;
                    }
                
                    var percentageX = Math.Round(positionX / width, 8);
                    Log.Info($"width: {width}, positionX : {positionX}, percentageX: {percentageX} , second: {mr.TotalTime.TotalSeconds}");
                    var second = TimeSpan.FromSeconds(mr.TotalTime.TotalSeconds * percentageX);
                    storyboard?.Seek(second);
                    storyboard2?.Seek(second);
                }
              
            }
            
        }
    }

    private void BarMouseUp_Handler(MouseEventArgs e)
    {
        if (View is MusicView view)
        {
            if (startX != 0.0)
            {
                wo?.Pause();
                var position = e.GetPosition(view.ProgressLine);
                var width = view.ProgressLine.ActualWidth;
                var positionX = position.X;    
                
                if (positionX < 0)
                {
                    positionX = 0;
                }
                if (positionX > width)
                {
                    positionX = width;
                }
                
                var percentageX = Math.Round(positionX / width, 8);
                Log.Info($"width3: {width}, positionX : {positionX}, percentageX: {percentageX} , second: {mr.TotalTime.TotalSeconds}");
                mr?.Seek( Convert.ToInt64(mr.Length * percentageX), SeekOrigin.Begin);
                wo?.Init(mr);
                wo?.Play();
            }
        }
        startX = 0.0;
    }
    
    public void BarMouseUp(object sender,  MouseButtonEventArgs e)
    {
        BarMouseUp_Handler(e);
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
        var index = -1;
        for (var i = 0; i < FileNameItems.Count; i++)
            if (FileNameItems[i].Equals(SelectedItem))
            {
                index = i;
                break;
            }

        if (index >= 0)
            if (FileNameItems.Count > index + 1)
            {
                SelectedItem = FileNameItems[index + 1];
                StartHandler(SelectedItem);
            }
    }

    public void VolumeCommand()
    {
        // wo.Volume = 1f;    // 设置音量 0~1
    }

    public void PlayListCommand()
    {
    }

    public void Handle(ObservableCollection<FileNameItem> message)
    {
        FileNameItems = message;
    }


    public void Dispose()
    {
        _container?.Dispose();
        mr?.Dispose();
        wo?.Dispose();
        _eventAggregator.Unsubscribe(this, "music");
    }
}