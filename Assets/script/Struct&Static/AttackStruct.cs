using System;
using script;

[Serializable]
public struct AttackStruct {
    public float AttackSpeed;
    public int Damage;
    public SpecialAttack[] SpecialAttacks;
    [Serializable]
    public struct SpecialAttack
    {
        public Metrics.UniteType UniteType;
        public int Damage;
    }

    public int GetDamage(Metrics.UniteType type) {
        foreach (var sp in SpecialAttacks) {
            if (sp.UniteType == type) return sp.Damage;
        }
        return Damage;
    }
}
