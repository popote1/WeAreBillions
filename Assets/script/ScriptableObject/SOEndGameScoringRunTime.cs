using System;
using script;
using UnityEngine;

[CreateAssetMenu(fileName = "newEndGamesScoringRunTime", menuName = "Scriptable Objects/EndGameScoring/RunTime")]
public class SOEndGameScoringRunTime : SoEndGameScoring {
    public ScoringOption[] ScoringOptions;
        
    public override Tuple<string, int> GetScore() {
        foreach (var option in ScoringOptions) {
            if (option.IsValide(Mathf.RoundToInt(StaticData.GameTimer))) {
                return new Tuple<string, int>(option.Desctription, option.ScoreGain);
            }
        }  
        return new Tuple<string, int>("Pas de Scoring", 0);
    }
}