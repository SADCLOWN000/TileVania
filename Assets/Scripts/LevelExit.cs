using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] AudioClip openingDoorSFX;
    [SerializeField] float levelLoadDelay = 1f;
    private Animator openingDoorAnimator;

    private void Awake()
    {
        openingDoorAnimator = GetComponent<Animator>();
        openingDoorAnimator.gameObject.GetComponent<Animator>().enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        AudioSource.PlayClipAtPoint(openingDoorSFX, Camera.main.transform.position);
        openingDoorAnimator.gameObject.GetComponent<Animator>().enabled = true;
        StartCoroutine(LoadNextLevel());
    }

    IEnumerator LoadNextLevel()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        
        int currentSceneIndext = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndext + 1;
        
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(nextSceneIndex);
    }
}
