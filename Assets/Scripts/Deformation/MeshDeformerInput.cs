using System.Collections;
using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 10f; // Kuvvet miktarı
    public float radius = 1f; // Etki yarıçapı
    public float waveSpeed = 1f; // Dalga hızı
    public float waveAmplitude = 1f; // Dalga genliği

    private Coroutine waveCoroutine; // Mevcut dalga yayılma işlemi Coroutine'i

    private Vector3[] vertices;
    private Vector3[] originalVertices;

    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;
            vertices = mesh.vertices;
            originalVertices = mesh.vertices.Clone() as Vector3[];
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Her tıklamada bir defa çalışır
        {
            if (waveCoroutine != null)
            {
                // Eğer mevcut dalga yayılma işlemi varsa, iptal et
                StopCoroutine(waveCoroutine);
            }

            HandleInput();
        }

        // Mesh yüzeyini dalga denklemine göre güncelle
        ApplyWaveEquation();
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if (deformer)
            {
                Vector3 point = hit.point;
                point += hit.normal * 0.1f;

                Vector3 oppositePoint = GetOppositeCorner(hit.collider.bounds, point);
                waveCoroutine = StartCoroutine(ApplyWaveEffect(point, oppositePoint));
            }
        }
    }

    Vector3 GetOppositeCorner(Bounds bounds, Vector3 point)
    {
        Vector3 oppositeCorner = new Vector3(
            Mathf.Lerp(bounds.min.x, bounds.max.x, 1f - Mathf.InverseLerp(bounds.min.x, bounds.max.x, point.x)),
            Mathf.Lerp(bounds.min.y, bounds.max.y, 1f - Mathf.InverseLerp(bounds.min.y, bounds.max.y, point.y)),
            Mathf.Lerp(bounds.min.z, bounds.max.z, 1f - Mathf.InverseLerp(bounds.min.z, bounds.max.z, point.z))
        );
        return oppositeCorner;
    }

    IEnumerator ApplyWaveEffect(Vector3 start, Vector3 end)
    {
        float elapsedTime = 0f;
        float duration = 1f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime * waveSpeed; // waveSpeed ile çarpılarak dalganın hızı etkilenir.
            float falloff = CalculateFalloff(elapsedTime, duration);
            Vector3 waveCenter = Vector3.Lerp(start, end, falloff * waveAmplitude); // Dalga genliği etkisini artırmak için waveAmplitude ile çarpın.

            Collider[] colliders = Physics.OverlapSphere(waveCenter, radius);
            foreach (var collider in colliders)
            {
                MeshDeformer deformer = collider.GetComponent<MeshDeformer>();
                if (deformer)
                {
                    float waveForce = force * falloff;
                    deformer.AddDeformingForce(waveCenter, waveForce, radius);
                }
            }
            yield return null;
        }
    }

    float CalculateFalloff(float t, float duration)
    {
        float halfDuration = duration / 2f;
        float distance = Mathf.Abs(t - halfDuration);
        return 1f - distance / halfDuration;
    }

    void ApplyWaveEquation()
    {
        if (vertices == null || originalVertices == null || vertices.Length != originalVertices.Length)
        {
            // Dizi referansları doğrulanamadı, işlemi durdur.
            return;
        }

        float deltaTime = Time.deltaTime;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 originalPosition = originalVertices[i];
            Vector3 newPosition = new Vector3(
                originalPosition.x,
                originalPosition.y,
                originalPosition.z + CalculateWaveHeight(originalPosition, Time.time)
            );
            vertices[i] = newPosition;
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            Mesh mesh = meshFilter.mesh;
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }
    }

    float CalculateWaveHeight(Vector3 position, float time)
    {
        float c = waveSpeed;
        float x = position.x;
        float y = position.y;
        float t = time;
        float u = waveAmplitude;

        float waveHeight = u * Mathf.Sin(c * t - c * x - c * y);
        return waveHeight;
    }
}
