using UnityEngine;
using System.Collections.Generic;

namespace FootballWhackaMolePrototype.Mole
{
    public class MolePoolService
    {
        private readonly NormalMole _normalMolePrefab;
        private readonly FastMole _fastMolePrefab;

        private readonly Transform _poolContainer;

        private readonly Queue<NormalMole> _normalPool = new();
        private readonly Queue<FastMole> _fastPool = new();


        public MolePoolService(MoleData_SO moleData_SO, Transform poolContainer)
        {
            _normalMolePrefab = moleData_SO.normalMolePrefab;
            _fastMolePrefab = moleData_SO.fastMolePrefab;
            _poolContainer = poolContainer;
        }

        public BaseMole GetMole(MoleTypeEnum moleType)
        {
            switch (moleType)
            {
                case MoleTypeEnum.Normal:
                    return GetNormalMole();

                case MoleTypeEnum.Fast:
                    return GetFastMole();

                default:
                    return null;
            }
        }

        private NormalMole GetNormalMole()
        {
            NormalMole mole;

            if (_normalPool.Count > 0)
            {
                mole = _normalPool.Dequeue();
            }
            else
            {
                mole = Object.Instantiate(_normalMolePrefab, _poolContainer);
            }

            mole.gameObject.SetActive(true);
            return mole;
        }

        private FastMole GetFastMole()
        {
            FastMole mole;

            if (_fastPool.Count > 0)
            {
                mole = _fastPool.Dequeue();
            }
            else
            {
                mole = Object.Instantiate(_fastMolePrefab,_poolContainer);
            }

            mole.gameObject.SetActive(true);
            return mole;
        }

        public void ReturnMole(BaseMole mole)
        {
            if (mole == null)
                return;

            mole.gameObject.SetActive(false);
            mole.transform.SetParent(_poolContainer);
            mole.transform.localPosition = Vector3.zero;
            mole.transform.localRotation = Quaternion.identity;

            if (mole is FastMole fastMole)
            {
                _fastPool.Enqueue(fastMole);
            }
            else if (mole is NormalMole normalMole)
            {
                _normalPool.Enqueue(normalMole);
            }
        }
    }
}
