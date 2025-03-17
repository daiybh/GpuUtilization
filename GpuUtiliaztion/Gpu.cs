using System.Diagnostics;
using System;
using System.Collections.Generic;
using Nvidia.Nvml;
using System.Text;
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
	public List<GpuInfo> gpuInfoList = new List<GpuInfo>();
	public GPU()
    {
        NvGpu.NvmlInitV2();
        fetchGpuList();
}
	~GPU()
	{
		NvGpu.NvmlShutdown();
	}
	public class GpuInfo
    {
        public int gpuId = 0;
        public IntPtr deviceHandle = IntPtr.Zero;
        public string Title;

		//public string busId;
		//public string busId_dec;
		//public string allString;
		public System.Drawing.Color color;
		
		public int GpuUtilization = 0;
		public NvmlEnableState nvmlEnableState = NvmlEnableState.NVML_FEATURE_DISABLED;


    }
    private static string GetStringFromSByteArray(sbyte[] data)
    {
        if (data == null)
            throw new SystemException("Data can't be null");

        byte[] busIdData = Array.ConvertAll(data, (a) => (byte)a);
        return Encoding.Default.GetString(busIdData).Replace("\0", "");
    }
    public List<GpuInfo> GetGpuList()
    {
        return gpuInfoList; ;
    }
    void fetchGpuList()
    {
        gpuInfoList.Clear();

        uint gpuCount = NvGpu.NvmlDeviceGetCountV2();
        if (gpuCount > 0)
        {

            for (int i = 0; i < gpuCount; i++)
            {
                var gpu = NvGpu.NvmlDeviceGetHandleByIndex((uint)i);
                var name = NvGpu.NvmlDeviceGetName(gpu, 100);
                var info = NvGpu.NvmlDeviceGetPciInfoV3(gpu);
                var busId = GetStringFromSByteArray(info.busId);
                var nvmlEnableState = NvGpu.NvmlDeviceGetDisplayMode(gpu);

                
                var pci = busId.Trim().Split(':');
                int pci_bus = int.Parse(pci[1], System.Globalization.NumberStyles.HexNumber);
                var gpuInfo = new GpuInfo()
				{
					Title = $"{((nvmlEnableState == NvmlEnableState.NVML_FEATURE_DISABLED) ? "  " : "3D")}-[{pci_bus:D2},0x{pci[1]}] {name.Replace("NVIDIA ","")} ",
					gpuId = i,
					deviceHandle = gpu,
					color = colors[i % colors.Count],
					nvmlEnableState = nvmlEnableState
				};

                gpuInfoList.Add(gpuInfo);
            }
        }
    }
    public void Fetch_GpuUtilization()
    {
        foreach(var gpu in gpuInfoList)
        {     
            var utilization = new NvmlUtilization();
            var gpuUtilization = NvGpu.NvmlDeviceGetUtilizationRates(gpu.deviceHandle);
			gpu.GpuUtilization = (int)gpuUtilization.gpu;

			
        }
    }
    

 //   static public List<GpuInfo> GetGpuList2()
 //   {
 //       // return ["aa", "bb"];
 //       var process = new Process
 //       {
 //           StartInfo = new ProcessStartInfo
 //           {
 //               FileName = "C:\\Windows\\System32\\nvidia-smi.exe",
 //               //FileName = "cmd.exe",
 //               // Arguments = "--query-gpu=name,pci.bus_id,utilization.gpu --format=csv,noheader,nounits",
 //               Arguments = "--query-gpu=name,pci.bus_id --format=csv,noheader,nounits",
 //               UseShellExecute = false,
 //               RedirectStandardOutput = true,
 //               CreateNoWindow = true
 //           }
 //       };
 //       process.Start();
 //       string output = process.StandardOutput.ReadToEnd().Trim();
 //       process.WaitForExit();
 //       /*
	//	  NVIDIA GeForce RTX 2080 Ti, 00000000:09:00.0
	//	  NVIDIA GeForce GTX 1080 Ti, 00000000:44:00.0
	//	 */
 //       var lines = output.Replace("\r", "").Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

 //       List<GpuInfo> gpuList = new List<GpuInfo>();
 //       foreach (var item in lines)
 //       {
 //           var ls = item.Split(',');
 //           var pci = ls[1].Trim().Split(':');

 //           int pci_bus = int.Parse(pci[1], System.Globalization.NumberStyles.HexNumber);
 //           var gpuInfo = new GpuInfo
 //           {
 //               //allString = item,
 //               Title = $"{pci_bus:D2}:0x{pci[1]}:{ls[0]}",
 //               //busId = pci[1],
 //               color = colors[gpuList.Count % colors.Count],
 //           };
 //           gpuList.Add(gpuInfo);
 //       }
 //       return gpuList;
 //   }
 //   static public List<GPUUtiliaztions> GetGpuUtilization2()
	//{
	//	var process = new Process
	//	{
	//		StartInfo = new ProcessStartInfo
	//		{
	//			FileName = "nvidia-smi",
	//			 Arguments = "--query-gpu=name,pci.bus_id,utilization.gpu --format=csv,noheader,nounits",
	//			//Arguments = "--query-gpu=utilization.gpu --format=csv,nounits,noheader",
	//			UseShellExecute = false,
	//			RedirectStandardOutput = true,
	//			CreateNoWindow = true
	//		}
	//	};
	//	process.Start();
	//	string output = process.StandardOutput.ReadToEnd().Trim();
	//	process.WaitForExit();
	//	/*
	//	 * nvidia-smi --query-gpu=name,pci.bus_id,utilization.gpu --format=csv,noheader,nounits
	//	  name, pci.bus_id, utilization.gpu
	//	  NVIDIA GeForce RTX 2080 Ti, 00000000:09:00.0, 41
	//	  NVIDIA GeForce GTX 1080 Ti, 00000000:44:00.0, 13
	//	 */
	//	var lines = output.Replace("\r", "").Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
	//	List< GPUUtiliaztions> gpuUtiliaztions = new List<GPUUtiliaztions>();
	//	foreach (var line in lines)
	//	{
	//		var items = line.Split(',');
	//		var gpuUtiliaztion = new GPUUtiliaztions
	//		{
	//			GpuName = items[0],
	//			GpuBusId = items[1],
	//			title = items[0] + "," + items[1],
	//			GpuUtilization = int.Parse(items[2]),
	//		};
	//		gpuUtiliaztions.Add(gpuUtiliaztion);
	//	}
	//	return gpuUtiliaztions;
	//}

}