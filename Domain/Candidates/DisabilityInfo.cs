using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Candidates
{
    public sealed class DisabilityInfo
    {
        public Guid? DisabilityTypeId { get; private set; }
        public string? Description { get; private set; }
        public bool IsVisibleToEmployer { get; private set; }

        private DisabilityInfo(
            Guid? disabilityTypeId,
            string? description,
            bool isVisibleToEmployer
        )
        {
            DisabilityTypeId = disabilityTypeId;
            Description = description;
            IsVisibleToEmployer = isVisibleToEmployer;
        }

        public static DisabilityInfo Hidden()
        {
            return new DisabilityInfo(null, null, false);
        }

        public static DisabilityInfo Create(
            Guid? disabilityTypeId,
            string? description,
            bool isVisibleToEmployer
        )
        {
            return new DisabilityInfo(
                disabilityTypeId,
                description,
                isVisibleToEmployer
            );
        }

        public void Hide()
        {
            IsVisibleToEmployer = false;
        }

        public void Show()
        {
            IsVisibleToEmployer = true;
        }
    }
}
