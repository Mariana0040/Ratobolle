using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GerenciadorDeMesas : MonoBehaviour
{
    // Usaremos um padrão Singleton para que qualquer cliente possa acessar este script facilmente.
    public static GerenciadorDeMesas instance;

    private List<Cadeira> todasAsCadeiras = new List<Cadeira>();

    void Awake()
    {
        // Configuração do Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // As cadeiras se registram com o gerente quando o jogo começa.
    public void RegistrarCadeira(Cadeira cadeira)
    {
        if (!todasAsCadeiras.Contains(cadeira))
        {
            todasAsCadeiras.Add(cadeira);
        }
    }

    // O cliente chama esta função para pedir um lugar para sentar.
    public Cadeira SolicitarCadeiraDisponivel()
    {
        // Procura por todas as cadeiras que não estão ocupadas.
        List<Cadeira> cadeirasLivres = todasAsCadeiras.Where(c => !c.estaOcupada).ToList();

        if (cadeirasLivres.Count > 0)
        {
            // Pega uma cadeira aleatória da lista de cadeiras livres.
            int indiceAleatorio = Random.Range(0, cadeirasLivres.Count);
            Cadeira cadeiraEscolhida = cadeirasLivres[indiceAleatorio];

            // Marca a cadeira como ocupada para que ninguém mais a pegue.
            cadeiraEscolhida.Ocupar();
            return cadeiraEscolhida;
        }

        // Retorna nulo se não houver cadeiras livres.
        Debug.LogWarning("Não há cadeiras disponíveis no restaurante!");
        return null;
    }
}