namespace assemEmulator;

class Memory {
    private int[] memory;

    public Memory (int Size) {
        memory = new int[Size];
    }

    public int QuereyAddress(int address) {
        return memory[address];
    }

    public void setAddress(int address, int value) {
        memory[address] = value;
    }

    public void DumpMemory(string path) {
        System.IO.File.WriteAllLines(path, memory.Select(x => x.ToString()));
    }

    public void LoadMachineCode(List<int> code, int address = 0) {
        for (int i = address; i < code.Count; i++) {
            memory[i] = code[i];
        }
    }
}

class Register {
    private int value;

    public Register() {
        value = 0;
    }

    public void SetRegister(int val) {
        value = val;
    }

    public int GetRegister() {
        return value;
    }
}
