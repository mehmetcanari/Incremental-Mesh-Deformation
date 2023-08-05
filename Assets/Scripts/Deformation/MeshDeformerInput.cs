using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 10f; // Kuvvet miktarı
    public float radius = 1f; // Etki yarıçapı

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
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
                deformer.AddDeformingForce(point, force, radius);
            }
        }
    }
}