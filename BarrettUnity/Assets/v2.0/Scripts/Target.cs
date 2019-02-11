using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public string ColorName;
    private Manager GameManager;

    void Start()
    {
        GameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (gameObject.activeSelf)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.TargetHit(this.gameObject.name);
                DestroyThis();
            }
        }
        
    }

    private void DestroyThis()
    {
        gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
