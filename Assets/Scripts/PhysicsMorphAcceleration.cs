using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class PhysicsMorphAcceleration : MonoBehaviour
{
    private static readonly int _deformationProperty = Shader.PropertyToID("_Deformation");
    private static readonly int _centerProperty = Shader.PropertyToID("_Center");

    [SerializeField]
    private Morph _morph;

    private Material _material;
    private Rigidbody _body;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _body = GetComponent<Rigidbody>();
        _material.SetVector(_centerProperty, _morph.Center);
    }

    private void FixedUpdate()
    {
        if (!_morph.IsEnabled) return;
        var velocityAtCenter = _body.GetPointVelocity(transform.TransformPoint(_morph.Center));
        var deformation = _morph.UpdateDeformation(velocityAtCenter, Time.fixedDeltaTime);
        _material.SetVector(_deformationProperty, transform.InverseTransformDirection(deformation));
    }

    [Serializable]
    public class Morph
    {
        [SerializeField]
        private bool _isEnabled = false;
        [SerializeField]
        private Vector3 _center = Vector3.zero;
        [SerializeField]
        private float _deformationRate = 10;
        [SerializeField]
        private float _relaxRate = 1000;
        [SerializeField] 
        private float _relaxDamping = 0.9f;

        private Vector3 _previousVelocity;
        private Vector3 _deformationVelocity;
        private Vector3 _deformation;

        public Vector3 Center => _center;
        public bool IsEnabled => _isEnabled;

        public Vector3 UpdateDeformation(Vector3 velocityAtCenter, float deltaTime)
        {
            var acceleration = (velocityAtCenter - _previousVelocity) / deltaTime;

            _deformationVelocity += acceleration * _deformationRate * deltaTime;
            _deformationVelocity -= _deformation * _relaxRate * deltaTime;
            _deformationVelocity *= _relaxDamping;
            _deformation += _deformationVelocity * deltaTime;

            _previousVelocity = velocityAtCenter;

            return _deformation;
        }
    }
}