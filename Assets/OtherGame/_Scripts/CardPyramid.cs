using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum pCardState{
    drawpile,
    tableau,
    unavailable,
    temporary,
    target,
    discard
}
public class CardPyramid : Card
{
    [Header("Set Dynamically: CardPyramid")]
    public pCardState state = pCardState.drawpile;
    public List<CardPyramid> hiddenBy = new List<CardPyramid>();
    public int layoutID;
    public SlotDef slotDef;
    public override void OnMouseUpAsButton()
    {
        Pyramid.S.CardClicked(this);
        base.OnMouseUpAsButton();
    }
}