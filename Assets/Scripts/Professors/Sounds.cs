using System;
using UnityEngine;

namespace Professors
{
    public class Sounds : MonoBehaviour
    {
        public static void MakeSound(Sound sound)
        {
            // ReSharper disable once Unity.PreferNonAllocApi
            Collider[] colliders = Physics.OverlapSphere(sound.Position, sound.HearingRange);

            Debug.Log($"nb professors detected : {colliders.Length}");
            foreach (Collider collider in colliders)
            {
                Debug.Log(collider.gameObject.name);
                if (collider.gameObject.layer != LayerMask.NameToLayer("Professor") || !collider.transform.parent.TryGetComponent(out IHear hearer)) continue;

                SoundType soundType =
                    Vector3.Distance(collider.transform.position, sound.Position) < sound.HearingRange / 2
                        ? SoundType.Alerting
                        : SoundType.Interesting;
                
                sound.SetType(soundType);
                hearer.RespondToSound(sound);
            }
        }
    }
}
