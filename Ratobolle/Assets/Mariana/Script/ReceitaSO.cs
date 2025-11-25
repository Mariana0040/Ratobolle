// Salve como ReceitaSO.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Nova Receita", menuName = "Cozinha/Receita")]
public class ReceitaSO : ScriptableObject
{
    [Header("Informações da Receita")]
    public string nomeReceita;
    public Sprite emojiPrato; // O emoji que o rato vai mostrar
    public Sprite paginaReceita; // O emoji que o rato vai mostrar

    [Header("Ingredientes Necessários")]
    public List<InventoryItem> ingredientesNecessarios;

    [Header("Resultado Final")]
    [Tooltip("O item que será criado e entregue ao cliente.")]
    public CollectibleItemData_V2 pratoFinal;
}