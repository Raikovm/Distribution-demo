using LiveCharts.Defaults;

namespace WpfApp;

public partial class MainWindow
{
    public ObservableCollection<string> FileNames { get; }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        FileNames = new(DataReader.GetFileNames());
    }

    private void DatasetComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ICollection<string> selectedDatasets = DatasetComboBox.SelectedItem is string selectedItem
            ? new[] { selectedItem }
            : Array.Empty<string>();

        DrawCharts(selectedDatasets);
    }

    private void DrawCharts(ICollection<string> selectedDatasets)
    {
        Chart.Series.Clear();

        var result = selectedDatasets.Select(x =>
        {
            IList<double> dataPoints = DataReader.ReadData(x);

            (double mean, double stdDev, double variance) = MomentsMethod(dataPoints);
            return new
            {
                dataset = x,
                mean,
                variance,
                stdDev,
                Range = (Start: 0, end: dataPoints.Count - 1)
            };
        }).ToList();

        StringBuilder stringBuilder = new();
        result.ForEach(x =>
        {
            string s = $"""
                        Dataset = {x.dataset}
                        μ = {$"{x.mean:N4}"}
                        σ = {$"{x.stdDev:N4}"}
                        σ^2 = {$"{x.variance:N4}"}
                        """;
            stringBuilder.Append(s);
        });
        ResultsTextBlock.Text = stringBuilder.ToString();

        foreach (string fileName in selectedDatasets)
        {
            IList<double> dataPoints = DataReader.ReadData(fileName);

            LineSeries series = new()
            {
                Title = fileName,
                Values = new ChartValues<double>(dataPoints),
                PointGeometry = null,
            };

            Chart.Series.Add(series);
        }

        foreach (var data in result)
        {
            ChartValues<ObservablePoint> observablePoints = new();
            for (double x = data.Range.Start; x <= data.Range.end; x += 0.1)
            {
                double y = NormalDistribution(x, data.mean, Math.Sqrt(data.stdDev));
                observablePoints.Add(new ObservablePoint(x, y));
            }

            LineSeries lineSeries = new()
            {
                Title = $"{data.dataset} distribution",
                Values = observablePoints,
                PointGeometry = null,
            };

            Chart.Series.Add(lineSeries);
        }
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
