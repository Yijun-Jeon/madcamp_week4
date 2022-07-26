using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    public TMP_Text nickName;

    // Start is called before the first frame update
    void Start()
    {
        nickName = GetComponent<TMP_Text>();
    }

    public void SetNickName(string nickName)
    {
        this.nickName.text = nickName;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
