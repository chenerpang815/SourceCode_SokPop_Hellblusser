using System.Collections.Generic;
using System;
using UnityEngine;

public class WeightedRandomBag<T>
{
    private struct Entry
    {
        public float accumulatedWeight;
        public T item;
    }

    private List<Entry> entries = new List<Entry>();
    private float accumulatedWeight;

    public void AddEntry(T item, float weight)
    {
        accumulatedWeight += weight;
        entries.Add(new Entry { item = item, accumulatedWeight = accumulatedWeight });
    }

    public T Choose ()
    {
        //int r = Mathf.FloorToInt(TommieRandom.instance.RandomRange(0,entries.Count - 1));
        //return entries[r].item;

        float r = TommieRandom.instance.RandomValue(1f) * accumulatedWeight; //TommieRandom.instance.RandomValue(1f) * accumulatedWeight;

        foreach (Entry entry in entries)
        {
            if (entry.accumulatedWeight >= r)
            {
                return entry.item;
            }
        }
        return default(T); //should only happen when there are no entries
    }

    public T ChooseGeneration ( string _log )
    {
        //int r = Mathf.FloorToInt(TommieRandom.instance.GenerationRandomRange(0, entries.Count - 1,"random pick_" + _log));
        //return entries[r].item;

        float r = TommieRandom.instance.GenerationRandomValue(1f, "weightedRandomBag_" + _log) * accumulatedWeight; //TommieRandom.instance.RandomValue(1f) * accumulatedWeight;

        foreach (Entry entry in entries)
        {
            if (entry.accumulatedWeight >= r)
            {
                return entry.item;
            }
        }
        return default(T); //should only happen when there are no entries
    }
}