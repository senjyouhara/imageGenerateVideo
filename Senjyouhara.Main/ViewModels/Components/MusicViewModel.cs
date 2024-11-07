using PropertyChanged;
using Senjyouhara.Main.Core.Manager.Dialog;

namespace Senjyouhara.Main.ViewModels.Components;

[AddINotifyPropertyChangedInterface]
public class MusicViewModel : MyScreen
{
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