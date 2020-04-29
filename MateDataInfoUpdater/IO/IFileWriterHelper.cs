using System.Collections.Generic;
using MetadataInformationUpdater;

namespace MateDataInfoUpdater.IO
{
    public interface IFileWriterHelper
    {
        void WriteToFile(string path, List<DDRecord> records);
    }
}
