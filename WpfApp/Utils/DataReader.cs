﻿namespace WpfApp.Utils;

public static class DataReader
{
    private static readonly CultureInfo RuCulture = new("ru-ru");
    
    public static IList<double> ReadData(string fileName)
    {
        string path = $"{Environment.CurrentDirectory}/Data/{fileName}";
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
    
    public static IList<DataPoint> ReadDataPoints(string fileName)
    {
        string path = $"{Environment.CurrentDirectory}/Data/{fileName}";
        using TextFieldParser parser = new(path);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(" ");

        List<DataPoint> probabilities = new();
        string[]? fields = parser.ReadFields();
        while (fields is not null)
        {
            double n = double.Parse(fields[0], RuCulture);
            double probability = double.Parse(fields[1], RuCulture);
            probabilities.Add(new DataPoint(n, probability));
            fields = parser.ReadFields();
        }

        return probabilities;
    }
}