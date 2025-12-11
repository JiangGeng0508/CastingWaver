using System;
using Godot;
using Godot.Collections;
using static CastingWaver.SpellStackManager;
// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace CastingWaver;

public partial class HexPattern : Node
{
    public static readonly bool IsWorld3D = true;
    private static bool _isListMode;
    public static readonly System.Collections.Generic.Dictionary<string, Action> Patterns = new()
    {
        //零向量
        {"QQQQQ",() =>
        {
            GD.Print("Zero Vector");
            PushStack(IsWorld3D ? Vector3.Zero : Vector2.Zero);
        }},
        //X+
        {"QQQQQEA",() =>
        {
            GD.Print("X+ Vector");
            PushStack(IsWorld3D ? Vector3.Right : Vector2.Right);
        }},
        //X-
        {"EEEEEQA",() =>
        {
            GD.Print("X- Vector");
            PushStack(IsWorld3D ? Vector3.Left : Vector2.Left);
        }},
        //Y+
        {"QQQQQEW",() =>
        {
            GD.Print("Y+ Vector");
            PushStack(IsWorld3D ? Vector3.Up : Vector2.Up);
        }},
        //Y-
        {"EEEEEQW",() =>
        {
            GD.Print("Y- Vector");
            PushStack(IsWorld3D ? Vector3.Down : Vector2.Down);
        }},
        //Z+
        {"QQQQQED",() =>
        {
            GD.Print("Z+ Vector");
            PushStack(Vector3.Back);
        }},
        //Z-
        {"EEEEEQD",() =>
        {
            GD.Print("Z- Vector");
            PushStack(Vector3.Forward);
        }},
        {"EAWAE",() =>
        {
            GD.Print("Tau");
            PushStack(Mathf.Tau);
        }},
        {"QDWDQ",() =>
        {
            GD.Print("Pi");
            PushStack(Mathf.Pi);
        }},
        {"AAQ",() =>
        {
            GD.Print("E");
            PushStack(Mathf.E);
        }},
        //False
        { "DEDQ", () => 
        {
            GD.Print("False");
            PushStack(false);
        }},
        //True
        { "AEAQ", () => 
        {   
            GD.Print("True");
            PushStack(true);
        }},
        //入栈一个打印
        {"D",() =>
        {
            GD.Print("HelloStack");
            PushStack("Hello World!");
        }},
        //仅出栈
        {"A", () =>
        {
            GD.Print("ByeStack");
            PopStack();
        }},
        //弹出栈顶的元素
        {"AQA", () =>
        {
            GD.Print("PopStack");
            PopStack();
        }},
        //打印栈顶的元素,不弹出
        {"ADA", () =>
        {
            GD.Print("PeekStack");
            var d = PopStack();
            if (d.VariantType == default)
            {
                GD.PrintErr("Stack is null");
                return;
            }
            GD.Print($"[{d.VariantType}]{d}");
            PushStack(d);
        }},
        //构造向量
        {"EQQQQQ",() =>
        {
            GD.Print("Construct Vector");
            var z = PopStack();
            var y = PopStack();
            var x = PopStack();
            PushStack(new Vector3(x.AsSingle(), y.AsSingle(), z.AsSingle()));
        }},
        //分离向量
        {"QEEEEE",() =>
        {
            GD.Print("Deconstruct Vector");
            var v = PopStack();
            if (v.VariantType != Variant.Type.Vector3)
            {
                GD.PrintErr("Invalid type");
                return;
            }
            var vector = v.AsVector3();
            PushStack(vector.X);
            PushStack(vector.Y);
            PushStack(vector.Z);
        }},
        //求和
        {"WAAW",() =>
        {
            GD.Print("Sum");
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(a.AsVector3()+b.AsVector3());
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(b.AsVector3() + Vector3.One * a.AsSingle());
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(a.AsVector3() + Vector3.One * b.AsSingle());
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(a.AsSingle() + b.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //求差
        {"WDDW",() =>
        {
            GD.Print("Dif");
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(b.AsVector3() - a.AsVector3());
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(b.AsVector3() - Vector3.One * a.AsSingle());
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(Vector3.One * b.AsSingle() - a.AsVector3());
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(b.AsSingle() - a.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //求积
        {"WAQAW",()=>{
            GD.Print("Mul");
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(b.AsVector3().Dot(a.AsVector3()));
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(b.AsVector3() * a.AsSingle());
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(b.AsSingle() * a.AsVector3());
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(b.AsSingle() * a.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //求商|叉乘
        {"SDEDW",() =>
        {
            GD.Print("Sub|Cross");
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(b.AsVector3().Cross(a.AsVector3()));
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(b.AsVector3() / a.AsSingle());
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(new Vector3(
                        b.AsSingle() / a.AsVector3().X,
                        b.AsSingle() / a.AsVector3().Y,
                        b.AsSingle() / a.AsVector3().Z));
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(b.AsSingle() / a.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //求绝对值|模
        {"WQAQW",()=>{
            GD.Print("Abs|Len");
            var a =  PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Bool:
                    PushStack(a.AsBool()? 1f : 0f);
                    break;
                case Variant.Type.Vector3:
                    PushStack(a.AsVector3().Length());
                    break;
                case Variant.Type.Float:
                    PushStack(Mathf.Abs(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //求幂
        {"WEDEW",() =>
        {
            GD.Print("Pow");
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(b.AsVector3() * b.AsVector3().Dot(a.AsVector3()) / a.AsVector3().Length());
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(new Vector3(
                        Mathf.Pow(b.AsSingle(),a.AsVector3().X),
                        Mathf.Pow(b.AsSingle(),a.AsVector3().Y),
                        Mathf.Pow(b.AsSingle(),a.AsVector3().Z)));
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(new Vector3(
                        Mathf.Pow(b.AsVector3().X,a.AsSingle()),
                        Mathf.Pow(b.AsVector3().Y,a.AsSingle()),
                        Mathf.Pow(b.AsVector3().Z,a.AsSingle())));
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(Mathf.Pow(b.AsSingle(), a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //取底
        {"EWQ",()=>{
            GD.Print("Floor");
            var a =  PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Vector3:
                    PushStack(a.AsVector3().Floor());
                    break;
                case Variant.Type.Float:
                    PushStack(Mathf.Floor(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
            }
        },
        //取顶
        {"QWE",()=>{
            GD.Print("Ceil");
            var a =  PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Vector3:
                    PushStack(a.AsVector3().Round());
                    break;
                case Variant.Type.Float:
                    PushStack(Mathf.Round(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //求余
        {"ADDWAAD",() =>
        {
            GD.Print("Mod");
            var a =  PopStack();
            var b = PopStack();
            switch (a.VariantType, b.VariantType)
            {
                case (Variant.Type.Vector3, Variant.Type.Vector3):
                    PushStack(new Vector3(
                        b.AsVector3().X % a.AsVector3().X,
                        b.AsVector3().Y % a.AsVector3().Y,
                        b.AsVector3().Z % a.AsVector3().Z));
                    break;
                case (Variant.Type.Float, Variant.Type.Vector3):
                    PushStack(new Vector3(
                        b.AsSingle() % a.AsVector3().X,
                        b.AsSingle() % a.AsVector3().Y,
                        b.AsSingle() % a.AsVector3().Z));
                    break;
                case (Variant.Type.Vector3, Variant.Type.Float):
                    PushStack(new Vector3(
                        b.AsVector3().X % a.AsSingle(),
                        b.AsVector3().Y % a.AsSingle(),
                        b.AsVector3().Z % a.AsSingle()));
                    break;
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(b.AsSingle() % a.AsSingle());
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //取符号/最近的轴
        {"QQQQQAWW",() =>
        {
            GD.Print("Sign|MaxAxis");
            var a =  PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Vector3:
                    PushStack(a.AsVector3().MaxAxisIndex() switch
                    {
                        Vector3.Axis.X => Vector3.Right,
                        Vector3.Axis.Y => Vector3.Up,
                        Vector3.Axis.Z => Vector3.Back,
                        _ => throw new ArgumentOutOfRangeException()
                    });
                    break;
                case Variant.Type.Float:
                    PushStack(Mathf.Sign(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //随机数
        {"EQQQ",() =>
        {
            GD.Print("Random");
            PushStack(() =>
            {
                GD.Randomize();
                return GD.Randf();
            });
        }},
        //交换 AAWDD
        {"AAWDD",() =>
        {
            GD.Print("Swap");
            var a = PopStack();
            var b = PopStack();
            PushStack(a);
            PushStack(b);
        }},
        //提升 AAEAA
        {"AAEAA",() =>
        {
            var a = PopStack();
            var b = PopStack();
            var c = PopStack();
            PushStack(b);
            PushStack(a);
            PushStack(c);
        }},
        //下沉 DDQDD
        {"DDQDD",() =>
        {
            var a = PopStack();
            var b = PopStack();
            var c = PopStack();
            PushStack(a);
            PushStack(c);
            PushStack(b);
        }},
        //复制 AADAA
        {"AADAA",() =>
        {
            var a = PopStack();
            PushStack(a);
            PushStack(a);
        }},
        //预测 AAEDD
        {"AAEDD",() =>
        {
            var a = PopStack();
            var b = PopStack();
            PushStack(b);
            PushStack(a);
            PushStack(b);
        }},
        //回想 DDQAA
        {"DDQAA",() =>
        {
            var a = PopStack();
            var b = PopStack();
            PushStack(a);
            PushStack(b);
            PushStack(a);
        }},
        //增殖 AADAADAA
        {"AADAADAA",() =>
        {
            var a = PopStack();
            var b = PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Float:
                    for (var i = 0; i < a.AsSingle(); i++) PushStack(b);
                    break;
                case Variant.Type.Int:
                    for (var i = 0; i < a.AsInt32(); i++) PushStack(b);
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
            PushStack(b);
        }},
        //双倍复制 AADADAAW
        {"AADADAAW",() =>
        {
            var a = PopStack();
            var b = PopStack();
            PushStack(b);
            PushStack(b);
            PushStack(a);
            PushStack(a);
        }},
        //反思 QWAEAWQAEAQA
        {"QWAEAWQAEAQA",() =>
        {
            PushStack(StackCount());
        }},
        //挑拣 DDAD
        {"DDAD",() =>
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
        }},
        //挑拣复制 AADA
        {"AADA",() =>
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
        }},
        //\esc QQQAW
        {"QQQAW",() =>
        {
            PushStack(new Array<string>());
        }},
        //( QQQ
        {"QQQ",() =>
        {
            PushStack(new Array<string>());
            _isListMode = true;
            GD.Print("RecordStart");
        }},
        //) EEE
        {"EEE",() =>
        {
            _isListMode = false;
            GD.Print("RecordEnd");
        }},
        //\b EEEDW
        { "EEEDW", () =>
        {
            var list = PopStack();
            if (list.VariantType != Variant.Type.Array)
            {
                GD.PrintErr("Invalid type");
                return;
            }

            var array = list.AsGodotArray();
            if(array.Count == 0)
            {
                GD.PrintErr("Array is empty");
                return;
            }
            array.RemoveAt(array.Count - 1);
            PushStack(array);
        }},
        //run DEAQQ
        { "DEAQQ", () =>
        {
            var list = PopStack();
            if (list.VariantType != Variant.Type.Array)
            {
                GD.PrintErr("Invalid type");
                return;
            }

            var array = list.AsGodotArray();
            for (var index = 0; index < array.Count; index++)
            {
                var str = array[index];
                var pattern = str.AsString();
                if (pattern == "AQDEE") return;
                if (pattern == "QWAQDE") break;
                if (pattern == "QQAED")
                {
                    PushStack(array.Count - index - 1);
                    continue;
                }

                Cast(pattern, true);
            }
        }},
        //foreach DADAD
        
        //any->bool AW
        {
            "AW", () =>
            {
                var a = PopStack();
                switch (a.VariantType)
                {
                    case Variant.Type.Bool:
                        PushStack(a.AsBool());
                        break;
                    case Variant.Type.Int:
                        PushStack(a.AsInt32() != 0);
                        break;
                    case Variant.Type.Float:
                        PushStack(!Mathf.IsZeroApprox(a.AsSingle()));
                        break;
                    case Variant.Type.Vector2:
                        PushStack(a.AsVector2() != Vector2.Zero);
                        break;
                    case Variant.Type.Vector3:
                        PushStack(a.AsVector3() != Vector3.Zero);
                        break;
                    default:
                        GD.PrintErr("Invalid type");
                        break;
                }
            }
        },
        //bool->num WQAQW
        //not DW
        {
            "DW", () =>
            {
                var a = PopStack();
                switch (a.VariantType)
                {
                    case Variant.Type.Bool:
                        PushStack(!a.AsBool());
                        break;
                    case Variant.Type.Int:
                        PushStack(a.AsInt32() == 0);
                        break;
                    case Variant.Type.Float:
                        PushStack(Mathf.IsZeroApprox(a.AsSingle()));
                        break;
                    case Variant.Type.Vector2:
                        PushStack(a.AsVector2() == Vector2.Zero);
                        break;
                    case Variant.Type.Vector3:
                        PushStack(a.AsVector3() == Vector3.Zero);
                        break;
                    default:
                        GD.PrintErr("Invalid type");
                        break;
                }
            }
        },
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
        {"QQQQQAA",() =>
        {
            var a = PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Float:
                    PushStack(Mathf.Sin(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //cos QQQQQAD
        {"QQQQQAD",() =>
        {
            var a = PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Float:
                    PushStack(Mathf.Cos(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //tan WQQQQQADQ
        {"WQQQQQADQ",() =>
        {
            var a = PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Float:
                    PushStack(Mathf.Tan(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //asin DDEEEEE
        {"DDEEEEE",() =>
        {
            var a = PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Float:
                    PushStack(Mathf.Asin(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //acos ADEEEEE
        {"ADEEEEE",() =>
        {
            var a = PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Float:
                    PushStack(Mathf.Acos(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //atan EADEEEEEW
        {"EADEEEEEW",() =>
        {
            var a = PopStack();
            switch (a.VariantType)
            {
                case Variant.Type.Float:
                    PushStack(Mathf.Atan(a.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //atan x y DEADEEEEEWD
        {"DEADEEEEEWD",() =>
        {
            var y = PopStack();
            var x = PopStack();
            switch (x.VariantType, y.VariantType)
            {
                case (Variant.Type.Float, Variant.Type.Float):
                    PushStack(Mathf.Atan2(x.AsSingle(), y.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
        //ln EQAQE
        {"EQAQE",() =>
        {
            var x = PopStack();
            switch (x.VariantType)
            {
                case (Variant.Type.Float):
                    PushStack(Mathf.Log(x.AsSingle()));
                    break;
                default:
                    GD.PrintErr("Invalid type");
                    break;
            }
        }},
    };

    public override void _Ready()
    {
        Patterns.Add("QAQ", () =>
        {
            GD.Print("Push player into stack");
            if(GetTree().GetNodeCountInGroup("Player") == 0)
            {
                GD.PrintErr("Get player failed");
                return;
            }
            PushStack(GetTree().GetNodesInGroup("Player")[0].GetPath());
        });
        Patterns.Add("AWQQQWAQW", () =>
        {
            GD.Print("Gave player an impulse");
            var d2 = PopStack();
            var d1 = PopStack();
            if(IsWorld3D)
            {
                if (d2.VariantType != Variant.Type.Vector3 || d1.VariantType != Variant.Type.NodePath)
                {
                    GD.PrintErr("Invalid type");
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
                    GD.PrintErr("Invalid type");
                    return;
                }
                if (GetNode(d1.AsNodePath()) is RigidBody2D body)
                {
                    body.ApplyCentralImpulse(d2.AsVector2() * 40f);
                }
            }
        });
    }

    public const string NumPrefix = "AQAA";
    public const string NegPrefix = "DEDD";

    public static void Cast(string pattern,bool fromlist = false)
    {
        if (pattern != "EEE" && _isListMode && !fromlist)
        {
            var list = PopStack();
            if (list.VariantType != Variant.Type.Array)
            {
                GD.PrintErr($"Invalid type:{list.VariantType}");
                return;
            }

            GD.Print("Recording");
            var array = list.AsGodotArray();
            array.Add(pattern);
            PushStack(array);
            return;
        }
        if(Patterns.TryGetValue(pattern, out var spell))   
            spell.Invoke();
        else
        {
            //输入立即数
            if (pattern.StartsWith(NumPrefix))
            { 
                var str = pattern[NumPrefix.Length..];
                var num = ParseNum(str);
                PushStack(num);
                return;
            }
            //输入立即数的相反数
            if (pattern.StartsWith(NegPrefix))
            {
                var str = pattern[NegPrefix.Length..];
                var num = ParseNum(str);
                PushStack(-num);
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