using System.Text.RegularExpressions;

namespace assemEmulator;

class Program {
    static void Main(string[] args) {
        Assembler assembler = new Assembler();

        Memory ram = new Memory(5);
        CPU cpu = new CPU(ref ram);

        List<int> machineCode = new List<int>();
        
        string assembly = "LDR r1 3 \nSTR r1 5";
        assembler.AssembleFromString(assembly);

        machineCode = assembler.GetMachineCode();
        ram.LoadMachineCode(machineCode);

        ram.setAddress(3, 5);

        for(int i = 0; i < machineCode.Count; i++) {
            cpu.FetchDecodeExecCycle();
        }

        ram.DumpMemory("memoryDump.txt");
    }
}

