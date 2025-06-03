using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraGreyScale : MonoBehaviour
{
    public Material grayscaleMaterial;
    private bool useGrayscale = false;

    // Public method to toggle grayscale
    public void SetGreyscale(bool active)
    {
        useGrayscale = active;
    }

    // Called by Unity after the camera finishes rendering
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (useGrayscale && grayscaleMaterial != null)
            Graphics.Blit(src, dest, grayscaleMaterial);
        else
            Graphics.Blit(src, dest);
    }
}
