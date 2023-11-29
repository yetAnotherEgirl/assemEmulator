namespace assemEmulator;

class machineCodeLine {
    public int instruction;
    public List<int> arguments;
    public int AddressMode;

    public machineCodeLine(int instruction, List<int> arguments, int AddressMode) {
        this.instruction = instruction;
        this.arguments = arguments;
        this.AddressMode = AddressMode;
    }

    public machineCodeLine() {
        instruction = 0;
        arguments = new List<int>();
        AddressMode = 0;
    }
}