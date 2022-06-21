using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlace : MonoBehaviour,IDropHandler
{
   public void OnDrop(PointerEventData eventData)
    {
        //CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>();
        CardController card = eventData.pointerDrag.GetComponent<CardController>();

        if (card && card.model.canUse)
        {
            //card.cardParent = this.transform;
            card.movement.cardParent = this.transform;
            card.DropField();
        }
    }
}
