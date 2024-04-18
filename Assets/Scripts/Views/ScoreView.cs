using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    #region Player1
    public TextMeshProUGUI Player1HPtxt;
    public TextMeshProUGUI Player1ShieldLvl;
    public TextMeshProUGUI Player1ShieldHP;
    public Image Player1ShieldFill;
    public int Player1DisplayedHP;
    public Image Player1HPFill;
    public GameObject[] Player1CardsHolders;
    #endregion
    
    #region Player2
    public TextMeshProUGUI Player2HPtxt;
    public TextMeshProUGUI Player2ShieldLvl;
    public TextMeshProUGUI Player2ShieldHP;
    public Image Player2ShieldFill;
    public int Player2DisplayedHP;
    public Image Player2HPFill;
    public GameObject[] Player2CardsHolders; //TODO write a class for this
    #endregion

    #region Banner
    public GameObject BannerItem;
    public TextMeshProUGUI BannerTotalScore;
    //TODO finish this later
    #endregion

    public async Task Init(Dictionary<int, PlayerScoreConfig> playersScoreData)
    {
        var p1MaxHp = GetMaxHp(playersScoreData[1].PlayerCards);
        var p2MaxHp = GetMaxHp(playersScoreData[2].PlayerCards);

        Player1HPtxt.text = $"0/{p1MaxHp}";
        Player2HPtxt.text = $"0/{p2MaxHp}";

        Player1ShieldLvl.text = $"{playersScoreData[1].ShieldLvl}";
        Player2ShieldLvl.text = $"{playersScoreData[2].ShieldLvl}";
        
    }

    private int GetMaxHp(List<Card> playerCards)
    {
        var totalHP = 0;
        if (playerCards == null || playerCards.Count == 0)
        {
            return 1;
        }
        foreach (var card in playerCards)
        {
            totalHP += card.GetHP();
        }

        return totalHP;
    }

    public async Task Load(Dictionary<int, PlayerScoreConfig> playersScoreData)
    {
        LoadPlayerHP(playersScoreData[1], Player1CardsHolders, Player1HPFill, Player1HPtxt);
        await LoadPlayerHP(playersScoreData[2], Player2CardsHolders, Player2HPFill, Player2HPtxt);
        LoadShield(playersScoreData[1].ShieldHp, Player1ShieldFill, playersScoreData[1].ShieldLvl, Player1ShieldLvl);
        LoadShield(playersScoreData[2].ShieldHp, Player2ShieldFill, playersScoreData[2].ShieldLvl, Player2ShieldLvl);
    }

    private async Task LoadPlayerHP(PlayerScoreConfig config, GameObject[] cardHolders, Image hpFill, TextMeshProUGUI hpText)
    {
        var playerCards = config.PlayerCards;
        var totalHP = GetMaxHp(playerCards);
        int cardHolderIndex = 0;
        var relativeHP = 0;
        if (playerCards != null)
        {
            foreach (var card in playerCards) 
            {
            relativeHP += card.GetHP();
            await AddCardToHP(hpFill,  (float)relativeHP/totalHP, card, cardHolders[cardHolderIndex]);
            cardHolderIndex++;
            hpText.text = $"{relativeHP}/{totalHP}"; 
            }
        }
        hpText.text = $"{totalHP}/{totalHP}"; 
        hpFill.fillAmount = 1;
    }

    private async Task LoadShield(int configShieldHp, Image shieldFill, int configShieldLvl, TextMeshProUGUI player2ShieldLvl)
    {
        var cardLoadTime = 0.2f;
        var runTime = 0f;
        player2ShieldLvl.text = configShieldLvl.ToString();
        while (runTime < cardLoadTime)
        {
            runTime += Time.deltaTime;
            shieldFill.fillAmount = (runTime / cardLoadTime);
        }

        shieldFill.fillAmount = 1;
    }

    private async Task AddCardToHP(Image hpFill, float fillTarget, Card card, GameObject cardHolder)
    {
        var cardLoadTime = 0.2f;
        var runTime = 0f;
        card.transform.parent = cardHolder.transform;
        while (runTime < cardLoadTime)
        {
            hpFill.fillAmount = fillTarget;//TODO gradualise this
            runTime += Time.deltaTime;
            card.transform.localPosition /= ((cardLoadTime - runTime) / cardLoadTime);
        }
        
        hpFill.fillAmount = fillTarget;
        card.transform.localPosition = Vector3.zero;
    }

    public void ExecuteStreak(AttackData attackData)
    {
        
    }
}
    
public struct ScoreViewData
{
    
}
