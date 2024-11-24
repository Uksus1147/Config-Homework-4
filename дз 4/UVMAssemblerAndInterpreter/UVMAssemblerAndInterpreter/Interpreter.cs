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
        int B = ((commandBytes[0] >> 7) & 0b1) |    
                ((commandBytes[1] & 0b11) << 1);   


        int C = ((commandBytes[1] >> 2) & 0b111111) << 16 |
                ((commandBytes[2] & 0xFF) << 8) |
                (commandBytes[3] & 0xFF);

        Console.WriteLine($"Выполняем команду: A={A}, B={B}, C={C}");

        switch (A)
        {
            case 121:
                if (B < 0 || B >= Registers.Length)
                    throw new IndexOutOfRangeException($"LOAD_CONST: Неверный индекс регистра B={B}");
                Registers[B] = C;
                Console.WriteLine($"LOAD_CONST: Регистр[{B}] = {C}");
                break;

            case 113: 
                if (C < 0 || C >= Registers.Length)
                    throw new IndexOutOfRangeException($"READ_MEMORY: Неверный индекс регистра C={C}");
                int memoryAddress = Registers[C];
                if (memoryAddress < 0 || memoryAddress >= Memory.Length)
                    throw new IndexOutOfRangeException($"READ_MEMORY: Адрес памяти {memoryAddress} выходит за пределы [0, {Memory.Length - 1}]");
                Registers[B] = Memory[memoryAddress];
                Console.WriteLine($"READ_MEMORY: Регистр[{B}] = Memory[{memoryAddress}] ({Memory[memoryAddress]})");
                break;

            case 36: 
                if (B < 0 || B >= Registers.Length)
                    throw new IndexOutOfRangeException($"WRITE_MEMORY: Неверный индекс регистра B={B}");
                if (C < 0 || C >= Registers.Length)
                    throw new IndexOutOfRangeException($"WRITE_MEMORY: Неверный индекс регистра C={C}");
                int writeAddress = Registers[B];
                if (writeAddress < 0 || writeAddress >= Memory.Length)
                    throw new IndexOutOfRangeException($"WRITE_MEMORY: Адрес памяти {writeAddress} выходит за пределы [0, {Memory.Length - 1}]");
                Memory[writeAddress] = Registers[C];
                Console.WriteLine($"WRITE_MEMORY: Memory[{writeAddress}] = {Registers[C]}");
                break;

            case 66: 
                int D = commandBytes.Length > 4 ? commandBytes[4] : 0;
                if (D < 0 || D >= Registers.Length)
                    throw new IndexOutOfRangeException($"BITWISE_SHIFT_RIGHT: Неверный индекс регистра D={D}");
                if (C < 0 || C >= Registers.Length)
                    throw new IndexOutOfRangeException($"BITWISE_SHIFT_RIGHT: Неверный индекс регистра C={C}");
                Registers[C] = Registers[D] >> 1;
                Console.WriteLine($"BITWISE_SHIFT_RIGHT: Регистр[{C}] = Регистр[{D}] >> 1 ({Registers[C]})");
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
