﻿using System;
using DiscordMafia.Voting;

namespace DiscordMafia.Activity
{
    public class HealActivity: BaseActivity
    {
        public InGamePlayerInfo Patient { get; protected set; }

        public HealActivity(InGamePlayerInfo doctor, InGamePlayerInfo patient)
            : base(doctor)
        {
            Patient = patient;
            Patient.HealedBy = this;
        }

        protected override void OnCancel(InGamePlayerInfo onlyAgainstTarget)
        {
            if (onlyAgainstTarget == null || Patient == onlyAgainstTarget)
            {
                if (Patient != null)
                {
                    Patient.HealedBy = null;
                }
                IsCanceled = true;
                base.OnCancel(onlyAgainstTarget);
            }
        }

        public override bool HasActivityAgainst(InGamePlayerInfo target)
        {
            return target == Patient;
        }
    }
}
