using UnityEngine;

public class Cadeira : MonoBehaviour
{
    public bool estaOcupada = false;

    void Start()
    {
        // Ao iniciar, a cadeira se apresenta ao "Ma�tre" para ser gerenciada.
        if (GerenciadorDeMesas.instance != null)
        {
            GerenciadorDeMesas.instance.RegistrarCadeira(this);
        }
        else
        {
            Debug.LogError("GerenciadorDeMesas n�o encontrado na cena!", this);
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