#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Infrastructure.Settings;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class PageSettings : object
{
    #region Constructor
    public PageSettings() : base()
    {
    }
    #endregion /Constructor

    #region Properties
    
    #region public bool RevealOnScroll { get; set; }
    /// <summary>
    /// An optional feature that makes the descriptions fade IN/OUT, as you scroll UP/DOWN the page
    /// true  = descriptions fade in/out while scrolling
    /// false = everything is always visible
    /// </summary>
    public bool RevealOnScroll { get; set; }
    #endregion /public bool RevealOnScroll { get; set; }

    #region public int Rotate {  get; set; }
    /// <summary>
    /// Angle of Rotating Flip Card
    /// </summary>
    public int Rotate {  get; set; }
    #endregion /public int Rotate {  get; set; }
    
    #endregion /Properties
}
