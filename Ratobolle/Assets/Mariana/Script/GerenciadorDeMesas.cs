using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GerenciadorDeMesas : MonoBehaviour
{
    // Usaremos um padr�o Singleton para que qualquer cliente possa acessar este script facilmente.
    public static GerenciadorDeMesas instance;

    private List<Cadeira> todasAsCadeiras = new List<Cadeira>();

    void Awake()
    {
        // Configura��o do Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // As cadeiras se registram com o gerente quando o jogo come�a.
    public void RegistrarCadeira(Cadeira cadeira)
    {
        if (!todasAsCadeiras.Contains(cadeira))
        {
            todasAsCadeiras.Add(cadeira);
        }
    }

    // O cliente chama esta fun��o para pedir um lugar para sentar.
    public Cadeira SolicitarCadeiraDisponivel()
    {
        // Procura por todas as cadeiras que n�o est�o ocupadas.
        List<Cadeira> cadeirasLivres = todasAsCadeiras.Where(c => !c.estaOcupada).ToList();

        if (cadeirasLivres.Count > 0)
        {
            // Pega uma cadeira aleat�ria da lista de cadeiras livres.
            int indiceAleatorio = Random.Range(0, cadeirasLivres.Count);
            Cadeira cadeiraEscolhida = cadeirasLivres[indiceAleatorio];

            // Marca a cadeira como ocupada para que ningu�m mais a pegue.
            cadeiraEscolhida.Ocupar();
            return cadeiraEscolhida;
        }

        // Retorna nulo se n�o houver cadeiras livres.
        Debug.LogWarning("N�o h� cadeiras dispon�veis no restaurante!");
        return null;
    }
}