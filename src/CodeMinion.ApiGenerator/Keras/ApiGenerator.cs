using CodeMinion.Core;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Torch.ApiGenerator;

namespace CodeMinion.ApiGenerator.Keras
{
    // Routines: [x] means generated
    // ====================
    //[] Models
    //[] Core Layers
    //[] Convolutional Layers
    //[] Pooling Layers
    //[] Locally Connected Layers
    //[] Recurrent Layers
    //[] Embedding Layers
    //[] Merge Layers
    //[] Advance Activation Layers
    //[] Normalization Layers
    //[] Moise Layers
    //[] Layer wrapper
    //[] Sequence Processing
    //[] Text Processing
    //[] Image Processing
    //[] Losses
    //[] Metrices
    //[] Optimizers
    //[] Activations
    //[] Callbacks
    //[] Datasets
    //[] Applications
    //[] Initializers
    //[] Regularizers
    //[] Constraints
    //[] Vsualization
    //[] Scikit-Learn API
    //[] Utils

    class ApiGenerator : ICodeGenerator
    {
        private CodeGenerator _generator;

        public string Generate()
        {
            string result = "";


           
            return result;
        }

        string BaseUrl = "https://keras.io/";

        public Dictionary<string, string> LoadDocs()
        {
            throw new NotImplementedException();
        }
    }
}
