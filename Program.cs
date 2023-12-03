using System.ComponentModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.CompilerServices;


namespace assemEmulator;

class Program {
    static void Main(string[] args) {
        Assembler assembler = new Assembler();

        Memory ram = new Memory(20);
        CPU cpu = new CPU(ref ram);

        List<long> machineCode = new List<long>();


        string assembly = System.IO.File.ReadAllText("assembly.aqa");
        assembler.AssembleFromString(assembly);
        machineCode = assembler.GetMachineCode();

        ram.LoadMachineCode(machineCode);
        ram.SetAddress(11, 10);

        cpu.Run();
    }
}

