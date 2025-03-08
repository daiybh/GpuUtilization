using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;

namespace WindowsFormsApp1
{
    public partial class Form1: Form
    {
        List<System.Drawing.Color> colors = new List<System.Drawing.Color>(){
            
            System.Drawing.Color.Red ,
            System.Drawing.Color.Blue,
            System.Drawing.Color.Green ,

            System.Drawing.Color.Yellow,
            System.Drawing.Color.Purple,
            System.Drawing.Color.Brown,
            System.Drawing.Color.Orange,
            System.Drawing.Color.Black,
        };
        public Form1()
        {
            InitializeComponent();
            var gpuInfoList = GetGpuList();

            // mychart.ChartAreas.Add(chartArea);

            foreach (var chartArea in mychart.ChartAreas)
            {
                chartArea.AxisX.LabelStyle.Format = "HH:mm:ss"; // X轴时间格式
                chartArea.AxisX.IntervalType = DateTimeIntervalType.Seconds; // X轴间隔
                chartArea.AxisX.Interval = 1; // 每秒显示一个点
                chartArea.AxisX.Title = "时间";
                chartArea.AxisY.Title = "数值";
            }
            mychart.Series.Clear();
            // 初始化数据
            int i = 0;
            foreach (var item in gpuInfoList)
            {
                Console.WriteLine(item);
                var ls = new Series(item)
                {
                    ChartType = SeriesChartType.Line,
                    XValueType = ChartValueType.DateTime,
                    BorderWidth = 10,
                };

                 ls.Color = colors[i++];
                filterToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(item, null, (s, e) =>
                {
                    if (s is ToolStripMenuItem menuItem)
                    {
                        onShowItem(menuItem.Text);
                        //   menuItem.Checked = !menuItem.Checked; // 切换选中状态
                    }
                }));
                var tsb = new ToolStripButton(item, null, (sender, e) =>
                  {

                      if (sender is ToolStripButton toolStripButton)
                      {
                          onShowItem(toolStripButton.Text);
                      }
                  });
                tsb.ForeColor = ls.Color;
                toolStrip1.Items.Add(tsb);
                mychart.Series.Add(ls);
            }
            foreach (var item in filterToolStripMenuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.Checked = true;
                }
            }
            foreach (var item in toolStrip1.Items)
            {
                if (item is ToolStripButton button)
                {
                    button.Checked = true;
                }

            }
           
        }
        private void onShowItem(string text)
        {
            mychart.Series[text].Enabled = !mychart.Series[text].Enabled;
            foreach (var item in filterToolStripMenuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    if (menuItem.Text == text)
                    {
                        menuItem.Checked = !menuItem.Checked;
                    }
                }
            }
            foreach(var item in toolStrip1.Items)
            {
                if (item is ToolStripButton button)
                {
                    if (button.Text == text)
                    {
                        button.Checked = !button.Checked;
                    }
                }
            }
        }       
        public string[] GetGpuList()
        {
           // return ["aa", "bb"];
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Windows\\System32\\nvidia-smi.exe",
                    //FileName = "cmd.exe",
                    // Arguments = "--query-gpu=name,pci.bus_id,utilization.gpu --format=csv,noheader,nounits",
                    Arguments = "--query-gpu=name,pci.bus_id --format=csv,noheader,nounits",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            /*
              NVIDIA GeForce RTX 2080 Ti, 00000000:09:00.0
              NVIDIA GeForce GTX 1080 Ti, 00000000:44:00.0
             */
            var lines = output.Replace("\r", "").Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (var item in lines)
            //{
            //    var gpuInfo = new GpuInfo
            //    {
            //        Title = item,
            //      //  brushes = brushList[gpuInfoList.Count],
            //    };
            //  //  gpuInfoList.Add(gpuInfo);
            //}
            return lines;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            var Utilizations = GetGpuUtilization();
            for (int i = 0; i < mychart.Series.Count; i++)
            {
                if (i >= Utilizations.Length) continue;

                if (mychart.Series[i].Points.Count > 20)
                {
                    mychart.Series[i].Points.RemoveAt(0);
                }
                mychart.Series[i].Points.AddXY( DateTime.Now,Int32.Parse(Utilizations[i]));
            }
            // 自动调整 X 轴范围
            mychart.ChartAreas[0].AxisX.Minimum = mychart.Series[0].Points[0].XValue;
            mychart.ChartAreas[0].AxisX.Maximum = mychart.Series[0].Points[mychart.Series[0].Points.Count - 1].XValue;

            mychart.Invalidate(); // 重新绘制
        }

        public string[] GetGpuUtilization()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "nvidia-smi",
                    // Arguments = "--query-gpu=name,pci.bus_id,utilization.gpu --format=csv,noheader,nounits",
                    Arguments = "--query-gpu=utilization.gpu --format=csv,nounits,noheader",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            /*
             * nvidia-smi --query-gpu=name,pci.bus_id,utilization.gpu --format=csv,noheader,nounits
              name, pci.bus_id, utilization.gpu
              NVIDIA GeForce RTX 2080 Ti, 00000000:09:00.0, 41
              NVIDIA GeForce GTX 1080 Ti, 00000000:44:00.0, 13
             */
            var lines = output.Replace("\r", "").Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            return lines;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost; 
            alwaysTopMostToolStripMenuItem.Checked = this.TopMost;
            toolStripButton_AlwaysTopMost.Checked = this.TopMost;
        }
    }
}
