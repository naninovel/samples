using Naninovel;
using UnityEngine;

[RequireComponent(typeof(RenderCanvas))]
public class ScaleToScreen : MonoBehaviour
{
    private class Matcher : CameraMatcher
    {
        private readonly Transform transform;
        private readonly Vector2 referenceSize;
        private readonly Vector3 initialScale;

        public Matcher (ICameraManager cameraManager, Vector2 referenceSize, Transform transform)
            : base(cameraManager, transform.gameObject)
        {
            this.transform = transform;
            this.referenceSize = referenceSize;
            initialScale = transform.localScale;
        }

        protected override void ApplyScale (float scaleFactor)
        {
            transform.localScale = initialScale * scaleFactor;
        }

        protected override bool TryGetReferenceSize (out Vector2 referenceSize)
        {
            referenceSize = this.referenceSize;
            return true;
        }
    }

    [SerializeField] private AspectMatchMode matchMode;
    [SerializeField] private float customMatchRatio;

    private Matcher matcher;

    private void Awake ()
    {
        var renderCanvas = GetComponent<RenderCanvas>();
        var cameraManager = Engine.GetService<ICameraManager>();
        matcher = new Matcher(cameraManager, renderCanvas.Size, transform);
        matcher.MatchMode = matchMode;
        matcher.CustomMatchRatio = customMatchRatio;
    }

    private void OnEnable () => matcher.Start();

    private void OnDisable () => matcher.Stop();
}
