// O ScriptableObject permanece o mesmo
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class CollectibleItemData_V2 : ScriptableObject
{
    public string itemName = "Novo Item";
    public Sprite icon;

    [Header("Visualização no Jogo")]
    [Tooltip("O prefab do modelo 3D que representa este item quando segurado.")]
    public GameObject modelPrefab; // << ADICIONE ESTA LINHA!
}
