using System.IO;

namespace WpfApp.Utils;

public static class DataReader
{
    private static readonly CultureInfo RuCulture = new("ru-ru");
    private static readonly string DataPath = $"{Environment.CurrentDirectory}/Data/";

    public static IList<string> GetFileNames() =>
        Directory.GetFiles(DataPath).Select(Path.GetFileName).ToList()!;

    public static IList<double> ReadData(string fileName)
    {
        string path = $"{DataPath}/{fileName}";
        using TextFieldParser parser = new(path);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(" ");

        List<double> probabilities = new();
        string[]? fields = parser.ReadFields();
        while (fields is not null)
        {
            double probability = double.Parse(fields[1], RuCulture);
            probabilities.Add(probability);
            fields = parser.ReadFields();
        }

        return probabilities;
    }
}