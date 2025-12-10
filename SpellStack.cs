using System.Collections.Generic;
using Godot;

namespace CastingWaver;

public partial class SpellStack : Node
{
    private static readonly Stack<Callable> Stack = [];
    public static void PushStack(Callable spell) => Stack.Push(spell);

    public static Variant PopStack()
    {
        return Stack.Count == 0 ? false : Stack.Pop().Call();
    }

    public static void Clear() => Stack.Clear();
    
}