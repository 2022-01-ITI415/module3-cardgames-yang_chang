using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState{
    drawpile,
    tableau,
    temporary,
    target,
    discard
}
public class CardProspector : Card
{
    [Header("Set Dynamically: CardProspector")]
    public eCardState state = eCardState.drawpile;
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    public int layoutID;
    public SlotDef slotDef;
    public override void OnMouseUpAsButton()
    {
        Prospector_.S.CardClicked(this);
        base.OnMouseUpAsButton();
    }
}