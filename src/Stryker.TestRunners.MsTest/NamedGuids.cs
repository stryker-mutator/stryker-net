using System.Security.Cryptography;
using System.Text;

namespace Stryker.TestRunners.MsTest;
internal class NamedGuids
{
    private static readonly Guid isoOidNamespace = new("6ba7b812-9dad-11d1-80b4-00c04fd430c8");
    private static readonly int version = 5;

    public static Guid GetFromName(string name)
    {
        var namespaceBytes = isoOidNamespace.ToByteArray();

        SwapMSB(ref namespaceBytes);

        var nameBytes = Encoding.UTF8.GetBytes(name);
        var data = namespaceBytes.Concat(nameBytes).ToArray();

        var hash = SHA1.HashData(data);

        var newGuid = new byte[16];

        Array.Copy(hash, newGuid, 16);

        newGuid[6] = (byte)((newGuid[6] & 0x0F) | (version << 4));

        newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

        SwapMSB(ref newGuid);

        return new Guid(newGuid);
    }


    private static void SwapMSB(ref byte[] bytes)
    {
        SwapBytes(ref bytes, 0, 3);
        SwapBytes(ref bytes, 1, 2);
        SwapBytes(ref bytes, 4, 5);
        SwapBytes(ref bytes, 6, 7);
    }

    private static void SwapBytes(ref byte[] bytes, int leftIndex, int rightIndex) =>
        (bytes[rightIndex], bytes[leftIndex]) = (bytes[leftIndex], bytes[rightIndex]);
}
