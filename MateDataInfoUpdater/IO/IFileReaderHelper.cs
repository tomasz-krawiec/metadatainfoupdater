﻿using System.Collections.Generic;
using MetadataInformationUpdater;

namespace MateDataInfoUpdater.IO
{
    public interface IFileReaderHelper
    {
        List<DDRecord> ReadLines(string path);
    }
}
