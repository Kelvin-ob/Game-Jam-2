using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraRendererSwitcher : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    [SerializeField] private int normalRenderer = 0;
    [SerializeField] private int pcCamRenderer = 1;

    public void UseNormalRenderer()
    {
        SetRenderer(normalRenderer);
    }

    public void UsePCRenderer()
    {
        SetRenderer(pcCamRenderer);
    }

    private void SetRenderer(int rendererIndex)
    {
        UniversalAdditionalCameraData cameraData =
            targetCamera.GetComponent<UniversalAdditionalCameraData>();

        cameraData.SetRenderer(rendererIndex);
    }
}