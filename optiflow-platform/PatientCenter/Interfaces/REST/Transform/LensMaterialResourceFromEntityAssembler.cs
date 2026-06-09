using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Interfaces.REST.Resources;

namespace optiflow_platform.PatientCenter.Interfaces.REST.Transform;

public static class LensMaterialResourceFromEntityAssembler
{
    public static LensMaterialResource ToResourceFromEntity(LensMaterial m) =>
        new(m.Id, m.FullName, m.IndexValue, m.BasePrice, m.Description);
}