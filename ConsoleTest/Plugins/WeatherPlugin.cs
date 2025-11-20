using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest.Plugins;

public class WeatherPlugin
{
    [KernelFunction("get_location_weather")]
    [Description("获取指定地区的天气")]
    public string GetLocationWeather(string city)
    {
        return $"{city} 天气：小雨";
    }
}
