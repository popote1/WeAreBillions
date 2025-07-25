using System;
using script;
using UnityEngine;

[Serializable]
public struct TransformStruct {
    public float TransformationMod ;
    public SpecialTransformation[] SpecialTransformations;
    [Serializable]
    public struct SpecialTransformation {
        public Metrics.UniteType UniteType;
        public float TransformationMod;
    }

    public float GetTransformationMod(Metrics.UniteType type)
    {
        foreach (var sp in SpecialTransformations) {
            if (sp.UniteType == type) return sp.TransformationMod;
        }
        return TransformationMod; 
    }
}