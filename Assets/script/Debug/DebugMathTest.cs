using UnityEngine;

namespace script {
    public class DebugMathTest :MonoBehaviour
    {
        [SerializeField] private float _baseValue = 0;
        [SerializeField] private float _logedValue = 0;
        [SerializeField] private float _delogedValue = 0;

        [ContextMenu("Log")]
        private void logThat() {
            _logedValue = Mathf.Log(_baseValue);
        }
        
        [ContextMenu("Delog")]
        private void DelogTheValue() {
            _delogedValue = Mathf.Pow(_logedValue, 10);
        }
    }
}