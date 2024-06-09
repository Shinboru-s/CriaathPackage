using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Criaath.MiniTools;
using UnityEngine;


namespace Criaath.UI
{
    public class CriaathUITest : MonoBehaviour
    {
#if DOTWEEN_ENABLED
        public AnimationSequencerController Test;
        public void PlaySequencer()
        {
            Test.Play();
        }
#endif

        public void PlayTest()
        {
            CriaathDebugger.Log("CriaathUI");
        }
    }
}
