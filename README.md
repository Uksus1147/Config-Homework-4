# Config-Homework-4

Этот проект реализует простой ассемблер и интерпретатор для пользовательского языка ассемблера с использованием YAML файлов для ввода и вывода. Программа читает команды ассемблера из YAML файла, компилирует их в бинарный код, а затем интерпретирует бинарный файл для выполнения команд. Результаты сохраняются в YAML формате.

# Возможности
Ассемблер: Читает команды из YAML файла, компилирует их в бинарный формат и записывает лог.
Интерпретатор: Выполняет скомпилированные бинарные команды и сохраняет финальное состояние памяти в YAML файл.
Логирование: Ассемблер генерирует лог файл в формате YAML для отладки и проверки.
Требования
.NET 6.0 или более поздняя версия
Библиотека YamlDotNet для парсинга и сериализации YAML.

# Использование
Запустите программу с помощью следующих аргументов командной строки:

UVMAssemblerAndInterpreter <source.yaml> <output.bin> <log.yaml> <result.yaml>
Где:

<source.yaml>: YAML файл, содержащий команды ассемблера.
<output.bin>: Путь к бинарному файлу, который будет сгенерирован ассемблером.
<log.yaml>: Путь к лог-файлу, где будут записаны операции ассемблера.
<result.yaml>: YAML файл, в который будет сохранено состояние памяти после интерпретации.

# Классы
# Assembler
Этот класс отвечает за:

Загрузку команд ассемблера из YAML файла.
Компиляцию этих команд в бинарные данные.
Сохранение бинарных данных в .bin файл.
Генерацию лог-файла в формате YAML.

# Interpreter
Этот класс отвечает за:

Загрузку бинарных данных из .bin файла.
Выполнение команд в бинарном формате.
Сохранение финального состояния памяти в .yaml файл.

# Command
Представляет собой одну команду ассемблера, содержащую:

CommandName: Название команды.
A, B, C, D: Операнды для команды.
Юнит-тесты
В проекте включены юнит-тесты для класса Interpreter, которые проверяют правильность выполнения команд. Тесты охватывают следующие команды:

LOAD_CONST: Загружает константу в регистр.
READ_MEMORY: Читает значение из памяти в регистр.
WRITE_MEMORY: Записывает значение из регистра в память.
BITWISE_SHIFT_RIGHT: Выполняет побитовый сдвиг вправо на значении регистра.
