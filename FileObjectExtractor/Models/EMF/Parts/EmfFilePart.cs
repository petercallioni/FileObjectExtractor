using System.Collections.Generic;

namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public abstract class EmfFilePart
    {
        protected List<IEmfField> Fields = new List<IEmfField>();

        public void Initialize(Queue<byte> data)
        {
            foreach (IEmfField field in Fields)
            {
                field.Initialize(data);
            }
        }
    }
}