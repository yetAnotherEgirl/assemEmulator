using System.ComponentModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.CompilerServices;

//https://filestore.aqa.org.uk/resources/computing/AQA-75162-75172-ALI.PDF

namespace assemEmulator;

class Program {
    static void Main(string[] args) {
        Assembler assembler = new Assembler();

        Memory ram = new Memory(7);
        CPU cpu = new CPU(ref ram);

        List<long> machineCode = new List<long>();
        
        string assembly = System.IO.File.ReadAllText("assembly.aqa");
        assembler.AssembleFromString(assembly);
        machineCode = assembler.GetMachineCode();

        ram.LoadMachineCode(machineCode);
        ram.SetAddress(5, 10);
        cpu.FetchDecodeExecCycle();
        cpu.FetchDecodeExecCycle();

        ram.DumpMemory("memory");
        cpu.DumpRegisters("registers");
    }
}

