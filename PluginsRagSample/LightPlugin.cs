using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginsRagSample
{
    public class LightPlugin
    {
        private List<Model> _models = new List<Model>()
        {
            new Model{Id = 1, Name = "东区的大灯", IsColsed = false},
            new Model{Id = 2, Name = "客厅的大灯", IsColsed = false},
            new Model{Id = 3, Name = "主卧的大灯", IsColsed = true}
        };


        [KernelFunction("get_light_list")]
        [Description("获取家里所有灯的状态")]
        public List<Model> GetLightList()
        {
            return _models;
        }

        [KernelFunction("open_light")]
        [Description("通过 id 打开指定灯")]
        public bool OpenLight(int id)
        {
            var light = _models.FirstOrDefault(x => x.Id == id);
            light.IsColsed = false;
            return true;
        }

        [KernelFunction("close_light")]
        [Description("通过 id 关闭指定灯")]
        public bool CloseLight(int id)
        {
            var light = _models.FirstOrDefault(x => x.Id == id);
            light.IsColsed = true;
            return true;
        }

        public class Model
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsColsed { get; set; }
        }
    }
}
