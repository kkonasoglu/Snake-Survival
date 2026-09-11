using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SnakeMovement))]
public class SnakeVisualState : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite straightBodySprite;
    [SerializeField] private Sprite cornerBodySprite;
    [SerializeField] private Sprite tailSprite;

    private SnakeMovement snakeMovement;

    private void Awake()
    {
        snakeMovement = GetComponent<SnakeMovement>();
    }
    private void LateUpdate()
    {
        UpdateHeadRotation();
        UpdateBodySprites();
    }

    private void UpdateHeadRotation()
    {
        Vector2 dir = snakeMovement.CurrentDirection;
        float angle = 0f;

        if(dir == Vector2.up) angle = 0f;
        else if(dir == Vector2.down) angle = 180f;
        else if(dir == Vector2.left) angle = 90f;
        else if(dir == Vector2.right) angle = -90f;

        transform.rotation = Quaternion.Euler(0,0,angle);
    }

    private void UpdateBodySprites()
    {
        IReadOnlyList<Transform> segments = snakeMovement.Segments;
        for(int i = 1; i< segments.Count; i++)
        {
            SpriteRenderer sr = segments[i].GetComponent<SpriteRenderer>();
            if(sr == null) continue;
            if(i == segments.Count - 1)
            {
                sr.sprite = tailSprite;
                Vector2 toBack = (Vector2)(segments[i].position - segments[i-1].position);
                sr.transform.rotation = Quaternion.Euler(0,0,GetDirectionAngle(toBack));
            }
            else
            {
                Vector2 toPrev = (Vector2)(segments[i-1].position - segments[i].position);
                Vector2 toNext = (Vector2)(segments[i+1].position - segments[i].position);

                if(toPrev.x == toNext.x ||toPrev.y == toNext.y)
                {
                    sr.sprite = straightBodySprite;
                    sr.transform.rotation = Quaternion.Euler(0,0, toPrev.x != 0 ? 90f : 0f);
                }
                else
                {
                    sr.sprite  = cornerBodySprite;
                    sr.transform.rotation = Quaternion.Euler(0,0, GetCornerAngle(toPrev, toNext));
                }
            }
        }
    }

    private float GetDirectionAngle(Vector2 dir)
    {
        if(dir == Vector2.up) return 0f;
        if(dir == Vector2.down) return 180f;
        if(dir == Vector2.left) return 90f;
        if(dir == Vector2.right) return -90f;
        return 0f;
    }

    private float GetCornerAngle(Vector2 toPrev, Vector2 toNext)
    {
        Vector2 sum = toPrev + toNext;
        if(sum == new Vector2(-1,-1)) return 0f;
        if(sum == new Vector2(1,-1)) return 90f;
        if(sum == new Vector2(1,1)) return 180f;
        if(sum == new Vector2(-1,1)) return -90f;
        return 0f;
    }

    


    
}
