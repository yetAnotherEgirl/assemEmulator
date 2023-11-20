namespace assemEmulator;

class machineCodeLine {
    public int instruction;
    public List<int> arguments;
    public bool inAddressMode;

    public machineCodeLine(int instruction, List<int> arguments, bool inAddressMode) {
        this.instruction = instruction;
        this.arguments = arguments;
        this.inAddressMode = inAddressMode;
    }

    public machineCodeLine() {
        instruction = 0;
        arguments = new List<int>();
        inAddressMode = false;
    }
}