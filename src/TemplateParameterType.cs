namespace Simplet
{
    public enum TemplateParameterType
    {
        /// <summary>
        /// No parameter to distinguish.
        /// Every file is in his own class.
        /// </summary>
        None,
        /// <summary>
        /// The name of the file is used as a parameter.
        /// Files in the same directory are combined.
        /// </summary>
        File,
        /// <summary>
        /// The name of the directory is used as a parameter.
        /// Files with the same name are combined.
        /// </summary>
        Directory,
        /// <summary>
        /// The name of the path is used as a parameter.
        /// Files with the same extension are combined.
        /// </summary>
        Extension,
    }
}
