using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{

    public TextMeshProUGUI puntos;
	public TextMeshProUGUI chaos;

	public GameObject[] vidas;
    
	void Awake () 
	{
		if (SceneManager.GetActiveScene().buildIndex != 0)
		{
			DontDestroyOnLoad(gameObject);
		}
	}

	public SceneFader sceneFader;

	private void Start()
	{
		sceneFader = GetComponentInChildren<SceneFader>();
	}

	void Update () {
		puntos.text = GameManager.Instance.PuntosTotales.ToString();
		chaos.text = GameManager.Instance.ChaosTotales.ToString();
	}

	public void ActualizarPuntos(int puntosTotales)
	{
		puntos.text = puntosTotales.ToString();
	}

	public void ActualizarChaos(int chaosTotales)
	{
		chaos.text = chaosTotales.ToString();
	} 

	public void DesactivarVida(int indice) {
		vidas[indice].SetActive(false);
	}

	public void ActivarVida(int indice) {
		vidas[indice].SetActive(true);
	}
}
