using UnityEngine;
 
public class TreadScroll : MonoBehaviour
{
    public Renderer rend; 
    public float scrollY = 0;
    public float speedMultiplier = 1;
    public EnemyBehavior behavior;
    void Update()
    {
        scrollY += Time.deltaTime * behavior.pathfinder.velocity.magnitude * speedMultiplier; 
        rend.material.SetTextureOffset("_BaseMap", new Vector2(0, scrollY));
    }
}
