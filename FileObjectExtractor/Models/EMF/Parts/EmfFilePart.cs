using FileObjectExtractor.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public abstract class EmfFilePart
    {
        protected List<IEmfField> Fields = new List<IEmfField>();

        public void Initialize(StringBuilder input)
        {
            foreach (IEmfField field in Fields)
            {
                field.Initialize(input);
            }
        }
    }
}