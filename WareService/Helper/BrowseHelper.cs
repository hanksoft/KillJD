using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

namespace WareDealer.Helper
{
    
[StructLayout(LayoutKind.Sequential)]
public class INTERNET_CACHE_ENTRY_INFOW
{
    public uint dwStructSize;
    public string lpszSourceUrlName;
    public string lpszLocalFileName;
    public uint CacheEntryType;
    public uint dwUseCount;
    public uint dwHitRate;
    public uint dwSizeLow;
    public uint dwSizeHigh;
    public FILETIME LastModifiedTime;
    public FILETIME ExpireTime;
    public FILETIME LastAccessTime;
    public FILETIME LastSyncTime;
    public IntPtr lpHeaderInfo;
    public uint dwHeaderInfoSize;
    public string lpszFileExtension;
    public uint dwReserved; //union uint dwExemptDelta;
}
public class BrowseHelper
{

    //[DllImport("wininet.dll")]
    //public static extern IntPtr FindFirstUrlCacheEntryEx(
    //  string lpszUrlSearchPattern,
    //  uint dwFlags,
    //  uint dwFilter,
    //  Int64 GroupId,
    //  IntPtr lpFirstCacheEntryInfo,
    //  ref uint lpdwFirstCacheEntryInfoBufferSize,
    //  Pointer lpGroupAttributes,
    //  Pointer pcbGroupAttributes,
    //  Pointer lpReserved
    //);

    [DllImport("wininet.dll")]
    //Starts a filtered enumeration of the Internet cache.
    public static extern IntPtr FindFirstUrlCacheEntryEx(
        //A pointer to a string that contains the source name pattern to search for. 
        //This parameter can only be set to "cookie:", "visited:", or NULL. 
        //Set this parameter to "cookie:" to enumerate the cookies 
        //or "visited:" to enumerate the URL History entries in the cache. 
        //If this parameter is NULL, FindFirstUrlCacheEntryEx returns all content entries in the cache.
      string lpszUrlSearchPattern,
        //Controls the enumeration. No flags are currently implemented; this parameter must be set to zero.
      uint dwFlags,
        //A bitmask indicating the type of cache entry and its properties. 
        //The cache entry types include: 
        //history entries (URLHISTORY_CACHE_ENTRY), 
        //cookie entries (COOKIE_CACHE_ENTRY), 
        //and normal cached content (NORMAL_CACHE_ENTRY).
      uint dwFilter,
        //ID of the cache group to be enumerated. Set this parameter to zero to enumerate all entries that are not grouped.
      Int64 GroupId,
        //Pointer to a INTERNET_CACHE_ENTRY_INFO  structure to receive the cache entry information.
      IntPtr lpFirstCacheEntryInfo,
        //Pointer to variable that indicates the size of the structure referenced by the lpFirstCacheEntryInfo parameter, in bytes.
      ref uint lpdwFirstCacheEntryInfoBufferSize,
        //This parameter is reserved and must be NULL.
      IntPtr lpGroupAttributes,
        //This parameter is reserved and must be NULL.
      IntPtr pcbGroupAttributes,
        //his parameter is reserved and must be NULL.
      IntPtr lpReserved
    );

    
 //FindNextUrlCacheEntryEx(vHandle, (IntPtr)null, ref vFirstCacheEntryInfoBufferSize, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
//FindNextUrlCacheEntryEx(vHandle, vBuffer, ref vFirstCacheEntryInfoBufferSize, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)


    [DllImport("wininet.dll")]
    public static extern bool FindCloseUrlCache(IntPtr hEnumHandle);

    [DllImport("wininet.dll")]
    public static extern bool FindNextUrlCacheEntryEx(
        IntPtr hEnumHandle,
        IntPtr lpFirstCacheEntryInfo,
        ref uint lpdwFirstCacheEntryInfoBufferSize,
        Pointer lpGroupAttributes,
        Pointer pcbGroupAttributes,
        Pointer lpReserved);

    public uint NORMAL_CACHE_ENTRY = 0x00000001;

    private void button1_Click(object sender, EventArgs e)
    {
        //IntPtr vHandle;
        //INTERNET_CACHE_ENTRY_INFOW vInternetCacheEntryInfo = new INTERNET_CACHE_ENTRY_INFOW();
        //uint vFirstCacheEntryInfoBufferSize = 0;
        ////FindFirstUrlCacheEntryEx(null, 0, NORMAL_CACHE_ENTRY, 0, (IntPtr)null, ref vFirstCacheEntryInfoBufferSize, null, null, null);
        //IntPtr vBuffer = Marshal.AllocHGlobal((int)vFirstCacheEntryInfoBufferSize);
        ////vHandle = FindFirstUrlCacheEntryEx(null, 0, NORMAL_CACHE_ENTRY, 0, vBuffer, ref vFirstCacheEntryInfoBufferSize, null, null, null);
        //while (vHandle != null)
        //{
        //    Marshal.PtrToStructure(vBuffer, vInternetCacheEntryInfo);
        //    //richTextBox1.AppendText(vInternetCacheEntryInfo.lpszSourceUrlName + "\r\n");
        //    Marshal.FreeCoTaskMem(vBuffer);

        //    FindNextUrlCacheEntryEx(vHandle, (IntPtr)null, ref vFirstCacheEntryInfoBufferSize,
        //      null, null, null);
        //    vBuffer = Marshal.AllocHGlobal((int)vFirstCacheEntryInfoBufferSize);
        //    if (!FindNextUrlCacheEntryEx(vHandle, vBuffer,
        //       ref vFirstCacheEntryInfoBufferSize, null, null, null)) break;
        //}
        //Marshal.FreeCoTaskMem(vBuffer);
    }
}


}
