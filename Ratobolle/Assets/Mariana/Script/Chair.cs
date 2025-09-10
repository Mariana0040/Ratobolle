using UnityEngine;

public class Chair : MonoBehaviour
{
    public bool estaOcupada = false;

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