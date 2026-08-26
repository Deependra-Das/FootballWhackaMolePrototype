using UnityEngine;

namespace FootballWhackaMolePrototype.Event
{
    public class ScoreUpdatedEvent
    {
        public readonly int CurrentScore;

        public ScoreUpdatedEvent(int currentScore)
        {
            CurrentScore = currentScore;
        }
    }
}