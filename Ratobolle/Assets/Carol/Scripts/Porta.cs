using UnityEngine;

public class Porta : MonoBehaviour
{
    public Transform porta;
    public float anguloAberto = 90f;
    public float velocidade = 2f;

    private bool aberta = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            aberta = !aberta;
        }

        Quaternion rotacaoAlvo = Quaternion.Euler(0, aberta ? anguloAberto : 0, 0);
        porta.localRotation = Quaternion.Slerp(porta.localRotation, rotacaoAlvo, Time.deltaTime * velocidade);
    }
}
