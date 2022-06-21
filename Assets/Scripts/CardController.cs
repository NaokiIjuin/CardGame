using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public CardView view; // �J�[�h�̌����ڂ̏���
    public CardModel model; // �J�[�h�̃f�[�^������
    public CardMovement movement;

    private void Awake()
    {
        view = GetComponent<CardView>();
        movement = GetComponent<CardMovement>();
    }

    public void Init(int cardID,bool isPlayerCard) // �J�[�h�𐶐��������ɌĂ΂��֐�
    {
        model = new CardModel(cardID,isPlayerCard); // �J�[�h�f�[�^�𐶐�
        view.Show(model); // �\��
    }

    public void DestroyCard(CardController card)
    {
        Destroy(card.gameObject);
    }

    public void DropField()
    {
        GameManager.instance.ReduceManaPoint(model.cost);
        model.FieldCard = true;
        model.canUse = false;
        view.SetCanUsePanel(model.canUse);
    }
}
