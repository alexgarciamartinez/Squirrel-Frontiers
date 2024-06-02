using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{

    [SerializeField] private string transitionTo;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            Debug.Log("TOCANDO PORTAL");
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(transitionTo);

            if (transitionTo == "Third Map")
            {
                GameManager.Instance.ShowDialog();
            }

            if (transitionTo == "Final Map")
            {
                GameManager.Instance.ShowSuperDialog();
            }
        }
    }
    
}
