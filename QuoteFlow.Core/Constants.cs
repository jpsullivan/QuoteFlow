namespace QuoteFlow.Core
{
    public class Constants
    {
        public const int DefaultPasswordResetTokenExpirationHours = 24;

        public const string Sha1HashAlgorithmId = "SHA1";
        public const string Sha512HashAlgorithmId = "SHA512";
        public const string PBKDF2HashAlgorithmId = "PBKDF2";

        public const string UploadsFolderName = "uploads";
        public const string DownloadsFolderName = "downloads";
        public const string ContentFolderName = "content";

        public const string OctetStreamContentType = "application/octet-stream";

        #region Asset Searching

        /// <summary>
        /// Used to retrieve a standard IssueTypes.
        /// </summary>
        public const string ALL_STANDARD_MANUFACTURERS = "-2";

        /// <summary>
        /// Used to retrieve a subtask IssueTypes.
        /// </summary>
        public const string ALL_SUB_TASK_ISSUE_TYPES = "-3";

        /// <summary>
        /// Used to retrieve all IssueTypes.
        /// </summary>
        public const string ALL_ISSUE_TYPES = "-4";

        /// <summary>
        /// Used in the generic <see cref="#getConstantObject(String, String)"/> method
        /// </summary>
        public const string PRIORITY_CONSTANT_TYPE = "Priority";

        /// <summary>
        /// Used in the generic <see cref="#getConstantObject(String, String)"/> method
        /// </summary>
        public const string STATUS_CONSTANT_TYPE = "Status";

        /// <summary>
        /// Used in the generic <see cref="#getConstantObject(String, String)"/> method
        /// </summary>
        public const string RESOLUTION_CONSTANT_TYPE = "Resolution";

        /// <summary>
        /// Used in the generic <see cref="#getConstantObject(String, String)"/> method
        /// </summary>
        public const string ISSUE_TYPE_CONSTANT_TYPE = "IssueType";

        #endregion
    }
}
