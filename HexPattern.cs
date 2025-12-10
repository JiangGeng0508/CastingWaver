using System;
using Godot;
using static CastingWaver.SpellStack;

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
            PushStack(() => IsWorld3D ? Vector3.Zero : Vector2.Zero);
        })},
        { "DEDQ", new Spell(() => 
        {   
            PushStack(() => false);
        })},
        { "AEAQ", new Spell(() => 
        {   
            PushStack(() => true);
        })},
        //压入一个打印元素到栈顶
        {"D",new Spell(() =>
        {
            GD.Print("PushStackHello");
            PushStack(() => { GD.Print("Hello World!"); });
        })},
        //弹出栈顶的元素
        {"A", new Spell(PopStack)},
        //打印栈顶的元素
        {"AQA", new Spell(() =>
        {
            var d = PopStack();
            if (d.VariantType == Variant.Type.String && d.AsString() == "OutOfStack") return;
            GD.Print(d);
            PushStack(() => d);
        })},
        {"EQQQQQ",new Spell(() =>
        {
            var z = PopStack();
            var y = PopStack();
            var x = PopStack();
            PushStack(() => new Vector3(x.AsSingle(), y.AsSingle(), z.AsSingle()));
        })},
        {"QEEEEE",new Spell(() =>
        {
            var v = PopStack();
            if (v.VariantType != Variant.Type.Vector3)
            {
                GD.PrintErr("Invalid type");
                return;
            }
            var vector = v.AsVector3();
            PushStack(() => vector.X);
            PushStack(()=> vector.Y);
            PushStack(()=> vector.Z);
        })},
        //SUM WAAW
        {"WAAW",new Spell(() =>
        {
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(()=>a.AsVector3()+b.AsVector3());
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(() => b.AsVector3() + Vector3.One * a.AsSingle());
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(() => a.AsVector3() + Vector3.One * b.AsSingle());
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(() => a.AsSingle() + b.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //Sub WDDW
        {"WDDW",new Spell(() =>
        {
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(() => b.AsVector3() - a.AsVector3());
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(() => b.AsVector3() - Vector3.One * a.AsSingle());
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(() => Vector3.One * b.AsSingle() - a.AsVector3());
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(() => b.AsSingle() - a.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //Mul WAQAW
        {"WAQAW",new Spell(()=>{
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(() => b.AsVector3().Dot(a.AsVector3()));
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(() => b.AsVector3() * a.AsSingle());
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(() => b.AsSingle() * a.AsVector3());
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(() => b.AsSingle() * a.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //Sub SDEDW
        {"SDEDW",new Spell(() =>
        {
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(() => b.AsVector3().Cross(a.AsVector3()));
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(() => b.AsVector3() / a.AsSingle());
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(() => new Vector3(
                        b.AsSingle() / a.AsVector3().X,
                        b.AsSingle() / a.AsVector3().Y,
                        b.AsSingle() / a.AsVector3().Z));
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(() => b.AsSingle() / a.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //Abs/Mod WQAQW
        {"WQAQW",new Spell(()=>{
            var a =  PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Vector3:
                    PushStack(() => a.AsVector3().Length());
                    break;
                case Variant.Type.Float:
                    PushStack(() => Mathf.Abs(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //EXP WEDEW
        {"WEDEW",new Spell(() =>
        {
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(() => b.AsVector3() * b.AsVector3().Dot(a.AsVector3()) / a.AsVector3().Length());
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(() => new Vector3(
                        Mathf.Pow(b.AsSingle(),a.AsVector3().X),
                        Mathf.Pow(b.AsSingle(),a.AsVector3().Y),
                        Mathf.Pow(b.AsSingle(),a.AsVector3().Z)));
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(() => new Vector3(
                        Mathf.Pow(b.AsVector3().X,a.AsSingle()),
                        Mathf.Pow(b.AsVector3().Y,a.AsSingle()),
                        Mathf.Pow(b.AsVector3().Z,a.AsSingle())));
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(() => b.AsSingle() * a.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //FLOOR EWQ
        {"EWQ",new Spell(()=>{
            var a =  PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Vector3:
                    PushStack(() => a.AsVector3().Floor());
                    break;
                case Variant.Type.Float:
                    PushStack(() => Mathf.Floor(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }})},
        //UPER QWE
        {"QWE",new Spell(()=>{
            var a =  PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Vector3:
                    PushStack(() => a.AsVector3().Round());
                    break;
                case Variant.Type.Float:
                    PushStack(() => Mathf.Round(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //% ADDWAAD
        {"ADDWAAD",new Spell(() =>
        {
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(() => new Vector3(
                        b.AsVector3().X % a.AsVector3().X,
                        b.AsVector3().Y % a.AsVector3().Y,
                        b.AsVector3().Z % a.AsVector3().Z));
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(() => new Vector3(
                        b.AsSingle() % a.AsVector3().X,
                        b.AsSingle() % a.AsVector3().Y,
                        b.AsSingle() % a.AsVector3().Z));
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(() => new Vector3(
                        b.AsVector3().X % a.AsSingle(),
                        b.AsVector3().Y % a.AsSingle(),
                        b.AsVector3().Z % a.AsSingle()));
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(() => b.AsSingle() % a.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //Signal/Axis QQQQQAWW
        {"QQQQQAWW",new Spell(() =>
        {
            var a =  PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Vector3:
                    PushStack(() => a.AsVector3().MaxAxisIndex() switch
                    {
                        Vector3.Axis.X => Vector3.Right,
                        Vector3.Axis.Y => Vector3.Up,
                        Vector3.Axis.Z => Vector3.Back
                    });
                    break;
                case Variant.Type.Float:
                    PushStack(() => Mathf.Sign(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //Random EQQQ
        {"EQQQ",new Spell(() =>
        {
            PushStack(() =>
            {
                GD.Randomize();
                return GD.Randf();
            });
        })},
    };
    //   {"",new Spell(()=>{})},
    //   PushStack(()=>);

    public override void _Ready()
    {
        Patterns.Add("QAQ", new Spell(() =>
        {
            GD.Print("Push player into stack");
            if(GetTree().GetNodeCountInGroup("Player") == 0) return;
            PushStack(() => GetTree().GetNodesInGroup("Player")[0].GetPath());
        }));
        Patterns.Add("AWQQQWAQW", new Spell(() =>
        {
            GD.Print("Gave player an impulse");
            var d2 = PopStack();
            var d1 = PopStack();
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
    public const string NegPrefix = "DEDD";

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
                var num = ParseNum(str);
                PushStack(Callable.From(() => num));
                return;
            }
            if (pattern.StartsWith(NegPrefix))
            {
                var str = pattern[NegPrefix.Length..];
                var num = ParseNum(str);
                PushStack(Callable.From(() => -num));
                return;
            }
            GD.Print($"Invalid pattern: {pattern}");
        }
    }

    private static float ParseNum(string str)
    {
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

        return num;
    }
}