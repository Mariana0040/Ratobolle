using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GerenciadorRestaurante : MonoBehaviour
{
    public static GerenciadorRestaurante Instance { get; private set; }

    [Header("Configurações")]
    public Transform pontoDeSaida;
    [Tooltip("Arraste para cá todos os objetos que têm o script 'Chair'.")]
    public List<Chair> todasAsChairs; // Mantenha esta lista pública para arrastar no Inspector

    [Header("Controle de Paciência")]
    public int maxEntradasNaCozinha = 3;
    private int entradasAtuaisNaCozinha = 0;

    private List<ClienteRatoAI> clientesAtuais = new List<ClienteRatoAI>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // MODIFICADO: O método agora aceita um objeto 'Chair'
    public void RegistrarChair(Chair chair)
    {
        if (!todasAsChairs.Contains(chair))
        {
            todasAsChairs.Add(chair);
        }
    }


    // --- ESTA É A FUNÇÃO CORRIGIDA ---
    public Chair SolicitarCadeiraLivre()
    {
        // 1. Encontra todas as cadeiras que NÃO estão ocupadas
        List<Chair> cadeirasDisponiveis = todasAsChairs.Where(c => !c.estaOcupada).ToList();

        // 2. Se não houver nenhuma, retorna nulo.
        if (cadeirasDisponiveis.Count == 0)
        {
            Debug.LogWarning("Não há mais cadeiras disponíveis para um novo cliente!");
            return null;
        }

        // 3. Escolhe uma cadeira aleatória da lista de disponíveis
        int indiceAleatorio = Random.Range(0, cadeirasDisponiveis.Count);
        Chair cadeiraEscolhida = cadeirasDisponiveis[indiceAleatorio];

        // 4. OCUPA A CADEIRA IMEDIATAMENTE! (Esta é a correção)
        // Antes de entregar a cadeira ao cliente, o gerente já a marca como ocupada.
        cadeiraEscolhida.Ocupar();
        Debug.Log($"Cadeira '{cadeiraEscolhida.gameObject.name}' foi reservada.");

        // 5. Retorna a cadeira já reservada.
        return cadeiraEscolhida;
    }

    // O resto do script continua igual...
    public void RegistrarNovoCliente(ClienteRatoAI novoCliente)
    {
        clientesAtuais.Add(novoCliente);
    }

    public void RegistrarEntradaNaCozinha()
    {
        entradasAtuaisNaCozinha++;
        if (entradasAtuaisNaCozinha > maxEntradasNaCozinha)
        {
            MandarClientesEmbora();
        }
    }

    private void MandarClientesEmbora()
    {
        foreach (var cliente in new List<ClienteRatoAI>(clientesAtuais))
        {
            cliente.IrEmbora();
        }
        clientesAtuais.Clear();
    }
}