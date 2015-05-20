namespace QuoteFlow
{
    public class Constants
    {
        public const string AdminRoleName = "Admins";

        public const string CurrentUserOwinEnvironmentKey = "quoteflow.user";
        public const int DefaultPasswordResetTokenExpirationHours = 24;

        public const string ReturnUrlViewDataKey = "returnUrl";

        public const string UploadFileNameTemplate = "{0}{1}";
        public const string UploadsFolderName = "uploads";

        public const string Sha1HashAlgorithmId = "SHA1";
        public const string Sha512HashAlgorithmId = "SHA512";
        public const string PBKDF2HashAlgorithmId = "PBKDF2";
    }
}