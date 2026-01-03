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
    private float[] _data = null!;

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
    public float VectorizedSumAvx()
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
    public float VectorizedSumAvx2()
    {
        if (!Avx2.IsSupported) return StandardSum();

        var vSum = Vector256<float>.Zero;
        var span = _data.AsSpan();
        int i = 0;

        // Process 8 floats at a time using CPU registers
        for (; i <= span.Length - 8; i += 8)
        {
            var vector = Vector256.Create(span.Slice(i, 8));
            vSum = Avx2.Add(vSum, vector);
        }

        float total = 0;
        for (int j = 0; j < 8; j++) total += vSum.GetElement(j);

        // Clean up remaining elements
        for (; i < span.Length; i++) total += span[i];

        return total;
    }

    // [Benchmark]
    public float VectorizedSumPlainLogic()
    {
        float sum = 0;

        int i = 0;
        int len = _data.Length;
        for (; i < len; i += 8)
        {
            sum += _data[i]
                    + _data[i + 1]
                    + _data[i + 2]
                    + _data[i + 3]
                    + _data[i + 4]
                    + _data[i + 5]
                    + _data[i + 6]
                    + _data[i + 7];
        }

        // Clean up remaining elements
        for (; i < len; i++)
        {
            sum += _data[i];
        }

        return sum;
    }

    // P/Invoke declaration for the Zig function

    [DllImport("engine", CallingConvention = CallingConvention.Cdecl)]
    private static extern float vectorizedSum16(float[] data, nuint len);

    public static float GetFastSum(float[] data)
    {
        // We pass the array and its length to the Zig engine
        return vectorizedSum16(data, (nuint)data.Length);
    }

    [Benchmark]
    public float ZigLib()
    {
        return GetFastSum(_data);
    }
}