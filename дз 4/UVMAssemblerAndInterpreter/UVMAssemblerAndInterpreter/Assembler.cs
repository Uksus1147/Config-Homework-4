﻿using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Assembler
{
    public void Assemble(string sourceFilePath, string binaryOutputPath, string logOutputPath)
    {
        var commands = LoadCommandsFromYaml(sourceFilePath);

        var binaryData = CompileCommands(commands);

        File.WriteAllBytes(binaryOutputPath, binaryData);

        var logData = GenerateLog(commands);
        File.WriteAllText(logOutputPath, logData);

    }
    private List<Command> LoadCommandsFromYaml(string filePath)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance) 
            .IgnoreUnmatchedProperties() 
            .Build();

        using (var reader = new StreamReader(filePath))
        {
            return deserializer.Deserialize<List<Command>>(reader);
        }
    }
    private byte[] CompileCommands(List<Command> commands)
    {
        var binaryData = new List<byte>();

        foreach (var command in commands)
        {
            var commandBytes = command.A switch
            {
                CommandType.Load => Load.Serialize(command),

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

public class Load
{
    public static byte[] Serialize(Command command)
    {
        var bytes = new byte[5];
        bytes[0] = (byte)((command.A & 0b1111111) | (command.B & 0x1) << 7);
        bytes[1] = (byte)(((command.B >> 1) & 0b111) | (command.C & 0b111111) << 2);
        bytes[2] = (byte)(((command.C >> 6) & 0b11111111));
        bytes[3] = (byte)(((command.C >> (6 + 8)) & 0b11111111));
        bytes[4] = (byte)(((command.C >> (6 + 8 + 8)) & 0b11111111));


        // Отладочный вывод
        Console.WriteLine($"Команда: {command.CommandName}, A: {command.A}, B: {command.B}, C: {command.C}, D: {command.D}");
        Console.WriteLine($"Скомпилированные байты: {BitConverter.ToString(bytes)}");

        return bytes;
    }

    public Command DeSerialize(byte[] array)
    {
        var command = new Command();
        command.A = array[0] & 0b01111111;
        command.B = (array[0] >> 7) | (array[1] & 0b11);
        command.C = (array[1] >> 2) | (array[2]) | (array[3]) | (array[4]);
        return command;
    }

}

public class Read
{
    public byte[] Serialize(Command command)
    {
        var bytes = new byte[5];
        bytes[0] = (byte)((command.A & 0b1111111) | (command.B & 0b1) << 7);
        bytes[1] = (byte)(((command.B >> 1) & 0b11) | ((command.C & 0b111) << 2) | ((command.D & 0b111) << 5));
        bytes[2] = (byte)(((command.D >> 3) & 0b11111111));
        bytes[3] = (byte)(command.D >> (8 + 3));
        bytes[4] = (byte)(0);


        // Отладочный вывод
        Console.WriteLine($"Команда: {command.CommandName}, A: {command.A}, B: {command.B}, C: {command.C}, D: {command.D}");
        Console.WriteLine($"Скомпилированные байты: {BitConverter.ToString(bytes)}");

        return bytes;
    }

    public Command DeSerialize(byte[] array)
    {
        var command = new Command();
        command.A = array[0] & 0b01111111;
        command.B = (array[0] >> 7) | (array[1] & 0b11);
        command.C = ((array[1] >> 2) & 0b111);
        command.D = ((array[1] >> 5) & 0b111) | (array[2] << 3) | (array[3] << (8+3)); 
        return command;
    }
}

public class Write
{
    public byte[] Serialize(Command command)
    {
        var bytes = new byte[5];
        bytes[0] = (byte)((command.A & 0x7f) | (command.B & 0x1) << 7);
        bytes[1] = (byte)(((command.B >> 1) & 0x3) | (command.C & 0b111) << 2);
        bytes[2] = (byte)(0);
        bytes[3] = (byte)(0);
        bytes[4] = (byte)(0);


        // Отладочный вывод
        Console.WriteLine($"Команда: {command.CommandName}, A: {command.A}, B: {command.B}, C: {command.C}, D: {command.D}");
        Console.WriteLine($"Скомпилированные байты: {BitConverter.ToString(bytes)}");

        return bytes;
    }

    public Command DeSerialize(byte[] array)
    {
        var command = new Command();
        command.A = array[0] & 0b01111111;
        command.B = (array[0] >> 7) | (array[1] & 0b11);
        command.C = ((array[1] >> 2) & 0b111);
        return command;
    }
}

public class ShiftRight
{
    public byte[] Serialize(Command command)
    {
        var bytes = new byte[5];
        bytes[0] = (byte)((command.A & 0b1111111) | (command.B & 0b1) << 7);
        bytes[1] = (byte)(((command.B >> 1) & 0b11) | (command.C & 0b111111) << 2);
        bytes[2] = (byte)((command.C >> 6) & 0b11111111);
        bytes[3] = (byte)(((command.C >> (6 + 8)) | (command.D & 0b111) << 1));


        // Отладочный вывод
        Console.WriteLine($"Команда: {command.CommandName}, A: {command.A}, B: {command.B}, C: {command.C}, D: {command.D}");
        Console.WriteLine($"Скомпилированные байты: {BitConverter.ToString(bytes)}");

        return bytes;
    }

    public Command DeSerialize(byte[] array)
    {
        var command = new Command();
        command.A = array[0] & 0b01111111;
        command.B = (array[0] >> 7) | (array[1] & 0b11);
        command.C = ((array[1] >> 2) & 0b111111) | (array[2] << 6) | ((array[3] << (6+8)) & 0b1);
        command.D = array[3] >> 1;
        return command;
    }
}


