namespace assemEmulator;

class CPU {
    int ProgramCounter = 0;
    int MemoryAddressRegister = 0;
    long memoryDataRegister;
    machineCodeLine instructionRegister;
    Register[] registers = new Register[12];
     Memory RAM;

    public CPU(ref Memory ram) {
        RAM = ram;
        for (int i = 0; i < registers.Length; i++) {
            registers[i] = new Register();
        }
        instructionRegister = new machineCodeLine();
    }

    public void FetchDecodeExecCycle () {
        Fetch();
        Decode();
        //Execute(); 

    }

    private void Fetch() {
        MemoryAddressRegister = ProgramCounter;
        memoryDataRegister = RAM.QuereyAddress(MemoryAddressRegister);
        ProgramCounter++;
    }

    private void Decode() {
        instructionRegister = new machineCodeLine();
        instructionRegister.instruction = (int)(memoryDataRegister >> Constants.opCodeOffset * Constants.bitsPerNibble);

        int registerValues = (int)(memoryDataRegister >> Constants.registerOffset * Constants.bitsPerNibble);
        registerValues &= 0xFF;

        instructionRegister.arguments.Add((registerValues & 0xF0));
        instructionRegister.arguments.Add((registerValues & 0x0F));

        int signBit = (int)(memoryDataRegister >> Constants.signBitOffset * Constants.bitsPerNibble) & 1;
        instructionRegister.inAddressMode = (signBit == 1);

        long mask = (1L << Constants.signBitOffset * Constants.bitsPerNibble) - 1;
        instructionRegister.arguments.Add((int)(memoryDataRegister & mask));
    }

    private void Execute() {
        switch (instructionRegister.instruction) {
            default:
                throw new System.ArgumentException("invalid instruction passed to execute");
            case 1: //LDR
                MemoryAddressRegister = instructionRegister.arguments[1];
                memoryDataRegister = RAM.QuereyAddress(MemoryAddressRegister);
                registers[instructionRegister.arguments[0]].SetRegister(memoryDataRegister);
                break;
        }
    }

    public void DumpRegisters(string path) {
        System.IO.File.WriteAllLines(path, registers.Select(x => x.GetRegister().ToString()));
    }
}