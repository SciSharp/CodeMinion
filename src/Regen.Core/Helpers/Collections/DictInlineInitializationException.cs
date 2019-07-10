﻿using System;

namespace Regen.Helpers.Collections {
    [Serializable]
    public class DictInlineInitializationException : Exception {
        public DictInlineInitializationException() { }
        public DictInlineInitializationException(string message) : base(message) { }
        public DictInlineInitializationException(string message, Exception inner) : base(message, inner) { }
    }
}