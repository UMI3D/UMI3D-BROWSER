using System.Collections.Generic;

namespace umi3d.common.interaction.form
{
    public abstract class DivDto : UMI3DDto
    {
        public ulong id { get; set; }
        public string type { get; set; }
        public string tooltip { get; set; }
        public List<StyleDto> styles { get; set; }
        public List<DivDto> FirstChildren { get; set; }
    }
}