using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

const string outputDirectory = "output";
const string jsonPath = "output/library-data.json";
const string xmlPath = "output/library-data.xml";

Directory.CreateDirectory(outputDirectory);

var sourceData = DemoDataFactory.Create();

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("ЛР7: Сериализация и десериализация");

while (true)
{
    Console.WriteLine("\n--- МЕНЮ ---");
    Console.WriteLine("1. Сериализовать (сохранить) в JSON");
    Console.WriteLine("2. Сериализовать (сохранить) в XML");
    Console.WriteLine("3. Десериализовать (прочитать) из JSON");
    Console.WriteLine("4. Десериализовать (прочитать) из XML");
    Console.WriteLine("5. Сравнить размеры файлов");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите действие: ");

    var choice = Console.ReadLine();
    if (choice == "0") break;

    switch (choice)
    {
        case "1":
            SerializerService.ExportJson(jsonPath, sourceData);
            Console.WriteLine($"[Успех] Данные сохранены в {Path.GetFullPath(jsonPath)}");
            break;
        case "2":
            SerializerService.ExportXml(xmlPath, sourceData);
            Console.WriteLine($"[Успех] Данные сохранены в {Path.GetFullPath(xmlPath)}");
            break;
        case "3":
            if (File.Exists(jsonPath))
            {
                var fromJson = SerializerService.ImportJson(jsonPath);
                Console.WriteLine($"[Прочитано JSON] Книг: {fromJson.Books.Count}, Авторов: {fromJson.Authors.Count}, Жанров: {fromJson.Genres.Count}");
            }
            else
            {
                Console.WriteLine("[Ошибка] Файл JSON не найден. Сначала сохраните данные.");
            }
            break;
        case "4":
            if (File.Exists(xmlPath))
            {
                var fromXml = SerializerService.ImportXml(xmlPath);
                Console.WriteLine($"[Прочитано XML] Книг: {fromXml.Books.Count}, Авторов: {fromXml.Authors.Count}, Жанров: {fromXml.Genres.Count}");
            }
            else
            {
                Console.WriteLine("[Ошибка] Файл XML не найден. Сначала сохраните данные.");
            }
            break;
        case "5":
            if (File.Exists(jsonPath) && File.Exists(xmlPath))
            {
                var jsonFileSize = new FileInfo(jsonPath).Length;
                var xmlFileSize = new FileInfo(xmlPath).Length;

                Console.WriteLine($"- JSON: {jsonFileSize} bytes");
                Console.WriteLine($"- XML:  {xmlFileSize} bytes");

                if (jsonFileSize < xmlFileSize)
                    Console.WriteLine("Вывод: JSON компактнее для этого набора данных.");
                else if (jsonFileSize > xmlFileSize)
                    Console.WriteLine("Вывод: XML компактнее для этого набора данных.");
                else
                    Console.WriteLine("Вывод: размеры JSON и XML совпали.");
            }
            else
            {
                Console.WriteLine("[Ошибка] Для сравнения размеров оба файла (JSON и XML) должны быть созданы (пункты 1 и 2).");
            }
            break;
        default:
            Console.WriteLine("[Ошибка] Неизвестная команда.");
            break;
    }
}

public static class SerializerService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static void ExportJson(string path, LibraryExportModel data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        File.WriteAllText(path, json, Encoding.UTF8);
    }

    public static LibraryExportModel ImportJson(string path)
    {
        var json = File.ReadAllText(path, Encoding.UTF8);
        return JsonSerializer.Deserialize<LibraryExportModel>(json, JsonOptions) ?? new LibraryExportModel();
    }

    public static void ExportXml(string path, LibraryExportModel data)
    {
        var serializer = new XmlSerializer(typeof(LibraryExportModel));
        using var stream = File.Create(path);
        serializer.Serialize(stream, data);
    }

    public static LibraryExportModel ImportXml(string path)
    {
        var serializer = new XmlSerializer(typeof(LibraryExportModel));
        using var stream = File.OpenRead(path);
        return (LibraryExportModel)(serializer.Deserialize(stream) ?? new LibraryExportModel());
    }
}

[XmlRoot("library")]
public sealed class LibraryExportModel
{
    [XmlArray("authors")]
    [XmlArrayItem("author")]
    public List<AuthorExportModel> Authors { get; set; } = [];

    [XmlArray("genres")]
    [XmlArrayItem("genre")]
    public List<GenreExportModel> Genres { get; set; } = [];

    [XmlArray("books")]
    [XmlArrayItem("book")]
    public List<BookExportModel> Books { get; set; } = [];
}

public sealed class AuthorExportModel
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlElement("name")]
    public string Name { get; set; } = string.Empty;
}

public sealed class GenreExportModel
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlElement("name")]
    public string Name { get; set; } = string.Empty;
}

public sealed class BookExportModel
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlElement("title")]
    public string Title { get; set; } = string.Empty;

    [XmlElement("publicationYear")]
    public int PublicationYear { get; set; }

    [XmlElement("authorId")]
    public int AuthorId { get; set; }

    [XmlArray("genreIds")]
    [XmlArrayItem("genreId")]
    public List<int> GenreIds { get; set; } = [];
}

public static class DemoDataFactory
{
    public static LibraryExportModel Create()
    {
        return new LibraryExportModel
        {
            Authors =
            [
                new AuthorExportModel { Id = 1, Name = "Isaac Asimov" },
                new AuthorExportModel { Id = 2, Name = "J.R.R. Tolkien" },
                new AuthorExportModel { Id = 3, Name = "George Orwell" }
            ],
            Genres =
            [
                new GenreExportModel { Id = 1, Name = "Sci-Fi" },
                new GenreExportModel { Id = 2, Name = "Fantasy" },
                new GenreExportModel { Id = 3, Name = "Drama" }
            ],
            Books =
            [
                new BookExportModel { Id = 1, Title = "Foundation", PublicationYear = 1951, AuthorId = 1, GenreIds = [1] },
                new BookExportModel { Id = 2, Title = "The Hobbit", PublicationYear = 1937, AuthorId = 2, GenreIds = [2] },
                new BookExportModel { Id = 3, Title = "Animal Farm", PublicationYear = 1945, AuthorId = 3, GenreIds = [3] },
                new BookExportModel { Id = 4, Title = "The Lord of the Rings", PublicationYear = 1954, AuthorId = 2, GenreIds = [2, 3] }
            ]
        };
    }
}
