using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class InteractiveMenuController : MonoBehaviour
{
    [Header("Refer�ncias do Personagem e C�mera")]
    public Transform player; // Arraste o objeto do rato aqui
    public Camera mainCamera; // A c�mera principal da cena

    [Header("Pontos de Navega��o")]
    public Transform menuStartPoint; // Ponto inicial e central do menu
    public Transform settingsPoint;  // Ponto para onde o rato vai para as configura��es
    public Transform tutorialPoint;  // Ponto de transi��o para o tutorial
    public Transform gameStartPoint; // Ponto de partida para a cena do jogo

    // NOVO: Adicione uma refer�ncia para o ponto da c�mera
    [Header("Pontos de Posi��o da C�mera")]
    public Transform cameraStartPoint; // Arraste seu "PontoCameraPrincipal" aqui


    [Header("Par�metros de Anima��o")]
    public float rollDuration = 1.5f; // Dura��o de cada rolamento
    public float rollRotationAmount = 360f; // Quantos graus ele gira ao rolar

    [Header("Refer�ncias de UI")]
    public GameObject mainMenuCanvas; // O Canvas com os bot�es Iniciar, Tutorial, etc.
    public GameObject settingsCanvas; // O Canvas com as configura��es
    public Button startGameButton;
    public Button tutorialButton;
    public Button settingsButton;
    public Button backFromSettingsButton; // Bot�o "Voltar" das configura��es

    private const string TutorialCompletedKey = "TutorialCompleted";

    void Start()
    {
        // Garante que a escala de tempo est� normal
        Time.timeScale = 1f;

        // Posiciona o player no ponto inicial
        player.position = menuStartPoint.position;
        player.rotation = menuStartPoint.rotation;

        // Garante que a c�mera comece no lugar certo
        mainCamera.transform.position = cameraStartPoint.position;
        mainCamera.transform.rotation = cameraStartPoint.rotation;

        // Desativa o menu de configura��es no in�cio
        settingsCanvas.SetActive(false);

        // Verifica se o tutorial foi completado para habilitar o bot�o de iniciar
        bool tutorialCompleted = PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1;
        startGameButton.interactable = tutorialCompleted;

        // Adiciona os listeners (a��es) aos bot�es
        startGameButton.onClick.AddListener(HandleStartGame);
        tutorialButton.onClick.AddListener(HandleTutorial);
        settingsButton.onClick.AddListener(HandleSettings);
        backFromSettingsButton.onClick.AddListener(HandleBackFromSettings);
    }

    private void HandleStartGame()
    {
        mainMenuCanvas.SetActive(false); // Esconde a UI para focar na anima��o

        // Cria uma sequ�ncia de anima��es com DOTween
        Sequence sequence = DOTween.Sequence();

        // 1. Primeiro rolamento para frente
        sequence.Append(player.DORotate(new Vector3(rollRotationAmount, 0, 0), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        sequence.Join(player.DOMove(player.position + new Vector3(0, 0, 5), rollDuration)); // Move 5 unidades para frente

        // 2. Segundo rolamento para frente
        sequence.Append(player.DORotate(new Vector3(rollRotationAmount, 0, 0), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        sequence.Join(player.DOMove(gameStartPoint.position, rollDuration));

        // 3. C�mera se acopla ao player (vis�o em primeira pessoa)
        sequence.AppendCallback(() => {
            mainCamera.transform.SetParent(player);
            mainCamera.transform.DOLocalMove(new Vector3(0, 0.8f, 0.5f), 1f); // Ajuste esta posi��o para a vis�o desejada
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

        // 1. Rola para a frente em dire��o ao ponto do tutorial
        sequence.Append(player.DORotate(new Vector3(rollRotationAmount, 0, 0), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        sequence.Join(player.DOMove(tutorialPoint.position, rollDuration));

        // 2. C�mera foca no ponto do tutorial
        sequence.Join(mainCamera.transform.DOLookAt(tutorialPoint.position, rollDuration));

        // 3. Ao terminar a anima��o, carrega a cena do tutorial
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
        // Opcional: Desativar outros elementos da cena de menu se necess�rio
        // GameObject.Find("LuzesDoMenu").SetActive(false);
    }

    private void HandleSettings()
    {
        mainMenuCanvas.SetActive(false);

        Sequence sequence = DOTween.Sequence();

        // Rola para a esquerda (eixo Z) em dire��o ao ponto de configura��es
        sequence.Append(player.DORotate(new Vector3(0, 0, -rollRotationAmount), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad));
        sequence.Join(player.DOMove(settingsPoint.position, rollDuration));
        sequence.Join(mainCamera.transform.DOLookAt(settingsPoint.position, rollDuration));

        // Ao completar, mostra o menu de configura��es
        sequence.OnComplete(() => {
            settingsCanvas.SetActive(true);
        });
    }

    private void HandleBackFromSettings()
    {
        settingsCanvas.SetActive(false);

        Sequence sequence = DOTween.Sequence();

        // Rola de volta para a direita (eixo Z) em dire��o ao ponto central
        sequence.Append(player.DORotate(new Vector3(0, 0, rollRotationAmount), rollDuration, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad));
        sequence.Join(player.DOMove(menuStartPoint.position, rollDuration));
        sequence.Join(mainCamera.transform.DOLookAt(menuStartPoint.position, rollDuration));

        // CORRE��O: Em vez de apenas DOLookAt, vamos mover a c�mera de volta para a posi��o E rota��o originais
        sequence.Join(mainCamera.transform.DOMove(cameraStartPoint.position, rollDuration));
        sequence.Join(mainCamera.transform.DORotate(cameraStartPoint.rotation.eulerAngles, rollDuration));


        // Ao completar, mostra o menu principal novamente
        sequence.OnComplete(() => {
            mainMenuCanvas.SetActive(true);
        });
    }
}