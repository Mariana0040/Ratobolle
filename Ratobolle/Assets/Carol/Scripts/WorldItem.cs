// WorldItem.cs (no objeto 3D que pode ser coletado)
using UnityEngine;
public class WorldItem : MonoBehaviour
{
    public CollectibleItemData itemData; // Arraste o ScriptableObject aqui
    public int quantity = 1;
    // ... (l�gica de highlight, se houver)
}