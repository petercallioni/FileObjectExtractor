using System.Collections.Generic;
using System.Text;

namespace MicrosoftObjectExtractor.Models.EMF
{
    public abstract class EmfFilePart
    {
        protected List<EmfField> Fields = new List<EmfField>();

        public void Initialize(StringBuilder input)
        {
            foreach (EmfField field in Fields)
            {
                field.Intitialize(input);
            }
        }
    }
}