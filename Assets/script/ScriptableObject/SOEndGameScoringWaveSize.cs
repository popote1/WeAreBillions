using System;
using script;
using UnityEngine;

[CreateAssetMenu(fileName = "newEndGamesScoringWaveSize", menuName = "Scriptable Objects/EndGameScoring/WaveSize")]
public class SOEndGameScoringWaveSize : SoEndGameScoring {
    public ScoringOption[] ScoringOptions;
        
    public override Tuple<string, int> GetScore() {
        foreach (var option in ScoringOptions) {
            if (option.IsValide(StaticData.zombieMaxCount)) {
                return new Tuple<string, int>(option.Desctription, option.ScoreGain);
            }
        }  
        return new Tuple<string, int>("Pas de Scoring", 0);
    }
}