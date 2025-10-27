using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Demo;
using System.IO;

public class HeadTracker : WebCamera
{
    public TextAsset faceCascade;
    private CascadeClassifier faceDetector;
    private Vector2 headCenter = Vector2.zero;
    private float headSize = 0f;

    public Transform playerCapsule;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float zoomSensitivity = 10f;
    public float rotationSensitivity = 40f;
    public float smoothFactor = 0.1f;

    private float targetZ = 0f;
    private float targetX = 0f;
    private Quaternion targetRotation;

    private float baseFaceSize = 400f;

    protected override void Awake()
    {
        base.Awake();
        base.forceFrontalCamera = true;

        string tempPath = Path.Combine(Application.persistentDataPath, "temp_haarcascade.xml");
        File.WriteAllBytes(tempPath, faceCascade.bytes);
        faceDetector = new CascadeClassifier(tempPath);
    }

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        Mat mat = OpenCvSharp.Unity.TextureToMat(input, TextureParameters);
        var gray = new Mat();
        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

        var faces = faceDetector.DetectMultiScale(gray, 1.1, 4, HaarDetectionType.ScaleImage, new Size(60, 60));

        if (faces.Length > 0)
        {
            var face = faces[0];
            headCenter = new Vector2(face.X + face.Width / 2f, face.Y + face.Height / 2f);
            headSize = face.Width;
            Cv2.Rectangle(mat, face, new Scalar(0, 255, 0), 2);
        }

        output = OpenCvSharp.Unity.MatToTexture(mat, output);

        UpdateCapsuleMovement(input);
        return true;
    }

    private void UpdateCapsuleMovement(WebCamTexture input)
    {
        if (playerCapsule == null || input == null) return;

        float moveX = (headCenter.x - (input.width / 2f)) / (input.width / 2f);
        float moveY = (headCenter.y - (input.height / 2f)) / (input.height / 2f);

        float rotX = Mathf.Clamp(-moveY * rotationSensitivity, -25f, 25f);
        float rotY = Mathf.Clamp(moveX * rotationSensitivity * 2f, -45f, 45f);
        targetRotation = Quaternion.Euler(rotX, rotY, 0);

        targetX = Mathf.Clamp(moveX * moveSpeed, -10f, 10f);

        if (headSize > 0)
        {
            float distanceFactor = (headSize - baseFaceSize) / baseFaceSize;
            targetZ = Mathf.Clamp(distanceFactor * zoomSensitivity, -10f, 10f);
        }

        Vector3 targetPosition = new Vector3(targetX, playerCapsule.position.y, targetZ);
        playerCapsule.position = Vector3.Lerp(playerCapsule.position, targetPosition, smoothFactor);
        playerCapsule.rotation = Quaternion.Slerp(playerCapsule.rotation, targetRotation, smoothFactor);
    }


}