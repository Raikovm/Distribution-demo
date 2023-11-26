namespace WpfApp;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        string[] fileNames = {
            "V01-V2-N1.txt",
            // "V01-V2-N2.txt", 
            // "V01-V2-N5.txt",
            // "V01-V2-N10.txt",
            // "V01-V2-N25.txt",
            // "V01-V2-N50.txt"
        };

        var result = fileNames.Select(x =>
        {
            IList<double> dataPoints = DataReader.ReadData(x);

            (double mean, double variance) = MomentsMethod(dataPoints);
            return new
            {
                dataset = x,
                expectedValue = mean,
                dispersion = variance,
            };
        }).ToList();
        
        CartesianChart chart = new();
        foreach (string fileName in fileNames)
        {
            IList<double> dataPoints = DataReader.ReadData(fileName);
            
            LineSeries series = new()
            {
                Title = fileName,
                Values = new ChartValues<double>(dataPoints),
                PointGeometrySize = 10
            };
            
            chart.Series.Add(series);
        }
        
        Content = chart;
    }

    public static (double mean, double variance) MomentsMethod(IList<double> probabilities)
    {
        double mean = probabilities.Select((t, i) => i * t).Sum();
        double variance = probabilities.Select((t, i) => t * Math.Pow(i - mean, 2)).Sum();

        return (mean, Math.Pow(variance, 2));
    }
}