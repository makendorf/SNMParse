using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace SNMParseGAH
{
    internal class SpeechPrinter
    {
        public SpeechPrinter()
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();

            synth.SetOutputToDefaultAudioDevice();

            synth.Speak("This example demonstrates a basic use of Speech Synthesizer");
        }
    }
}
