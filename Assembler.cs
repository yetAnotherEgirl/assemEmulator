using System.Net.Sockets;
using System.Reflection.PortableExecutable;

namespace assemEmulator;

//https://filestore.aqa.org.uk/resources/computing/AQA-75162-75172-ALI.PDF


class Assembler {
    
    public readonly string[] instructionSet = {"LDR", "STR", "ADD", "SUB", "MOV", "CMP", "B", "BEQ"};

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
        long output = Constants.addressIndicator;

        if (opperand[0] == Constants.decimalChar) {
            output = ((long)Constants.decimalIndicator) << (Constants.signBitOffset * Constants.bitsPerNibble);
            try {
                output += long.Parse(opperand.Substring(1));
            } catch {
                throw new System.ArgumentException("invalid decimal opperand");
            }
        }
        else if (opperand[0] == Constants.registerChar) {
            output = ((long)Constants.registerIndicator) << (Constants.signBitOffset * Constants.bitsPerNibble);
            try {
                output += long.Parse(opperand.Substring(1));
            } catch {
                throw new System.ArgumentException("invalid register opperand");
            }
        }
        else{
            output <<= Constants.signBitOffset * Constants.bitsPerNibble;
            if(opperand[0]== Constants.registerChar) opperand = opperand.Substring(1); 
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

    public long AssembleLabel(ref string[] line, string label) {
        
        long output = Array.IndexOf(line, label);

        if (output == -1) throw new System.ArgumentException("invalid label");

        return output;
    }
    
    public long CompileAssemblyLine(string assemblyLine) {
        if (string.IsNullOrEmpty(assemblyLine)) throw new System.ArgumentException("empty string passed to assembleLine");
        
        {
            int commentStart = assemblyLine.IndexOf(Constants.commentChar);
            if (commentStart != -1) assemblyLine = assemblyLine.Substring(0, commentStart);
        }
        if (string.IsNullOrEmpty(assemblyLine)) return 0;
        string[] splitLine = assemblyLine.Split(' ');
        if (Array.IndexOf(splitLine, "") != -1) splitLine = splitLine.Where(x => x != "").ToArray();

        int opCode = Array.IndexOf(instructionSet, splitLine[0]) + 1;
        if (opCode == 0) {
            if (splitLine.Length > 1) throw new System.ArgumentException("invalid Label, must be 1 word only");

            //Console.WriteLine("label recognised");
            return 0;
        }
        long output = 0;

        output += AssembleOpCode(opCode);

        switch (opCode)
        {
            default:
                throw new System.ArgumentException("invalid OpCode");
            case 1: //LDR
                output += AssembleRegister(splitLine[1]);
                output += AssembleMemoryReference(splitLine[2]);
                break;
            case 2: //STR
                output += AssembleRegister(splitLine[1]);
                output += AssembleMemoryReference(splitLine[2]);
                break;
            case 3: //ADD
                output += AssembleRegister(splitLine[1]);
                output += AssembleRegister(splitLine[2], 1);
                output += AssembleOpperand(splitLine[3]);
                break;
            case 4: //SUB
                output += AssembleRegister(splitLine[1]);
                output += AssembleRegister(splitLine[2], 1);
                output += AssembleOpperand(splitLine[3]);
                break;
            case 5: //MOV
                output += AssembleRegister(splitLine[1]);
                output += AssembleOpperand(splitLine[2]);
                break;
            case 6: //CMP
                output += AssembleRegister(splitLine[1]);
                output += AssembleOpperand(splitLine[2]);
                break;
            case 7: //B
                output += AssembleLabel(ref splitLine, splitLine[1]);
                break;
            case 8: //BEQ
                output += AssembleLabel(ref splitLine, splitLine[1]);
                break;
 
        }
        return output;
    }
 
    public List<long> GetMachineCode() {
        return machineCode;
    }
 }

