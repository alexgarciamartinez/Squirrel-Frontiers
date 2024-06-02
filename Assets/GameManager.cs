using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
	public HUD hud;

    public int PuntosTotales {get; private set;}

	public int ChaosTotales {get; private set;}

	public int vidas = 5;

	public string transitionedFromScene;

	[SerializeField] private FadeUI pauseMenu;
	[SerializeField] private FadeUI defeatPanel;
	[SerializeField] private FadeUI winPanel;
	[SerializeField] private FadeUI theDialog;
	[SerializeField] private FadeUI superDialog;
	[SerializeField] private float fadeTime;
	public bool gameIsPaused = false;

    private void Awake()
    {
		DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Cuidado! Mas de un GameManager en escena.");
        }
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
		{
			pauseMenu.FadeUIIn(fadeTime);
			Time.timeScale = 0;
			gameIsPaused = true;
		}
	}

	public void UnpauseGame()
	{
		Time.timeScale = 1;
		gameIsPaused = false;
	}

    public void SumarPuntos(int puntosASumar)
    {
        PuntosTotales += puntosASumar;
		hud.ActualizarPuntos(PuntosTotales);
    }

	public void SumarChaos(int chaosASumar)
	{
		ChaosTotales += chaosASumar;
		hud.ActualizarChaos(ChaosTotales);
	}

	public void RestarPuntos(int puntosARestar)
	{
		PuntosTotales -= puntosARestar;
		hud.ActualizarPuntos(PuntosTotales);
	}

	public void PerderVida() {
		vidas -= 1;

		if(vidas == 0)
		{
			// Reiniciamos el nivel.
            //SceneManager.LoadScene(0);
			defeatPanel.FadeUIIn(fadeTime);
			Time.timeScale = 0;
			gameIsPaused = true;
			Debug.Log("PERDEMOS");
		}

		hud.DesactivarVida(vidas);
	}

	public bool RecuperarVida() {
		if (vidas == 3)
		{
			return false;
		}

		hud.ActivarVida(vidas);
		vidas += 1;
		return true;
	}

	public void WinGame()
	{
		GameObject pointsText = winPanel.transform.GetChild(1).gameObject;
		var textMeshProSecondChild = pointsText.GetComponent<TextMeshProUGUI>();
		textMeshProSecondChild.text = PuntosTotales.ToString();
		winPanel.FadeUIIn(fadeTime);
		Time.timeScale = 0;
		gameIsPaused = true;
	}

	public void LoseGame()
	{
		defeatPanel.FadeUIIn(fadeTime);
		Time.timeScale = 0;
		gameIsPaused = true;
	}

	public void ShowDialog()
	{
		theDialog.FadeUIIn(fadeTime);
		Time.timeScale = 0;
		gameIsPaused = true;
	}

	public void ShowSuperDialog()
	{
		superDialog.FadeUIIn(fadeTime);
		Time.timeScale = 0;
		gameIsPaused = true;
	}
}
