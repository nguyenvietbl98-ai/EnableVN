using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Jobs
{
    public sealed class JobAccessibilityInfo
    {
        public bool SupportsWheelchairAccess { get; private set; }
        public bool SupportsRemoteWork { get; private set; }
        public bool SupportsFlexibleTime { get; private set; }
        public bool ProvidesAssistiveDevices { get; private set; }
        public string? AdditionalSupportDescription { get; private set; }

        private JobAccessibilityInfo(
            bool supportsWheelchairAccess,
            bool supportsRemoteWork,
            bool supportsFlexibleTime,
            bool providesAssistiveDevices,
            string? additionalSupportDescription
        )
        {
            SupportsWheelchairAccess = supportsWheelchairAccess;
            SupportsRemoteWork = supportsRemoteWork;
            SupportsFlexibleTime = supportsFlexibleTime;
            ProvidesAssistiveDevices = providesAssistiveDevices;
            AdditionalSupportDescription = additionalSupportDescription;
        }

        public static JobAccessibilityInfo Create(
            bool supportsWheelchairAccess,
            bool supportsRemoteWork,
            bool supportsFlexibleTime,
            bool providesAssistiveDevices,
            string? additionalSupportDescription
        )
        {
            return new JobAccessibilityInfo(
                supportsWheelchairAccess,
                supportsRemoteWork,
                supportsFlexibleTime,
                providesAssistiveDevices,
                additionalSupportDescription
            );
        }
    }
}
