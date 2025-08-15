using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjetilTomate : MonoBehaviour
{
    public Transform pontoDeRespawn;

    void Start()
    {
        Destroy(gameObject, 5f); // O tomate se autodestrói após 5 segundos
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (pontoDeRespawn != null)
            {
                collision.transform.position = pontoDeRespawn.position;
            }
        }

        // Destroi o tomate ao colidir com qualquer coisa (exceto o próprio chefe)
        // Dê ao seu chefe uma tag "Enemy" para evitar que o tomate se destrua nele mesmo
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}