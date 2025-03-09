using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
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
                    using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                    {
                        string content = reader.ReadToEnd();
                        Console.WriteLine($"{Path.GetFileName(filePath)}: {content}");
                    }
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
                    if (DirectoryHasPermission(filePath))
                    {
                        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                        {
                            await writer.WriteLineAsync(Path.GetFileName(filePath));
                        }

                        // Дозаписываем текущую дату
                        using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
                        {
                            await writer.WriteLineAsync(DateTime.Now.ToString());
                        }

                    }
                    else
                    {
                        Console.WriteLine($"Нет прав на запись: {filePath}");
                    }
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

        static bool DirectoryHasPermission(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(filePath);
                AuthorizationRuleCollection rules = dirInfo.GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier)); ;
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (rule.AccessControlType == AccessControlType.Allow &&
                        principal.IsInRole(rule.IdentityReference.Value) &&
                        (rule.FileSystemRights & FileSystemRights.WriteData) == FileSystemRights.WriteData)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка проверки прав доступа: {ex.Message}");
            }
            return false;
        }
    }
}
