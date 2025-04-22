using UnityEngine;
using System.Collections;

public class StickBehaviour : MonoBehaviour
{
    public Transform targetLocation;
    public bool moveLeft;
    public bool shouldGiveTrigger;
    public float timeToMove;
    public GameManager gm;

    private void Start()
    {
        StartCoroutine(MoveOverTime());
    }

    IEnumerator MoveOverTime()
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(targetLocation.position.x, startPos.y, startPos.z);

        while (elapsedTime < timeToMove)
        {
            float t = elapsedTime / timeToMove;
            transform.position = Vector3.Lerp(startPos, targetPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exact
        if (shouldGiveTrigger)
        {
            gm.PlayAudioOneShot();
        }
        Destroy(this.gameObject);
    }
}
