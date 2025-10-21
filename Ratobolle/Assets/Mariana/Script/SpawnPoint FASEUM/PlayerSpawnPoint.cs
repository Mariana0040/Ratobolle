using UnityEngine;

// Este script � apenas um marcador. N�o precisa de l�gica no Update.
// Ele nos ajuda a encontrar o ponto de in�cio em cada cena.
public class PlayerSpawnPoint : MonoBehaviour
{
    // Desenha um Gizmo na Scene View para facilitar a visualiza��o
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // Desenha uma esfera na posi��o do objeto
        Gizmos.DrawSphere(transform.position, 0.5f);
        // Desenha uma seta azul apontando na dire��o "para frente" do spawn
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }
}