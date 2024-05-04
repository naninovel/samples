using UnityEngine;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="IBackgroundActor"/> implementation using <see cref="SpineController"/> to represent the actor.
    /// </summary>
    [ActorResources(typeof(SpineController), false)]
    public class SpineBackground : SpineActor<BackgroundMetadata>, IBackgroundActor
    {
        private BackgroundMatcher matcher;

        public SpineBackground (string id, BackgroundMetadata meta, EmbeddedAppearanceLoader<GameObject> loader)
            : base(id, meta, loader) { }

        public override async UniTask InitializeAsync ()
        {
            await base.InitializeAsync();
            matcher = BackgroundMatcher.CreateFor(ActorMeta, TransitionalRenderer);
        }

        public override void Dispose ()
        {
            base.Dispose();
            matcher?.Stop();
        }
    }
}
