using System.Collections.Generic;

namespace FileObjectExtractor.Models.EMF.Parts
{
    public interface IEmfField
    {
        bool Initialize(Queue<byte> data);
    }
}