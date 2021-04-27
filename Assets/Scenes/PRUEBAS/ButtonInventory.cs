using UnityEngine;
using UnityEngine.UI;

public class ButtonInventory : MonoBehaviour
{
    [SerializeField] private DataResource data;

    void Start()
    {
        this.GetComponentInChildren<Text>().text = this.data.DisplayName;
        //this.GetComponentInChildren<Image>().sprite = this.data.Image;
    }

    void Update()
    {

    }
}
