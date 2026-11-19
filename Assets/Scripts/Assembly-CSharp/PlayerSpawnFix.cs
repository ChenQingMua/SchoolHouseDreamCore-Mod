using UnityEngine;

public class PlayerSpawnFix : MonoBehaviour
{
    public float randomOffsetRadius = 20f;

    void Start()
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        Vector3 randomOffset = new Vector3(Random.Range(-randomOffsetRadius, randomOffsetRadius), 0, Random.Range(-randomOffsetRadius, randomOffsetRadius));
        transform.position += randomOffset;

        bool valid = false;

        for (int i = 0; i < 30; i++)
        {
            if (IsPositionFree(cc))
            {
                valid = true;
                break;
            }
            transform.position += new Vector3(0, 3f, 0);
        }

        if (!valid)
        {
            transform.position += new Vector3(0, 15f, 0);
        }

        if (cc != null) cc.enabled = true;
    }

    private bool IsPositionFree(CharacterController cc)
    {
        if (cc == null)
        {
            Collider[] overlaps = Physics.OverlapCapsule(transform.position + Vector3.up * 1f, transform.position + Vector3.up * 2f, 1f);
            foreach (var col in overlaps)
            {
                if (col.gameObject != gameObject && !col.isTrigger) return false;
            }
            return true;
        }

        Vector3 originalPos = transform.position;
        cc.Move(Vector3.up * 0.001f);

        bool isHitting = (cc.collisionFlags & CollisionFlags.Sides) != 0 || (cc.collisionFlags & CollisionFlags.Above) != 0;

        transform.position = originalPos;

        return !isHitting;
    }
}
