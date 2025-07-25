using System;
using TMPro;
using UnityEngine;

public class UIEndGameScoringElement : MonoBehaviour {
    [SerializeField] private TMP_Text _txtLabel;
    [SerializeField] private TMP_Text _txtDescription;
    [SerializeField] private UIFBScoreEndGame _txtScore;

    public void DiplayScoringElement(SoEndGameScoring egs) {
        Tuple<string, int> data= egs.GetScore();

        _txtLabel.text = egs.Title;
        _txtDescription.text = data.Item1;
        _txtScore.StartDisplaying(data.Item2);
        
    }
}
