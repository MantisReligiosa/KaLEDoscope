using Common;

namespace ServiceInterfaces
{
    public interface IActivationManager
    {
        LicenseInfo ActualLicenseInfo { get; set; }
        void ApplyLicense(LicenseInfo licenseInfo);
        string GetRequestCode();
        string GetActivationKey(LicenseInfo licenseInfo);
        bool TryActivate(string activationKey, out LicenseInfo licenseInfo);
    }
}
