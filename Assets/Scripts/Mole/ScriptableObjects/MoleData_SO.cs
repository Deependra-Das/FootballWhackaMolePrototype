using UnityEngine;

namespace FootballWhackaMolePrototype.Mole
{
    [CreateAssetMenu(fileName = "MoleData_SO", menuName = "Scriptable Objects/MoleData_SO")]
    public class MoleData_SO : ScriptableObject
    {
        public NormalMole normalMolePrefab;
        public FastMole fastMolePrefab;
    }
}
