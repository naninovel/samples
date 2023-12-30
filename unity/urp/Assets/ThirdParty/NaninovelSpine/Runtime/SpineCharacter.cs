using Naninovel.Commands;

namespace Naninovel
{
    /// <summary>
    /// A <see cref="ICharacterActor"/> implementation using <see cref="SpineController"/> to represent the actor.
    /// </summary>
    [ActorResources(typeof(SpineController), false)]
    public class SpineCharacter : SpineActor<CharacterMetadata>, ICharacterActor, LipSync.IReceiver
    {
        public CharacterLookDirection LookDirection
        {
            get => TransitionalRenderer.GetLookDirection(ActorMetadata.BakedLookDirection);
            set => TransitionalRenderer.SetLookDirection(value, ActorMetadata.BakedLookDirection);
        }

        protected virtual CharacterLipSyncer LipSyncer { get; private set; }

        public SpineCharacter (string id, CharacterMetadata metadata)
            : base(id, metadata) { }

        public override async UniTask InitializeAsync ()
        {
            await base.InitializeAsync();
            LipSyncer = new CharacterLipSyncer(Id, Controller.ChangeIsSpeaking);
        }

        public override void Dispose ()
        {
            LipSyncer.Dispose();
            base.Dispose();
        }

        public UniTask ChangeLookDirectionAsync (CharacterLookDirection lookDirection, float duration,
            EasingType easingType = default, AsyncToken asyncToken = default)
        {
            return TransitionalRenderer.ChangeLookDirectionAsync(lookDirection,
                ActorMetadata.BakedLookDirection, duration, easingType, asyncToken);
        }

        public void AllowLipSync (bool active) => LipSyncer.SyncAllowed = active;
    }
}
