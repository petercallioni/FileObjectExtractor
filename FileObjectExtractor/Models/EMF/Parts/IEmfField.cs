using System.Collections.Generic;

namespace FileObjectExtractor.Models.EMF.Parts
{
    public interface IEmfField
    {
        void Initialize(Queue<byte> data);
    }
}