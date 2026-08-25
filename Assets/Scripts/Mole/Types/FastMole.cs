using UnityEngine;

namespace FootballWhackaMolePrototype.Mole
{
    public class FastMole : BaseMole
    {
        [SerializeField] private float _visibleDuration = 3f;
        [SerializeField] private int _score = 3;

        public override int Score => _score;

        protected override float VisibleDuration => _visibleDuration;
    }
}