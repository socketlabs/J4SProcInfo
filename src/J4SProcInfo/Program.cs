using Microsoft.Win32;
using Spectre.Console;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace J4SProcInfo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool f = Avx512F.IsSupported;
            bool bw = Avx512BW.IsSupported;
            bool vb = Avx512Vbmi.IsSupported;
            bool bmi = Bmi2.IsSupported;
            bool allFeaturesPresent = f && bw && vb && bmi;

            AnsiConsole.Write(new Rule("[bold cyan]J4SProcInfo[/]").RuleStyle("cyan"));
            AnsiConsole.MarkupLine("[grey]Java4Shell CPU capability check[/]\n");

            var system = new Table();
            system.Border(TableBorder.Rounded);
            system.AddColumn("[bold]System[/]");
            system.AddColumn("[bold]Details[/]");
            system.AddRow("Host", Markup.Escape(Environment.MachineName));
            system.AddRow("CPU", Markup.Escape(CpuName()));
            system.AddRow("Architecture", RuntimeInformation.ProcessArchitecture.ToString());
            system.AddRow("Logical cores", Environment.ProcessorCount.ToString());
            system.AddRow("OS", Markup.Escape(RuntimeInformation.OSDescription));
            AnsiConsole.Write(system);
            AnsiConsole.WriteLine();

            var features = new Table();
            features.Border(TableBorder.Rounded);
            features.AddColumn("[bold]CPU feature[/]");
            features.AddColumn("[bold]Status[/]");
            AddFeatureRow(features, "AVX512F", f);
            AddFeatureRow(features, "AVX512BW", bw);
            AddFeatureRow(features, "AVX512VBMI", vb);
            AddFeatureRow(features, "BMI2", bmi);
            AnsiConsole.Write(features);
            AnsiConsole.WriteLine();

            var result = allFeaturesPresent
                ? "[yellow]All four CPU features are present.[/] The vulnerable code path may be reachable on an unpatched JVM."
                : "[green]One or more CPU features are missing.[/] The vulnerable AVX-512 Base64 stub is not used on this CPU.";

            AnsiConsole.Write(new Panel(result)
                .Header("[bold]Assessment[/]")
                .BorderColor(allFeaturesPresent ? Color.Yellow : Color.Green));
            AnsiConsole.MarkupLine("[grey]This check reports CPU capabilities only; it does not inspect a JVM or its patch status.[/]");

            if (!Console.IsInputRedirected)
            {
                AnsiConsole.MarkupLine("\n[grey]Press any key to exit...[/]");
                Console.ReadKey(intercept: true);
            }
        }

        private static void AddFeatureRow(Table table, string name, bool supported)
        {
            table.AddRow(
                name,
                supported ? "[green]Supported[/]" : "[red]Not supported[/]");
        }

        private static string CpuName()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                    return Registry.GetValue(@"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0",
                                             "ProcessorNameString", null)?.ToString()?.Trim() ?? "unknown";
                if (OperatingSystem.IsLinux())
                    return File.ReadLines("/proc/cpuinfo")
                               .FirstOrDefault(l => l.StartsWith("model name"))?
                               .Split(':', 2)[1].Trim() ?? "unknown";
            }
            catch { }
            return "unknown";
        }
    }
}
