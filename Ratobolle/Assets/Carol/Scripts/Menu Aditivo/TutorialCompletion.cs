using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialCompletion : MonoBehaviour
{
    private const string TutorialCompletedKey = "TutorialCompleted";

    // Chame este m�todo a partir de um bot�o "Concluir" no final do seu tutorial
    public void CompleteTutorial()
    {
        // 1. Salva o progresso indicando que o tutorial foi feito
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
        PlayerPrefs.Save();

        // 2. Descarrega a cena do tutorial
        SceneManager.UnloadSceneAsync("SuaCenaDeTutorial"); // SUBSTITUA O NOME

        // 3. Recarrega a cena do menu para que ela atualize o estado do bot�o "Iniciar"
        SceneManager.LoadScene("SuaCenaDeMenu"); // SUBSTITUA O NOME
    }
}