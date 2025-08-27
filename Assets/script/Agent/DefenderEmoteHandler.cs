using System;
using UnityEngine;

public class DefenderEmoteHandler: MonoBehaviour
{
        [SerializeField] private Defender _target;

        [SerializeField] private VFXPoolManager.VFXPooledType _vfxTypeEmoteAlert = VFXPoolManager.VFXPooledType.EmoteAlert;
        [SerializeField] private VFXPoolManager.VFXPooledType _vfxTypeEmoteSurprise = VFXPoolManager.VFXPooledType.EmoteSurprise;
        [SerializeField] private Transform _posEmotes;

        private void Start() {
                _target.OnSuprise+= TargetOnOnSuprise;
                _target.OnChangeAlertCallingStat+= TargetOnOnChangeAlertCallingStat;
        }

        private void Update() {
                _posEmotes.rotation = Quaternion.identity;
        }

        private void OnDestroy() {
                _target.OnSuprise-= TargetOnOnSuprise;
                _target.OnChangeAlertCallingStat -= TargetOnOnChangeAlertCallingStat; 
        }

        private void TargetOnOnChangeAlertCallingStat(object sender, bool e) {
                
                if (VFXPoolManager.Instance == null && e) return;
                VfxPoolableMono vfx= VFXPoolManager.Instance.GetPooledVFXOfType(_vfxTypeEmoteAlert);
                vfx.transform.SetParent(_posEmotes);
                vfx.transform.localPosition = Vector3.zero;
                vfx.transform.localRotation = Quaternion.identity;
                vfx.transform.localScale = Vector3.one;

        }

        private void TargetOnOnSuprise(object sender, EventArgs e) {
                if (VFXPoolManager.Instance == null) return;
                VfxPoolableMono vfx= VFXPoolManager.Instance.GetPooledVFXOfType(_vfxTypeEmoteSurprise);
                vfx.transform.SetParent(_posEmotes);
                vfx.transform.localPosition = Vector3.zero;
                vfx.transform.localRotation = Quaternion.identity;
                vfx.transform.localScale = Vector3.one;
        }
}
