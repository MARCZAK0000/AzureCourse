using System;
using System.Collections.Generic;

namespace AzureSql.Database;

public partial class BuildVersion
{
    public byte SystemInformationId { get; set; }

    public string DatabaseVersion { get; set; } = null!;

    public DateTime VersionDate { get; set; }

    public DateTime ModifiedDate { get; set; }
}
