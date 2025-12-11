using System;
using System.Collections.Generic;
using Godot;
using static CastingWaver.SpellStackManager;
// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace CastingWaver;

public partial class HexPattern : Node
{
    public static bool IsWorld3D = true;
    private static bool IsListMode = false;
    public static readonly Dictionary<string, Spell> Patterns = new()
    {
        //打印字符串
        {"W", new Spell(() => {GD.Print("Short Line");})},
        //零向量
        {"QQQQQ",new Spell(() =>
        {
            PushStack(() => IsWorld3D ? Vector3.Zero : Vector2.Zero);
        })},
        //X+
        {"QQQQQEA",new Spell(() =>
        {
            PushStack(() => IsWorld3D ? Vector3.Right : Vector2.Right);
        })},
        //X-
        {"EEEEEQA",new Spell(() =>
        {
            PushStack(() => IsWorld3D ? Vector3.Left : Vector2.Left);
        })},
        //Y+
        {"QQQQQE",new Spell(() =>
        {
            PushStack(() => IsWorld3D ? Vector3.Up : Vector2.Up);
        })},
        //Y-
        {"EEEEEQ",new Spell(() =>
        {
            PushStack(() => IsWorld3D ? Vector3.Down : Vector2.Down);
        })},
        //Z+
        {"QQQQQED",new Spell(() =>
        {
            PushStack(() => Vector3.Back);
        })},
        //Z-
        {"EEEEEQD",new Spell(() =>
        {
            PushStack(() => Vector3.Forward);
        })},
        {"EAWAE",new Spell(() =>
        {
            PushStack(() => Mathf.Tau);
        })},
        {"QDWDQ",new Spell(() =>
        {
            PushStack(() => Mathf.Pi);
        })},
        {"AAQ",new Spell(() =>
        {
            PushStack(() => Mathf.E);
        })},
        //False
        { "DEDQ", new Spell(() => 
        {   
            PushStack(() => false);
        })},
        //True
        { "AEAQ", new Spell(() => 
        {   
            PushStack(() => true);
        })},
        //入栈一个打印
        {"D",new Spell(() =>
        {
            GD.Print("PushStackHello");
            PushStack(() => { GD.Print("Hello World!"); });
        })},
        //出栈一次
        {"A", new Spell(PopStack)},
        //打印栈顶的元素并压回
        {"AQA", new Spell(() =>
        {
            var d = PopStack();
            if (d.VariantType == Variant.Type.String && d.AsString() == "OutOfStack") return;
            GD.Print($"peek:{d}");
            PushStack(() => d);
        })},
        //构造
        {"EQQQQQ",new Spell(() =>
        {
            var z = PopStack();
            var y = PopStack();
            var x = PopStack();
            PushStack(() => new Vector3(x.AsSingle(), y.AsSingle(), z.AsSingle()));
        })},
        //分离
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
        //求和
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
        //求差
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
        //求积
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
        //求商|叉乘
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
        //求绝对值|模
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
        //求幂
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
        //取底
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
        //取顶
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
        //求余
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
        //取符号/最近的轴
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
        //随机数
        {"EQQQ",new Spell(() =>
        {
            PushStack(() =>
            {
                GD.Randomize();
                return GD.Randf();
            });
        })},
        //交换 AAWDD
        {"AAWDD",new Spell(() =>
        {
            var a = PopStack();
            var b = PopStack();
            PushStack(() => a);
            PushStack(() => b);
        })},
        //提升 AAEAA
        {"AAEAA",new Spell(() =>
        {
            var a = PopStack();
            var b = PopStack();
            var c = PopStack();
            PushStack(() => b);
            PushStack(() => a);
            PushStack(() => c);
        })},
        //下沉 DDQDD
        {"DDQDD",new Spell(() =>
        {
            var a = PopStack();
            var b = PopStack();
            var c = PopStack();
            PushStack(() => a);
            PushStack(() => c);
            PushStack(() => b);
        })},
        //复制 AADAA
        {"AADAA",new Spell(() =>
        {
            var a = PopStack();
            PushStack(() => a);
            PushStack(() => a);
        })},
        //预测 AAEDD
        {"AAEDD",new Spell(() =>
        {
            var a = PopStack();
            var b = PopStack();
            PushStack(() => b);
            PushStack(() => a);
            PushStack(() => b);
        })},
        //回想 DDQAA
        {"DDQAA",new Spell(() =>
        {
            var a = PopStack();
            var b = PopStack();
            PushStack(() => a);
            PushStack(() => b);
            PushStack(() => a);
        })},
        //增殖 AADAADAA
        {"AADAADAA",new Spell(() =>
        {
            var a = PopStack();
            var b = PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Float:
                    for (var i = 0; i < a.AsSingle(); i++) PushStack(() => b);
                    break;
                case Variant.Type.Int:
                    for (var i = 0; i < a.AsInt32(); i++) PushStack(() => b);
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
            PushStack(() => b);
        })},
        //双倍复制 AADADAAW
        {"AADADAAW",new Spell(() =>
        {
            var a = PopStack();
            var b = PopStack();
            PushStack(() => b);
            PushStack(() => b);
            PushStack(() => a);
            PushStack(() => a);
        })},
        //反思 QWAEAWQAEAQA
        {"QWAEAWQAEAQA",new Spell(() =>
        {
            PushStack(() => StackCount());
        })},
        //挑拣 DDAD
        {"DDAD",new Spell(() =>
        {
            var a = PopStack();
            var array = SpellStack.ToArray();
            switch (a.VariantType)
            {
                case Variant.Type.Float:
                    PushStack(array[^(int)a.AsSingle()]);
                    break;
                case Variant.Type.Int:
                    PushStack(array[^a.AsInt32()]);
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //挑拣复制 AADA
        {"AADA",new Spell(() =>
        {
            var a = PopStack();
            var array = SpellStack.ToArray();
            switch (a.VariantType)
            {
                case Variant.Type.Float:
                    PushStack(array[^(int)a.AsSingle()]);
                    break;
                case Variant.Type.Int:
                    PushStack(array[^a.AsInt32()]);
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        })},
        //\esc QQQAW
        //( QQQ
        {"QQQ",new Spell(() =>
        {
            IsListMode = true;
        })},
        //) EEE
        {"EEE",new Spell(() =>
        {
            IsListMode = false;
        })},
        //\b EEEDW
        
        //run DEAQQ
        //run&break? QWAQDE
        //foreach DADAD
        //return AQDEE
        //last count QQAED
        
        //any->bool AW
        //bool->num WQAQW
        //not DW
        //or WAW
        //and WDW
        //nor DWA
        //t?a:b AWDD
        //== AD
        //!= DA
        //> E
        //< Q
        //>= EE
        //<= QQ
        
        //sin QQQQQAA
        //cos QQQQQAD
        //tan WQQQQQADQ
        //asin DDEEEEE
        //acos ADEEEEE
        //atan EADEEEEEW
        //atan x y DEADEEEEEWD
        //ln EQAQE
    };
    //   {"",new Spell(()=>{})},
    //   PushStack(()=>);

    public override void _Ready()
    {
        Patterns.Add("QAQ", new Spell(() =>
        {
            GD.Print("Push player into stack");
            if(GetTree().GetNodeCountInGroup("Player") == 0)
            {
                GD.PrintErr("Get player failed");
                return;
            }
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
            //输入立即数的相反数
            if (pattern.StartsWith(NegPrefix))
            {
                var str = pattern[NegPrefix.Length..];
                var num = ParseNum(str);
                PushStack(Callable.From(() => -num));
                return;
            }
            GD.Print($"Invalid pattern: {pattern}");
        }
        return;
        
        static float ParseNum(string str)
        {
            var num = 0f;
            foreach (var t in str)
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
            return num;
            }
    }
}