using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyData", menuName = "Scriptable Objects/Economy/Currency_Data")]
public class CurrencyData : ScriptableObject
{
    public string currencyName;       // "BioKredi" veya "GenKredi"
    public string symbol;             // "BK" veya "GK"
    public Sprite icon;               // Editorde atayabileceğin ikon
    public int startingAmount;        // Oyuna ilk baslarken oyuncuya verilecek miktar
    public CurrencyType currency;
}
