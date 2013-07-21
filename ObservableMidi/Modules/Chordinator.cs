using System.Collections.Generic;
using System.Threading;
using ObservableMidi.Modules.Core;
using ObservableMidi.Modules.Data;
using Sanford.Multimedia.Midi;

namespace ObservableMidi.Modules
{
    public class Chordinator : TransformModule<Note, Chord>
    {
        private readonly List<Note> _notes = new List<Note>();

        protected override void OnReceive(Note data)
        {
            if(data.Message.Command == ChannelCommand.NoteOn)
                _notes.Add(data);

            if (_notes.Count == 3)
            {
                Thread.Sleep(1000);

                Out.Send(new Chord(_notes));
                _notes.Clear();
            }
        }
    }
}