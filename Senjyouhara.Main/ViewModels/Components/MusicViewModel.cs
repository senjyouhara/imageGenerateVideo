using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
using Senjyouhara.Main.Views;
using Senjyouhara.Main.Views.Components;
using Stylet;
using StyletIoC;
using Timer = System.Timers.Timer;

namespace Senjyouhara.Main.ViewModels.Components;

[AddINotifyPropertyChangedInterface]
public class MusicViewModel : MyScreen, IHandle<ObservableCollection<AudioFileItem>>, IDisposable
{
    private readonly IContainer _container;
    private readonly IEventAggregator _eventAggregator;

    public ObservableCollection<AudioFileItem> FileNameItems { get; set; }
    
    public bool IsShowVolumePop { get; set; }
    public bool ShowPause { get; set; }
    public bool ShowPlaylist { get; set; } = true;

    public int Volume { get; set; } = 100;
    public AudioFileItem SelectedItem { get; set; }
    
    public TimeSpan LoadedTime { get; set; }
    public TimeSpan TotalTime { get; set; }
    public string LoadedTimeStr => LoadedTime.ToString($"{(LoadedTime.Hours > 0 ? "hh\\:" : "")}mm\\:ss");
    public string TotalTimeStr => TotalTime.ToString($"{(TotalTime.Hours > 0 ? "hh\\:" : "")}mm\\:ss");

    private MediaFoundationReader mr;
    private WaveOut wo;
    private Storyboard storyboard;
    private Storyboard storyboard2;
    private Timer timer;


    public DelegateCommand<AudioFileItem> StartCommand => new(StartHandler);

    public MusicViewModel(IContainer container, IEventAggregator eventAggregator)
    {
        _container = container;
        _eventAggregator = eventAggregator;
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        _eventAggregator.Subscribe(this, "music");

        Application.Current.MainWindow.RemoveHandler(Mouse.MouseDownEvent,
            new MouseButtonEventHandler(GalbalPopContentMouseDown));
        Application.Current.MainWindow.AddHandler(Mouse.MouseDownEvent,
            new MouseButtonEventHandler(GalbalPopContentMouseDown), false);
        Application.Current.MainWindow.Deactivated -= HiddenVolumePop;
        Application.Current.MainWindow.Deactivated += HiddenVolumePop;
        View.RemoveHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(BarMouseUp));
        View.RemoveHandler(Mouse.MouseMoveEvent, new MouseEventHandler(BarMouseOver));
        View.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(BarMouseUp), false);
        View.AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(BarMouseOver), false);
    }

    private void Timer_Handler()
    {
        // // 创建一个定时器，每500毫秒检查一次播放进度
        timer?.Stop();
        timer = new (200);
        timer.Elapsed += (s, e) =>
        {
            if (LoadedTime.TotalSeconds >= SelectedItem.Time.TotalSeconds)
            {
                timer?.Stop();
                return;
            }
            LoadedTime = TimeSpan.FromSeconds(mr.CurrentTime.TotalSeconds);
        };
        timer.Start();
        
    }

    private void AudioDispose()
    {
        timer?.Stop();
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

    private void PlaybackStopped(object sender, StoppedEventArgs args)
    {
        NextCommand();
        // AudioDispose();
    }
    
    public void StartHandler(AudioFileItem item)
    {
        AudioDispose();
        ShowPause = true;
        if (wo != null)
        {
            wo.PlaybackStopped -= PlaybackStopped;
        }
        mr = new(item.FilePath);
        wo = new();
        wo.DeviceNumber = 0;
        wo.Init(mr);
        SelectedItem = item;
        LoadedTime = TimeSpan.Zero;
        TotalTime = mr.TotalTime;
        wo.PlaybackStopped += PlaybackStopped;
        VolumeCommand();
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
                    Timer_Handler();
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
                storyboard?.Resume();
                storyboard2?.Resume();
                LoadedTime = second;
                Timer_Handler();
                mr?.Seek( Convert.ToInt64(mr.Length * percentageX), SeekOrigin.Begin);
                // wo?.Init(mr);
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
                var second = TimeSpan.FromSeconds(mr.TotalTime.TotalSeconds * percentageX);
                Log.Info($"width3: {width}, positionX : {positionX}, percentageX: {percentageX} , second: {mr.TotalTime.TotalSeconds}");
                mr?.Seek( Convert.ToInt64(mr.Length * percentageX), SeekOrigin.Begin);
                storyboard?.Resume();
                storyboard2?.Resume();
                LoadedTime = second;
                Timer_Handler();
                // wo?.Init(mr);
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
        if (SelectedItem == null)
        {
            if (FileNameItems.Count > 0)
            {
                StartHandler(FileNameItems[0]);    
            }
            
        }
        else
        {
            wo?.Resume();
            Timer_Handler();
            if (View is MusicView view)
            {
                storyboard?.Resume();
                storyboard2?.Resume();
            }    
        }
        
    }

    public void PauseCommand()
    {
        ShowPause = false;
        wo?.Pause();
        timer?.Stop();
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
        if (wo != null) wo.Volume = Volume / 100; // 设置音量 0~1
    }

    public void PlayListCommand()
    {
        ShowPlaylist = !ShowPlaylist;
    }

    public void FileNameSelectedChange(object sender, SelectionChangedEventArgs e)
    {
        e.Handled = true;
        StartHandler(SelectedItem);
    }
    public void ListItemPlayMouseDown(object sender,  MouseButtonEventArgs e)
    {
    }
    
    public void Handle(ObservableCollection<AudioFileItem> message)
    {
        FileNameItems = message;
    }

    public void VolumeClick(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton btn)
        {
            IsShowVolumePop = false;
            IsShowVolumePop = true;    
        }
    }

    private void HiddenVolumePop(object sender, EventArgs e)
    {
        IsShowVolumePop = false;
    }
    
    public void GalbalPopContentMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (View is MusicView view)
        {
            // 当点击在Popup外部时关闭Popup
            if (IsShowVolumePop && view.VolumeButton != sender)
            {
                IsShowVolumePop = false;
            }
            
        }
    
    }
    
    public void PopContentMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    public void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        e.Handled = true;
        VolumeCommand();
    }

    public void Dispose()
    {
        _container?.Dispose();
        mr?.Dispose();
        wo?.Dispose();
        _eventAggregator.Unsubscribe(this, "music");
    }
}