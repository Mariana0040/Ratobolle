// Salve como CanvasInteracao.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CanvasInteracao : MonoBehaviour
{
    [Header("Referências")]
    public GameObject painelBalao; // O objeto pai que contém o fundo e os emojis
    public Transform containerDePedidos; // O objeto com o Horizontal Layout Group
    public GameObject prefabIconePedido; // O prefab do Image para o emoji

    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        painelBalao.SetActive(false);
    }

    // Faz o canvas sempre olhar para a câmera
    void LateUpdate()
    {
        if (painelBalao.activeSelf)
        {
            transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
        }
    }

    public void MostrarPedido(List<ReceitaSO> pedido)
    {
        // --- ADICIONE ESTA VERIFICAÇÃO DE SEGURANÇA ---
        // Se este objeto de canvas foi destruído, não continue.
        if (this == null || gameObject == null)
        {
            return; // Sai da função
        }

        // Limpa pedidos antigos
        foreach (Transform child in containerDePedidos)
        {
            Destroy(child.gameObject);
        }

        // Cria os novos ícones
        foreach (var receita in pedido)
        {
            GameObject icone = Instantiate(prefabIconePedido, containerDePedidos);
            icone.GetComponent<Image>().sprite = receita.emojiPrato;
        }

        painelBalao.SetActive(true);
    }

    public void EsconderBalao()
    {
        painelBalao.SetActive(false);
    }
}