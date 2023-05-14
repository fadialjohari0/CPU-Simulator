using Newtonsoft.Json;

namespace CPU
{
    interface IFileReader
    {
        List<Task> ReadTasksFromJson(string filePath);
    }

    public class TasksLoader : IFileReader
    {
        public List<Task> ReadTasksFromJson(string filePath)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<Task>>(fileContent);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File not found: {filePath}");
            }
            catch (JsonException)
            {
                Console.WriteLine($"Invalid JSON format in file: {filePath}");
            }

            return new List<Task>();
        }
    }
}