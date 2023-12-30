using Spine.Unity;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// Used by <see cref="SpineCharacter"/> to represent Spine character on scene.
    /// </summary>
    /// <remarks>
    /// Naninovel will accept custom components inherited from this one;
    /// use this to override behaviour of the virtual members.
    /// </remarks>
    [RequireComponent(typeof(SkeletonRenderer), typeof(RenderCanvas))]
    public class SpineController : MonoBehaviour
    {
        public virtual SkeletonRenderer SkeletonRenderer { get; private set; }
        public virtual MeshRenderer MeshRenderer { get; private set; }
        public virtual MeshFilter MeshFilter { get; private set; }
        public virtual RenderCanvas RenderCanvas { get; private set; }

        [SerializeField] private StringUnityEvent onAppearanceChanged;
        [SerializeField] private BoolUnityEvent onIsSpeakingChanged;

        public virtual void ChangeAppearance (string appearance, float duration = 0,
            EasingType easingType = default, Transition? transition = default)
        {
            if (!string.IsNullOrEmpty(appearance))
                onAppearanceChanged?.Invoke(appearance);
        }

        public virtual void ChangeIsSpeaking (bool speaking)
        {
            onIsSpeakingChanged?.Invoke(speaking);
        }

        public virtual void SetAnimation (string animationName)
        {
            if (SkeletonRenderer is SkeletonAnimation animation)
                animation.AnimationState.SetAnimation(0, animationName, true);
        }

        protected virtual void Awake ()
        {
            RenderCanvas = GetComponent<RenderCanvas>();
            MeshRenderer = GetComponent<MeshRenderer>();
            MeshFilter = GetComponent<MeshFilter>();
            SkeletonRenderer = GetComponent<SkeletonRenderer>();
            SkeletonRenderer.Initialize(false);
        }
    }
}
