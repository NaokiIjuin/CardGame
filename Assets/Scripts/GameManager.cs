using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class GameManager : MonoBehaviour
{
    [SerializeField] CardController cardPrefab;
    [SerializeField] Transform playerHand, playerField, enemyField;
    [SerializeField] Text playerLeaderHPText;
    [SerializeField] Text enemyLeaderHPText;
    [SerializeField] Text playerManaPointText;
    [SerializeField] Text playerDefaultManaPointText;

    public int playerManaPoint;
    public int playerDefaultManaPoint;

    bool isPlayerTurn = true; //
    List<int> deck = new List<int>() { 1, 2, 3, 1, 1, 2, 2, 3, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3 };  //

    public static GameManager instance;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    void Start()
    {
        StartGame();
    }

    void StartGame() // �����l�̐ݒ� 
    {
        enemyLeaderHP = 5000;
        playerLeaderHP = 50000;
        ShowLeaderHP();

        playerManaPoint = 1;
        playerDefaultManaPoint = 1;
        ShowManaPoint();

        // ������D��z��
        SetStartHand();

        // �^�[���̌���
        TurnCalc();
    }

    public void ShowManaPoint()
    {
        playerManaPointText.text = playerManaPoint.ToString();
        playerDefaultManaPointText.text = playerDefaultManaPoint.ToString();
    }

    public void ReduceManaPoint(int cost)
    {
        playerManaPoint -= cost;
        ShowManaPoint();
        SetCanUsePanelHand();
    }

    void SetCanUsePanelHand()
    {
        CardController[] playerHandCardList = playerHand.GetComponentsInChildren<CardController>();

        foreach (CardController card in playerHandCardList)
        {
            if (card.model.cost <= playerManaPoint)
            {
                card.model.canUse = true;
                card.view.SetCanUsePanel(card.model.canUse);
            }
            else
            {
                card.model.canUse = false;
                card.view.SetCanUsePanel(card.model.canUse);
            }
        }
    }

    void CreateCard(int cardID, Transform place)
    {
        CardController card = Instantiate(cardPrefab, place);
        if (place == playerHand)
        {
            card.Init(cardID, true);
        }
        else
        {
            card.Init(cardID,false);
        }  
    }

    void DrawCard(Transform hand) // �J�[�h������
    {
        // �f�b�L���Ȃ��Ȃ�����Ȃ�
        if (deck.Count == 0)
        {
            return;
        }
        CardController[] playerHandCardList = playerHand.GetComponentsInChildren<CardController>();
        if (playerHandCardList.Length < 9)
        {
            // �f�b�L�̈�ԏ�̃J�[�h�𔲂����A��D�ɉ�����
            int cardID = deck[0];
            deck.RemoveAt(0);
            CreateCard(cardID, hand);
        }

        SetCanUsePanelHand();
    }

    void SetStartHand() // ��D��3���z��
    {
        for (int i = 0; i < 3; i++)
        {
            DrawCard(playerHand);
        }
    }

    void TurnCalc() // �^�[�����Ǘ�����
    {
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            EnemyTurn();
        }
    }

    public void ChangeTurn() // �^�[���G���h�{�^���ɂ��鏈��
    {
        isPlayerTurn = !isPlayerTurn; // �^�[�����t�ɂ���
        TurnCalc(); // �^�[���𑊎�ɉ�
    }

    void PlayerTurn()
    {
        Debug.Log("Player�̃^�[��");

        CardController[] playerFieldCardList = playerField.GetComponentsInChildren<CardController>();
        SetAttackableFieldCard(playerFieldCardList, true);
        
        playerDefaultManaPoint++;
        playerManaPoint = playerDefaultManaPoint;
        ShowManaPoint();

        DrawCard(playerHand); // ��D���ꖇ������
    }

    void EnemyTurn()
    {
        Debug.Log("Enemy�̃^�[��");
       
        CardController[] enemyFieldCardList = enemyField.GetComponentsInChildren<CardController>();

        SetAttackableFieldCard(enemyFieldCardList, true);

        if (enemyFieldCardList.Length < 5)
        {
            CreateCard(UnityEngine.Random.Range(1,4), enemyField); // �J�[�h������
        }

        CardController[] enemyFieldCardListSecond = enemyField.GetComponentsInChildren<CardController>();

        while (Array.Exists(enemyFieldCardListSecond, card => card.model.canAttack))
        {
            CardController[] enemyCanAttackCardList = Array.FindAll(enemyFieldCardListSecond ,card => card.model.canAttack);
            CardController attackCard = enemyCanAttackCardList[0];

            AttackToLeader(attackCard, false);
            enemyFieldCardList = enemyField.GetComponentsInChildren<CardController>();
        }

        ChangeTurn(); // �^�[���G���h����
    }
    public void CardBattle(CardController attackCard ,CardController defenceCard)
    {
        if (!attackCard.model.canAttack || attackCard.model.PlayerCard == defenceCard.model.PlayerCard)
        {
            return;
        }

        if (attackCard.model.power > defenceCard.model.power)
        {
            defenceCard.DestroyCard(defenceCard);
        }
        if (attackCard.model.power < defenceCard.model.power)
        {
            attackCard.DestroyCard(attackCard);
        }
        if (attackCard.model.power == defenceCard.model.power)
        {
            attackCard.DestroyCard(attackCard);
            defenceCard.DestroyCard(defenceCard);
        }

        attackCard.model.canAttack = false;
        attackCard.view.SetCanAttackPanel(false);
    }

    void SetAttackableFieldCard(CardController[] cardList,bool canAttack)
    {
        foreach(CardController card in cardList)
        {
            card.model.canAttack = canAttack;
            card.view.SetCanAttackPanel(canAttack);
        }
    }

    public int playerLeaderHP;
    public int enemyLeaderHP;

    public void AttackToLeader(CardController attackCard,bool isPlayerCard)
    {
        if (!attackCard.model.canAttack)
        {
            return;
        }

        if (attackCard.model.PlayerCard)
        {
            enemyLeaderHP -= attackCard.model.power;
        }
        else
        {
            playerLeaderHP -= attackCard.model.power;
        }

        attackCard.model.canAttack = false;
        attackCard.view.SetCanAttackPanel(false);
        Debug.Log("�G��HP�́A" + enemyLeaderHP);
        ShowLeaderHP();
    }

    public void ShowLeaderHP()
    {
        if (playerLeaderHP <= 0)
        {
            playerLeaderHP = 0;
        }
        if (enemyLeaderHP <= 0)
        {
            enemyLeaderHP = 0;
        }

        playerLeaderHPText.text = playerLeaderHP.ToString();
        enemyLeaderHPText.text = enemyLeaderHP.ToString();
    }
}