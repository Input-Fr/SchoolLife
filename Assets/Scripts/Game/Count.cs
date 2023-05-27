using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public static class Count
    { 
        public static IEnumerable<(int remainingSeconds, WaitForSeconds waitForSeconds)> CountDown(int seconds)
        {
            if (seconds < 0) yield break;
            
            int remainingSeconds = seconds;

            while (remainingSeconds >= 0)
            {
                yield return (remainingSeconds, new WaitForSeconds(1));
                remainingSeconds--;
            }
        }
        
        public static IEnumerable<(int remainingSeconds, WaitForSeconds waitForSeconds)> CountUp(int seconds)
        {
            return CountDown(seconds).Select(T => (seconds - T.remainingSeconds, T.waitForSeconds));
        }
    }
}
