using UnityEngine;

public class TestTestureCréator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateNewTex()
    {
        Texture tex = new Texture2D(10, 10, TextureFormat.RGBA64, false);
    }
}
