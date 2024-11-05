using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    public enum ButtonResult
    {
        /// <summary>
        /// Abort. 点弹框的X设定为该值
        /// </summary>
        Abort = 3,
        /// <summary>
        /// Cancel.  点取消设定为该值
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// OK.  点确定设置为该值
        /// </summary>
        OK = 1,
    }
}
