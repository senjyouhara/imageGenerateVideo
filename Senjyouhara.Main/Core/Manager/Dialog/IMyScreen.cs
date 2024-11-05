using System;
using Stylet;

namespace Senjyouhara.Main.Core.Manager.Dialog;

public interface IMyScreen:IScreen
{
    public event EventHandler<EventArgs> ViewLoaded;

}