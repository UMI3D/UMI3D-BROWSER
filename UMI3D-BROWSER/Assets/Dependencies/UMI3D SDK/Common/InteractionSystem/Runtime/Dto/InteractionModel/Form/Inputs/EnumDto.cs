/*
Copyright 2019 - 2024 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Collections.Generic;
using System.Linq;
using umi3d.common.interaction.form;

public class EnumDto<T, G> : BaseInputDto
    where T : EnumValue<G> 
    where G : DivDto
{
    public List<T> values { get; set; }
    public bool canSelectMultiple { get; set; }

    public override object GetValue() => values.Where(e => e.isSelected);
}

public class EnumValue<G> where G : DivDto
{
    public G item { get; set; }
    public bool isSelected { get; set; }
}