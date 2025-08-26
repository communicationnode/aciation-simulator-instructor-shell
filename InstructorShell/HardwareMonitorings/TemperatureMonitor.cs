using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibreHardwareMonitor.Hardware;

[StructLayout(LayoutKind.Auto)]
public class TemperatureMonitor {

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public (float? cpuTemp, float? gpuTemp) GetTemperatures() {
        float? cpuTemp = null;
        float? gpuTemp = null;

        var computer = new Computer {
            IsCpuEnabled = true,
            IsGpuEnabled = true
        };

        computer.Open();
        computer.Accept(new UpdateVisitor());

        foreach (IHardware? hardware in computer.Hardware) {
            if (hardware.HardwareType == HardwareType.Cpu) {
                foreach (ISensor? sensor in hardware.Sensors) {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue) {
                        cpuTemp = sensor.Value;
                    }
                }
            }
            else if (hardware.HardwareType == HardwareType.GpuNvidia ||
                     hardware.HardwareType == HardwareType.GpuAmd) {
                foreach (var sensor in hardware.Sensors) {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue) {
                        gpuTemp = sensor.Value;
                    }
                }
            }
        }

        computer.Close();
        return (cpuTemp, gpuTemp);
    }
}

public class UpdateVisitor : IVisitor {

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void VisitComputer(IComputer computer) {
        computer.Traverse(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void VisitHardware(IHardware hardware) {
        hardware.Update();
        foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void VisitSensor(ISensor sensor) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void VisitParameter(IParameter parameter) { }
}