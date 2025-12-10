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
        {"A", new Spell(SpellStack.PopStack)},
        {"ADA", new Spell(() => {GD.Print(SpellStack.PopStack());})}
    };

    public const string NumPrefix = "AQAA";

    public static void Cast(string pattern)
    {
        if(Patterns.TryGetValue(pattern, out var spell))   
            spell.Execute();
        else
        {
            if (pattern.StartsWith(NumPrefix))
            {
                var str = pattern[NumPrefix.Length..];
                var num = 0f;
                foreach (var t in str)
                {
                    switch (t)
                    {
                        case 'W':
                            num += 1;
                            break;
                        case 'Q':
                            num += 5;
                            break;
                        case 'A':
                            num *= 2;
                            break;
                        case 'E':
                            num += 10;
                            break;
                        case 'D':
                            num /= 2;
                            break;
                    }
                }

                SpellStack.PushStack(Callable.From(() =>num));
                return;
            }
            GD.Print($"Invalid pattern: {pattern}");
        }
    }
}