namespace Optimize
{
    public interface IAnalyzer
    {
        void CalculatePerformanceResults(Parameters parameters, double startCapital, bool showOutput = false);
    }
}
