using System.IO;
using System.Text;

namespace AsyncTaskFile
{
    internal class Program
    {
        static async Task Main()
        {
            List<string> args1 = new List<string> { "1", "2","3","6","10" };
            List<string> args2 = new List<string> { "a", "b", "s", "6sd", "10_15" };
            string dir1 = "c:\\Otus\\TestDir1";
            string dir2 = "c:\\Otus\\TestDir2";

            await CreateFiles(dir1, args1);
            ReadFiles(dir1);

            await CreateFiles(dir2, args2);
            ReadFiles(dir2);
        }
        static void ReadFiles(string dir)
        {
            foreach (string filePath in Directory.GetFiles(dir, "*.txt"))
            {
                try
                {
                    string content = File.ReadAllText(filePath, Encoding.UTF8);
                    Console.WriteLine($"{Path.GetFileName(filePath)}: {content}");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"Файл не найден: {filePath}");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Нет доступа к файлу: {filePath}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Ошибка чтения: {filePath} - {ex.Message}");
                }
            }
        }
        static async Task CreateFiles(string dir, List<string> args) 
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (!dirInfo.Exists) dirInfo.Create();

            foreach (string arg in args)
            {
                string filePath = Path.Combine(dir, $"File{arg}.txt");

                try
                {
                    // Создаёт или перезаписывает файл
                    await File.WriteAllTextAsync(filePath, $"{Path.GetFileName(filePath)}: {DateTime.Now}\n", Encoding.UTF8);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Ошибка доступа: {filePath}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Ошибка ввода-вывода: {filePath} - {ex.Message}");
                }
            }
        }
    }
}
