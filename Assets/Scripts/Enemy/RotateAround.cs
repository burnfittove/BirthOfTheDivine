using UnityEngine;

namespace Enemy
{
    public class RotateAround : MonoBehaviour
    {
        [SerializeField] private Transform rotationPoint;
        [SerializeField] private float rotationSpeed;
        protected virtual void Update()
        {
            transform.RotateAround(rotationPoint.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}