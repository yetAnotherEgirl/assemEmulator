namespace assemEmulator;

class CPU {
    int ProgramCounter = 0;
    int MemoryAddressRegister = 0;
    int memoryDataRegister;
    AssembledLine instructionRegister;
    Register[] registers = new Register[12];
     Memory RAM;

    public CPU(ref Memory ram) {
        RAM = ram;
        for (int i = 0; i < registers.Length; i++) {
            registers[i] = new Register();
        }
    }

    public void FetchDecodeExecCycle () {

        Fetch();
        Decode();
        Execute(); 

    }

    private void Fetch() {
        MemoryAddressRegister = ProgramCounter;
        memoryDataRegister = RAM.QuereyAddress(MemoryAddressRegister);
        

        ProgramCounter++;
    }

    private void Decode() {
        int encodedInstruction = memoryDataRegister;
        instructionRegister = new AssembledLine();
        List<int> arguments = new List<int>();
        while (encodedInstruction != 0) {
            arguments.Add(encodedInstruction & 0xFF);
            encodedInstruction >>= 8;
        }
        arguments.Reverse();
        instructionRegister.instruction = arguments[0];
        arguments.RemoveAt(0);
        instructionRegister.arguments = arguments;
        
    }

    private void Execute() {
        switch (instructionRegister.instruction) {
            case 1: //LDR
                MemoryAddressRegister = instructionRegister.arguments[1];
                memoryDataRegister = RAM.QuereyAddress(MemoryAddressRegister);
                registers[instructionRegister.arguments[0]].SetRegister(memoryDataRegister);
                break;
            case 2: //STR
                MemoryAddressRegister = instructionRegister.arguments[1];
                memoryDataRegister = registers[instructionRegister.arguments[0]].GetRegister();
                RAM.setAddress(MemoryAddressRegister, memoryDataRegister);
                break;
        }
    }

    public void DumpRegisters(string path) {
        System.IO.File.WriteAllLines(path, registers.Select(x => x.GetRegister().ToString()));
    }
}