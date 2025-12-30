using System.Linq;
using Godot;

namespace CastingWaver;

public partial class CastingWaver : Control
{
    [Export] private PackedScene _hexLineScene;
    private TileMapLayer _hexCanvas;
    private Line2D _hexLine;
    private Line2D _cursorLine;
    private Control _dirAssist;
    private Control _numAssist;
    private Label _numCount;
    private RichTextLabel _stackVisual;

    private static string RawMapping(Vector2I coord) => coord switch
    {
        (2, -1) => "D",
        (1, -1) => "E",
        (-1, 0) => "W",
        (-2, 1) => "A",
        (-1, 1) => "Z",
        (1, 0) => "X",
        _ => "S"
    };

    private static string GetLocalAngle(string pattern)
    {
        var result = "";
        // result += pattern[0];
        for (var index = 1; index < pattern.Length; index++)
        {
            var prev = pattern[index - 1];
            var c = pattern[index];
            switch (prev, c)
            {
                case ('D', 'D'):
                case ('E', 'E'):
                case ('W', 'W'):
                case ('A', 'A'):
                case ('Z', 'Z'):
                case ('X', 'X'):
                    result += 'W';
                    break;
                case ('D', 'E'):
                case ('E', 'W'):
                case ('W', 'A'):
                case ('A', 'Z'):
                case ('Z', 'X'):
                case ('X', 'D'):
                    result += 'Q';
                    break;
                case ('D', 'W'):
                case ('E', 'A'):
                case ('W', 'Z'):
                case ('A', 'X'):
                case ('Z', 'D'):
                case ('X', 'E'):
                    result += 'A';
                    break;
                case ('D', 'X'):
                case ('E', 'D'):
                case ('W', 'E'):
                case ('A', 'W'):
                case ('Z', 'A'):
                case ('X', 'Z'):
                    result += 'E';
                    break;
                case ('D', 'Z'):
                case ('E', 'X'):
                case ('W', 'D'):
                case ('A', 'E'):
                case ('Z', 'W'):
                case ('X', 'A'):
                    result += 'D';
                    break;
            }
        }

        return result;
    }
    public override void _Ready()
    {
        _hexCanvas = GetNode<TileMapLayer>("HexCanvas");
        _cursorLine = GetNode<Line2D>("CursorLine");
        _dirAssist = GetNode<Control>("CursorLine/DirAssist");
        _numAssist = GetNode<Control>("CursorLine/NumAssist");
        _numCount = GetNode<Label>("CursorLine/NumAssist/NumCount");
        _stackVisual = GetNode<RichTextLabel>("StackVisual");
        CreateHexLine();
    }
    private void CreateHexLine()
    {
        _hexLine = _hexLineScene.Instantiate<Line2D>();
        AddChild(_hexLine);
    }
    private bool _mouse1Pressed;
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if(mouseButton.ButtonIndex == MouseButton.Left)
            {
                _mouse1Pressed = mouseButton.IsPressed();
                if (_mouse1Pressed)
                    TryAddHexNode(mouseButton.Position);
                if (mouseButton.IsReleased())
                {
                    Casting();
                    UpdateCursorLine();
                    UpdateDirAssist();
                    UpdateNumAssist();
                }
            }
            else if(mouseButton.ButtonIndex == MouseButton.Right && mouseButton.IsPressed())
            {
                ClearHexNode();
            }
        }
        if (@event is InputEventMouseMotion mouseEvent)
        {
            if (!_mouse1Pressed) return;
            TryAddHexNode(mouseEvent.Position);
            UpdateCursorLine();
            UpdateDirAssist();
            UpdateNumAssist();
        }
    }

    private void Casting()
    {
        if (_hexLine.Points.Length <= 2)
        {
            ClearHexNode();
            return;
        }
        var result = ParseHexLine();
        HexPattern.Cast(result);
        foreach (var point in _hexLine.Points) SetCanvas(point);
        GetNode<LineEdit>("LineEdit").Text = result;
        CreateHexLine();
        ShowStack();
    }

    private string ParseHexLine()
    {
        var pattern = _hexLine.Points;
        var resultRaw = "";
        for (var index = 0; index < pattern.Length - 1; index++)
        {
            var coord1 = _hexCanvas.LocalToMap(pattern[index]);
            var coord2 = _hexCanvas.LocalToMap(pattern[index + 1]);
                        
            resultRaw += RawMapping(coord2 - coord1);
            if(RawMapping(coord2 - coord1) == "S") GD.Print(coord2 - coord1);
        }
        var result = GetLocalAngle(resultRaw);
        return result;
    }

    private void UpdateCursorLine()
    {
        if(_hexLine.Points.Length == 0)
        {
            _cursorLine.Hide();
            return;
        }
        _cursorLine.Show();
        _cursorLine.Points = [_hexLine.Points.Last(), GetLocalMousePosition()];
    }
    private Vector2 GetHexPosition(Vector2 pos) => _hexCanvas.MapToLocal(_hexCanvas.LocalToMap(pos));

    private void ClearHexCanvas()
    {
        foreach (var node in GetTree().GetNodesInGroup("HexLines"))
        {
            if (node is not Line2D line) continue;
            foreach (var point in line.Points) SetCanvas(point, false);
            line.Points = [];
        }
    }
    private void ClearHexNode()
    {
        foreach (var point in _hexLine.Points) SetCanvas(point, false);
        _hexLine.Points = [];
        UpdateCursorLine();
    }
    private void ClearHexStack()
    {
        SpellStackManager.ClearStack();
        ShowStack();
    }

    private void SetCanvas(Vector2 pos, bool occupy = true)
    {
        var coord = _hexCanvas.LocalToMap(pos);
        var tile = occupy ? new Vector2I(1, 0) : new Vector2I(0, 0);
        _hexCanvas.SetCell(coord, 0, tile);
    }
    private bool IsCellOccupied(Vector2I coord) => _hexCanvas.GetCellAtlasCoords(coord) == new Vector2I(1, 0);
    private bool IsCellOccupied(Vector2 pos) => IsCellOccupied(_hexCanvas.LocalToMap(pos));
    private void TryAddHexNode(Vector2 pos)
    {
        var hexPos = GetHexPosition(pos);
        if(hexPos.DistanceTo(pos) > 17f) return;
        if (_hexLine.Points.Length > 0)
        {
            if(_hexLine.Points.Last() == hexPos) return;
            if(_hexLine.Points.Length > 1 && _hexLine.Points[^2] == hexPos)
            {
                _hexLine.RemovePoint(_hexLine.Points.Length - 1);
                UpdateCursorLine();
                return;
            }
            if(!ValidPos(hexPos)) return;
            if (_hexLine.Points.Where((point, i) =>
                    Equals(point, hexPos) && i != 0 && Equals(_hexLine.Points[i - 1], _hexLine.Points.Last())).Any())
            {
                return;
            }
        }
        if(IsCellOccupied(hexPos)) return;
        _hexLine.AddPoint(hexPos);
        UpdateCursorLine();
        UpdateDirAssist();
        UpdateNumAssist();
    }
    
    private bool ValidPos(Vector2 pos)
    {
        var rawDir = _hexCanvas.LocalToMap(pos) - _hexCanvas.LocalToMap(_hexLine.Points.Last());
        return RawMapping(rawDir) != "S" && !IsCellOccupied(pos);
    }

    private void UpdateDirAssist()
    {
        if(!_dirAssistEnabled || _hexLine.Points.Length < 2)
        {
            _dirAssist.Hide();
            return;
        }
        var lastCoord = _hexCanvas.LocalToMap(_hexLine.Points.Last());
        var secondLastCoord = _hexCanvas.LocalToMap(_hexLine.Points[^2]);
        switch (RawMapping(lastCoord - secondLastCoord))
        {
            case "D":
                _dirAssist.SetRotationDegrees(0f);
                break;
            case "E":
                _dirAssist.SetRotationDegrees(-60f);
                break;
            case "W":
                _dirAssist.SetRotationDegrees(-120f);
                break;
            case "A":
                _dirAssist.SetRotationDegrees(180f);
                break;
            case "Z":
                _dirAssist.SetRotationDegrees(120f);
                break;
            case "X":
                _dirAssist.SetRotationDegrees(60f);
                break;
            default:
                _dirAssist.Hide();
                return;
        }

        foreach (var node in _dirAssist.GetChildren())
        {
            if(node is Label label) label.SetRotationDegrees(-_dirAssist.RotationDegrees);
        }
        
        _dirAssist.Show();
        _dirAssist.Position = _hexLine.Points.Last();
    }

    private static string NumPrefix => HexPattern.NumPrefix;
    private static string NegPrefix => HexPattern.NegPrefix;
    private static float ParseNum(string str) => HexPattern.ParseNum(str);
    private void UpdateNumAssist()
    {
        if(!_numAssistEnabled || _hexLine.Points.Length < NumPrefix.Length)
        {
            _numAssist.Hide();
            return;
        }

        var pattern = ParseHexLine();
        if (pattern.StartsWith(NumPrefix))
        {
            var str = pattern[NumPrefix.Length..];
            _numCount.Text = $"{ParseNum(str)}";
            _numAssist.Show();
            _numAssist.Position = _hexLine.Points.Last();
        }
        else if (pattern.StartsWith(NegPrefix))
        {
            var str = pattern[NegPrefix.Length..];
            _numCount.Text = $"-{ParseNum(str)}";
            _numAssist.Show();
            _numAssist.Position = _hexLine.Points.Last();
        }
        else
        {
            _numAssist.Hide();
        }   
    }

    private void ShowStack()
    {
        _stackVisual.Text = SpellStackManager.PrintStack();
    }
    
    
    private bool _dirAssistEnabled;
    private bool _numAssistEnabled;
    
    private void ToggleDirAssist(bool toggle)
        => _dirAssistEnabled = toggle;
    private void ToggleNumAssist(bool toggle)
        => _numAssistEnabled = toggle;
}