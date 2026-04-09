using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

const string outputDirectory = "output";
const string jsonPath = "output/library-data.json";
const string xmlPath = "output/library-data.xml";

Directory.CreateDirectory(outputDirectory);

var sourceData = DemoDataFactory.Create();

SerializerService.ExportJson(jsonPath, sourceData);
SerializerService.ExportXml(xmlPath, sourceData);

var fromJson = SerializerService.ImportJson(jsonPath);
var fromXml = SerializerService.ImportXml(xmlPath);

var jsonFileSize = new FileInfo(jsonPath).Length;
var xmlFileSize = new FileInfo(xmlPath).Length;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("ЛР7: Сериализация и десериализация\n");

Console.WriteLine($"JSON экспортирован: {Path.GetFullPath(jsonPath)}");
Console.WriteLine($"XML экспортирован:  {Path.GetFullPath(xmlPath)}\n");

Console.WriteLine($"JSON: книг {fromJson.Books.Count}, авторов {fromJson.Authors.Count}, жанров {fromJson.Genres.Count}");
Console.WriteLine($"XML:  книг {fromXml.Books.Count}, авторов {fromXml.Authors.Count}, жанров {fromXml.Genres.Count}\n");

Console.WriteLine("Сравнение размеров:");
Console.WriteLine($"- JSON: {jsonFileSize} bytes");
Console.WriteLine($"- XML:  {xmlFileSize} bytes");

if (jsonFileSize < xmlFileSize)
{
    Console.WriteLine("Вывод: JSON компактнее для этого набора данных.");
}
else if (jsonFileSize > xmlFileSize)
{
    Console.WriteLine("Вывод: XML компактнее для этого набора данных.");
}
else
{
    Console.WriteLine("Вывод: размеры JSON и XML совпали.");
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
