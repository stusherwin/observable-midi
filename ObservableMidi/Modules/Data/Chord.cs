using System.Collections.Generic;

namespace ObservableMidi.Modules.Data
{
    public class Chord
    {
        private readonly List<Note> _notes = new List<Note>();

        public Chord(IEnumerable<Note> notes)
        {
            _notes = new List<Note>(notes);
        }

        public List<Note> Notes { get { return _notes; } }
    }
}