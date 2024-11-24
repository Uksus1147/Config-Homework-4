using System;
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
            var commandBytes = ParseCommand(command);
            binaryData.AddRange(commandBytes);
        }

        return binaryData.ToArray();
    }

    private byte[] ParseCommand(Command command)
    {
        var bytes = new byte[5];

        bytes[0] = (byte)((command.A & 0b01111111) | ((command.B & 0b1) << 7));


        bytes[1] = (byte)(((command.B >> 1) & 0b11) | ((command.C >> 16) & 0b111111) << 2);


        bytes[2] = (byte)((command.C >> 8) & 0xFF);  
        bytes[3] = (byte)(command.C & 0xFF);         

        Console.WriteLine($"Команда: {command.CommandName}, A: {command.A}, B: {command.B}, C: {command.C}");
        Console.WriteLine($"Скомпилированные байты: {BitConverter.ToString(bytes)}");

        return bytes;
    }


    private string GenerateLog(List<Command> commands)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        return serializer.Serialize(commands);
    }
}

public class Command
{
    public string CommandName { get; set; } 
    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }
    public int D { get; set; }

}

