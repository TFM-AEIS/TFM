using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    private int money = 0;
    private Text moneyText;

    public int Money
    {
        get { return this.money; }
        set { this.OnMoneyUpdate(value); }
    }

    //---------------------------------------------------//
    // Unity Events
    //---------------------------------------------------//

    void Start()
    {
        this.moneyText = GetComponent<Text>();
    }

    //---------------------------------------------------//
    // Private Methods
    //---------------------------------------------------//

    private void OnMoneyUpdate(int quantity)
    {
        this.money = quantity;
        this.moneyText.text = this.money.ToString();
    }
}
