using UnityEngine;

// Este script é apenas um marcador. Não precisa de lógica no Update.
// Ele nos ajuda a encontrar o ponto de início em cada cena.
public class PlayerSpawnPoint : MonoBehaviour
{
    // Desenha um Gizmo na Scene View para facilitar a visualização
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // Desenha uma esfera na posição do objeto
        Gizmos.DrawSphere(transform.position, 0.5f);
        // Desenha uma seta azul apontando na direção "para frente" do spawn
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }
}