using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    public interface IDialogManager:IWindowManager
    {
        IDialogResult ShowModal(object rootModel);
        IDialogResult ShowModal(object rootModel, IDialogParameters parameters);
        void ShowModalAsync(object rootModel);
        void ShowModalAsync(object rootModel, IDialogParameters parameters);
        void ShowModalAsync(object rootModel, Action<IDialogResult> callback);
        void ShowModalAsync(object rootModel, IDialogParameters parameters, Action<IDialogResult> callback);
        
        IDialogResult ShowWindowDialog(object rootModel);
        IDialogResult ShowWindowDialog(object rootModel, IDialogParameters parameters);
        
        void ShowWindowDialogAsync(object rootModel);
        void ShowWindowDialogAsync(object rootModel, IDialogParameters parameters);
        void ShowWindowDialogAsync(object rootModel, Action<IDialogResult> callback);
        void ShowWindowDialogAsync(object rootModel, IDialogParameters parameters, Action<IDialogResult> callback);
    }
}
