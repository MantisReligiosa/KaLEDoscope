using System;

namespace ServiceInterfaces
{
    public interface IActivationManager
    {
        bool IsActivationInfoExists { get; }
        bool IsFullAccess { get; }
        DateTime TrialExpirationDate { get; }
        bool IsTrialKeyOriginal(string activationKey);
        void AddTrial(string activationKey, DateTime trialExpirationDate);
        void SetFullAccess();

        string GetRequestCode();
        string GetFullyActivationKey(string requestCode);
        string GetTrialActivationKey(string requestCode);
        bool IsFullyActivationKeyValid(string activationKey);
        bool IsTrialActivationKeyValid(string activationKey);
    }
}
