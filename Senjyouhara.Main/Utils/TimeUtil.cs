using System;

namespace Senjyouhara.Main.Utils;

public static class TimeUtil
{
    
    public static TimeSpan GetTimeSpanBymillSecond(double times)
    {
        //天
        var days = Math.Floor(times / (1000 * 60 * 60 * 24));
        //小时
        var hours = Math.Floor(times / 1000 / 60 / 60 - days * 24);
        //分钟
        var minutes = Math.Floor(times / 1000 / 60 - days * 24 * 60 - hours * 60);
        //秒
        var seconds = Math.Floor(times / 1000 - days * 24 * 60 * 60 - hours * 60 * 60 - minutes * 60);
        TimeSpan t = new TimeSpan(0, (int)hours, (int)minutes, (int)seconds, (int)(times % 1000));
        return t;
    }
    
}