using System.Collections;
using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 10f; // Kuvvet miktarı
    public float radius = 1f; // Etki yarıçapı

    private Coroutine waveCoroutine; // Mevcut dalga yayılma işlemi Coroutine'i

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
                point += hit.normal * 0.1f; // Deformasyonun daha belirgin olması için normalin yönünde bir ofset ekliyoruz.

                // İlk tıklanan köşeden aynı düzlemdeki uç köşeye doğru dalga yayılımı
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
        float duration = 1f; // 1 saniyede dalga yayılacak
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float falloff = CalculateFalloff(elapsedTime, duration);
            Vector3 waveCenter = Vector3.Lerp(start, end, falloff);

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
}
