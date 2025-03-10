using System.Diagnostics;
using System;
using System.Collections.Generic;
class GPU
{
	static public List<System.Drawing.Color> colors = new List<System.Drawing.Color>(){

			System.Drawing.Color.Red ,
			System.Drawing.Color.Blue,
			System.Drawing.Color.Green ,

			System.Drawing.Color.Yellow,
			System.Drawing.Color.Purple,
			System.Drawing.Color.Brown,
			System.Drawing.Color.Orange,
			System.Drawing.Color.Black,
		};
	static public string[] GetGpuList()
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
	public class GPUUtiliaztions
	{
        public string GpuName;
		public string GpuBusId;
		public string title;
        public int GpuUtilization;

	}
	static public List<GPUUtiliaztions> GetGpuUtilization()
	{
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "nvidia-smi",
				 Arguments = "--query-gpu=name,pci.bus_id,utilization.gpu --format=csv,noheader,nounits",
				//Arguments = "--query-gpu=utilization.gpu --format=csv,nounits,noheader",
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
		List< GPUUtiliaztions> gpuUtiliaztions = new List<GPUUtiliaztions>();
		foreach (var line in lines)
		{
			var items = line.Split(',');
			var gpuUtiliaztion = new GPUUtiliaztions
			{
				GpuName = items[0],
				GpuBusId = items[1],
				title = items[0] + "," + items[1],
				GpuUtilization = int.Parse(items[2]),
			};
			gpuUtiliaztions.Add(gpuUtiliaztion);
		}
		return gpuUtiliaztions;
	}

}