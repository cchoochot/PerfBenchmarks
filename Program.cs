using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.InteropServices;

public class Program
{
    public static void Main(string[] args) => BenchmarkRunner.Run<TradingBenchmarks>();
}

[MemoryDiagnoser] // Tracks GC allocations
public class TradingBenchmarks
{
    private float[] _data;

    [Params(1000, 100000)] // Test with small and large datasets
    public int Size;

    [GlobalSetup]
    public void Setup()
    {
        _data = new float[Size];
        Random.Shared.NextBytes(MemoryMarshal.AsBytes(_data.AsSpan()));
    }

    [Benchmark(Baseline = true)]
    public float StandardSum()
    {
        float sum = 0;
        for (int i = 0; i < _data.Length; i++)
        {
            sum += _data[i];
        }
        return sum;
    }

    [Benchmark]
    public float VectorizedSum()
    {
        if (!Avx.IsSupported) return StandardSum();

        var vSum = Vector256<float>.Zero;
        var span = _data.AsSpan();
        int i = 0;

        // Process 8 floats at a time using CPU registers
        for (; i <= span.Length - 8; i += 8)
        {
            var vector = Vector256.Create(span.Slice(i, 8));
            vSum = Avx.Add(vSum, vector);
        }

        float total = 0;
        for (int j = 0; j < 8; j++) total += vSum.GetElement(j);

        // Clean up remaining elements
        for (; i < span.Length; i++) total += span[i];

        return total;
    }

    [Benchmark]
    public float VectorizedSum16()
    {
        if (!Avx.IsSupported) return StandardSum();

        int factor = 8;

        var vSum = Vector256<float>.Zero;
        var span = _data.AsSpan();
        int i = 0;

        // Process <factor> floats at a time using CPU registers
        for (; i <= span.Length - factor; i += factor)
        {
            var vector = Vector256.Create(span.Slice(i, factor));
            vSum = Avx2.Add(vSum, vector);
        }

        float total = 0;
        for (int j = 0; j < factor; j++) total += vSum.GetElement(j);

        // Clean up remaining elements
        for (; i < span.Length; i++) total += span[i];

        return total;
    }

    [Benchmark]
    public float VectorizedSumPlainLogic()
    {
        if (!Avx.IsSupported) return StandardSum();

        int factor = 8;

        // var vSum = Vector256<float>.Zero;
        var span = _data.AsSpan();
        int i = 0;

        float sum = 0f;

        // Process <factor> floats at a time using CPU registers
        for (; i <= span.Length - factor; i += factor)
        {
            sum += span[i] + span[i + 1] + span[i + 2] + span[i + 3]
                 + span[i + 4] + span[i + 5] + span[i + 6] + span[i + 7];
        }

        // Clean up remaining elements
        for (; i < span.Length; i++)
        {
            sum += span[i];
        }

        return sum;
    }
}