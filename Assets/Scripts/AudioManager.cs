using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;

    public AudioClip acornEffect;
    public AudioClip background;
    public AudioClip prefightTheme;
    public AudioClip slashEffect;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
		DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Cuidado! Mas de un AudioManager en escena.");
        }
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void AcornSFX()
    {
        SFXSource.clip = acornEffect;
        SFXSource.Play();
    }

    public void SlashSFX()
    {
        SFXSource.clip = slashEffect;
        SFXSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 4)
        {
            musicSource.Stop();
        }
    }
}
