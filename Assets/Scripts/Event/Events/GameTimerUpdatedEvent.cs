using UnityEngine;

namespace FootballWhackaMolePrototype.Event
{
    public class GameTimerUpdatedEvent
    {
        public readonly float RemainingTime;

        public GameTimerUpdatedEvent(float remainingTime)
        {
            RemainingTime = remainingTime;
        }
    }
}
