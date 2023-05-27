using UnityEngine;

namespace Professors
{
    public enum SoundType
    {
        Default = -1,
        Interesting,
        Alerting
    }
    public class Sound
    {
        public readonly Vector3 Position;
        public readonly float HearingRange;
        private SoundType _type;

        public Sound(Vector3 pos, float range)
        {
            Position = pos;
            HearingRange = range;
        }

        public void SetType(SoundType soundType)
        {
            _type = soundType;
        }

        public SoundType GetSoundType()
        {
            return _type;
        }
    }
}
