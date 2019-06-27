//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace MulticlassClassificationML.Model.DataModels
{
    public class ModelInput
    {
        [ColumnName("PixelValues"), LoadColumn(0,63)]
        [VectorType(64)]
        public float[] PixelValues;


        [ColumnName("Number"), LoadColumn(64)]
        public float Number { get; set; }

    }
}
