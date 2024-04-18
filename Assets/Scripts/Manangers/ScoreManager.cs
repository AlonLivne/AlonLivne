using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ScoreManager{
    private Dictionary<int, PlayerScoreConfig> _playersScoreData;
    private Dictionary<int, SymbolScoringData> _symbolsData;
    private ScoreView _view;
    private ScoreData _ScoreData;
    public Action ScoreLoadingDone;
    
    public ScoreManager(ScoreConfig config, ScoreView view)
    {
        _view = view;
        _playersScoreData = config.PlayersScoreConfig;
        _symbolsData = config.SymbolsData;
        
        _view.Init(config.PlayersScoreConfig);
    }

    public async Task LoadScoreView()
    {
        await _view.Load(_playersScoreData);
        ScoreLoadingDone?.Invoke();
    }

    public void ExecuteStreak(Streak streak, int player)
    {
        var streakSymbolId = streak.SymbolID;
        var playerCards = _playersScoreData[player].PlayerCards;
        var scorePerStreak = _symbolsData[streakSymbolId]?.Values;
        var baseScore = CalculateBaseScore(scorePerStreak, streak);
        var modified = baseScore;
        var attackData = new AttackData(baseScore, player%2+1);
        foreach (var playerCard in playerCards)
        {
            var powerUp = playerCard.GetPowerUp(streakSymbolId);
            if (powerUp != null)
            {
                modified += (int)(baseScore * (powerUp.Value.Combo3 - 1));
                attackData.DamageModifiers.Add((playerCard,(int)(baseScore * (powerUp.Value.Combo3 - 1))));
            }
        }

        attackData.TotalDamage = modified;
        
        _view.ExecuteStreak(attackData);
    }
    
    private int CalculateBaseScore(List<int> scorePerStreak, Streak streak)
    {
        var ans = scorePerStreak.First() * streak.Count;
        return ans;
    }
}

public struct ScoreConfig
{
    public Dictionary<int, SymbolScoringData> SymbolsData;
    public Dictionary<int, PlayerScoreConfig> PlayersScoreConfig;

    public ScoreConfig(Dictionary<int, PlayerScoreConfig> scoresConfigs, Dictionary<int, SymbolScoringData> symbolsScoringData)
    {
        PlayersScoreConfig = scoresConfigs;
        SymbolsData = symbolsScoringData;
    }
}

public struct AttackData
{
    public List<(Card, int)> DamageModifiers;
    public int BaseDamage;
    public int TotalDamage;
    public int AttackedPlayer;

    public AttackData(int baseDamage, int attackedPlayer)
    {
        BaseDamage = baseDamage;
        AttackedPlayer = attackedPlayer;
        TotalDamage = 0;
        DamageModifiers = new List<(Card, int)>();
    }
}

public struct PlayerScoreConfig
{
    public List<Card> PlayerCards;
    public int ShieldHp;
    public int ShieldLvl;

    public PlayerScoreConfig(List<CardData> playerCards, ShieldData playerShieldData)
    {
        //PlayerCards = playerCards; //TODO write cards factory and generate cards
        ShieldLvl = playerShieldData.ShieldLvl;
        ShieldHp = playerShieldData.ShieldLvl;
        PlayerCards = null;
    }
}