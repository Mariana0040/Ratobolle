using UnityEngine;
using DG.Tweening;

public class TutorialSceneController : MonoBehaviour
{
    [Header("Referências da Transição")]
    public GameObject playerPrefab; // Arraste o Prefab do seu Player (rato) aqui
    public Transform entryPoint;    // Arraste o "PontoEntradaTutorial" que você criou

    void Start()
    {
        // Cria uma instância do player no ponto de entrada
        GameObject playerInstance = Instantiate(playerPrefab, entryPoint.position, entryPoint.rotation);

        // Ponto final do rolamento dentro da cena do tutorial
        Vector3 endRollPosition = playerInstance.transform.position + playerInstance.transform.forward * 8f; // Rola 8 unidades para frente

        // Inicia uma animação de rolamento IMEDIATAMENTE quando a cena começa
        // Isso cria a ilusão de que ele continuou rolando da cena do menu
        playerInstance.transform.DORotate(new Vector3(360, 0, 0), 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        playerInstance.transform.DOMove(endRollPosition, 1.5f).SetEase(Ease.Linear);
    }
}