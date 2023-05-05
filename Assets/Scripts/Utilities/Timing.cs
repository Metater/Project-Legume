using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Timing
{
    private static readonly Dictionary<string, List<double>> timing = new();

    private static IEnumerator Print(string name)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);

            Debug.Log($"{name} has {timing[name].Count} interval(s) per second");
        }
    }

    public static void Interval(MonoBehaviour mb, string name)
    {
        if (!timing.TryGetValue(name, out var times))
        {
            times = new();
            timing[name] = times;

            mb.StartCoroutine(Print(name));
        }

        double time = Time.unscaledTimeAsDouble;
        times.Add(time);
        times.RemoveAll(t => t < time - 1);
    }
}