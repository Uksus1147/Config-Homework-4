using NUnit.Framework;
using NUnit.Framework.Internal;

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
        File.WriteAllText("source.asm", "LOAD_CONST 0 42");
        var assembler = new Assembler();

        assembler.Assemble("source.asm", "source.bin", "source.log");
        var interpreter = new Interpreter();
        interpreter.Execute("source.bin", "result.yaml");

        Assert.That(interpreter.Registers[0], Is.EqualTo(42));
    }

    [Test]
    public void ExecuteCommand_ReadMemory_Success()
    {
        File.WriteAllText("source.asm", @"
LOAD_CONST 0 42
LOAD_CONST 1 0
WRITE_MEMORY 1 0
LOAD_CONST 0 11
READ_MEMORY 0 1 0
".Trim());
        var assembler = new Assembler();

        assembler.Assemble("source.asm", "source.bin", "source.log");
        var interpreter = new Interpreter();
        interpreter.Execute("source.bin", "result.yaml");

        Assert.That(interpreter.Registers[0], Is.EqualTo(42));
    }



    [Test]
    public void ExecuteCommand_WriteMemory_ShouldWriteToMemory()
    {
        File.WriteAllText("source.asm", @"
LOAD_CONST 0 42
LOAD_CONST 1 0
WRITE_MEMORY 1 0
".Trim());
        var assembler = new Assembler();

        assembler.Assemble("source.asm", "source.bin", "source.log");
        var interpreter = new Interpreter();
        interpreter.Execute("source.bin", "result.yaml");

        Assert.That(interpreter.Memory[0], Is.EqualTo(42));
    }

    [Test]
    public void ExecuteCommand_BitwiseShiftRight_ShouldShiftRight()
    {
        File.WriteAllText("source.asm", @"
LOAD_CONST 0 1
LOAD_CONST 1 1
WRITE_MEMORY 1 1
LOAD_CONST 0 42
LOAD_CONST 1 1
SHIFT_RIGHT 1 0 0
".Trim());
        var assembler = new Assembler();

        assembler.Assemble("source.asm", "source.bin", "source.log");
        var interpreter = new Interpreter();
        interpreter.Execute("source.bin", "result.yaml");

        Assert.That(interpreter.Memory[0], Is.EqualTo(21));

    }

    [Test]
    public void TestingProgramm()
    {
        File.WriteAllText("source.asm", @"
LOAD_CONST 0 2
LOAD_CONST 1 0
WRITE_MEMORY 1 0
LOAD_CONST 0 4
LOAD_CONST 1 1
WRITE_MEMORY 1 0
LOAD_CONST 0 8
LOAD_CONST 1 2
WRITE_MEMORY 1 0
LOAD_CONST 0 16
LOAD_CONST 1 3
WRITE_MEMORY 1 0
LOAD_CONST 0 1
LOAD_CONST 1 4
WRITE_MEMORY 1 0
LOAD_CONST 0 1
LOAD_CONST 1 5
WRITE_MEMORY 1 0
LOAD_CONST 0 1
LOAD_CONST 1 6
WRITE_MEMORY 1 0
LOAD_CONST 0 1
LOAD_CONST 1 7
WRITE_MEMORY 1 0
LOAD_CONST 1 0
READ_MEMORY 0 1 0
LOAD_CONST 1 4
SHIFT_RIGHT 1 8 0
LOAD_CONST 1 1
READ_MEMORY 0 1 0
LOAD_CONST 1 5
SHIFT_RIGHT 1 9 0
LOAD_CONST 1 2
READ_MEMORY 0 1 0
LOAD_CONST 1 6
SHIFT_RIGHT 1 10 0
LOAD_CONST 1 3
READ_MEMORY 0 1 0
LOAD_CONST 1 7
SHIFT_RIGHT 1 11 0
".Trim());
        var assembler = new Assembler();

        assembler.Assemble("source.asm", "source.bin", "source.log");
        var interpreter = new Interpreter();
        interpreter.Execute("source.bin", "result.yaml");
        var mer = interpreter.Memory;
    }

}

[TestFixture]
public class AssemblerTests
{
    private Assembler _assembler;

    [SetUp]
    public void Setup()
    {
        _assembler = new Assembler();
    }

    [Test]
    public void LoadConst_ShouldEncodeCorrectly()
    {

        // Arrange
        Command command = new Command
        {
            CommandName = "LOAD_CONST",
            A = 121,
            B = 2,
            C = 114
        };

        // Act
        byte[] result = Load.Serialize(command);
        var test = Load.DeSerialize(result);

        // Assert
        Assert.That(result, Is.EqualTo(new byte[] { 0x79, 0xC9, 0x01, 0x00, 0x00 }),
            "LOAD_CONST должен кодироваться как 0x79, 0xC9, 0x01");
        Assert.That(test.A, Is.EqualTo(121));
        Assert.That(test.B, Is.EqualTo(2));
        Assert.That(test.C, Is.EqualTo(114));
    }

    [Test]
    public void ReadConst_ShouldEncodeCorrectly()
    {
        // Arrange
        Command command = new Command
        {
            CommandName = "READ_CONST",
            A = 113,
            B = 3,
            C = 6,
            D = 12
        };

        // Act
        byte[] result = Read.Serialize(command);
        var test = Read.DeSerialize(result);

        // Assert
        Assert.That(result, Is.EqualTo(new byte[] { 0xF1, 0x99, 0x01, 0x00, 0x00 }));
        Assert.That(test.A, Is.EqualTo(113));
        Assert.That(test.B, Is.EqualTo(3));
        Assert.That(test.C, Is.EqualTo(6));
        Assert.That(test.D, Is.EqualTo(12));
    }

    [Test]
    public void WriteConst_ShouldEncodeCorrectly()
    {
        // Arrange
        Command command = new Command
        {
            CommandName = "WRITE_CONST",
            A = 36,
            B = 4,
            C = 7
        };

        // Act
        byte[] result = Write.Serialize(command);
        var test = Write.DeSerialize(result);

        // Assert
        Assert.That(result, Is.EqualTo(new byte[] { 0x24, 0x1E, 0x00, 0x00, 0x00 }));
        Assert.That(test.A, Is.EqualTo(36));
        Assert.That(test.B, Is.EqualTo(4));
        Assert.That(test.C, Is.EqualTo(7));
    }

    [Test]
    public void BitwiseShiftRight_ShouldEncodeCorrectly()
    {
        // Arrange
        Command command = new Command
        {
            CommandName = "BITWISE_SHIFT_RIGHT",
            A = 66, // 0x8E
            B = 4,
            C = 291,
            D = 3
        };

        // Act

        byte[] result = ShiftRight.Serialize(command);
        var test = ShiftRight.DeSerialize(result);

        // Assert
        Assert.That(result, Is.EqualTo(new byte[] {0x42, 0x8E, 0x04, 0x06, 0x00 }));
        Assert.That(test.A, Is.EqualTo(66));
        Assert.That(test.B, Is.EqualTo(4));
        Assert.That(test.C, Is.EqualTo(291));
        Assert.That(test.D, Is.EqualTo(3));
    }

}

