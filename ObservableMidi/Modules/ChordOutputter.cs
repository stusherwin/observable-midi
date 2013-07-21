using ObservableMidi.Modules.Core;
using ObservableMidi.Modules.Data;

namespace ObservableMidi.Modules
{
    public class ChordOutputter : TransformModule<Chord, Note>
    {
        protected override void OnReceive(Chord data)
        {
            data.Notes.ForEach(n => Send(n));
        }
    }
}
