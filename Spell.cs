using System;
using Godot;

namespace CastingWaver;

public class Spell
{
    private readonly Callable _defaultCall;
    protected static void PushStack(Callable spell) => SpellStack.PushStack(spell);
    protected static Variant? PopStack() => SpellStack.PopStack();

    public virtual void Execute()
    {
        _defaultCall.Call();
    }

    public Spell(Callable call)
    {
        _defaultCall = call;
    }
    public Spell(Action action)
    {
        _defaultCall = Callable.From(action);
    }
    public Spell(Func<Variant> func)
    {
        _defaultCall = Callable.From(func);
    }
}