using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whip.App.Model
{
    /// <summary>
    /// Defines the shake sensitivity data.
    /// </summary>
    public class ShakeSensitivityData
    {
        /// <summary>
        /// Any vector that has a magnitude (after reducing gravitation force) bigger than this parameter value is considered as a shake vector (Def: 0.2)
        /// </summary>
        public double ShakeMagnitudeWithoutGravitationThreshold { get; set; }

        /// <summary>
        /// This parameter determines how many consecutive still vectors are required to stop a shake signal (Def: 20 = 400ms).
        /// </summary>
        public int StillCounterThreshold { get; set; }

        /// <summary>
        /// This parameter determines the maximum allowed magnitude (after reducing gravitation) for a still vector to be considered (Def: 0.02).
        /// </summary>
        public double StillMagnitudeWithoutGravitationThreshold { get; set; }

        /// <summary>
        /// The maximum number of still vectors needed to create a still vector average. Instead of averaging the entire still signal, we just look at the top recent still vectors. This is performed as runtime optimization (Def: 20).
        /// </summary>
        public int MaximumStillVectorsNeededForAverage { get; set; }

        /// <summary>
        /// The minimum number of still vectors needed to create a still vector average. Without enough vectors, the average won’t be stable and thus will be ignored (Def: 5).
        /// </summary>
        public int MinimumStillVectorsNeededForAverage { get; set; }

        /// <summary>
        /// Determines the number of shake vectors needed in order to recognize a shake (Def: 10).
        /// </summary>
        public int MinimumShakeVectorsNeededForShake { get; set; }

        /// <summary>
        /// Shake vectors with a magnitude lower than this parameter won’t be considered for gesture classification (Def: 0.2).
        /// </summary>
        public double WeakMagnitudeWithoutGravitationThreshold { get; set; }

        /// <summary>
        /// Determines the number of moves required to get a shake signal (Def: 3).
        /// </summary>
        public int MinimumRequiredMovesForShake { get; set; }

        /// <summary>
        /// The minimum sound delay in milliseconds.
        /// </summary>
        public int MinimumSoundDelayInMilliseconds { get; set; }

        /// <summary>
        /// The open intervall for fast movement Y. 
        /// </summary>
        public int OpenIntervalForFastMovementYInMillis { get; set; }

        /// <summary>
        /// Creates a default ShakeSensitivityData instance.
        /// </summary>
        public ShakeSensitivityData()
        {
            ShakeMagnitudeWithoutGravitationThreshold = 0.2;
            StillCounterThreshold = 20;
            StillMagnitudeWithoutGravitationThreshold = 0.02;
            MaximumStillVectorsNeededForAverage = 20;
            MinimumStillVectorsNeededForAverage = 5;
            MinimumShakeVectorsNeededForShake = 10;
            WeakMagnitudeWithoutGravitationThreshold = 0.2;
            MinimumRequiredMovesForShake = 3;
            MinimumSoundDelayInMilliseconds = 500;
            OpenIntervalForFastMovementYInMillis = 300;
        }
    }
}
