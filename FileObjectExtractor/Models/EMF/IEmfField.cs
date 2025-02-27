using System.Collections.Generic;

namespace FileObjectExtractor.Models.EMF
{
    public interface IEmfField
    {
        void Initialize(Queue<byte> data);
    }
}