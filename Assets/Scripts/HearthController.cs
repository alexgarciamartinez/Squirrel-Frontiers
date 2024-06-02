using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HearthController : MonoBehaviour
{
    PlayerController player;

    private GameObject[] heartContainers;
    private Image[] heartFils;
    public Transform heartsParent;
    public GameObject heartContainerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        heartContainers = new GameObject[PlayerController.Instance.maxHealth];
        heartFils = new Image[PlayerController.Instance.maxHealth];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < PlayerController.Instance.maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    void SetFilledHearts()
    {
        for (int i = 0; i < heartFils.Length; i++)
        {
            if (i < PlayerController.Instance.maxHealth)
            {
                heartFils[i].fillAmount = 1;
            }
            else
            {
                heartFils[i].fillAmount = 0;
            }
        }
    }

    void InstantiateHeartContainer()
    {
        for (int i = 0; i < PlayerController.Instance.maxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFils[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }
}
