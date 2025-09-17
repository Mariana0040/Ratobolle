using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class InteractiveMenuController : MonoBehaviour
{
    [Header("Referências do Personagem e Câmera")]
    public Transform player; // Arraste o objeto do rato aqui
    public Camera mainCamera; // A câmera principal da cena

    [Header("Pontos de Navegação")]
    public Transform menuStartPoint; // Ponto inicial e central do menu
    public Transform settingsPoint;  // Ponto para onde o rato vai para as configurações
    public Transform tutorialPoint;  // Ponto de transição para o tutorial
    public Transform gameStartPoint; // Ponto de partida para a cena do jogo

    // NOVO: Adicione uma referência para o ponto da câmera
    [Header("Pontos de Posição da Câmera")]
    public Transform cameraStartPoint; // Arraste seu "PontoCameraPrincipal" aqui


    [Header("Parâmetros de Animação")]
    public float rollDuration = 1.5f; // Duração de cada rolamento
    public float rollRotationAmount = 360f; // Quantos graus ele gira ao rolar

    [Header("Referências de UI")]
    public GameObject mainMenuCanvas; // O Canvas com os botões Iniciar, Tutorial, etc.
    public GameObject settingsCanvas; // O Canvas com as configurações
    public Button startGameButton;
    public Button tutorialButton;
    public Button settingsButton;
    public Button backFromSettingsButton; // Botão "Voltar" das configurações

    private const string TutorialCompletedKey = "TutorialCompleted";

    void Start()
    {
        // Garante que a escala de tempo está normal
        Time.timeScale = 1f;

        // Posiciona o player no ponto inicial
        player.position = menuStartPoint.position;
        player.rotation = menuStartPoint.rotation;

        // Garante que a câmera comece no lugar certo
        mainCamera.transform.position = cameraStartPoint.position;
        mainCamera.transform.rotation = cameraStartPoint.rotation;

        // Desativa o menu de configurações no início
        settingsCanvas.SetActive(false);

        // Verifica se o tutorial foi completado para habilitar o botão de iniciar
        bool tutorialCompleted = PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1;
        startGameButton.interactable = tutorialCompleted;

        // Adiciona os listeners (ações) aos botões
        startGameButton.onClick.AddListener(HandleStartGame);
        tutorialButton.onClick.AddListener(HandleTutorial);
        settingsButton.onClick.AddListener(HandleSettings);
        backFromSettingsButton.onClick.AddListener(HandleBackFromSettings);
    }

    private void HandleStartGame()
    {
        mainMenuCanvas.SetActive(false); // Esconde a UI para focar na animação

        // Cria uma sequência de animações com DOTween
        Sequence sequence = DOTween.Sequence();

        // 1. Primeiro rolamento para frente
        sequence.Append(player.DORotate(new Vector3(rollRotationAmount, 0, 0), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        sequence.Join(player.DOMove(player.position + new Vector3(0, 0, 5), rollDuration)); // Move 5 unidades para frente

        // 2. Segundo rolamento para frente
        sequence.Append(player.DORotate(new Vector3(rollRotationAmount, 0, 0), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        sequence.Join(player.DOMove(gameStartPoint.position, rollDuration));

        // 3. Câmera se acopla ao player (visão em primeira pessoa)
        sequence.AppendCallback(() => {
            mainCamera.transform.SetParent(player);
            mainCamera.transform.DOLocalMove(new Vector3(0, 0.8f, 0.5f), 1f); // Ajuste esta posição para a visão desejada
            mainCamera.transform.DOLocalRotate(Vector3.zero, 1f);
        });

        // 4. Aguarda um pouco e carrega a cena do jogo
        sequence.AppendInterval(1.5f);
        sequence.OnComplete(() => {
            SceneManager.LoadScene("SuaCenaDeJogo"); // SUBSTITUA PELO NOME DA SUA CENA
        });
    }

    private void HandleTutorial()
    {
        mainMenuCanvas.SetActive(false);

        Sequence sequence = DOTween.Sequence();

        // 1. Rola para a frente em direção ao ponto do tutorial
        sequence.Append(player.DORotate(new Vector3(rollRotationAmount, 0, 0), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        sequence.Join(player.DOMove(tutorialPoint.position, rollDuration));

        // 2. Câmera foca no ponto do tutorial
        sequence.Join(mainCamera.transform.DOLookAt(tutorialPoint.position, rollDuration));

        // 3. Ao terminar a animação, carrega a cena do tutorial
        sequence.OnComplete(() => {
            StartCoroutine(LoadTutorialScene());
        });
    }

    private IEnumerator LoadTutorialScene()
    {
        // Carrega a cena do tutorial aditivamente
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("CenaM&C", LoadSceneMode.Additive); // SUBSTITUA O NOME
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Desativa os objetos da cena do menu para focar apenas no tutorial
        mainMenuCanvas.SetActive(false);
        // Opcional: Desativar outros elementos da cena de menu se necessário
        // GameObject.Find("LuzesDoMenu").SetActive(false);
    }

    private void HandleSettings()
    {
        mainMenuCanvas.SetActive(false);

        Sequence sequence = DOTween.Sequence();

        // Rola para a esquerda (eixo Z) em direção ao ponto de configurações
        sequence.Append(player.DORotate(new Vector3(0, 0, -rollRotationAmount), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad));
        sequence.Join(player.DOMove(settingsPoint.position, rollDuration));
        sequence.Join(mainCamera.transform.DOLookAt(settingsPoint.position, rollDuration));

        // Ao completar, mostra o menu de configurações
        sequence.OnComplete(() => {
            settingsCanvas.SetActive(true);
        });
    }

    private void HandleBackFromSettings()
    {
        settingsCanvas.SetActive(false);

        Sequence sequence = DOTween.Sequence();

        // Rola de volta para a direita (eixo Z) em direção ao ponto central
        sequence.Append(player.DORotate(new Vector3(0, 0, rollRotationAmount), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad));
        sequence.Join(player.DOMove(menuStartPoint.position, rollDuration));
        sequence.Join(mainCamera.transform.DOLookAt(menuStartPoint.position, rollDuration));

        // CORREÇÃO: Em vez de apenas DOLookAt, vamos mover a câmera de volta para a posição E rotação originais
        sequence.Join(mainCamera.transform.DOMove(cameraStartPoint.position, rollDuration));
        sequence.Join(mainCamera.transform.DORotate(cameraStartPoint.rotation.eulerAngles, rollDuration));


        // Ao completar, mostra o menu principal novamente
        sequence.OnComplete(() => {
            mainMenuCanvas.SetActive(true);
        });
    }
}