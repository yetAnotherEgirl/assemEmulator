namespace assemEmulator;

class Memory {
    private long[] memory;

    public Memory (int Size) {
        memory = new long[Size];
    }

    public long QuereyAddress(int address) {
        return memory[address];
    }

    public void setAddress(int address, long value) {
        if (address > memory.Length) throw new System.ArgumentException("address out of bounds");
        memory[address] = value;
    }

    public void DumpMemory(string path) {
        const string DumpPath = "dumps";
        string[] memoryDump = new string[memory.Length];

        for(int i = 0; i < memory.Length; i++) {
            memoryDump[i] = $" address {i}: {memory[i]}";
        }
        System.IO.Directory.CreateDirectory($"./{DumpPath}");
        path = DumpPath + "/" + path + ".Dump";
        File.WriteAllLines(path, memoryDump);
    }

    public void LoadMachineCode(List<long> code, int address = 0) {
        if(address + code.Count > memory.Length) throw new System.ArgumentException("code too large for memory");
        if(address + code.Count  > memory.Length - 3) Console.WriteLine("warning: code may be too large for memory");
        for (int i = address; i < code.Count; i++) {
            memory[i] = code[i];
        }
    }
}

class Register {
    private long value;

    public Register() {
        value = 0;
    }

    public void SetRegister(long val) {
        value = val;
    }

    public long GetRegister() {
        return value;
    }
}
