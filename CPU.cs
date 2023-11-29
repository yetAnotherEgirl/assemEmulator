namespace assemEmulator;

class CPU {
    int ProgramCounter = 0;

    long ALU = 0;
    Register MemoryAddressRegister = new Register();
    Register memoryDataRegister = new Register();
    machineCodeLine instructionRegister;
    Register[] registers = new Register[13];
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
        Execute(); 
    }

    private void Fetch() {
        MemoryAddressRegister.SetRegister(ProgramCounter);
        long x = RAM.QuereyAddress(MemoryAddressRegister.GetRegister());
        memoryDataRegister.SetRegister(x);
        ProgramCounter++;
    }

    private void Decode() {
        instructionRegister = new machineCodeLine();
        instructionRegister.instruction = (int)(memoryDataRegister.GetRegister() >> Constants.opCodeOffset * Constants.bitsPerNibble);

        int registerValues = (int)(memoryDataRegister.GetRegister() >> Constants.registerOffset * Constants.bitsPerNibble);
        registerValues &= 0xFF;

        instructionRegister.arguments.Add((registerValues & 0x0F));
        instructionRegister.arguments.Add((registerValues & 0xF0));

        int signBit = (int)(memoryDataRegister.GetRegister() >> Constants.signBitOffset * Constants.bitsPerNibble) & 1;
        instructionRegister.AddressMode = signBit;

        long mask = (1L << Constants.signBitOffset * Constants.bitsPerNibble) - 1;
        instructionRegister.arguments.Add((int)(memoryDataRegister.GetRegister() & mask));
    }

    private void Execute() {
        switch (instructionRegister.instruction) {
            default:
                throw new System.ArgumentException("invalid instruction passed to execute");
            case 1: 
                LDR();
                break;
            case 2: //STR
                STR();
                break;
            case 3: //ADD
                ADD();
                break;
            case 4: //SUB
                SUB();
                break;
                
        }

        void LDR() {
            if (!(instructionRegister.AddressMode == Constants.addressIndicator)) 
            throw new System.ArgumentException("CPU is not in address mode when reading address");

            MemoryAddressRegister.SetRegister(instructionRegister.arguments[2]);
            memoryDataRegister.SetRegister(RAM.QuereyAddress(MemoryAddressRegister.GetRegister()));
            registers[instructionRegister.arguments[1]].SetRegister(memoryDataRegister.GetRegister());
        }

        void STR() {
            if (!(instructionRegister.AddressMode == Constants.addressIndicator)) 
            throw new System.ArgumentException("CPU is not in address mode when writing address");
            
            MemoryAddressRegister.SetRegister(instructionRegister.arguments[2]);
            memoryDataRegister.SetRegister(registers[instructionRegister.arguments[0]].GetRegister());
            RAM.SetAddress(MemoryAddressRegister.GetRegister(), memoryDataRegister.GetRegister());
        }

        void ADD() {
            if(instructionRegister.AddressMode == Constants.addressIndicator) {
                MemoryAddressRegister.SetRegister(instructionRegister.arguments[2]);
                memoryDataRegister.SetRegister(RAM.QuereyAddress(MemoryAddressRegister.GetRegister()));
            } else if(instructionRegister.AddressMode == Constants.registerIndicator) {
                MemoryAddressRegister.SetRegister(instructionRegister.arguments[2]);
                memoryDataRegister.SetRegister(registers[MemoryAddressRegister.GetRegister()].GetRegister());
            }
            else {
                memoryDataRegister.SetRegister(instructionRegister.arguments[2]);
            }
            ALU = memoryDataRegister.GetRegister();
            memoryDataRegister.SetRegister(registers[instructionRegister.arguments[1]].GetRegister());
            ALU += memoryDataRegister.GetRegister();
            registers[instructionRegister.arguments[0]].SetRegister(ALU);
        }

        void SUB() {
            if(instructionRegister.AddressMode == Constants.addressIndicator) {
                MemoryAddressRegister.SetRegister(instructionRegister.arguments[2]);
                memoryDataRegister.SetRegister(RAM.QuereyAddress(MemoryAddressRegister.GetRegister()));
            } else if(instructionRegister.AddressMode == Constants.registerIndicator) {
                MemoryAddressRegister.SetRegister(instructionRegister.arguments[2]);
                memoryDataRegister.SetRegister(registers[MemoryAddressRegister.GetRegister()].GetRegister());
            }
            else {
                memoryDataRegister.SetRegister(instructionRegister.arguments[2]);
            }
            ALU = memoryDataRegister.GetRegister();
            memoryDataRegister.SetRegister(registers[instructionRegister.arguments[1]].GetRegister());
            ALU -= memoryDataRegister.GetRegister();
            registers[instructionRegister.arguments[0]].SetRegister(ALU);
        }
    }

    public void DumpRegisters(string fileName, string DumpPath = "dumps") {
        string[] memoryDump = new string[registers.Length + 4];

        for(int i = 0; i < registers.Length; i++) {
            memoryDump[i] = $" register {i}: {registers[i].DumpRegister()}";
        }
        memoryDump[registers.Length] = $" PC: {ProgramCounter}";
        memoryDump[registers.Length + 1] = $" ALU: {ALU}";
        memoryDump[registers.Length + 2] = $" MAR: {MemoryAddressRegister.DumpRegister()}";
        memoryDump[registers.Length + 3] = $" MDR: {memoryDataRegister.DumpRegister()}";

        System.IO.Directory.CreateDirectory($"./{DumpPath}");
        fileName = DumpPath + "/" + fileName + ".Dump";
        File.WriteAllLines(fileName, memoryDump);
    }
}