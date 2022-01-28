using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    [RequireComponent(typeof(Swerve))]
    public class Mover : MonoBehaviour
    {
        private Swerve _swerve;
        private Transform _transform;
        [SerializeField] private MoverType _type = MoverType.X;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _rotationSpeed = 1;
        [SerializeField] private bool _fixedRotation = true;
        private void Awake()
        {
            _swerve = GetComponent<Swerve>();
            _transform = transform;
            _swerve.OnSwerve.AddListener(Move);
        }
        private void Move()
        {
            Vector3 offset;
            switch (_type)
            {
                case MoverType.X:
                    offset = Vector3.right * _swerve.XRate;
                    break;
                case MoverType.Y:
                    offset = Vector3.up * _swerve.YRate;
                    break;
                case MoverType.Z:
                    offset = Vector3.forward * _swerve.YRate;
                    break;
                case MoverType.XZ:
                    offset = new Vector3(_swerve.Difference.x, 0, _swerve.Difference.y) * _swerve.Rate;
                    break;
                default:
                    offset = Vector3.zero;
                    break;
            }
            Vector3 targetPosition = _transform.position + offset;
            Vector3 direction = targetPosition - _transform.position;
            if (!_fixedRotation && direction.sqrMagnitude > 0)
                _transform.rotation = Quaternion.LerpUnclamped(_transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * _rotationSpeed);
            _transform.position = Vector3.MoveTowards(_transform.position, targetPosition, Time.deltaTime * _speed);
        }
        private enum MoverType { X, Y, Z, XZ }
    }
}