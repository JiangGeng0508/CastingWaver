using Godot;
using Godot.Collections;

namespace CastingWaver.scene;

[Tool]
public partial class DocFlower : FoldableContainer
{
    private VBoxContainer _container;
    [Export] private Dictionary<string, string> _docsDic = new()
    {
        { "Introduction", "This is the introduction." }
    };
    
    public override void _Ready()
    {
        base._Ready();
        _container = GetNode<VBoxContainer>("VBoxContainer");
        foreach (var (key,value) in _docsDic)
        {
            AddDoc($"{key}:{value}");
        }
    }

    private void AddDoc(string doc) => _container.AddChild(new Label { Text = doc });
}