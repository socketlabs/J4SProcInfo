# J4SProcInfo

J4SProcInfo is a small Windows command-line diagnostic for checking whether a host's CPU exposes the instruction-set features associated with the Java4Shell vulnerable code path. It reports the machine name, processor, architecture, operating system, logical core count, and support for AVX512F, AVX512BW, AVX512VBMI, and BMI2.

For background, see [Java4Shell](https://calif.io/java4shell).

## Usage

Download and extract the self-contained `J4SProcInfo.exe` release, then run it from PowerShell in the extracted folder:

```powershell
.\J4SProcInfo.exe
```

You can also launch `J4SProcInfo.exe` from File Explorer. No .NET installation is required. The program takes no command-line arguments and prints its results to the console.

## Interpreting the result

If all four features are present, the program reports that the vulnerable path may be reachable on an unpatched JVM. If one or more are missing, it reports that the vulnerable AVX-512 Base64 stub is not used. This is a CPU capability check only: it does not inspect a JVM, verify its version or patch level, or confirm that a system is vulnerable.

The console output uses tables and colored status labels. Colors are omitted in these examples.

### Example: CPU prerequisites present

```text
J4SProcInfo
Java4Shell CPU capability check

╭───────────────┬──────────────────────╮
│ System        │ Details              │
├───────────────┼──────────────────────┤
│ Host          │ EXAMPLE-PC            │
│ CPU           │ Example Processor     │
│ Architecture  │ X64                   │
│ Logical cores │ 16                    │
│ OS            │ Microsoft Windows ...│
╰───────────────┴──────────────────────╯

╭─────────────┬───────────────╮
│ CPU feature │ Status        │
├─────────────┼───────────────┤
│ AVX512F     │ Supported     │
│ AVX512BW    │ Supported     │
│ AVX512VBMI  │ Supported     │
│ BMI2        │ Supported     │
╰─────────────┴───────────────╯

╭─ Assessment ─────────────────────────────────────────╮
│ All four CPU features are present. The vulnerable    │
│ code path may be reachable on an unpatched JVM.      │
╰─────────────────────────────────────────────────────╯
This check reports CPU capabilities only; it does not inspect a JVM or its patch status.
```

### Example: one or more CPU prerequisites missing

```text
J4SProcInfo
Java4Shell CPU capability check

╭─────────────┬───────────────╮
│ CPU feature │ Status        │
├─────────────┼───────────────┤
│ AVX512F     │ Not supported │
│ AVX512BW    │ Supported     │
│ AVX512VBMI  │ Supported     │
│ BMI2        │ Supported     │
╰─────────────┴───────────────╯

╭─ Assessment ─────────────────────────────────────────╮
│ One or more CPU features are missing. The vulnerable │
│ AVX-512 Base64 stub is not used on this CPU.         │
╰─────────────────────────────────────────────────────╯
This check reports CPU capabilities only; it does not inspect a JVM or its patch status.
```

Use this diagnostic only on systems you own or are authorized to assess.
