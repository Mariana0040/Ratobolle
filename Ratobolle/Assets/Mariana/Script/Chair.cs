using UnityEngine;

public class Chair : MonoBehaviour
{
    public bool estaOcupada = false;
    public Transform pontoDeSentar; // <-- ADICIONE ESTA LINHA

    void Start()
    {
        // Ao iniciar, a cadeira se apresenta ao Gerenciador para ser gerenciada.
        if (GerenciadorRestaurante.Instance != null)
        {
            GerenciadorRestaurante.Instance.RegistrarChair(this);
        }
        else
        {
            Debug.LogError("GerenciadorRestaurante não encontrado na cena!", this);
        }

        // Validação para garantir que o ponto de sentar foi atribuído
        if (pontoDeSentar == null)
        {
            Debug.LogWarning("A cadeira " + gameObject.name + " não tem um 'PontoDeSentar' configurado!", this);
        }
    }

    public void Ocupar()
    {
        estaOcupada = true;
    }

    public void Liberar()
    {
        estaOcupada = false;
    }
}