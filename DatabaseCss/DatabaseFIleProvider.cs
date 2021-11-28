using System;
using System.IO;
using System.Linq;
using System.Text;
using DatabaseCss.DAL;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace DatabaseCss
{
    public class DatabaseFileProvider : IFileProvider
    {
        private class DataBaseFileInfo : IFileInfo
        {
            private readonly string _content;
            public DataBaseFileInfo(CssFile file)
            {
                if (file != null){
                    Name = file.Name;
                    PhysicalPath = file.Name;
                    LastModified = file.LastUpdateDate;
                    Length = Encoding.GetEncoding("UTF-8").GetByteCount(file.Css);
                    Exists = true;
                    _content = file.Css;
                }
            }
            public Stream CreateReadStream()
            {
                return new MemoryStream(Encoding.GetEncoding("UTF-8").GetBytes(_content ?? ""));
            }

            public bool Exists { get; }

            public bool IsDirectory => false;
            public DateTimeOffset LastModified { get; }
            public long Length { get; }
            public string Name { get; }
            public string PhysicalPath { get; }
        }

        private class DataBaseChangeToken : IChangeToken
        {
            private bool _hasChanged;
            public DataBaseChangeToken(DataBaseWatcher watcher)
            {
                watcher.RegisterCallback(() => _hasChanged = true);
            }
        
            public IDisposable RegisterChangeCallback(Action<object> callback, object state)
            {
                throw new NotImplementedException();
            }

            public bool ActiveChangeCallbacks => false;
            public bool HasChanged => _hasChanged;
        }
        
        private readonly DataBaseContext _context;
        public DatabaseFileProvider(DataBaseContext context)
        {
            _context = context;
        }
        
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new NotFoundDirectoryContents();
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (!subpath.StartsWith("/database/")) return new DataBaseFileInfo(null);
            
            var file = _context.CssFiles.Single(file => file.Name.Equals(subpath));
            _context.Entry(file).Reload();
            return new DataBaseFileInfo(file);

        }

        public IChangeToken Watch(string filter)
        {
            return new DataBaseChangeToken(DataBaseWatcher.GetInstance());
        }
    }
}