using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Layout_: Layout{
    public override async void ReadLayout(string xmlText){
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);
        xml = xmlr.xml["xml"][0];
        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));
        SlotDef tSD;
        PT_XMLHashList slotsX = xml["slot"];
        for(int i =0; i<slotsX.Count; i++){
            tSD = new SlotDef();
            if (slotsX[i].HasAtt("type")){
                tSD.type = slotsX[i].att("type");
            }
            else{
                tSD.type = "slot";
            }
            tSD.x = float.Parse(slotsX[i].att("x"));
            tSD.y = float.Parse(slotsX[i].att("y"));
            tSD.layerID = int.Parse(slotsX[i].att("layer"));
            tSD.layerName = sortingLayerNames[tSD.layerID];
            switch(tSD.type){
                case "slot":
                tSD.faceUp = (slotsX[i].att("faceup")=="1");
                tSD.id = int.Parse(slotsX[i].att("id"));
                if (slotsX[i].HasAtt("hiddenby")){
                    string[] hiding = slotsX[i].att("hiddenby").Split(',');
                    foreach(string s in hiding){
                        tSD.hiddenBy.Add(int.Parse(s));
                    }
                }
                slotDefs.Add(tSD);
                break;
                case "drawpile":
                    tSD.stagger.x = float.Parse(slotsX[i].att("xstagger"));
                    drawPile = tSD;
                    break;
                case "temporary":
                    temporaryPile = tSD;
                    break;
                case "discardpile":
                    discardPile = tSD;
                    break;
            }
        }
    }
}
/*public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}*/
