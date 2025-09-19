using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class InteractiveMenuController : MonoBehaviour
{
    [Header("Referências Principais")]
    public MenuPlayerRoller playerRoller;
    public Camera mainCamera;

    [Header("Pontos de Navegação")]
    public Transform menuStartPoint;
    public Transform settingsPoint;
    public Transform tutorialPoint;
    public Transform gameStartPoint;

    [Header("Pontos de Posição da Câmera")]
    public Transform cameraStartPoint;
    public Transform cameraSettingsPoint;

    [Header("Referências de UI")]
    public GameObject mainMenuCanvas;
    public GameObject settingsCanvas;
    public Button startGameButton;
    public Button tutorialButton;
    public Button settingsButton;
    public Button backFromSettingsButton;

    private const string TutorialCompletedKey = "TutorialCompleted";
    private bool isAnimating = false;

    void Start()
    {
        Time.timeScale = 1f;
        playerRoller.transform.position = menuStartPoint.position;
        playerRoller.transform.rotation = menuStartPoint.rotation;
        mainCamera.transform.position = cameraStartPoint.position;
        mainCamera.transform.rotation = cameraStartPoint.rotation;

        settingsCanvas.SetActive(false);
        bool tutorialCompleted = PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1;
        startGameButton.interactable = tutorialCompleted;

        startGameButton.onClick.AddListener(HandleStartGame);
        tutorialButton.onClick.AddListener(HandleTutorial);
        settingsButton.onClick.AddListener(HandleSettings);
        backFromSettingsButton.onClick.AddListener(HandleBackFromSettings);

        // Inicia o rolamento ocioso no começo do jogo
        playerRoller.StartIdleRoll();
    }

    private void HandleSettings()
    {
        if (isAnimating) return;
        StartCoroutine(SettingsCoroutine());
    }

    private IEnumerator SettingsCoroutine()
    {
        isAnimating = true;
        mainMenuCanvas.SetActive(false);

        // O rato rola para o ponto de configurações
        bool playerAnimationFinished = false;
        playerRoller.AnimateToPoint(settingsPoint, () => {
            playerAnimationFinished = true;
        });

        // Câmera se move para a posição de observação
        mainCamera.transform.DOMove(cameraSettingsPoint.position, 1.8f).SetEase(Ease.OutSine);
        mainCamera.transform.DOLookAt(settingsPoint.position, 1.8f).SetEase(Ease.OutSine);

        yield return new WaitUntil(() => playerAnimationFinished);

        // Garante a rotação final e RECOMEÇA o rolamento ocioso no novo local
        playerRoller.AnimateRotationTo(menuStartPoint.rotation, 0.5f);
        yield return new WaitForSeconds(0.5f);
        playerRoller.StartIdleRoll();

        settingsCanvas.SetActive(true);
        isAnimating = false;
    }

    public void HandleBackFromSettings()
    {
        if (isAnimating) return;
        StartCoroutine(BackFromSettingsCoroutine());
    }

    private IEnumerator BackFromSettingsCoroutine()
    {
        isAnimating = true;
        settingsCanvas.SetActive(false);

        // Câmera volta para a posição inicial
        mainCamera.transform.DOMove(cameraStartPoint.position, 1.5f);
        mainCamera.transform.DORotate(cameraStartPoint.rotation.eulerAngles, 1.5f);

        // Rato rola de volta para o ponto inicial
        bool playerAnimationFinished = false;
        playerRoller.AnimateToPoint(menuStartPoint, () => {
            playerAnimationFinished = true;
        });

        yield return new WaitUntil(() => playerAnimationFinished);

        // Garante a rotação final e RECOMEÇA o rolamento ocioso no local inicial
        playerRoller.AnimateRotationTo(menuStartPoint.rotation, 0.2f);
        yield return new WaitForSeconds(0.2f);
        playerRoller.StartIdleRoll();

        mainMenuCanvas.SetActive(true);
        isAnimating = false;
    }

    // As outras funções (HandleTutorial, HandleStartGame) chamam AnimateToPoint ou AnimateToCamera,
    // que já cuidam de parar o rolamento ocioso, então elas não precisam de mudanças.

    private void HandleTutorial() { if (isAnimating) return; StartCoroutine(TutorialCoroutine()); }
    private void HandleStartGame() { if (isAnimating) return; StartCoroutine(StartGameCoroutine()); }

    private IEnumerator TutorialCoroutine()
    {
        isAnimating = true;
        mainMenuCanvas.SetActive(false);
        mainCamera.transform.DOMove(mainCamera.transform.position + playerRoller.transform.forward * 2f, 1.8f).SetEase(Ease.InSine);

        bool animationFinished = false;
        playerRoller.AnimateToCamera(mainCamera, () => { animationFinished = true; });
        yield return new WaitUntil(() => animationFinished);

        SceneTransitionData.SetData(playerRoller.transform, mainCamera.transform);
        yield return StartCoroutine(LoadTutorialScene());
    }

    private IEnumerator StartGameCoroutine()
    {
        isAnimating = true;
        mainMenuCanvas.SetActive(false);
        bool animationFinished = false;
        playerRoller.AnimateToPoint(gameStartPoint, () => { animationFinished = true; });
        yield return new WaitUntil(() => animationFinished);

        mainCamera.transform.SetParent(playerRoller.transform);
        mainCamera.transform.DOLocalMove(new Vector3(0, 0.8f, 0.5f), 1f);
        mainCamera.transform.DOLocalRotate(Vector3.zero, 1f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("FASE UM");
    }

    private IEnumerator LoadTutorialScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("CenaM&C", LoadSceneMode.Additive);
        while (!asyncLoad.isDone) yield return null;
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
    }
}