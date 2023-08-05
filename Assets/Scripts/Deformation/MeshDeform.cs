using UnityEngine;

public class MeshDeformer : MonoBehaviour
{
    public float springForce = 20f; // Yay kuvveti
    public float damping = 5f; // Yavaşlatma katsayısı
    public float maxDepression = 0.5f; // En fazla çökme miktarı

    private Mesh deformingMesh;
    private Vector3[] originalVertices, displacedVertices;
    private Vector3[] vertexVelocities;

    private void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
        vertexVelocities = new Vector3[originalVertices.Length];
    }

    public void AddDeformingForce(Vector3 point, float force, float radius)
    {
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            ApplyForceToVertex(i, point, force, radius);
        }
    }

    private void ApplyForceToVertex(int i, Vector3 point, float force, float radius)
    {
        Vector3 vertex = displacedVertices[i];
        float sqrMagnitude = (vertex - point).sqrMagnitude;
        if (sqrMagnitude > radius * radius)
        {
            return;
        }

        float distance = Mathf.Sqrt(sqrMagnitude);
        float falloff = CalculateFalloff(distance, radius);

        Vector3 velocity = (force * falloff) * (point - vertex).normalized;
        vertexVelocities[i] += velocity;
    }

    private float CalculateFalloff(float distance, float radius)
    {
        // Force'u uzaklığa göre hesaplayan formül
        float normalizedDistance = distance / radius;
        return 1f / (1f + normalizedDistance * normalizedDistance);
    }

    private void Update()
    {
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    private void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        displacement -= velocity * Time.deltaTime;
        displacedVertices[i] = originalVertices[i] + displacement * (1 - damping * Time.deltaTime);
        vertexVelocities[i] = Vector3.zero;
    }
}
