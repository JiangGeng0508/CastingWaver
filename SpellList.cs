using System.Collections.Generic;

namespace CastingWaver;

public class SpellList : Stack<Spell>
{
    public void Run()
    {
        foreach (var unused in this)
        {
            Pop().Execute();
        }        
    }

    public void PushSpell(Spell spell)
    {
        Push(spell);
    }
}