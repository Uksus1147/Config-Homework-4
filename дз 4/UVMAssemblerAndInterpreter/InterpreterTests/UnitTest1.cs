using NUnit.Framework;

[TestFixture]
public class InterpreterTests
{
    private Interpreter _interpreter;

    [SetUp]
    public void Setup()
    {
        _interpreter = new Interpreter();
    }

    [Test]
    public void TestLoadConstCommand()
    {
        var interpreter = new Interpreter();

        byte[] command = new byte[] { 0x79, 0x01, 0x00, 0x2A, 0x00 }; 
        interpreter.ExecuteCommand(command);

        Assert.AreEqual(42, interpreter.Registers[2]);
    }

    [Test]
    public void ExecuteCommand_ReadMemory_Success()
    {
        _interpreter.Memory[10] = 12345; 
        _interpreter.Registers[2] = 10; 

        byte[] commandBytes = new byte[]
        {
        113, 0, 0, 2, 0  
        };

        _interpreter.ExecuteCommand(commandBytes);

        Assert.AreEqual(12345, _interpreter.Registers[0], "Регистр B должен содержать значение из памяти по адресу C.");
    }



    [Test]
    public void ExecuteCommand_WriteMemory_ShouldWriteToMemory()
    {
        _interpreter.Registers[0] = 10; 
        _interpreter.Registers[1] = 1234; 

        byte[] command = new byte[] { 36, 0, 0, 1, 0 }; 
        _interpreter.ExecuteCommand(command);

        Assert.AreEqual(1234, _interpreter.Memory[10]);
    }


    [Test]
    public void ExecuteCommand_BitwiseShiftRight_ShouldShiftRight()
    {
        _interpreter.Registers[0] = 16; 

        byte[] command = new byte[] { 66, 0, 0, 0, 0 }; 
        _interpreter.ExecuteCommand(command);

        Assert.AreEqual(8, _interpreter.Registers[0]); 
    }
}
