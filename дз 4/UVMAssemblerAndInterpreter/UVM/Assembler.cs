using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Assembler
{
    public void Assemble(string sourceFilePath, string binaryOutputPath, string logOutputPath)
    {
        var commands = LoadCommands(sourceFilePath);

        var binaryData = CompileCommands(commands);

        File.WriteAllBytes(binaryOutputPath, binaryData);

        var logData = GenerateLog(commands);
        File.WriteAllText(logOutputPath, logData);

    }



    private List<Command> LoadCommands(string filePath)
    {
        var commands = new List<Command>();
        string input  = File.ReadAllText(filePath);
        foreach (var line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = line.Split();
            CommandType type = parts[0] switch
            {
                "LOAD_CONST" => CommandType.Load,
                "READ_MEMORY" => CommandType.Read,
                "WRITE_MEMORY" => CommandType.Write,
                "SHIFT_RIGHT" => CommandType.ShiftRight
            };
            commands.Add(new Command
            {
                CommandName = parts[0],
                A = (int)type,
                B = int.Parse(parts[1]),
                C = int.Parse(parts[2]),
                D = parts.Length > 4 ? int.Parse(parts[3]) : 0
            });
        }
        return commands;
    }

    private byte[] CompileCommands(List<Command> commands)
    {
        var binaryData = new List<byte>();

        foreach (var command in commands)
        {
            var commandBytes = (CommandType)command.A switch
            {
                CommandType.Load => Load.Serialize(command),
                CommandType.Read => Read.Serialize(command),
                CommandType.Write => Write.Serialize(command),
                CommandType.ShiftRight => ShiftRight.Serialize(command),
                _ => throw new NotImplementedException(),
            };
            binaryData.AddRange(commandBytes);
        }

        return binaryData.ToArray();
    }
    private string GenerateLog(List<Command> commands)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        return serializer.Serialize(commands);
    }
}

public enum CommandType{

    Load = 121,
    Read = 113,
    Write = 36,
    ShiftRight = 66
}
public class Command
{
    public string CommandName { get; set; } 
    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }
    public int D { get; set; }

}


public static class Load
{
    
    public static byte[] Serialize(Command command)
    {
        var bytes = new byte[5];
        bytes[0] = (byte)((command.A & 0b1111111) | (command.B & 0x1) << 7);
        bytes[1] = (byte)(((command.B >> 1) & 0b111) | (command.C & 0b111111) << 2);
        bytes[2] = (byte)(((command.C >> 6) & 0b11111111));
        bytes[3] = (byte)(((command.C >> (6 + 8)) & 0b11111111));
        bytes[4] = (byte)(((command.C >> (6 + 8 + 8)) & 0b11111111));



        return bytes;
    }

    public static Command DeSerialize(byte[] array)
    {
        var command = new Command();
        command.A = array[0] & 0b01111111;
        command.B = (array[0] >> 7) | ((array[1] & 0b11) << 1);
        command.C = (array[1] >> 2) | (array[2] << 6) | (array[3] << (6+8)) | (array[4] << (6+8+8));
        return command;
    }

}

public static class Read
{
    public static byte[] Serialize(Command command)
    {
        var bytes = new byte[5];
        bytes[0] = (byte)((command.A & 0b1111111) | (command.B & 0b1) << 7);
        bytes[1] = (byte)(((command.B >> 1) & 0b11) | ((command.C & 0b111) << 2) | ((command.D & 0b111) << 5));
        bytes[2] = (byte)(((command.D >> 3) & 0b11111111));
        bytes[3] = (byte)(command.D >> (8 + 3));
        bytes[4] = (byte)(0);



        return bytes;
    }

    public static Command DeSerialize(byte[] array)
    {
        var command = new Command();
        command.A = array[0] & 0b01111111;
        command.B = (array[0] >> 7) | ((array[1] & 0b11) << 1);
        command.C = ((array[1] >> 2) & 0b111);
        command.D = ((array[1] >> 5) & 0b111) | (array[2] << 3) | (array[3] << (8+3)); 
        return command;
    }
}

public static class Write
{
    public static byte[] Serialize(Command command)
    {
        var bytes = new byte[5];
        bytes[0] = (byte)((command.A & 0x7f) | (command.B & 0x1) << 7);
        bytes[1] = (byte)(((command.B >> 1) & 0x3) | (command.C & 0b111) << 2);
        bytes[2] = (byte)(0);
        bytes[3] = (byte)(0);
        bytes[4] = (byte)(0);



        return bytes;
    }

    public static Command DeSerialize(byte[] array)
    {
        var command = new Command();
        command.A = array[0] & 0b01111111;
        command.B = (array[0] >> 7) | ((array[1] & 0b11) << 1);
        command.C = ((array[1] >> 2) & 0b111);
        return command;
    }
}

public static class ShiftRight
{
    public static byte[] Serialize(Command command)
    {
        var bytes = new byte[5];
        bytes[0] = (byte)((command.A & 0b1111111) | (command.B & 0b1) << 7);
        bytes[1] = (byte)(((command.B >> 1) & 0b11) | (command.C & 0b111111) << 2);
        bytes[2] = (byte)((command.C >> 6) & 0b11111111);
        bytes[3] = (byte)(((command.C >> (6 + 8)) | (command.D & 0b111) << 1));

        return bytes;
    }

    public static Command DeSerialize(byte[] array)
    {
        var command = new Command();
        command.A = array[0] & 0b01111111;
        command.B = (array[0] >> 7) | ((array[1] & 0b11) << 1);
        command.C = ((array[1] >> 2) & 0b111111) | (array[2] << 6) | ((array[3] << (6+8)) & 0b1);
        command.D = array[3] >> 1;
        return command;
    }
}


