using Naninovel.Commands;
using UnityEngine;

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
            get => TransitionalRenderer.GetLookDirection(ActorMeta.BakedLookDirection);
            set => TransitionalRenderer.SetLookDirection(value, ActorMeta.BakedLookDirection);
        }

        protected virtual CharacterLipSyncer LipSyncer { get; private set; }

        public SpineCharacter (string id, CharacterMetadata meta, EmbeddedAppearanceLoader<GameObject> loader)
            : base(id, meta, loader) { }

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

        public virtual UniTask ChangeLookDirectionAsync (CharacterLookDirection lookDirection, float duration,
            EasingType easingType = default, AsyncToken asyncToken = default)
        {
            return TransitionalRenderer.ChangeLookDirectionAsync(lookDirection,
                ActorMeta.BakedLookDirection, duration, easingType, asyncToken);
        }

        public virtual void AllowLipSync (bool active) => LipSyncer.SyncAllowed = active;
    }
}
