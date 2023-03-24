using UnityEngine;

public class Paralax : MonoBehaviour
{
    [SerializeField] private Transform[] backgrounds;
    [SerializeField] private float[] parallaxScales;
    [SerializeField] private float smoothing = 1f;

    private Transform cameraTransform;
    private Vector3 previousCameraPosition;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
    }

    private void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float parallax = (previousCameraPosition.x - cameraTransform.position.x) * parallaxScales[i];
            float backgroundTargetPositionX = backgrounds[i].position.x + parallax;
            Vector3 backgroundTargetPosition = new Vector3(backgroundTargetPositionX, backgrounds[i].position.y, backgrounds[i].position.z);
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPosition, smoothing * Time.deltaTime);
        }
        previousCameraPosition = cameraTransform.position;
    }
}
