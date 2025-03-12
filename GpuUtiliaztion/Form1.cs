using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;

namespace WindowsFormsApp1
{
    public partial class Form1: Form
    {
        // 导入 User32.dll 的 ReleaseCapture 和 SendMessage 方法
        [DllImport("user32.dll")]
        private static extern void ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern void SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        private void mychart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            mychart_DoubleClick(sender, e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                changeWindowStyle();
            }
            else if(e.KeyCode==Keys.T)
            {
                setTopMost();
            }
        }
        public Form1()
        {
            InitializeComponent();
            this.MouseDown += Form1_MouseDown;
            this.mychart.MouseDown += Form1_MouseDown;
            
            this.mychart.MouseDoubleClick += this.mychart_MouseDoubleClick;
            this.KeyPreview = true;
            toolStripButton_AlwaysTopMost.Checked = false;
            var gpuInfoList = GPU.GetGpuList();


            // mychart.ChartAreas.Add(chartArea);

            foreach (var chartArea in mychart.ChartAreas)
            {
                chartArea.AxisX.LabelStyle.Format = "HH:mm:ss"; // X轴时间格式
                chartArea.AxisX.IntervalType = DateTimeIntervalType.Seconds; // X轴间隔
                chartArea.AxisX.Interval = 1; // 每秒显示一个点
                chartArea.AxisX.Title = "Time";
                chartArea.AxisY.Title = "Value";
            }
            mychart.Series.Clear();
            // 初始化数据
            int i = 0;
            foreach (var item in gpuInfoList)
            {
                Console.WriteLine(item);
                var ls = new Series(item.Title)
                {
                    ChartType = SeriesChartType.Line,
                    XValueType = ChartValueType.DateTime,
                    BorderWidth = 1,
                    Color= item.color,
                };
                filterToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(item.Title, null, (s, e) =>
                {
                    if (s is ToolStripMenuItem menuItem)
                    {
                        onShowItem(menuItem.Text);
                        //   menuItem.Checked = !menuItem.Checked; // 切换选中状态
                    }
                }));
                var tsb = new ToolStripButton(item.Title, null, (sender, e) =>
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
       

       
        private void timer1_Tick(object sender, EventArgs e)
        {

            int g_minY = 100;
            int g_maxY = -1;
            var Utilizations =GPU. GetGpuUtilization();
            
            for (int i = 0; i < mychart.Series.Count; i++)
            {
                if (i >= Utilizations.Count) continue;

                if (mychart.Series[i].Points.Count > 30)
                {
                    mychart.Series[i].Points.RemoveAt(0);
                }
                var value = Utilizations[i].GpuUtilization;
                mychart.Series[i].Points.AddXY( DateTime.Now,value);
                int minY = 100;
                int maxY = -1;
                foreach (var point in mychart.Series[i].Points)
                {
                    if (point.YValues[0] < minY) minY = (int)point.YValues[0];
                    if (point.YValues[0] > maxY) maxY = (int)point.YValues[0];
                }
                if (minY < g_minY) g_minY = minY;
                if (maxY > g_maxY) g_maxY = maxY;
            }
            // 自动调整 X 轴范围
            mychart.ChartAreas[0].AxisX.Minimum = mychart.Series[0].Points[0].XValue;
            mychart.ChartAreas[0].AxisX.Maximum = mychart.Series[0].Points[mychart.Series[0].Points.Count - 1].XValue;

            mychart.ChartAreas[0].AxisY.Minimum = 0 ; // 适当增加缓冲区
            mychart.ChartAreas[0].AxisY.Maximum = g_maxY + 5;

            mychart.Invalidate(); // 重新绘制
        }

       void setTopMost()
        {
            this.TopMost = !this.TopMost;
            alwaysTopMostToolStripMenuItem.Checked = this.TopMost;
            toolStripButton_AlwaysTopMost.Checked = this.TopMost;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            setTopMost();
        }
        void changeWindowStyle()
        {
            bool b = this.FormBorderStyle.Equals(FormBorderStyle.None);
            //b = xx++%2==0;
            this.FormBorderStyle = b ? FormBorderStyle.Sizable : FormBorderStyle.None;
            this.menuStrip1.Visible = b;
            this.panel_toolbar.Visible = b;
        }

        private void mychart_DoubleClick(object sender, EventArgs e)
        {
            changeWindowStyle();

        }

    }
}
