using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CardMovement : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public Transform cardParent;
    bool canDrag = true;

    public void OnBeginDrag(PointerEventData eventData)
    {
        CardController card = GetComponent<CardController>();
        canDrag = true;

        if (!card.model.FieldCard)
        {
            if (!card.model.canUse)
            {
                canDrag = false;
            }
        }
        else
        {
            if (!card.model.canAttack)
            {
                canDrag = false;
            }
        }
        

        if (!canDrag)
        {
            return;
        }

        cardParent = transform.parent;
        transform.SetParent(cardParent.parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag)
        {
            return;
        }

        transform.SetParent(cardParent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
