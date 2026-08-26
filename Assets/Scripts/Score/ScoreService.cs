using UnityEngine;
using FootballWhackaMolePrototype.Event;

namespace FootballWhackaMolePrototype.Score
{
    public class ScoreService
    {
        private int _currentScore;
        private EventBusService _eventBusServiceObj;

        public ScoreService(EventBusService eventBusService)
        {
            _eventBusServiceObj = eventBusService;
        }

        public void UpdateScore(int value)
        {
            _currentScore += value;
            RaiseScoreUpdatedEvent();
        }

        public void ResetScore()
        {
            _currentScore = 0;
            RaiseScoreUpdatedEvent();
        }

        private void RaiseScoreUpdatedEvent()
        {
            _eventBusServiceObj.Publish(new ScoreUpdatedEvent(_currentScore));
        }
    }
}
