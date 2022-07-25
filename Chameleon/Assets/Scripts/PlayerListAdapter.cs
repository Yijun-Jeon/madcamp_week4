using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerListAdapter : MonoBehaviour
{

    public GameObject contents;
    public GameObject playerInfo;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            Instantiate<GameObject>(this.playerInfo, contents.transform);
        }

    }

    public void UpdateItems()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

