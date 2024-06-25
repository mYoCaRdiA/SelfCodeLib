using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProblemDetailPanel : MonoBehaviour
{
    public Text type;
    public Text partName;
    public Text responsiblePerson;
    public Text deadLine;
    public Text description;
    public Text rectificationOpinions;
    public Button hitBack;
    public Button rectification;
    public Button close;
    // Start is called before the first frame update
    void Start()
    {
        close.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
