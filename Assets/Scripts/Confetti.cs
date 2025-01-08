using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confetti : MonoBehaviour
{
    public List<GameObject> _confetti;

    private void Start()
    {
        StartCoroutine(InvokeConfetti());
    }
    private IEnumerator InvokeConfetti()
    {
        while (true) 
        {
            foreach(var confetti in _confetti)
            {
                yield return new WaitForSeconds(0.25f);
                confetti.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                confetti.gameObject.SetActive(false);
            }
        }
    }
}
