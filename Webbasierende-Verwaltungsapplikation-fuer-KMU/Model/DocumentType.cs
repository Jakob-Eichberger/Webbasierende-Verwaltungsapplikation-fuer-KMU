namespace Webbasierende_Verwaltungsapplikation_fuer_KMU.Model
{

    /// <summary>
    /// Type of Document.
    /// </summary>
    /// <example>
    /// Rechnung, Auftragsbestätigung, Kosten Voranschlag ...
    /// </example>
    public enum DocumentType
    {
        Standard = 0, CompanyIntern = 1, Invoice = 2
    }
}