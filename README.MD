# 简介

使用 nvidia-smi.exe 命令获取当前 GPU 使用情况，并返回一个包含每个 GPU 使用情况的列表。

使用 System.Windows.Forms.DataVisualization.Charting 创建一个图表，并使用 System.Windows.Forms.Timer 创建一个定时器，以每秒更新图表。

# TODO

- [ ] 添加图形界面，显示 GPU Clock情况。
- [ ] 添加图形界面，显示 GPU 使用情况的统计信息。
- [ ] 添加图形界面，显示 GPU 使用情况的趋势图。
- [ ] "compute_cap"

The CUDA Compute Capability, represented as Major DOT Minor.
Section about utilization properties
Utilization rates report how busy each GPU is over time, and can be used to determine how much an application is using the GPUs in the system.
Note: On MIG-enabled GPUs, querying the utilization of encoder, decoder, jpeg, ofa, gpu, and memory is not currently supported.

- [Y] "utilization.gpu"

Percent of time over the past sample period during which one or more kernels was executing on the GPU.
The sample period may be between 1 second and 1/6 second depending on the product.

- [ ] "utilization.memory"

Percent of time over the past sample period during which global (device) memory was being read or written.
The sample period may be between 1 second and 1/6 second depending on the product.

- [ ] "utilization.encoder"

Percent of time over the past sample period during which one or more kernels was executing on the Encoder Engine.
The sample period may be between 1 second and 1/6 second depending on the product.

- [ ] "utilization.decoder"

Percent of time over the past sample period during which one or more kernels was executing on the Decoder Engine.
The sample period may be between 1 second and 1/6 second depending on the product.

- [ ] "utilization.jpeg"

Percent of time over the past sample period during which one or more kernels was executing on the Jpeg Engine.
The sample period may be between 1 second and 1/6 second depending on the product.

- [ ] "utilization.ofa"

Percent of time over the past sample period during which one or more kernels was executing on the Optical Flow Accelerator Engine.
The sample period may be between 1 second and 1/6 second depending on the product.