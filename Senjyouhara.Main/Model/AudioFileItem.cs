using System;
using PropertyChanged;

namespace Senjyouhara.Main.Model;

[AddINotifyPropertyChangedInterface]
public class AudioFileItem: FileNameItem
{

    public string Title { get; set; }
    
    public string Art { get; set; }
    
    public string Album { get; set; }
    
    public TimeSpan Time { get; set; }
    
    public string TimeStr
    {
        get
        {
            return Time.ToString();
        }
    }
}