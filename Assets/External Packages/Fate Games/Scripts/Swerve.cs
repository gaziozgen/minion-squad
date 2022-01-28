using UnityEngine;
using UnityEngine.Events;
namespace FateGames
{
    public class Swerve : MonoBehaviour
    {
        private Vector2 anchor = Vector2.zero;
        private float rate = 0;
        private float xRate = 0;
        private float yRate = 0;
        private Vector2 difference = Vector2.zero;
        [SerializeField] private float range = Screen.width * 0.4f;
        [SerializeField] private bool isFloating = true;
        [SerializeField] private UnityEvent onStart;
        [SerializeField] private UnityEvent onSwerve;
        [SerializeField] private UnityEvent onRelease;

        public float Rate { get => rate; }
        public float XRate { get => xRate; }
        public float YRate { get => yRate; }
        public Vector2 Difference { get => difference; }
        public UnityEvent OnStart { get => onStart; }
        public UnityEvent OnSwerve { get => onSwerve; }
        public UnityEvent OnRelease { get => onRelease; }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Reset();
                onStart.Invoke();
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 difference = (Vector2)Input.mousePosition - anchor;
                if (difference.magnitude > range && isFloating)
                {
                    anchor = (Vector2)Input.mousePosition - difference.normalized * range;
                    difference = (Vector2)Input.mousePosition - anchor;
                }
                this.difference = difference.normalized;
                rate = Mathf.Clamp(difference.magnitude / range, 0, 1);
                xRate = Mathf.Clamp(difference.x / range, -1, 1);
                yRate = Mathf.Clamp(difference.y / range, -1, 1);
                onSwerve.Invoke();
            }
            else if (Input.GetMouseButtonUp(0)) onRelease.Invoke();
        }
        public void Reset()
        {
            anchor = Input.mousePosition;
            difference = Vector2.zero;
            rate = 0;
        }
    }
}
