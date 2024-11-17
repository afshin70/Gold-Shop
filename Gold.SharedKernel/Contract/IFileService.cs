using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Gold.SharedKernel.Contract
{
    public interface IFileService
    {
        bool DeleteFile(string? fileName, string Path);
        bool IsExist(IFormFile file, string pathRoot);
        bool IsExist(string fileName, string pathRoot);
        bool IsExist(IFormFile file, string pathRoot, string exeptionFileName);
        bool IsExist(string fileName, string exeptionFileName, string pathRoot);
        bool SaveFile(IFormFile file, string fileName, string savePath);
        Task<bool> SaveFileAsync(IFormFile file, string fileName, string savePath);
        bool SaveImageFile(byte[] img, int width, int height, string savePath, string imageName);
        bool SaveImageFile(byte[] img, string savePath, string imageName);
        bool SaveImageFile(IFormFile img, int width, int height, string savePath, string imageName);
        bool SaveImageFile(IFormFile img, string savePath, string imageName);
        bool UpdateFile(IFormFile newFile, string newFileName, string oldFileName, string savePath);
        Task<bool> UpdateFileAsync(IFormFile newFile, string oldFileName, string updatePath);
        Task<bool> UpdateFileAsync(IFormFile newFile, string newFileName, string oldFileName, string updatePath);
        bool UpdateImageFile(IFormFile newImg, int width, int height, string path, string NewimageName, string oldImageName);
        Task<bool> UploadFileAsync(IFormFile file);
        Task<bool> UploadFileAsync(IFormFile file, string pathRoot);
        Task<bool> UploadFileAsync(IFormFile file, string pathRoot, string fileName);

        Task<byte[]> GetFileBytesAsync(string filePath);
        byte[] GetFileBytes(string filePath);
        string GetMimeTypeForFileExtension(string filePath);
        bool CopyFile(string sourceFilePath, string destinationFilePath, bool overWrite = true);
		byte[] CreateExcelFileFromDataTable(DataTable SearchedList, double[] ColumnWidth, string titleName, bool RightToLeft = false);
	}
}