using Godot;
using Godot.Collections;

namespace CastingWaver;

public static class HexPattern
{
    public static readonly Dictionary<string, Callable> Patterns = new()
    {
        {"WWW", Callable.From(() => GD.Print("Line"))},
        {"QAQ", Callable.From(() => GD.Print("Diamond"))},
    };
    public static void Cast(string pattern)
    {
        if(Patterns.TryGetValue(pattern, out var action))   
            action.Call();
        else
        {
            GD.Print($"Invalid pattern: {pattern}");
        }
    }
}