using Godot;
using Godot.Collections;
// ReSharper disable StringLiteralTypo

namespace CastingWaver.scene;

public partial class DocFlower : FoldableContainer
{
    private VBoxContainer _container;
    [Export] private Dictionary<string, string> _docsDic = new()
    {
        { "Introduction", "This is the introduction." },
        { "QQQQQ", "Zero Vector" },
        { "QQQQQEA", "X+ Vector" },
        { "EEEEEQA", "X- Vector" },
        { "QQQQQEW", "Y+ Vector" },
        { "EEEEEQW", "Y- Vector" },
        { "QQQQQED", "Z+ Vector" },
        { "EEEEEQD", "Z- Vector" },
        { "EAWAE", "Tau" },
        { "QDWDQ", "Pi" },
        { "AAQ", "E" },
        { "DEDQ", "False" },
        { "AEAQ", "True" },
        { "D", "入栈一个HelloWorld" },
        { "A", "仅出栈" },
        { "AQA", "弹出栈顶的元素" },
        { "ADA", "检查栈顶的元素" },
        { "EQQQQQ", "构造向量" },
        { "QEEEEE", "分离向量" },
        { "WAAW", "求和" },
        { "WDDW", "求差" },
        { "WAQAW", "求积" },
        { "WDEDW", "求商|叉乘" },
        { "WQAQW", "求绝对值|模" },
        { "WEDEW", "求幂" },
        { "EWQ", "取底" },
        { "QWE", "取顶" },
        { "ADDWAAD", "求余" },
        { "QQQQQAWW", "取符号/最近的轴" },
        { "EQQQ", "随机数" },
        { "AAWDD", "交换" },
        { "AAEAA", "提升" },
        { "DDQDD", "下沉" },
        { "AADAA", "复制" },
        { "AAEDD", "预测" },
        { "DDQAA", "回想" },
        { "AADAADAA", "增殖" },
        { "AADADAAW", "双倍复制" },
        { "QWAEAWQAEAQA", "反思" },
        { "DDAD", "挑拣" },
        { "AADA", "挑拣复制" },
        { "QQQAW", "\\esc" },
        { "QQQ", "(" },
        { "EEE", ")" },
        { "EEEDW", "\\b" },
        { "DEAQQ", "run" },
        { "DADAD", "foreach" },
        { "AW", "any->bool" },
        { "DW", "not" },
        { "WAW", "or" },
        { "WDW", "and" },
        { "DWA", "xor" },
        { "AWDD", "t?a:b" },
        { "AD", "==" },
        { "DA", "!=" },
        { "E", ">" },
        { "Q", "<" },
        { "EE", ">=" },
        { "QQ", "<=" },
        { "QQQQQAA", "sin" },
        { "QQQQQAD", "cos" },
        { "WQQQQQADQ", "tan" },
        { "DDEEEEE", "asin" },
        { "ADEEEEE", "acos" },
        { "EADEEEEEW", "atan" },
        { "DEADEEEEEWD", "atan x y" },
        { "EQAQE", "ln" },
        { "QAQ", "Push player into stack" },
        { "AWQQQWAQW", "Gave player an impulse" }
    };
    
    public override void _Ready()
    {
        base._Ready();
        _container = GetNode<VBoxContainer>("Scroll/DocContainer");
        foreach (var (key,value) in _docsDic)
        {
            AddDoc($"{key}:{value}");
        }
    }

    private void AddDoc(string doc) => _container.AddChild(new Label { Text = doc });
}