using System;
using System.Collections.Generic;
using PropertyChanged;

namespace Senjyouhara.Main.Model;

[AddINotifyPropertyChangedInterface]
public class QueueModel
{
    public List<FileNameItem> ImageList { get; set; } = new();

    public List<AudioFileItem> AudioList { get; set; } = new();

    public int OutputResolution { get; set; }

    public string Uid { get; set; }

    public DateTime CreateTime { get; set; }

    public string QueueName { get; set; }
}