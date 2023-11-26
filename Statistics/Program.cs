using Accord.Statistics.Distributions.Univariate;

IList<double> probabilities = DataReader.ReadData("V01-V2-N1.txt");

double mean = 0;
for (int i = 0; i < probabilities.Count; i++)
{
    mean += i * probabilities[i];
}

double variance = 0;
for (int i = 0; i < probabilities.Count; i++)
{
    variance += probabilities[i] * Math.Pow(i - mean, 2);
}

double normalMean = mean;
double normalVariance = Math.Pow(variance, 2);

NormalDistribution distribution = new(normalMean, variance);

Console.WriteLine("Hello, World!");







double EstimateMean(List<double> data)
{
    // The first moment of the sample is the sample mean
    return CalculateSampleMoment(data, 1);
}

double EstimateVariance(List<double> data, double estimatedMean)
{
    // The second moment of the sample minus the square of the estimated mean
    // gives an estimate for the variance
    return CalculateSampleMoment(data, 2) - Math.Pow(estimatedMean, 2);
}



double CalculateSampleMoment(List<double> data, int k)
{
    double sum = 0;
    foreach (double value in data)
    {
        sum += Math.Pow(value, k);
    }

    return sum / data.Count;
}
