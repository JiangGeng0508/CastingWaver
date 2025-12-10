using System.Collections.Generic;
using Godot;

namespace CastingWaver;

public static class HexPattern
{
    public static readonly Dictionary<string, Spell> Patterns = new()
    {
        {"W", new Spell(() => {GD.Print("Short Line");})},
        {"QAQ", new Spell(() => {GD.Print("Diamond");})},
        {"EA",new Spell(() =>
        {
            GD.Print("PushStack");
            SpellStack.PushStack(Callable.From(() => { GD.Print("PopFromStack"); }));
        })},
        {"A", new Spell(SpellStack.PopStack)}
    };
    public static void Cast(string pattern)
    {
        if(Patterns.TryGetValue(pattern, out var spell))   
            spell.Execute();
        else
        {
            GD.Print($"Invalid pattern: {pattern}");
        }
    }
}