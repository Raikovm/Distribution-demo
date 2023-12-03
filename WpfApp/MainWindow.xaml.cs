namespace WpfApp;

public partial class MainWindow
{
    public ObservableCollection<string> FileNames { get; set; }

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

        List<Result> result = selectedDatasets.Select(x =>
        {
            IList<double> dataPoints = DataReader.ReadData(x);

            (double expectedValue, double stdDev, double variance) = Calculate(dataPoints);
            return new Result
            (
                x,
                expectedValue,
                variance,
                stdDev,
                new(0, dataPoints.Count - 1)
            );
        }).ToList();

        DrawText(result);

        IEnumerable<LineSeries> dataPointsSeries = selectedDatasets.Select(fileName =>
        {
            IList<double> dataPoints = DataReader.ReadData(fileName);

            LineSeries series = new()
            {
                Title = fileName,
                Values = new ChartValues<double>(dataPoints),
                PointGeometry = null,
            };

            return series;
        });

        IEnumerable<LineSeries> normalDistributionSeries = result.Select(data =>
        {
            ChartValues<ObservablePoint> observablePoints = new();
            for (double x = data.Range.Start; x <= data.Range.End; x += 0.1)
            {
                double y = NormalDistribution(x, data.ExpectedValue, Math.Sqrt(data.StdDev));
                observablePoints.Add(new ObservablePoint(x, y));
            }

            LineSeries lineSeries = new()
            {
                Title = $"{data.DataSetName} distribution",
                Values = observablePoints,
                PointGeometry = null,
            };

            return lineSeries;
        });

        Chart.Series.AddRange(dataPointsSeries.Concat(normalDistributionSeries));
    }

    private void DrawText(List<Result> result)
    {

        StringBuilder stringBuilder = new();
        result.ForEach(x =>
        {
            string s = $"""
                        Dataset = {x.DataSetName}
                        μ = {$"{x.ExpectedValue:N4}"}
                        σ = {$"{x.Variance:N4}"}
                        σ^2 = {$"{x.StdDev:N4}"}
                        """;
            stringBuilder.Append(s);
        });
        ResultsTextBlock.Text = stringBuilder.ToString();
    }

    private static (double μ, double σ, double σ2) Calculate(IList<double> probabilities)
    {
        double expectedValue = probabilities.Select((t, i) => i * t).Sum();
        double variance = probabilities.Select((t, i) => t * Math.Pow(i - expectedValue, 2)).Sum();

        return (expectedValue, variance, Math.Pow(variance, 2));
    }

    // f(x) = 1/(σ * √(2π)) * e^(-(x - μ)^2 / (2σ^2))
    private static double NormalDistribution(double x, double expectedValue, double stdDev)
    {
        var factor = 1 / (stdDev * Math.Sqrt(2 * Math.PI));
        return factor * Math.Exp(-Math.Pow(x - expectedValue, 2) / (2 * Math.Pow(stdDev, 2)));
    }
}
