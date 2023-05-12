using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Timing
{
    private static readonly Dictionary<string, List<double>> timing = new();

    private static IEnumerator Print(string identifier)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);

            Debug.Log($"{identifier} has {timing[identifier].Count} interval(s) per second");
        }
    }

    public static void Interval(MonoBehaviour mb, string identifier)
    {
        if (!timing.TryGetValue(identifier, out var times))
        {
            times = new();
            timing[identifier] = times;

            mb.StartCoroutine(Print(identifier));
        }

        double time = Time.unscaledTimeAsDouble;
        times.Add(time);
        times.RemoveAll(t => t < time - 1);
    }
}