using System;
using System.Threading.Tasks;
using Naninovel.FX;
using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="MonoBehaviourActor{TMeta}"/> using <see cref="SpineController"/> to represent the actor.
    /// </summary>
    /// <remarks>
    /// Spine prefab is expected to have a <see cref="SpineController"/> component attached to the root object.
    /// </remarks>
    public abstract class SpineActor<TMeta> : MonoBehaviourActor<TMeta>, Blur.IBlurable
        where TMeta : OrthoActorMetadata
    {
        /// <summary>
        /// Controller component of the instantiated spine prefab associated with the actor.
        /// </summary>
        public virtual SpineController Controller { get; private set; }
        public override string Appearance { get => appearance; set => SetAppearance(value); }
        public override bool Visible { get => visible; set => SetVisibility(value); }

        protected virtual TransitionalRenderer TransitionalRenderer { get; private set; }
        protected virtual SpineDrawer Drawer { get; private set; }

        private readonly LocalizableResourceLoader<GameObject> prefabLoader;
        private string appearance;
        private bool visible;

        protected SpineActor (string id, TMeta meta, EmbeddedAppearanceLoader<GameObject> loader)
            : base(id, meta)
        {
            prefabLoader = loader;
        }

        public override async UniTask InitializeAsync ()
        {
            await base.InitializeAsync();

            Controller = await InitializeControllerAsync();
            TransitionalRenderer = TransitionalRenderer.CreateFor(ActorMeta, GameObject, true);
            Drawer = new SpineDrawer(Controller);

            SetVisibility(false);

            Engine.Behaviour.OnBehaviourUpdate += DrawSpine;
        }

        public override void Dispose ()
        {
            if (Engine.Behaviour != null)
                Engine.Behaviour.OnBehaviourUpdate -= DrawSpine;

            Drawer.Dispose();

            base.Dispose();

            prefabLoader?.ReleaseAll(this);
        }

        public virtual UniTask BlurAsync (float duration, float intensity, EasingType easingType = default, AsyncToken asyncToken = default)
        {
            return TransitionalRenderer.BlurAsync(duration, intensity, easingType, asyncToken);
        }

        public override UniTask ChangeAppearanceAsync (string appearance, float duration, EasingType easingType = default,
            Transition? transition = default, AsyncToken asyncToken = default)
        {
            this.appearance = appearance;
            if (Controller)
                Controller.ChangeAppearance(appearance, duration, easingType, transition);
            return UniTask.CompletedTask;
        }

        public override async UniTask ChangeVisibilityAsync (bool visible, float duration, EasingType easingType = default,
            AsyncToken asyncToken = default)
        {
            this.visible = visible;
            await TransitionalRenderer.FadeToAsync(visible ? TintColor.a : 0, duration, easingType, asyncToken);
        }

        protected virtual void SetAppearance (string appearance)
        {
            this.appearance = appearance;
            if (Controller)
                Controller.ChangeAppearance(appearance);
        }

        protected virtual void SetVisibility (bool visible) => ChangeVisibilityAsync(visible, 0).Forget();

        protected override Color GetBehaviourTintColor () => TransitionalRenderer.TintColor;

        protected override void SetBehaviourTintColor (Color tintColor)
        {
            if (!Visible) // Handle visibility-controlled alpha of the tint color.
                tintColor.a = TransitionalRenderer.TintColor.a;
            TransitionalRenderer.TintColor = tintColor;
        }

        protected virtual void DrawSpine () => Drawer.DrawTo(TransitionalRenderer, ActorMeta.PixelsPerUnit);

        protected virtual async Task<SpineController> InitializeControllerAsync ()
        {
            var prefabResource = await prefabLoader.LoadAsync(Id);
            if (!prefabResource.Valid)
                throw new Exception($"Failed to load Spine prefab for `{Id}` actor. Make sure the resource is set up correctly in the actor configuration.");
            var controller = Engine.Instantiate(prefabResource.Object).GetComponent<SpineController>();
            controller.gameObject.name = Id;
            controller.transform.SetParent(Transform);
            return controller;
        }
    }
}
