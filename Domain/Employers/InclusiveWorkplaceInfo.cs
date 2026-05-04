using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Employers
{
    public sealed class InclusiveWorkplaceInfo
    {
        public bool HasWheelchairAccess { get; private set; }
        public bool HasAccessibleRestroom { get; private set; }
        public bool SupportsFlexibleWorkingTime { get; private set; }
        public bool SupportsRemoteWork { get; private set; }
        public bool ProvidesAssistiveDevices { get; private set; }

        private InclusiveWorkplaceInfo(
            bool hasWheelchairAccess,
            bool hasAccessibleRestroom,
            bool supportsFlexibleWorkingTime,
            bool supportsRemoteWork,
            bool providesAssistiveDevices
        )
        {
            HasWheelchairAccess = hasWheelchairAccess;
            HasAccessibleRestroom = hasAccessibleRestroom;
            SupportsFlexibleWorkingTime = supportsFlexibleWorkingTime;
            SupportsRemoteWork = supportsRemoteWork;
            ProvidesAssistiveDevices = providesAssistiveDevices;
        }

        public static InclusiveWorkplaceInfo Create(
            bool hasWheelchairAccess,
            bool hasAccessibleRestroom,
            bool supportsFlexibleWorkingTime,
            bool supportsRemoteWork,
            bool providesAssistiveDevices
        )
        {
            return new InclusiveWorkplaceInfo(
                hasWheelchairAccess,
                hasAccessibleRestroom,
                supportsFlexibleWorkingTime,
                supportsRemoteWork,
                providesAssistiveDevices
            );
        }
    }
}
