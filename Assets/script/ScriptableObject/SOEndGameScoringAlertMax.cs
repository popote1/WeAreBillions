using System;
using script;
using UnityEngine;

[CreateAssetMenu(fileName = "newEndGamesScoringAlertMax", menuName = "Scriptable Objects/EndGameScoring/AlertMax")]
public class SOEndGameScoringAlertMax : SoEndGameScoring {
    public ScoringOption[] ScoringOptions;
        
    public override Tuple<string, int> GetScore() {
        foreach (var option in ScoringOptions) {
            if (option.IsValide(StaticData.AlertMaxLevel)) {
                return new Tuple<string, int>(option.Desctription, option.ScoreGain);
            }
        }  
        return new Tuple<string, int>("Pas de Scoring", 0);
    }
}