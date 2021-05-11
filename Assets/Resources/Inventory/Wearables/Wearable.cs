using System.Globalization;
using UnityEngine;

public class Wearable : MonoBehaviour
{
    [SerializeField] private Tooltip tooltip;

    NumberFormatInfo numberFormat = new CultureInfo("es-ES", false).NumberFormat;

    public int score
    {
        set
        {
            this.tooltip.info = "Valor: " + value.ToString("N0", this.numberFormat);
        }
    }
}
