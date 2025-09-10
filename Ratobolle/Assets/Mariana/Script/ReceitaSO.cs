// Salve como ReceitaSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Nova Receita", menuName = "Cozinha/Receita")]
public class ReceitaSO : ScriptableObject
{
    [Header("Informa��es da Receita")]
    public string nomeReceita;
    public Sprite emojiPrato; // O emoji que o rato vai mostrar

    [Header("Ingredientes Necess�rios")]
    public List<InventoryItem> ingredientesNecessarios;

    [Header("Resultado Final")]
    [Tooltip("O item que ser� criado e entregue ao cliente.")]
    public CollectibleItemData pratoFinal;
}