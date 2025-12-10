using System.Collections.Generic;
using Godot;

namespace CastingWaver;

public partial class HexPattern : Node
{
    public static readonly Dictionary<string, Spell> Patterns = new()
    {
        //打印字符串
        {"W", new Spell(() => {GD.Print("Short Line");})},
        {"QAQ", new Spell(() => {GD.Print("Diamond");})},
        //压入一个打印元素到栈顶
        {"EA",new Spell(() =>
        {
            GD.Print("PushStack");
            SpellStack.PushStack(Callable.From(() => { GD.Print("PopFromStack"); }));
        })},
        //弹出栈顶的元素
        {"A", new Spell(SpellStack.PopStack)},
        //弹出栈顶的元素并打印
        {"ADA", new Spell(() => {GD.Print(SpellStack.PopStack());})}
    };

    public override void _Ready()
    {
        Patterns.Add("AA", new Spell(() =>
        {
            if(GetTree().GetNodeCountInGroup("Player") == 0) return;
            SpellStack.PushStack(() => GetTree().GetNodesInGroup("Player")[0].GetPath());
        }));
        Patterns.Add("WA", new Spell(() =>
        {
            var d = SpellStack.PopStack();
            if (GetNode(d.AsNodePath()) is RigidBody2D body)
            {
                body.ApplyCentralImpulse(Vector2.Right * 200f);
            }
        }));
        Patterns.Add("WD", new Spell(() =>
        {
            var d = SpellStack.PopStack();
            if (GetNode(d.AsNodePath()) is RigidBody2D body)
            {
                body.ApplyCentralImpulse(Vector2.Left * 200f);
            }
        }));
    }

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