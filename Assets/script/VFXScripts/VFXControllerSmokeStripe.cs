using System;
using System.Timers;
using UnityEngine;
using Random = UnityEngine.Random;

public class VFXControllerSmokeStripe : MonoBehaviour
{
   [SerializeField] private LineRenderer _renderer;
   [SerializeField] private float FadeInTimer;
   [SerializeField] private bool _playOnAwake =true;
   private bool _isFadeIn;
   private float _timer = 0;


   private Material _material;
   public void Start() {
      _material = _renderer.material;
      
      _material.SetFloat("_FlipBookIndex", Mathf.FloorToInt(Random.Range(0,4.9f)));
      _material.SetFloat("_ScrollingOffSet", Random.Range(0,1));
      if( _playOnAwake)DoFadeIn();
   }

   [ContextMenu("Do FadeIn")]
   public void DoFadeIn() {
      _isFadeIn = true;
      _timer = 0;
   }

   private void Update()
   {
      if (_isFadeIn)
      {
         _timer += Time.deltaTime;
         _material.SetFloat("_Fade", _timer/FadeInTimer);

         if (_timer >= FadeInTimer)
         {
            _material.SetFloat("_Fade", _timer/FadeInTimer);
            _isFadeIn = false;
         }
      }
   }
}
