using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using UnityEngine;


namespace Criaath.UI
{
    public class CriaathUITest : MonoBehaviour
    {
        public AnimationSequencerController Test;

        public void PlaySequencer()
        {
            Test.Play();
        }
    }
}
