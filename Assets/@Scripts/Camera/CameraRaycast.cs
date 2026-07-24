using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
    public float transparencyAmount = 0.5f; // 투명도 수준 (0.0 ~ 1.0)
    private Color[] originalColors; // 원래 색상을 저장할 배열
    private Renderer[] currentRenderers; // 현재 투명도가 적용된 Renderer 컴포넌트들

    void Update()
    {
        RaycastHit[] hits;
        LayerMask layerMask = LayerMask.GetMask("Thing") | LayerMask.GetMask("Wall");
        hits = Physics.RaycastAll(transform.position, (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized, Mathf.Infinity, layerMask);

        if (hits.Length > 0)
        {
            Debug.DrawRay(transform.position, (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized * 10f, Color.red);

            // 모든 "Thing"과 "Wall" 레이어의 물체들의 투명도를 낮춤
            if (currentRenderers == null || currentRenderers.Length != hits.Length)
            {
                currentRenderers = new Renderer[hits.Length];
                originalColors = new Color[hits.Length];
            }

            for (int i = 0; i < hits.Length; i++)
            {
                int wallLayer = LayerMask.NameToLayer("Wall");
                int thingLayer = LayerMask.NameToLayer("Thing");

                if (hits[i].collider.gameObject.layer == wallLayer || hits[i].collider.gameObject.layer == thingLayer)
                {
                    Renderer renderer = hits[i].collider.gameObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        currentRenderers[i] = renderer;
                        originalColors[i] = renderer.material.color;
                        Color color = renderer.material.color;
                        color.a = transparencyAmount;
                        renderer.material.color = color;
                    }
                }
            }
        }
        else
        {
            // 이전에 투명도가 적용된 물체들의 투명도를 원래대로 되돌림
            if (currentRenderers != null)
            {
                for (int i = 0; i < currentRenderers.Length; i++)
                {
                    if (currentRenderers[i] != null)
                    {
                        currentRenderers[i].material.color = originalColors[i];
                    }
                }
                currentRenderers = null;
                originalColors = null;
            }
        }
    }
}