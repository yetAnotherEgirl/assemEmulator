using System.Net.Sockets;
using System.Reflection.PortableExecutable;

namespace assemEmulator;



class Assembler {
    public readonly string[] instructionSet = {"LDR", "STR"};
    private const char commentChar = ';';
    private const char decimalChar = '#';
    private const char registerChar = 'r';

    List<int> machineCode = new List<int>();

    public Assembler() {
        
    }

    public void AssembleFromString(string assembly) {
        List<string> assemblyLineList = assembly.Split('\n').ToList();
        assemblyLineList = assemblyLineList.Where(x => x != "").ToList();
        foreach(string assemblyLine in assemblyLineList) {
            machineCode.Add(CompileAssemblyLine(assemblyLine));
        }
    }

    AssembledLine assembleLine(string assemblyLine) {
        if (string.IsNullOrEmpty(assemblyLine)) throw new System.ArgumentException("empty string passed to assembleLine");
        
        {
            int commentStart = assemblyLine.IndexOf(commentChar);
            if (commentStart != -1) assemblyLine = assemblyLine.Substring(0, commentStart);
        }

        string[] splitLine = assemblyLine.Split(' ');
        if (Array.IndexOf(splitLine, "") != -1) splitLine = splitLine.Where(x => x != "").ToArray();
        int instruction = Array.IndexOf(instructionSet, splitLine[0]) + 1;
        
        AssembledLine assembledLine = new AssembledLine();
        assembledLine.instruction = instruction;
        
        switch (instruction) {
            case -1:
                throw new System.ArgumentException("invalid instruction passed to assembleLine");

            case 0: //LDR
                if (splitLine.Length != 3) throw new System.ArgumentException("invalid number of arguments passed to assembleLine");
                if (!splitLine[1].StartsWith(registerChar)) throw new System.ArgumentException("invalid argument passed to assembleLine, first argument should be a register");
                               
                try {
                    int address = int.Parse(splitLine[2]);
                }
                catch (System.FormatException) {
                    throw new System.ArgumentException("invalid argument passed to assembleLine, second argument should be a memory address");
                }
                assembledLine.arguments = new List<int> {int.Parse(splitLine[1].Substring(1)), int.Parse(splitLine[2])};
                break;
            case 1: //STR
                if (splitLine.Length != 3) throw new System.ArgumentException("invalid number of arguments passed to assembleLine");
                if (!splitLine[1].StartsWith(registerChar)) throw new System.ArgumentException("invalid argument passed to assembleLine, first argument should be a register");
                                
                try {
                    int address = int.Parse(splitLine[2]);
                }
                catch (System.FormatException) {
                    throw new System.ArgumentException("invalid argument passed to assembleLine, second argument should be a memory address");
                }
                assembledLine.arguments = new List<int> {int.Parse(splitLine[1].Substring(1)), int.Parse(splitLine[2])};
                break;
        }
        
        return assembledLine;
    }
 
    private int CompileAssemblyLine(string assemblyLine) {
        AssembledLine assembledLine = assembleLine(assemblyLine);
        int output = assembledLine.instruction;
        foreach (int arg in assembledLine.arguments) {
            output <<= 8;
            output += arg;
        }
        return output;
    }
 
    public List<int> GetMachineCode() {
        return machineCode;
    }
 }

 struct AssembledLine {
     public int instruction;
     public List<int> arguments;

    public AssembledLine() {
        this.instruction = 0;
        this.arguments = new List<int>();
    }

    public override string ToString() {
        string output = instruction.ToString();
        foreach (int arg in arguments) {
            output += " " + arg.ToString();
        }
        return output;
        }
 }