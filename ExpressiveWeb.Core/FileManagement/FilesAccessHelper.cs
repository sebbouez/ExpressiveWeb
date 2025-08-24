// *********************************************************
// 
// ExpressiveWeb.Core FilesAccessHelper.cs
// Copyright (c) Sébastien Bouez. All rights reserved.
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// *********************************************************

namespace ExpressiveWeb.Core.FileManagement;

public static class FilesAccessHelper
{
    private const int MaxTry = 3;
    private const int RetryDelay = 200;

    /// <summary>
    ///     Performs basic checks on the file path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool CheckFileIsValid(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        char[] invalidPathChars = Path.GetInvalidPathChars();
        if (filePath.Any(c => invalidPathChars.Contains(c)))
        {
            return false;
        }

        string fileName = Path.GetFileName(filePath);
        string? directoryPath = Path.GetDirectoryName(filePath);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        if (fileName.Any(c => invalidFileNameChars.Contains(c)))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(directoryPath))
        {
            try
            {
                Path.GetFullPath(directoryPath);
            }
            catch
            {
                return false;
            }
        }


        return true;
    }

    public static string ReadAllText(string filePath)
    {
        for (int i = 1; i <= MaxTry; ++i)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }
            catch (IOException)
            {
                // You may check error code to filter some exceptions, not every error
                // can be recovered.
                Thread.Sleep(RetryDelay);
            }
        }

        throw new IOException();
    }

    private static bool TryCreateDirectory(string filePath)
    {
        try
        {
            if (!string.IsNullOrEmpty(filePath) && Path.GetDirectoryName(filePath) is { } directory)
            {
                Directory.CreateDirectory(directory);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool WriteAllText(string filePath, string content)
    {
        for (int i = 1; i <= MaxTry; ++i)
        {
            try
            {
                Thread.Sleep(1000);
                File.WriteAllText(filePath, content);
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                if (!TryCreateDirectory(filePath))
                {
                    return false;
                }
            }
            catch (IOException) when (i <= MaxTry)
            {
                // You may check error code to filter some exceptions, not every error
                // can be recovered.
                Thread.Sleep(RetryDelay);
            }
        }

        return false;
    }

    public static async Task<bool> WriteAllTextAsync(string filePath, string content)
    {
        for (int i = 1; i <= MaxTry; ++i)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, content);
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                if (!TryCreateDirectory(filePath))
                {
                    return false;
                }
            }
            catch (IOException) when (i <= MaxTry)
            {
                // You may check error code to filter some exceptions, not every error
                // can be recovered.
                Thread.Sleep(RetryDelay);
            }
        }

        return false;
    }

    public static bool CopyFile(string sourceFilePath, string destinationFilePath)
    {
        if (!File.Exists(sourceFilePath))
        {
            return false;
        }

        if (!TryCreateDirectory(destinationFilePath))
        {
            return false;
        }

        using FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read);
        using FileStream destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write);
        sourceStream.CopyTo(destinationStream);

        return true;
    }
}