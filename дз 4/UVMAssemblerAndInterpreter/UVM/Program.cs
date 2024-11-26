using System;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Использование: UVMAssemblerAndInterpreter <source.yaml> <output.bin> <log.yaml> <result.yaml>");
                return;
            }

            string sourceFilePath = args[0];
            string binaryOutputPath = args[1];
            string logOutputPath = args[2];
            string resultFilePath = args[3];

            var assembler = new Assembler();
            assembler.Assemble(sourceFilePath, binaryOutputPath, logOutputPath);

            var interpreter = new Interpreter();
            interpreter.Execute(binaryOutputPath, resultFilePath);

            Console.WriteLine("Готово! Выполнено ассемблирование и интерпретация.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine("Нажмите Enter для завершения.");
        Console.ReadLine();
    }
}
