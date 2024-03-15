using System.Collections.Generic;

namespace umi3d.common.interaction.form
{
    /// <summary>
    /// the main componenent sent in the connectionFormDto of the divform system
    /// </summary>
    public class FormDto : DivDto
    {
        public string name { get; set; }
        public string description { get; set; }
    }
}