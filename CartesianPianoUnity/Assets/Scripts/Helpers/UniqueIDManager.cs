using UnityEngine;
using System.Collections;

public static class UniqueIDManager
{
    private static int next_id = 0;

    public static int NextID()
    {
        return next_id++;
    }
}

public class UID
{
    public int Value { get; private set; }
    public UID()
    {
        Value = UniqueIDManager.NextID();
    }
    public override string ToString()
    {
        return Value.ToString();
    }
}
