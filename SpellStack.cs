using System;
using System.Collections.Generic;
using Godot;

namespace CastingWaver;

public partial class SpellStack : Node
{
    private static readonly Stack<Callable> Stack = [];
    public static void PushStack(Callable spell) => Stack.Push(spell);
    public static void PushStack(Action action) => Stack.Push(Callable.From(action));
    public static void PushStack(Func<Variant> func) => Stack.Push(Callable.From(func));

    public static Variant PopStack()
    {
        return Stack.Count == 0 ? "OutOfStack" : Stack.Pop().Call();
    }

    public static void Clear() => Stack.Clear();
    
}