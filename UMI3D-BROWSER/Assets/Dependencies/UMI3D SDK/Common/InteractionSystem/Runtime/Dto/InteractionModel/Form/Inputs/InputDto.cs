using System;

namespace umi3d.common.interaction.form
{
    public class InputDto<T> : BaseInputDto
    {
        public string Name { get; set; }
        public T Value { get; set; }
        public TextType TextType { get; set; }

        public T PlaceHolder { get; set; }

        public override object GetPlaceHolder() => PlaceHolder;

        public override object GetValue() => Value;
    }
}