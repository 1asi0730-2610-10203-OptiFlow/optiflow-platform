namespace optiflow_platform.PatientCenter.Domain.Model.Entities;

public class LensMaterial
{
    protected LensMaterial()
    {
        FullName    = null!;
        IndexValue  = null!;
        Description = null!;
    }

    public int     Id          { get; private set; }
    public string  FullName    { get; private set; }
    public string  IndexValue  { get; private set; }
    public decimal BasePrice   { get; private set; }
    public string  Description { get; private set; }
}