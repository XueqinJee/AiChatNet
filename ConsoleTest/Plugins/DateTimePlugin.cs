using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest.Plugins;

public class DateTimePlugin
{

    [KernelFunction("get_current_time")]
    [Description("获取当前时间")]
    public DateTime GetCurrentTime()
    {
        return DateTime.Now;
    }
}
