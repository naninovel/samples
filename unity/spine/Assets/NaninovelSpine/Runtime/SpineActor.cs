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

        private LocalizableResourceLoader<GameObject> prefabLoader;
        private string appearance;
        private bool visible;

        protected SpineActor (string id, TMeta metadata)
            : base(id, metadata) { }

        public override async UniTask InitializeAsync ()
        {
            await base.InitializeAsync();

            prefabLoader = InitializeLoader(ActorMetadata);
            Controller = await InitializeControllerAsync(prefabLoader, Id, Transform);
            TransitionalRenderer = TransitionalRenderer.CreateFor(ActorMetadata, GameObject, true);
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

            prefabLoader?.UnloadAll();
        }

        public UniTask BlurAsync (float duration, float intensity, EasingType easingType = default, AsyncToken asyncToken = default)
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

        protected virtual void DrawSpine () => Drawer.DrawTo(TransitionalRenderer, ActorMetadata.PixelsPerUnit);

        private static LocalizableResourceLoader<GameObject> InitializeLoader (ActorMetadata actorMetadata)
        {
            var providerManager = Engine.GetService<IResourceProviderManager>();
            var localizationManager = Engine.GetService<ILocalizationManager>();
            return actorMetadata.Loader.CreateLocalizableFor<GameObject>(providerManager, localizationManager);
        }

        private static async Task<SpineController> InitializeControllerAsync (IResourceLoader<GameObject> loader, string actorId, Transform transform)
        {
            var prefabResource = await loader.LoadAsync(actorId);
            if (!prefabResource.Valid)
                throw new Exception($"Failed to load Spine prefab for `{actorId}` actor. Make sure the resource is set up correctly in the actor configuration.");
            var controller = Engine.Instantiate(prefabResource.Object).GetComponent<SpineController>();
            controller.gameObject.name = actorId;
            controller.transform.SetParent(transform);
            return controller;
        }
    }
}
