using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MapNodeClick : MonoBehaviour
{
    public GameObject playButton;
    public TextMeshProUGUI missionTitle;
    public TextMeshProUGUI missionDescription;
    public TextMeshProUGUI missionNumber;
    public StageInfo info;
    public string sceneToLoad;
    public bool loadWithBoard;
    public GameObject particleHighlight;
    public GameObject partcleBlock;
    public float yOffSet;

    protected GameObject effectInstance;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = SingletonManager.Get<AudioManager>();
        CheckToSpawnParticles();
    }

    public void LinkEventWithBoard()
    {
        missionTitle.text = info.missionTitle;
        missionDescription.text = info.missionDescription;
        missionNumber.text = info.missionNumber;
        playButton.GetComponent<ButtonAnimator>().onClick.RemoveAllListeners();   
        if(loadWithBoard) playButton.GetComponent<ButtonAnimator>().onClick.AddListener(LoadStage);
        else playButton.GetComponent<ButtonAnimator>().onClick.AddListener(LoadStageNoBoard);
    }

    public void LoadStage()
    {
        if(!string.IsNullOrEmpty(sceneToLoad)) SceneController.instance.LoadStage(sceneToLoad);
        if (audioManager)
        {
            audioManager.PlaySFX("Button_Click");
            audioManager.PlayBGM("BGM_Battle");
        }
    }
    public void LoadStageNoBoard()
    {
        if(!string.IsNullOrEmpty(sceneToLoad)) SceneController.instance.LoadScene(sceneToLoad);
        if (audioManager)
        {
            audioManager.PlaySFX("Button_Click");
            audioManager.PlayBGM("BGM_Battle");
        }
    }

    public void CheckToSpawnParticles()
    {
        if(effectInstance != null) DestroyParticle();

        if(!info.unlocked) 
        {
            effectInstance = Instantiate(partcleBlock, this.transform.position, partcleBlock.transform.rotation);
        }
        if(info.unlocked && !info.completed) 
        {
            effectInstance = Instantiate(particleHighlight, this.transform.position+(Vector3.up*yOffSet), particleHighlight.transform.rotation);
        }
    }

    public void DestroyParticle()
    {
        Destroy(effectInstance);
    }
}