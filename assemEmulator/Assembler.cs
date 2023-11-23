using System.Net.Sockets;
using System.Reflection.PortableExecutable;

namespace assemEmulator;



class Assembler {
    
    public readonly string[] instructionSet = {"LDR", "STR", "ADD"};

    List<long> machineCode = new List<long>();

    public Assembler() {
        
    }

    public void AssembleFromString(string assembly) {
        List<string> assemblyLineList = assembly.Split('\n').ToList();
        assemblyLineList = assemblyLineList.Where(x => x != "").ToList();
        foreach(string assemblyLine in assemblyLineList) {
            machineCode.Add(CompileAssemblyLine(assemblyLine));
        }
    }

    public long AssembleOpCode (int opCode) {    
        if (opCode == -1) throw new System.ArgumentException("invalid OpCode");
        long output = ((long)opCode) << Constants.opCodeOffset * Constants.bitsPerNibble;

        return output;
    }

    public long AssembleRegister(string register, int registerOffsetIndex = 0) {
        if (register[0] != Constants.registerChar) throw new System.ArgumentException("invalid register");
        int registerAddress = 0;
        try {
            registerAddress = int.Parse(register.Substring(1));
        } catch {
            throw new System.ArgumentException("invalid register");
        }
        int CurrentRegisterOffset = Constants.registerOffset + registerOffsetIndex;
        if (registerAddress < 0 || registerAddress > 15) throw new System.ArgumentException("invalid register address");
        long output = ((long)registerAddress) << CurrentRegisterOffset * Constants.bitsPerNibble;

        return output;
    }

    public long AssembleOpperand (string opperand) {
        long output = 1;
    

        if (opperand[0] == Constants.decimalChar) {
            output = 0;
            try {
                output = long.Parse(opperand.Substring(1));
            } catch {
                throw new System.ArgumentException("invalid decimal opperand");
            }
        }
        else{
            output <<= Constants.signBitOffset * Constants.bitsPerNibble;  
            try {
                output += long.Parse(opperand);
            } 
            catch {
                throw new System.ArgumentException("invalid opperand");
            }
        }

 
        return output;
    }
 
    public long AssembleMemoryReference(string memory) {
        long memoryReference = -1;
        try {
            memoryReference = AssembleOpperand(memory);
        } catch {
            throw new System.ArgumentException("invalid memory reference");
        }
        if (memoryReference < 0) throw new System.ArgumentException("invalid memory reference, must be positive");
        return memoryReference;
    }

    public long CompileAssemblyLine(string assemblyLine) {
        if (string.IsNullOrEmpty(assemblyLine)) throw new System.ArgumentException("empty string passed to assembleLine");
        
        {
            int commentStart = assemblyLine.IndexOf(Constants.commentChar);
            if (commentStart != -1) assemblyLine = assemblyLine.Substring(0, commentStart);
        }

        string[] splitLine = assemblyLine.Split(' ');
        if (Array.IndexOf(splitLine, "") != -1) splitLine = splitLine.Where(x => x != "").ToArray();

        int opCode = Array.IndexOf(instructionSet, splitLine[0]) + 1;

        long output = 0;

        output += AssembleOpCode(opCode);

        switch (opCode)
        {
            case 1:
                output += AssembleRegister(splitLine[1]);
                output += AssembleMemoryReference(splitLine[2]);
                break;
            case 2:
                output += AssembleRegister(splitLine[1]);
                output += AssembleMemoryReference(splitLine[2]);
                break;
            case 3:
                output += AssembleRegister(splitLine[1]);
                output += AssembleRegister(splitLine[2], 1);
                output += AssembleOpperand(splitLine[3]);
                break;
        }
        return output;
    }
 
    public List<long> GetMachineCode() {
        return machineCode;
    }
 }

