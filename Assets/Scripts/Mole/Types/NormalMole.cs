using UnityEngine;

namespace FootballWhackaMolePrototype.Mole
{
    public class NormalMole : BaseMole
    {
        [SerializeField] private float _visibleDuration = 5f;
        [SerializeField] private int _score = 1;

        public override int Score => _score;

        protected override float VisibleDuration => _visibleDuration;
    }
}