using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CastingWaver;

public partial class SpellStackManager : Node
{
    public static readonly Stack<Func<Variant>> SpellStack = [];
    public static void PushStack(Func<Variant> func) => SpellStack.Push(func);
    public static void PushStack(Variant variant)
    {
        if (variant.VariantType == default)
        {
            GD.PrintErr("Variant is null");
            return;
        }
        GD.Print("StackIn: " + variant);
        PushStack(() => variant);
    }

    public static int StackCount() => SpellStack.Count;
    public static Variant PopStack()
    {
        if (SpellStack.Count == 0)
        {
            GD.PrintErr("Stack is null");
            return default;
        }

        var d = SpellStack.Pop().Invoke();
        return d;
    }

    public static void Clear() => SpellStack.Clear();
    public static string PrintStack() => SpellStack.Aggregate("Stack:", (current, func) => current + ("\n" + func.Invoke()));
}