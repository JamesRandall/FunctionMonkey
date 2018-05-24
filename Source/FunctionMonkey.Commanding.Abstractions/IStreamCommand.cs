using System.IO;

namespace FunctionMonkey.Commanding.Abstractions
{
    /// <summary>
    /// If a command implements this interface then when its handler is executed
    /// it will have the Stream property set to the input of the function that was
    /// triggered with the stream. For example a blob triggered function.
    /// 
    /// This can be useful when you want to handle large blobs or the content of the
    /// blob is not known in advance.
    /// </summary>
    public interface IStreamCommand
    {
        /// <summary>
        /// Stream to the item
        /// </summary>
        Stream Stream { get; set; }

        /// <summary>
        /// The name of the item (e.g. blob name). This may be null if the stream is not associated with something with a name.
        /// </summary>
        string Name { get; }
    }
}
