using System.Text.RegularExpressions;
using Godot;
using Godot.Collections;
// ReSharper disable StringLiteralTypo

namespace CastingWaver;

public partial class HexPattern : Node
{
    public static bool IsWorld3D = true;
    public static readonly System.Collections.Generic.Dictionary<string, Spell> Patterns = new()
    {
        //打印字符串
        {"W", new Spell(() => {GD.Print("Short Line");})},
        {"QQQQQ",new Spell(() =>
        {
            SpellStack.PushStack(() => IsWorld3D ? Vector3.Zero : Vector2.Zero);
        })},
        { "DEDQ", new Spell(() => 
        {   
            SpellStack.PushStack(() => false);
        })},
        { "AEAQ", new Spell(() => 
        {   
            SpellStack.PushStack(() => false);
        })},
        //压入一个打印元素到栈顶
        {"D",new Spell(() =>
        {
            GD.Print("PushStack");
            SpellStack.PushStack(() => { GD.Print("PopFromStack"); });
        })},
        //弹出栈顶的元素
        {"A", new Spell(SpellStack.PopStack)},
        //打印栈顶的元素
        {"AQA", new Spell(() =>
        {
            var d = SpellStack.PopStack();
            if (d.VariantType == Variant.Type.String && d.AsString() == "OutOfStack") return;
            GD.Print(d);
            SpellStack.PushStack(() => d);
        })},
        {"EQQQQQ",new Spell(() =>
        {
            var z = SpellStack.PopStack();
            var y = SpellStack.PopStack();
            var x = SpellStack.PopStack();
            SpellStack.PushStack(() => new Vector3(x.AsSingle(), y.AsSingle(), z.AsSingle()));
        })}
    };

    public override void _Ready()
    {
        Patterns.Add("QAQ", new Spell(() =>
        {
            GD.Print("Push player into stack");
            if(GetTree().GetNodeCountInGroup("Player") == 0) return;
            SpellStack.PushStack(() => GetTree().GetNodesInGroup("Player")[0].GetPath());
        }));
        Patterns.Add("AWQQQWAQW", new Spell(() =>
        {
            GD.Print("Gave player an impulse");
            var d2 = SpellStack.PopStack();
            var d1 = SpellStack.PopStack();
            if(IsWorld3D)
            {
                if (d2.VariantType != Variant.Type.Vector3 || d1.VariantType != Variant.Type.NodePath)
                {
                    GD.PrintErr("Invalid stack");
                    return;
                }
                if (GetNode(d1.AsNodePath()) is RigidBody3D body)
                {
                    body.ApplyCentralImpulse(d2.AsVector3() * 20f);
                }
            }
            else
            {
                if (d2.VariantType != Variant.Type.Vector2 || d1.VariantType != Variant.Type.NodePath)
                {
                    GD.PrintErr("Invalid stack");
                    return;
                }
                if (GetNode(d1.AsNodePath()) is RigidBody2D body)
                {
                    body.ApplyCentralImpulse(d2.AsVector2() * 40f);
                }
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
            //输入立即数
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

                SpellStack.PushStack(Callable.From(() => num));
                return;
            }
            GD.Print($"Invalid pattern: {pattern}");
        }
    }
}