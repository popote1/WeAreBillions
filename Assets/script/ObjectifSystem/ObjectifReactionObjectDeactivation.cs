using UnityEngine;

public class ObjectifReactionObjectDeactivation : ObjectifReaction
{
    [SerializeField] private GameObject[] _gameObjects;

    protected override void DoReaction(int id)
    {
        foreach (var gameObject in _gameObjects)
        {
            if (gameObject == null) return;
            gameObject.SetActive(false);

        }

        base.DoReaction(id);
    }
}