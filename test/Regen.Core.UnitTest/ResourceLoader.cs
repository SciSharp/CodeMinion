using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Regen.Core.Tests.Exceptions;

//Scavenged from https://github.com/thomasgalliker/ResourceLoader

namespace Regen.Core.Tests {
    namespace Exceptions {
        public class ResourceNotFoundException : Exception {
            public ResourceNotFoundException(string resourceFileName)
                : base(string.Format("Resource ending with {0} not found.", resourceFileName)) { }
        }

        public class MultipleResourcesFoundException : Exception {
            public MultipleResourcesFoundException(string resourceFileName, string[] resourcePaths)
                : base(string.Format("Multiple resources ending with {0} found: {1}{2}", resourceFileName, Environment.NewLine, string.Join(Environment.NewLine, resourcePaths))) { }
        }
    }

    public interface IResourceLoader {
        /// <summary>
        ///     Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource stream.</returns>
        /// <param name="assembly">The assembly which embeds the resource.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName);

        /// <summary>
        ///     Attempts to find and return resources from within the specified assembly that match the given file pattern.
        /// </summary>
        /// <returns>The embedded resource as streams.</returns>
        /// <param name="assembly">The assembly which embeds the resource.</param>
        /// <param name="filePattern">Resource file pattern.</param>
        IEnumerable<Stream> GetEmbeddedResourceStreams(Assembly assembly, string filePattern);

        /// <summary>
        ///     Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource as a byte array.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        byte[] GetEmbeddedResourceByteArray(Assembly assembly, string resourceFileName);

        /// <summary>
        ///     Attempts to find and return the resources from within the specified assembly that match the given file pattern.
        /// </summary>
        /// <returns>The embedded resources as byte arrays.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        IEnumerable<byte[]> GetEmbeddedResourceByteArrays(Assembly assembly, string resourceFileName);

        /// <summary>
        ///     Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource as a string.</returns>
        /// <param name="assembly">The assembly which embeds the resource.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        /// <param name="encoding">Character encoding. Default is UTF8.</param>
        string GetEmbeddedResourceString(Assembly assembly, string resourceFileName, Encoding encoding = null);

        /// <summary>
        ///     Attempts to find and return the resources from within the specified assembly that match the given file pattern.
        /// </summary>
        /// <returns>The embedded resources as strings.</returns>
        /// <param name="assembly">The assembly which embeds the resource.</param>
        /// <param name="filePattern">Resource file pattern.</param>
        /// <param name="encoding">Character encoding. Default is UTF8.</param>
        IEnumerable<string> GetEmbeddedResourceStrings(Assembly assembly, string filePattern, Encoding encoding = null);
    }

    /// <summary>
    ///     Utility that can be used to find and load embedded resources into memory.
    /// </summary>
    public class ResourceLoader : IResourceLoader {
        static readonly Lazy<IResourceLoader> Implementation = new Lazy<IResourceLoader>(CreateResourceLoader, LazyThreadSafetyMode.PublicationOnly);

        public static IResourceLoader Current {
            get { return Implementation.Value; }
        }

        static IResourceLoader CreateResourceLoader() { return new ResourceLoader(); }

        public Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName) {
            var resourceNames = assembly.GetManifestResourceNames();

            var resourcePaths = resourceNames.Where(x => x.EndsWith(resourceFileName, StringComparison.CurrentCultureIgnoreCase)).ToArray();

            if (!resourcePaths.Any()) {
                throw new ResourceNotFoundException(resourceFileName);
            }

            if (resourcePaths.Length > 1) {
                throw new MultipleResourcesFoundException(resourceFileName, resourcePaths);
            }

            return assembly.GetManifestResourceStream(resourcePaths.Single());
        }

        public IEnumerable<Stream> GetEmbeddedResourceStreams(Assembly assembly, string resourceFileName) {
            var resourceNames = assembly.GetManifestResourceNames();

            var resourcePaths = resourceNames.Where(x => x.Contains(resourceFileName)).ToArray();
            foreach (var resourcePath in resourcePaths) {
                yield return assembly.GetManifestResourceStream(resourcePath);
            }
        }

        public byte[] GetEmbeddedResourceByteArray(Assembly assembly, string resourceFileName) {
            var stream = this.GetEmbeddedResourceStream(assembly, resourceFileName);

            using (var memoryStream = new MemoryStream()) {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public IEnumerable<byte[]> GetEmbeddedResourceByteArrays(Assembly assembly, string resourceFileName) {
            var streams = this.GetEmbeddedResourceStreams(assembly, resourceFileName);

            foreach (var stream in streams) {
                using (var memoryStream = new MemoryStream()) {
                    stream.CopyTo(memoryStream);
                    yield return memoryStream.ToArray();
                }
            }
        }

        public string GetEmbeddedResourceString(Assembly assembly, string resourceFileName, Encoding encoding = null) {
            var stream = this.GetEmbeddedResourceStream(assembly, resourceFileName);

            encoding = encoding ?? Encoding.UTF8;

            using (var streamReader = new StreamReader(stream, encoding)) {
                return streamReader.ReadToEnd();
            }
        }

        public IEnumerable<string> GetEmbeddedResourceStrings(Assembly assembly, string resourceFileName, Encoding encoding = null) {
            var streams = this.GetEmbeddedResourceStreams(assembly, resourceFileName);

            encoding = encoding ?? Encoding.UTF8;

            foreach (var stream in streams) {
                using (var streamReader = new StreamReader(stream, encoding)) {
                    yield return streamReader.ReadToEnd();
                }
            }
        }
    }
}