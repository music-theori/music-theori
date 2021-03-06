﻿using System;

namespace theori.Audio.Effects
{
    public sealed class Retrigger : Dsp, IMixable
    {
        /// <summary>
        /// Duration of the part to retrigger (in seconds)
        /// </summary>
        public double Duration = 0.1;

        /// <summary>
        /// How many times to loop before restarting the retriggered part
        /// </summary>
        public int LoopCount = 8;

        /// <summary>
        /// Amount(0,1) of time to mute before playing again, 1 is fully gated
        /// </summary>
        public float Gating;

        private float[] retriggerBuffer = new float[0];
        private int currentSample = 0;
        private int currentLoop = 0;

        public Retrigger(int sampleRate)
            : base(sampleRate)
        {
        }

        protected override void ProcessImpl(Span<float> buffer)
        {
            int numSamples = buffer.Length / 2;

            double sll = SampleRate * Duration;
            int sampleDuration = (int)(SampleRate * Duration);
            int sampleGatingLength = (int)(sll * Gating);

            if(retriggerBuffer.Length < (sampleDuration*2))
                Array.Resize(ref retriggerBuffer, sampleDuration * 2);

            for(int i = 0; i < numSamples; i++)
            {
                if(currentLoop == 0)
                {
                    // Store samples for later
                    if(currentSample > sampleGatingLength) // Additional gating
                    {
                        retriggerBuffer[currentSample*2] = (0.0f);
                        retriggerBuffer[currentSample*2+1] = (0.0f);
                    }
                    else
                    {
                        retriggerBuffer[currentSample*2] = buffer[i * 2];
                        retriggerBuffer[currentSample*2+1] = buffer[i * 2 + 1];
                    }
                }

                // Sample from buffer
                buffer[i * 2] = MathL.Lerp(buffer[i * 2], retriggerBuffer[currentSample * 2], Mix);
                buffer[i * 2 + 1] = MathL.Lerp(buffer[i * 2 + 1], retriggerBuffer[currentSample * 2 + 1], Mix);
		
                // Increase index
                currentSample++;
                if(currentSample >= sampleDuration)
                {
                    currentSample -= sampleDuration;
                    currentLoop++;
                    if(LoopCount != 0 && currentLoop >= LoopCount)
                    {
                        // Reset
                        currentLoop = 0;
                        currentSample = 0;
                    }
                }
            }
        }
    }
}
