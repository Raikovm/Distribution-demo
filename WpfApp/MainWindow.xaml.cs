using LiveCharts.Defaults;

namespace WpfApp;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        string[] fileNames = {
            // "V01-V2-N1.txt",
            // "V01-V2-N2.txt", 
            // "V01-V2-N5.txt",
            // "V01-V2-N10.txt",
            // "V01-V2-N25.txt",
            "V01-V2-N50.txt"
        };

        var result = fileNames.Select(x =>
        {
            IList<double> dataPoints = DataReader.ReadData(x);

            (double mean, double stddev, double variance) = MomentsMethod(dataPoints);
            return new
            {
                dataset = x,
                mean,
                variance,
                stddev,
                Range = (Start: 0, end: dataPoints.Count)
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
                PointGeometry = null,
            };

            chart.Series.Add(series);
        }


        foreach (var data in result)
        {
            ChartValues<ObservablePoint> observablePoints = new();
            for (double x = data.Range.Start; x <= data.Range.end; x += 0.1)
            {
                double y = NormalDistribution(x, data.mean, Math.Sqrt(data.stddev));
                observablePoints.Add(new ObservablePoint(x, y));
            }

            LineSeries lineSeries = new()
            {
                Title = $"{data.dataset} distribution",
                Values = observablePoints,
                PointGeometry = null,
            };

            chart.Series.Add(lineSeries);
        }

        Content = chart;
    }

    private static (double μ, double σ, double σ2) MomentsMethod(IList<double> probabilities)
    {
        double mean = probabilities.Select((t, i) => i * t).Sum();
        double variance = probabilities.Select((t, i) => t * Math.Pow(i - mean, 2)).Sum();

        return (mean, variance, Math.Pow(variance, 2));
    }

    // f(x) = 1/(σ * √(2π)) * e^(-(x - μ)^2 / (2σ^2))
    private static double NormalDistribution(double x, double mean, double stdDev)
    {
        var factor = 1 / (stdDev * Math.Sqrt(2 * Math.PI));
        return factor * Math.Exp(-Math.Pow(x - mean, 2) / (2 * Math.Pow(stdDev, 2)));
    }

}
