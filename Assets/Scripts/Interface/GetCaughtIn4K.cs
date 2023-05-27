using System;
using UnityEngine;

namespace Interface
{
    public class GetCaughtIn4K : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
