namespace Naninovel
{
    /// <summary>
    /// A <see cref="IBackgroundActor"/> implementation using <see cref="SpineController"/> to represent the actor.
    /// </summary>
    [ActorResources(typeof(SpineController), false)]
    public class SpineBackground : SpineActor<BackgroundMetadata>, IBackgroundActor
    {
        private BackgroundMatcher matcher;

        public SpineBackground (string id, BackgroundMetadata metadata)
            : base(id, metadata) { }

        public override async UniTask InitializeAsync ()
        {
            await base.InitializeAsync();
            matcher = BackgroundMatcher.CreateFor(ActorMetadata, TransitionalRenderer);
        }

        public override void Dispose ()
        {
            base.Dispose();
            matcher?.Stop();
        }
    }
}
