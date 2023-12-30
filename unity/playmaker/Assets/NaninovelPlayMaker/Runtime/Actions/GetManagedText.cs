using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class GetManagedText : FsmStateAction
    {
        [Tooltip("The category (document file name), which contains the managed text record.")]
        [UIHint(UIHint.FsmString)]
        public FsmString RecordCategory;

        [Tooltip("The key (ID) of the managed text record to get.")]
        [UIHint(UIHint.FsmString)]
        public FsmString RecordKey;

        [Tooltip("The retrieved value of the record.")]
        [UIHint(UIHint.Variable)]
        public FsmString RecordValue;

        public override void Reset ()
        {
            RecordCategory = null;
            RecordKey = null;
            RecordValue = null;
        }

        public override void OnEnter ()
        {
            var textManager = Engine.GetService<TextManager>();
            if (textManager is null) { Finish(); return; }

            RecordValue.Value = textManager.GetRecordValue(RecordKey.Value, RecordCategory.Value);

            Finish();
        }
    }
}
