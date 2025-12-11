using System;
using System.Collections.Generic;
using Godot;

namespace CastingWaver;

public partial class SpellStackManager : Node
{
    public static readonly Stack<Callable> SpellStack = [];
    public static void PushStack(Callable spell) => SpellStack.Push(spell);
    public static void PushStack(Action action) => SpellStack.Push(Callable.From(action));
    public static void PushStack(Func<Variant> func) => SpellStack.Push(Callable.From(func));
    public static int StackCount() => SpellStack.Count;
    public static Variant PopStack() => SpellStack.Count == 0 ? default : SpellStack.Pop().Call();

    public static void Clear() => SpellStack.Clear();
    
}