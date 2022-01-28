using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public static class ConfettiManager
    {
        public static Transform CreateConfettiBlast(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Transform confetti = ObjectPooler.Instance.SpawnFromPool("Confetti Blast", position, Quaternion.Euler(rotation)).transform;
            confetti.localScale = scale;
            return confetti;
        }
        public static Transform CreateConfettiDirectional(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Transform confetti = ObjectPooler.Instance.SpawnFromPool("Confetti Directional", position, Quaternion.Euler(rotation)).transform;
            confetti.localScale = scale;
            return confetti;
        }
    }
}

