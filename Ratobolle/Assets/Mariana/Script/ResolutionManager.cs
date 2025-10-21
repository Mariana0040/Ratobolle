using UnityEngine;
using TMPro; // Namespace do Text Mesh Pro
using UnityEngine.UI;
public class ResolutionManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown; // Dropdown do TMP
    public Toggle fullscreenToggle; // Toggle do TMP

    // Resoluções pré-definidas
    private Resolution[] resolutions = new Resolution[]
    {
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 800, height = 600 },
        new Resolution { width = 640, height = 480 }
    };

    void Start()
    {
        ConfigureDropdown();
        ConfigureFullscreenToggle();
    }

    void ConfigureDropdown()
    {
        resolutionDropdown.ClearOptions(); // Limpa opções existentes

        // Cria novas opções
        var options = new System.Collections.Generic.List<string>();
        foreach (var res in resolutions)
        {
            options.Add($"{res.width}x{res.height}");
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = 0; // Seleciona a primeira opção
        resolutionDropdown.RefreshShownValue();
    }

    public void ConfigureFullscreenToggle()
    {
        //fullscreenToggle.isOn = Screen.fullScreen; // Sincroniza com o estado atual
    }

    // Chamado quando o dropdown muda
    public void SetResolution(int index)
    {
        Resolution selected = resolutions[index];
        Screen.SetResolution(selected.width, selected.height, Screen.fullScreen);
    }

    // Chamado quando o toggle muda
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        // Atualiza a resolução para aplicar imediatamente
        Resolution current = resolutions[resolutionDropdown.value];
        Screen.SetResolution(current.width, current.height, isFullscreen);
    }
}