using System.Collections.Generic;

namespace FileObjectExtractor.Interfaces
{
    public interface IEmfField
    {
        void Initialize(Queue<byte> data);
    }
}