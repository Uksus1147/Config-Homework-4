using System;
using System.Collections.Generic;
using System.IO;    

public class Interpreter
{
    public int[] Memory = new int[1024];
    public int[] Registers = new int[8]; 


    public void Execute(string binaryFilePath, string resultFilePath)
    {
        Console.WriteLine("Интерпретатор: чтение бинарного файла...");
        var binaryData = File.ReadAllBytes(binaryFilePath);
        Console.WriteLine($"Интерпретатор: загружено {binaryData.Length} байт.");
        Console.WriteLine($"Содержимое бинарного файла: {BitConverter.ToString(binaryData)}");

        Console.WriteLine("Интерпретатор: выполнение команд...");
        for (int i = 0; i < binaryData.Length; i += 5)
        {
            byte[] commandBytes = new byte[5];
            Array.Copy(binaryData, i, commandBytes, 0, 5);

            Console.WriteLine($"Интерпретатор: выполнение команды с байтами {BitConverter.ToString(commandBytes)}...");
            ExecuteCommand(commandBytes);
        }


        Console.WriteLine("Интерпретатор: сохранение памяти в YAML...");
        SaveMemoryToYaml(resultFilePath);

        Console.WriteLine("Интерпретатор завершён.");
    }
    public void ExecuteCommand(byte[] commandBytes)
    {


        int A = commandBytes[0] & 0b01111111;       

        switch (A)
        {
            case 121:
                var commandLoad = Load.DeSerialize(commandBytes);
                Registers[commandLoad.B] = commandLoad.C;
                break;

            case 113: 
                var commandRead = Read.DeSerialize(commandBytes);
                int memoryAddress = Registers[commandRead.C];
                Registers[commandRead.B] = Memory[memoryAddress];
                break;

            case 36: 
                var commandWrite = Write.DeSerialize(commandBytes);
                int writeAddress = Registers[commandWrite.B];
                Memory[writeAddress] = Registers[commandWrite.C];
                break;

            case 66:
                var commandShift = ShiftRight.DeSerialize(commandBytes);
                Memory[commandShift.C] = Registers[commandShift.D] >> Memory[Registers[commandShift.B]];
                break;

            default:
                throw new InvalidOperationException($"Неизвестная команда: {A}");
        }
    }

    private void SaveMemoryToYaml(string filePath)
    {
        Console.WriteLine("Сохраняем память в YAML...");
        var memoryDump = new List<string>();
        for (int i = 0; i < Memory.Length; i++)
        {
            if (Memory[i] != 0)
            {
                memoryDump.Add($"- address: {i}\n  value: {Memory[i]}");
                Console.WriteLine($"Memory[{i}] = {Memory[i]}");
            }
        }

        if (memoryDump.Count == 0)
        {
            Console.WriteLine("Память пуста. Нечего сохранять.");
        }
        else
        {
            File.WriteAllText(filePath, string.Join("\n", memoryDump));
        }
    }


}
