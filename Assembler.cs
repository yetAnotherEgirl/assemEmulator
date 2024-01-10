using System.Net.Sockets;
using System.Reflection.PortableExecutable;

namespace assemEmulator;

//https://filestore.aqa.org.uk/resources/computing/AQA-75162-75172-ALI.PDF


class Assembler {
    
    public readonly string[] instructionSet = 
    {"LDR", "STR", "ADD", "SUB", "MOV", "CMP", "B", "BEQ", "BNE", 
    "BGT", "BLT", "AND", "ORR", "EOR", "MVN", "LSL", "LSR", "HALT"};

    public readonly string[] extendedInstructionSet = {"INPUT", "OUTPUT", "DUMP"};

    bool extendedInstructionSetEnabled = false;

    List<long> machineCode = new List<long>();

    public Assembler() {
        
    }

    public void AssembleFromString(string assembly) {

        List<string> assemblyLineList = assembly.Split('\n').ToList();
        assemblyLineList = assemblyLineList.Where(x => x != "").ToList();

        for (int i = 0; i < assemblyLineList.Count; i++)
        {
            string assemblyLine = assemblyLineList[i];
            int slashIndex = assemblyLine.IndexOf("\r");
            if (slashIndex != -1) assemblyLineList[assemblyLineList.IndexOf(assemblyLine)] = assemblyLine.Substring(0, slashIndex);
            assemblyLineList[i] = assemblyLine;
        }

        PreProcessAssembly(ref assemblyLineList);
        foreach(string assemblyLine in assemblyLineList) {
            machineCode.Add(CompileAssemblyLine(assemblyLine));
        }

        if (Array.IndexOf(assemblyLineList.ToArray(), "HALT") == -1)
            throw new System.ArgumentException("no HALT instruction found");
    }

    public void PreProcessAssembly(ref List<string> assemblyLineList) {
        List<string> preProcessorList = assemblyLineList.Where(x => x.Contains(Constants.preProcessorIndicator)).ToList();
        foreach(string preProcessorInstruction in preProcessorList) {
            string instruction = preProcessorInstruction.Substring(1);
            string[] splitInstruction = instruction.Split(' ');
            if (Array.IndexOf(splitInstruction, "") != -1) splitInstruction = splitInstruction.Where(x => x != "").ToArray();

            if (splitInstruction[0] ==  "EXTENDED" || splitInstruction[0] == "EXTENDED\r") {
                if (splitInstruction.Length != 1) throw new System.ArgumentException("invalid EXTENDED instruction");
                extendedInstructionSetEnabled = true;
            }

            if (splitInstruction[0] == "INCLUDE") {
                if (splitInstruction.Length != 3) throw new System.ArgumentException("invalid USING instruction");
                string path = Path.GetFullPath(splitInstruction[1] + ".aqa");
                string assembly = System.IO.File.ReadAllText(path);

                List<string> assemblyList = assembly.Split('\n').ToList();
                assemblyList = assemblyList.Where(x => x != "").ToList();

                if(splitInstruction[2] == "FIRST") {
                assemblyLineList.InsertRange(0, assemblyList);

                }
                else if(splitInstruction[2] == "LAST") {
                    assemblyLineList.AddRange(assemblyList);
                }
                else if(splitInstruction[2] == "HERE") {
                    int index = assemblyLineList.IndexOf(preProcessorInstruction);
                    assemblyLineList.InsertRange(index, assemblyList);
                }
                else {
                    throw new System.ArgumentException("invalid USING instruction");
                }
            }
        }

        assemblyLineList = assemblyLineList.Where(x => !x.Contains(Constants.preProcessorIndicator)).ToList();
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

  
        long output = 0;
        int opCode = 0;
        output += AssembleOpCode(splitLine, ref opCode);

        switch (opCode)
        {
            default:
                throw new System.ArgumentException("invalid operation");
            case 0: //label
                break;
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
            case 9: //BNE
                output += AssembleLabel(ref splitLine, splitLine[1]);
                break;
            case 10: //BGT
                output += AssembleLabel(ref splitLine, splitLine[1]);
                break;
            case 11: //BLT
                output += AssembleLabel(ref splitLine, splitLine[1]);
                break;
            case 12: //AND
                output += AssembleRegister(splitLine[1]);
                output += AssembleRegister(splitLine[2], 1);
                output += AssembleOpperand(splitLine[3]);
                break;
            case 13: //ORR
                output += AssembleRegister(splitLine[1]);
                output += AssembleRegister(splitLine[2], 1);
                output += AssembleOpperand(splitLine[3]);
                break;
            case 14: //EOR
                output += AssembleRegister(splitLine[1]);
                output += AssembleRegister(splitLine[2], 1);
                output += AssembleOpperand(splitLine[3]);
                break;
            case 15: //MVN
                output += AssembleRegister(splitLine[1]);
                output += AssembleOpperand(splitLine[2]);
                break;
            case 16: //LSL
                output += AssembleRegister(splitLine[1]);
                output += AssembleRegister(splitLine[2], 1);
                output += AssembleOpperand(splitLine[3]);
                break;
            case 17: //LSR
                output += AssembleRegister(splitLine[1]);
                output += AssembleRegister(splitLine[2], 1);
                output += AssembleOpperand(splitLine[3]);
                break;
            case 18: //HALT
                break;
            case 19: //INPUT    
                output += AssembleRegister(splitLine[1]);
                break;
            case 20: //OUTPUT
                output += AssembleRegister(splitLine[1]);
                break;
            case 21: //DUMP
                output += AssembleDumpMode(splitLine[1]);
                break;

 
        }
        return output;
    }
 
    public long AssembleOpCode (string[] line, ref int opCode) {  
        opCode = Array.IndexOf(instructionSet, line[0]) + 1;
        if (extendedInstructionSetEnabled && opCode == 0){
            opCode = Array.IndexOf(extendedInstructionSet, line[0]) + 1;
            if(opCode != 0) opCode += instructionSet.Length;
        }
        if (opCode == 0) {

            if (line.Length > 1) throw new System.ArgumentException(
                "invalid operation, not found in instructionset (did you mean to enable extended instruction set?)");

            //Console.WriteLine("label recognised");
            return 0;
        }
                
        
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
    
    public long AssembleDumpMode (string mode) {
        long output = 0;
        switch (mode) {
            case "memory\r":
                output = 0;
                break;
            case "registers\r":
                output = 1;
                break;
            case "all":
                output = 2;
                break;
            default:
                throw new System.ArgumentException("invalid dump mode");
        }
        return output;
    }

    public List<long> GetMachineCode() {
        return machineCode;
    }
 }

