namespace Tracker.Benchmark;

public class ConcatStringExample
{
    public void Run()
    {
        var numbers = Enumerable.Range(start: 1, count: 50_000).ToArray();
        Recorder.Execute(nameof(ConcatString), () => ConcatString(numbers));
        Recorder.Execute(nameof(StringBuilder), () => StringBuilder(numbers));

        // или можно руками вызывать
        // Console.WriteLine("Using string with +");
        // Recorder.Start();
        // ConcatString(numbers);
        // Recorder.Stop();
        //
        // Console.WriteLine("Using StringBuilder");
        // Recorder.Start();
        // StringBuilder(numbers);
        // Recorder.Stop();
    }

    private void StringBuilder(int[] numbers)
    {
        System.Text.StringBuilder builder = new();
        for (var i = 0; i < numbers.Length; i++)
        {
            builder.Append(numbers[i]);
            builder.Append(", ");
        }
    }

    private void ConcatString(int[] numbers)
    {
        var s = string.Empty;
        for (var i = 0; i < numbers.Length; i++)
        {
            s += numbers[i] + ", ";
        }
    }
}
