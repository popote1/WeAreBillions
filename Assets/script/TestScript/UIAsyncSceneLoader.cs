using System;
using System.Diagnostics;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UIAsyncSceneLoader : MonoBehaviour
{


    //public int TargetSceneIndex;
    [SerializeField] private CanvasGroup _canvasGroupPanal;
    [SerializeField] private float _fadeTime =0.5f;
    [SerializeField] private Image _imgLoading;
    [SerializeField] private float _postLoadingDelay =3f;
    [SerializeField] private float _loadingfillMaxSpeed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private AsyncOperation loading;

    private int _startSceneIndex;
    private float _timer = 0;

    private LoadingStat _loadingStat;

    private float _progressAmount;
    private string _sceneNameToLoad; 

    private enum LoadingStat {
        none,PreLoading,LoadNextScene, UnloadingPrevius,PostLoadingWait, PostLoading
    }
    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void StartLoadingScene(string sceneName)
    {
        _sceneNameToLoad = sceneName;
        StartLoadingScene();

    }
    private void StartLoadingScene()
    {
        _canvasGroupPanal.DOFade(1, _fadeTime);
        _loadingStat = LoadingStat.PreLoading;
        //_panelLoading.SetActive(true);


    }


    private void SetProgressAmount(float progressAmount) {
        _progressAmount = progressAmount;
    }

    private void UpdateFill() {
        _imgLoading.fillAmount = Mathf.Clamp(
            _progressAmount,
            _imgLoading.fillAmount - _loadingfillMaxSpeed * Time.deltaTime,
            _imgLoading.fillAmount + _loadingfillMaxSpeed * Time.deltaTime);
    }
    private void Update()
    {
        switch (_loadingStat)
        {
            case LoadingStat.none:
                break;
            case LoadingStat.LoadNextScene:
                ManageLoadingStat();
                break;
            case LoadingStat.UnloadingPrevius:
                ManageUnloadingScene();
                break;
            case LoadingStat.PreLoading:
                ManagePreLoad();
                break;
            case LoadingStat.PostLoadingWait:
                ManagePostLoadingWait();
                break;
            case LoadingStat.PostLoading:
                ManagePostLoad();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        UpdateFill();
    }

    private void ManagePostLoadingWait() {
        Debug.Log("PostLoadingDelay");
        _timer += Time.deltaTime;
        if (_timer >= _postLoadingDelay) {
            _loadingStat = LoadingStat.PostLoading;
            _canvasGroupPanal.alpha = 1;
            _canvasGroupPanal.DOFade(0, 1);
        }
    }

    private void ManagePreLoad() {
        if (_canvasGroupPanal.alpha == 1) {
            _loadingStat = LoadingStat.LoadNextScene;
            _startSceneIndex = SceneManager.GetActiveScene().buildIndex;
            loading= SceneManager.LoadSceneAsync(_sceneNameToLoad, LoadSceneMode.Additive);
        }
    }

    private void ManageLoadingStat() {
        SetProgressAmount(loading.progress/2);
        //_imgLoading.fillAmount = loading.progress/2;
        if (loading.isDone) { 
            _loadingStat = LoadingStat.UnloadingPrevius; 
            loading =SceneManager.UnloadSceneAsync(_startSceneIndex);
        }
        
    }

    private void ManageUnloadingScene() {
        Debug.Log("Unloading Scene");
        SetProgressAmount(loading.progress/2+0.5f);
        //_imgLoading.fillAmount = (loading.progress/2)+0.5f;
        if (loading.isDone) {
            _loadingStat = LoadingStat.PostLoadingWait;
        }
        
    }

    private void ManagePostLoad() {
        Debug.Log("PostLoading");
        if (_canvasGroupPanal.alpha == 0) {
            _loadingStat = LoadingStat.none; 
            gameObject.SetActive(false);
            
        }
    }
}
