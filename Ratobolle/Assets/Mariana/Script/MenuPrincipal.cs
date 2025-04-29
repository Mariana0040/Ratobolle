using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Elementos UI")]
    public GameObject painelPrincipal;
    public GameObject painelAudio;
    public GameObject painelConfig;

    [Header("Controles de �udio")]
    public AudioMixer mixerAudio;
    public Slider sliderVolume;

    [Header("Bot�es")]
    public Button btnContinuar;
    public Button btnMoveBogo;
    public Button btnSair;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Configura os listeners dos Bot�es
        btnContinuar.onClick.AddListener(Continuar);
        btnMoveBogo.onClick.AddListener(MoveBogo);
        btnSair.onClick.AddListener(Sair);

        // Inicia com apenas o painel principal vis�vel
        painelPrincipal.SetActive(true);
        painelAudio.SetActive(false);
        painelConfig.SetActive(false);

        // Configura o slider de volume
        if (sliderVolume != null && mixerAudio != null)
        {
            sliderVolume.onValueChanged.AddListener(AlterarVolume);
        }
    }

    public void AbrirAudioController()
    {
        painelPrincipal.SetActive (false);
        painelAudio.SetActive(true);
    }

    public void AbrirConfiguracoes()
    {
        painelPrincipal.SetActive(false);
         painelConfig.SetActive (true);
    }

    public void VoltarMenuPrincipal()
    {
        painelPrincipal.SetActive (true);
        painelAudio.SetActive(false);
        painelConfig.SetActive(false);
    }

    // M�todos para as a��es dos bot�es
    private void Continuar()
    {
        Debug.Log("Continuar pressionado");
    }
   
    private void MoveBogo()
    {
        Debug.Log("Move Bogo pressionado");
    }
    // M�todos para as a��es dos bot�es
    private void Sair()
    {
        Debug.Log("Sair pressionado");
        Application.Quit();

        // Para testar no editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // M�todo para controle de volume
    private void AlterarVolume(float valor)
    {
        mixerAudio.SetFloat("VolumeMaster", Mathf.Log10(valor) * 20);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
