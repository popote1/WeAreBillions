using System;
using script;
using UnityEngine;
	
public abstract class SoEndGameScoring : ScriptableObject
{
        public string Title;
        public abstract Tuple<string, int> GetScore();
        [Serializable]
        public struct ScoringOption {
                public ScoringCondition[] _Conditions;
                public int ScoreGain;
                [TextArea]public string Desctription;

                public bool IsValide(int compareValue) {
                        foreach (var contidion in _Conditions) {
                                if (!contidion.IsTrue(compareValue)) return false;
                        }
                        return true;
                }
                
        }
        [Serializable]
        public struct  ScoringCondition {
                public int baseValue;
                public Metrics.ConditionOperator Operator;
                
                public bool IsTrue(int compareValue) {
                        switch (Operator) {
                                case Metrics.ConditionOperator.Greater: return compareValue > baseValue;
                                case Metrics.ConditionOperator.GreaterOrEqual:return compareValue >= baseValue;
                                case Metrics.ConditionOperator.Equal:return compareValue == baseValue;
                                case Metrics.ConditionOperator.SmallerOrEqual:return compareValue <= baseValue;
                                case Metrics.ConditionOperator.Smaller:return compareValue <baseValue;
                                default:
                                        throw new ArgumentOutOfRangeException();
                        }
                        
                }
        }
}