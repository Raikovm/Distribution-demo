namespace WpfApp.Models;

public record Result(string DataSetName, double ExpectedValue, double Variance, double StdDev, Range Range);

public record Range(double Start, double End);