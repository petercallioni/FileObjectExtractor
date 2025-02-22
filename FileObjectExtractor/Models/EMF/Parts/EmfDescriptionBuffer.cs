﻿namespace FileObjectExtractor.Models.EMF.EmfPart
{
    public class EmfDescriptionBuffer : EmfFilePart
    {
        public EmfField<string> Contents = new(); // Variable Length

        public EmfDescriptionBuffer()
        {
            Fields.AddRange(new[]
            {
               Contents
            });
        }
    }
}