using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ECom.Models.ViewModels
{
    public class FileUploadViewModel
    {
        public string Label { get; set; } = "Upload Files";
        public string InputName { get; set; } = "files";
        public string InputId { get; set; } = "file-upload";
        public string PreviewId { get; set; } = "file-preview";
        public string ErrorId { get; set; } = "file-upload-error";
        public bool AllowMultiple { get; set; } = true;
        public int MaxFileCount { get; set; } = 10;
        public int MaxFileSizeMB { get; set; } = 10;
        public string AcceptedFileTypes { get; set; } = "image/*";
        public string FileTypeDescription { get; set; } = "PNG, JPG, GIF";
        public IFormFileCollection Files { get; set; }
        public Dictionary<string, string> AcceptedMimeTypes { get; set; } = new()
    {
        { "image/jpeg", ".jpg,.jpeg" },
        { "image/png", ".png" },
        { "image/gif", ".gif" }
    };
    }
}
